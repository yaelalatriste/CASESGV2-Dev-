namespace RDLCDesign
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class view_prueba
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Estatus { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Folio { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(12)]
        public string Mes { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Anio { get; set; }

        public decimal? Calificacion { get; set; }

        public decimal? Penalizacion { get; set; }
    }
}
