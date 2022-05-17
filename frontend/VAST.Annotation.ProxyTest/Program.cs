// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore;
using VAST.Annotation.Proxy.Data;
using VAST.Annotation.Proxy.Services;
using VAST.Ontology.Database;
using VAST.Ontology.Database.Models;

Console.WriteLine("Please provide the password for the VAST Annotator:");
string password = Console.ReadLine();

AnnoDataService dataService = new AnnoDataService("vast.annotator@gmail.com", password);
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
        Name = k.label ?? "",
        Value = k.name ?? "",
        IsImported = true,
        IsInSchema = true,
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
                Name = i.value.ToString() ?? "",
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

    //Store the data
    using (VastOntologyContext context = new VastOntologyContext())
    {
        context.Database.Migrate();

        context.Annotations.RemoveRange(context.Annotations);
        context.Items.RemoveRange(context.Items);
        context.Items.AddRange(dbItems);
        context.Annotations.AddRange(dbAnnotations);

        context.SaveChanges();
    }
}