// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using VAST.Annotation.Proxy.Data;
using VAST.Annotation.Proxy.Services;
using VAST.Annotation.ProxyTest;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

List<CsvRow> ReadCsv(string filePath)
{
    var result = new List<CsvRow>();
    var lines = File.ReadLines(filePath).Skip(1);  // Skip the header line

    foreach (var line in lines)
    {
        var fields = line.Split(';');
        var row = new CsvRow
        {
            TypeId = int.Parse(fields[0]),
            AuthorId = fields[1],
            Inserted = DateTime.Parse(fields[2]),
            PrimaryContextId = int.Parse(fields[3]),
            Source = fields[4],
            Target = fields[5],
            ContextName = fields[6]
        };
        result.Add(row);
    }

    return result;
}

var csvParsed = ReadCsv(@"D:\VAST\DB_Dumps\OntologyLinks.csv");

//Console.WriteLine("Please provide the password for the VAST Annotator:");
string password = "yikbir8"; // Console.ReadLine();

var optionsBuilder2 = new DbContextOptionsBuilder<VastOntologyContext>();
optionsBuilder2.UseNpgsql("Host=localhost;Port=6543;Database=OntologyTool;Username=postgres;Password=tGwBPi33XRUIIdygVYWVY");
var options2 = optionsBuilder2.Options;

//Store the data

using (VastOntologyContext context = new VastOntologyContext(options2))
{
    context.Collections.RemoveRange(context.Collections);

    context.Documents.RemoveRange(context.Documents);

    context.Annotations.RemoveRange(context.Annotations);

    context.Items.RemoveRange(context.Items);

    context.SaveChanges();
}

//AnnoDataService dataService = new AnnoDataService("vast.annotator@gmail.com", password);
AnnoDataService dataService = new AnnoDataService("vast.readonly@gmail.com", "GrurvObhonsyacPo12");
var resultCollections = await dataService.GetCollections();

List<AnnotationItem> allAnnotations = new List<AnnotationItem>();
//Get the dta from the API
if (resultCollections.success)
{
    int documentsCount = 0;
    int annotationCount = 0;

    List<Collection> dbCollections = new List<Collection>();
    List<Document> dbDocuments = new List<Document>();

    var collections = resultCollections.data;
    Console.WriteLine($"Collections: {collections.Length}");
    foreach (var collection in collections)
    {
        Console.WriteLine($"Collection: {collection.name}");

        var currentDbCollection = new Collection()
        {
            Name = collection.name,
            OriginalId = collection.id,
        };
        dbCollections.Add(currentDbCollection);

        var resultDocuments = await dataService.GetDocuments(collection.id);
        var documents = resultDocuments.data;
        documentsCount += documents.Length;
        foreach (var document in documents)
        {
            var currentDocument = new Document()
            {
                Name = document.name,
                OriginalId = document.id,
                Collection = currentDbCollection,
            };
            dbDocuments.Add(currentDocument);

            Console.WriteLine($"Document: {document.name}");
            var resultAnnotations = await dataService.GetAnnotations(collection.id, document.id);
            var annotations = resultAnnotations.data;
            Console.WriteLine($"Annotations: {annotations.Length}");
            annotationCount += annotations.Length;

            allAnnotations.AddRange(annotations.Where(a => a.spans.Length > 0));
        }
    }

    var vastSchema = await dataService.GetSchema();

    Console.WriteLine($"Total Collections: {collections.Length}");
    Console.WriteLine($"Total Annotations: {annotationCount}");
    Console.WriteLine($"Total Documents: {documentsCount}");

    //Transform the data to be stored in the database
    List<Value_Options> allKeywords = new List<Value_Options>();

    foreach (Group vastSchemaGroup in vastSchema.groups)
    {
        foreach (Value_Options[] vastValueOptions in vastSchemaGroup.value_options)
        {
            Value_Options currentOptions = new Value_Options();
            foreach (Value_Options vastValueOption in vastValueOptions)
            {
                currentOptions.group = vastValueOption.group ?? currentOptions.group;
                currentOptions.label = vastValueOption.label ?? currentOptions.label;
                currentOptions.name = vastValueOption.name ?? currentOptions.name;
                currentOptions.type = vastValueOption.type ?? currentOptions.type;
            }

            allKeywords.Add(currentOptions);
        }
    }

    var dbItems = allKeywords.Select(k => new Item()
    {
        Description = k.group ?? "",
        Name = k.label?.Replace("\\n", " ") ?? "",
        Value = k.name ?? "",
        IsImported = true,
        IsInSchema = true,
        ItemType = ItemType.Keyword,
        LastSyncTime = DateTime.Now.ToUniversalTime()
    }).ToList();

    List<Item> missingOptions = new List<Item>();
    foreach (AnnotationItem annotationItem in allAnnotations)
    {
        dbItems.AddRange(annotationItem.attributes
            .Where(a => dbItems.Count(d => d.Value?.ToLowerInvariant() == a.value?.ToString()?.ToLowerInvariant() || d.Name?.ToLowerInvariant() == a.value?.ToString()?.ToLowerInvariant()) == 0)
            .Select(i => new Item
            {
                IsImported = true,
                IsInSchema = false,
                Name = i.value.ToString()?.Replace("\\n", " ") ?? "",
                Value = i.value.ToString()?.ToLowerInvariant()?.Replace(" ", "_").Replace("/", "_") ?? "",
                LastSyncTime = DateTime.Now.ToUniversalTime(),
                Description = "",
                
            }).ToList());
    }


    var dbAnnotations = allAnnotations.Select(k => new Annotation
    {
        AnnotationItem = k.attributes.Select(a => a.value?.ToString()?.ToLowerInvariant()).Select(a => dbItems.FirstOrDefault(d => d.Value?.ToLowerInvariant() == a || d.Name?.ToLowerInvariant() == a)).Where(a => a != null).ToList(),
        CollectionId = k.collection_id,
        Description = string.Join("\r\n", k.spans.Select(s => s.segment)) ?? "",
        DocumentId = k.document_id,
        OriginalId = k._id ?? "",
        Created = DateTime.TryParse(k.created_at, out DateTime created) ? created.ToUniversalTime() : DateTime.MinValue,
        CreatedBy = k.created_by ?? "",
        IsImported = true,
        Document = dbDocuments.Where(d=>d.OriginalId==k.document_id).Single()
    }).ToList();

    //Go through all the CSV keyword/concept and concept/concept links and add them to the database
    var optionsBuilder = new DbContextOptionsBuilder<VastOntologyContext>();
    optionsBuilder.UseNpgsql("Host=localhost;Port=6543;Database=OntologyTool;Username=postgres;Password=tGwBPi33XRUIIdygVYWVY");
    var options = optionsBuilder.Options;


    string NormalizeString(string source)
    {
        return source.ToLower().Trim().Replace(" ", "_")
            .Replace("/", "_").Replace("\\n", "")
            .Replace(" ", "");
    }

    //Store the data
    using (VastOntologyContext context = new VastOntologyContext(options))
    {
        int row = 0;
        foreach (var link in csvParsed)
        {
            Console.WriteLine($"Row: {row++}");
            var sourceItem = dbItems.FirstOrDefault(i => NormalizeString(i.Value) == NormalizeString(link.Source));
            var targetItem = dbItems.FirstOrDefault(i => NormalizeString(i.Value) == NormalizeString(link.Target));

            if (sourceItem == null)
            {
                //Source item does not exist yet, so create it
                sourceItem = new Item()
                {
                    Description = "",
                    IsImported = false,
                    IsInSchema = false,
                    ItemType = ItemType.Concept,
                    LastSyncTime = DateTime.Now.ToUniversalTime(),
                    Name = link.Source.Replace("\\n", " "),
                    Value = link.Source.ToLower().Trim().Replace(" ", "_").Replace("/", "_").Replace("\\n", "")
                };
                dbItems.Add(sourceItem);
            }

            if (targetItem == null)
            {
                //Target item does not exist yet, so create it
                targetItem = new Item()
                {
                    Description = "",
                    IsImported = false,
                    IsInSchema = false,
                    ItemType = ItemType.Concept,
                    LastSyncTime = DateTime.Now.ToUniversalTime(),
                    Name = link.Target.Replace("\\n", " "),
                    Value = link.Target.ToLower().Trim().Replace(" ", "_").Replace("/", "_").Replace("\\n", "")
                };
                dbItems.Add(targetItem);
            }

            //Create the link
            var linkItem = new ItemLink()
            {
                Source = sourceItem,
                Target = targetItem,
                AuthorId = link.AuthorId,
                CreatedDate = link.Inserted.ToUniversalTime(),
                AuthorName = link.AuthorId,
                RelationshipType = context.RelationshipTypes.Single(rt => rt.Id== link.TypeId),
                PrimaryContext = context.Contexts.Single(pc => pc.Id == link.PrimaryContextId),
            };

            context.ItemLinks.Add(linkItem);
        }

        context.Collections.AddRange(dbCollections);

        context.Documents.AddRange(dbDocuments);

        context.Annotations.AddRange(dbAnnotations);

        context.Items.AddRange(dbItems);

        context.SaveChanges();

    }
}