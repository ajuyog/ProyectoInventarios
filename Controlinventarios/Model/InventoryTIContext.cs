using System;
using Microsoft.EntityFrameworkCore;
using Controlinventarios;
using Controlinventarios.Dto;

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
            //modelBuilder.Entity<CentroDeCostoDto>().HasKey(t => t.VlrNeto);
            modelBuilder.Entity<Asignacion>().HasKey(t => t.IdPersona);
            modelBuilder.Entity<Ensamble>().HasKey(t => t.Id);
            modelBuilder.Entity<Propiedades>().HasKey(t => t.id);
            modelBuilder.Entity<ElementType>().HasKey(t => t.id);
            modelBuilder.Entity<Persona>().HasKey(t => t.userId);
            modelBuilder.Entity<Area>().HasKey(t => t.id);
            
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<Area> inv_area { get; set; }
        public virtual DbSet<Persona> inv_persona { get; set; }
        public virtual DbSet<ElementType> inv_elementType { get; set; }
        public virtual DbSet<Propiedades> inv_propiedades { get; set; }
        public virtual DbSet<Ensamble> inv_ensamble { get; set; }
        public virtual DbSet<Asignacion> inv_asignacion { get; set; }
        public virtual DbSet<FacturacionTMK> inv_facturaciontmk { get; set; }
        public virtual DbSet<Marca> inv_marca { get; set; }
        public virtual DbSet<AspnetUsers> aspnetusers { get; set; }
        public virtual DbSet<Empresa> inv_empresa { get; set; }
    }
}