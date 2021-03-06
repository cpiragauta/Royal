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
    
    public partial class CentroOperacion
    {
        public CentroOperacion()
        {
            this.Teatro = new HashSet<Teatro>();
        }
    
        public int RowID { get; set; }
        public Nullable<int> CompaniaID { get; set; }
        public string CentroOperacionID { get; set; }
        public string Nombre { get; set; }
        public string Region { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<System.DateTime> FechaSincronizacion { get; set; }
        public Nullable<System.DateTime> FechaUltimaSincronizacion { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
        public string CreadoPor { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
    
        public virtual ICollection<Teatro> Teatro { get; set; }
    }
}
