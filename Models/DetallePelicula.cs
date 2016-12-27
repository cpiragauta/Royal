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
    
    public partial class DetallePelicula
    {
        public DetallePelicula()
        {
            this.Funcion = new HashSet<Funcion>();
        }
    
        public int RowID { get; set; }
        public int EncabezadoPeliculaID { get; set; }
        public int TipoFormatoID { get; set; }
        public int TipoIdiomaID { get; set; }
        public Nullable<int> EstadoID { get; set; }
        public string Codigo { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
    
        public virtual Estado Estado { get; set; }
        public virtual Opcion Opcion { get; set; }
        public virtual Opcion Opcion1 { get; set; }
        public virtual EncabezadoPelicula EncabezadoPelicula { get; set; }
        public virtual ICollection<Funcion> Funcion { get; set; }
    }
}
