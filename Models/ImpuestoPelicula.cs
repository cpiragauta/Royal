//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaPOS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImpuestoPelicula
    {
        public int RowID { get; set; }
        public Nullable<int> EncabezadoPeliculaID { get; set; }
        public Nullable<int> ImpuestoID { get; set; }
        public Nullable<System.DateTime> FechaInicial { get; set; }
        public Nullable<System.DateTime> FechaFinal { get; set; }
        public Nullable<short> Porcentaje { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
    
        public virtual Impuesto Impuesto { get; set; }
        public virtual EncabezadoPelicula EncabezadoPelicula { get; set; }
    }
}
