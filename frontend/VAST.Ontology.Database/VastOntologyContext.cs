using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VAST.Ontology.Database.Models;

namespace VAST.Ontology.Database
{
    public class VastOntologyContext : DbContext
    {
        public DbSet<Annotation> Annotations { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemLink> ItemLinks { get; set; }
        public DbSet<RelationshipType> RelationshipTypes { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Context> Contexts { get; set; }

        public VastOntologyContext(DbContextOptions<VastOntologyContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Annotation>(annotation =>
            {
                annotation.HasKey(a => a.Id);
                annotation.Property(a => a.Id).ValueGeneratedOnAdd();
                annotation.HasMany(a => a.AnnotationItem).WithMany(i => i.Annotations);
            });

            modelBuilder.Entity<Context>(context =>
            {
                context.HasKey(a => a.Id);
                context.Property(a => a.Id).ValueGeneratedOnAdd();
                context.HasMany(v => v.SecondaryItems).WithMany(il => il.Contexts);
                context.HasMany(v => v.PrimaryItems).WithOne(a => a.PrimaryContext).IsRequired(false);
                context.HasMany(v => v.PrimaryItemLinks).WithOne(a => a.PrimaryContext).IsRequired(false);
            });

            modelBuilder.Entity<Item>(item =>
            {
                item.HasKey(a => a.Id);
                item.Property(a => a.Id).ValueGeneratedOnAdd();
                item.HasMany(a => a.SourceLinks).WithOne(i => i.Source);
                item.HasMany(a => a.TargetLinks).WithOne(i => i.Target);
                item.HasMany(i => i.Annotations).WithMany(a => a.AnnotationItem);
                item.HasMany(i => i.Contexts).WithMany(a => a.SecondaryItems);
                item.HasOne(i => i.PrimaryContext).WithMany(c => c.PrimaryItems).IsRequired(false);
            });

            modelBuilder.Entity<ItemLink>(itemLink =>
            {
                itemLink.HasKey(a => a.Id);
                itemLink.Property(a => a.Id).ValueGeneratedOnAdd();
                itemLink.HasMany(il => il.Votes).WithOne(v => v.ItemLink);
                itemLink.HasOne(il => il.RelationshipType);
                itemLink.HasOne(il => il.Source).WithMany(il => il.SourceLinks);
                itemLink.HasOne(il => il.Target).WithMany(il => il.TargetLinks);
                itemLink.HasOne(il => il.PrimaryContext).WithMany(il => il.PrimaryItemLinks).IsRequired(false);
            });

            modelBuilder.Entity<RelationshipType>(relationshipType =>
            {
                relationshipType.HasKey(a => a.Id);
                relationshipType.Property(a => a.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Vote>(vote =>
            {
                vote.HasKey(a => a.Id);
                vote.Property(a => a.Id).ValueGeneratedOnAdd();
                vote.HasOne(v => v.ItemLink).WithMany(il => il.Votes);
            });

            modelBuilder.Entity<Collection>(vote =>
            {
                vote.HasKey(a => a.Id);
                vote.Property(a => a.Id).ValueGeneratedOnAdd();
                vote.HasMany(v => v.Documents).WithOne(d => d.Collection);
            });

            modelBuilder.Entity<Document>(vote =>
            {
                vote.HasKey(a => a.Id);
                vote.Property(a => a.Id).ValueGeneratedOnAdd();
                vote.HasOne(v => v.Collection).WithMany(il => il.Documents);
                vote.HasMany(v => v.Annotations).WithOne(a => a.Document);
            });
        }
    }
}
