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
    
    public partial class TipoMenu
    {
        public TipoMenu()
        {
            this.Menu = new HashSet<Menu>();
        }
    
        public int RowID { get; set; }
        public string Nombre { get; set; }
        public Nullable<bool> Activo { get; set; }
        public string CreadoPor { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
        public Nullable<int> Orden { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual ICollection<Menu> Menu { get; set; }
    }
}
