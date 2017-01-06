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
    
    public partial class SalaObjeto
    {
        public SalaObjeto()
        {
            this.MapaSala = new HashSet<MapaSala>();
        }
    
        public int RowID { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> TipoObjetoID { get; set; }
        public Nullable<bool> Numeracion { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<int> ServicioID { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual Opcion Opcion { get; set; }
        public virtual Opcion Opcion1 { get; set; }
        public virtual ICollection<MapaSala> MapaSala { get; set; }
    }
}
