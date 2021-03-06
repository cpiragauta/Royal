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
    
    public partial class Pais
    {
        public Pais()
        {
            this.Departamento = new HashSet<Departamento>();
            this.EncabezadoPelicula = new HashSet<EncabezadoPelicula>();
        }
    
        public int RowID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public string ErpID { get; set; }
    
        public virtual ICollection<Departamento> Departamento { get; set; }
        public virtual ICollection<EncabezadoPelicula> EncabezadoPelicula { get; set; }
    }
}
