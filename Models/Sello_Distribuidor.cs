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
    
    public partial class Sello_Distribuidor
    {
        public int RowID { get; set; }
        public Nullable<int> SelloID { get; set; }
        public Nullable<int> DistribuidorID { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public string CreadoPor { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual Tercero Tercero { get; set; }
        public virtual Tercero Tercero1 { get; set; }
    }
}
