using System;
using Microsoft.EntityFrameworkCore;
using Controlinventarios; 

namespace Controlinventarios.Model
{
    public partial class InventoryTIContext : DbContext
    {
        public InventoryTIContext()
        {
        }

        public InventoryTIContext(DbContextOptions<InventoryTIContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Element>().HasKey(t => t.id);
            modelBuilder.Entity<ElementType>().HasKey(t => t.id);
            modelBuilder.Entity<Persona>().HasKey(t => t.id);
            modelBuilder.Entity<Area>().HasKey(t => t.id);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Area> Area { get; set; }
        public virtual DbSet<Persona> Persona { get; set; }
        public virtual DbSet<ElementType> ElementType { get; set; }
        public virtual DbSet<Element> Element { get; set; }
    }
}