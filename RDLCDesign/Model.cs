using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace RDLCDesign
{
    public partial class Model : DbContext
    {
        public Model()
            : base("name=CedulasEvaluacion")
        {
        }

        public virtual DbSet<view_prueba> view_prueba { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<view_prueba>()
                .Property(e => e.Estatus)
                .IsUnicode(false);

            modelBuilder.Entity<view_prueba>()
                .Property(e => e.Folio)
                .IsUnicode(false);

            modelBuilder.Entity<view_prueba>()
                .Property(e => e.Mes)
                .IsUnicode(false);

            modelBuilder.Entity<view_prueba>()
                .Property(e => e.Calificacion)
                .HasPrecision(4, 1);

            modelBuilder.Entity<view_prueba>()
                .Property(e => e.Penalizacion)
                .HasPrecision(38, 2);
        }
    }
}
