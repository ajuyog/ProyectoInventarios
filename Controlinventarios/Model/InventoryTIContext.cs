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

        public virtual DbSet<Area> inv_area { get; set; }
        public virtual DbSet<Persona> inv_persona { get; set; }
        public virtual DbSet<ElementType> inv_elementType { get; set; }
        public virtual DbSet<Element> inv_element { get; set; }
        public virtual DbSet<Identificador> inv_identificador { get; set; }
    }
}