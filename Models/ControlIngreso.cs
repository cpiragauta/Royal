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
    
    public partial class ControlIngreso
    {
        public ControlIngreso()
        {
            this.BoletaVendida = new HashSet<BoletaVendida>();
            this.ProductoVendido = new HashSet<ProductoVendido>();
        }
    
        public int RowID { get; set; }
        public double Efectivo { get; set; }
        public double Total { get; set; }
        public double Cambio { get; set; }
        public Nullable<int> UsuarioID { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public Nullable<System.TimeSpan> Hora { get; set; }
    
        public virtual ICollection<BoletaVendida> BoletaVendida { get; set; }
        public virtual UsuarioSistema UsuarioSistema { get; set; }
        public virtual ICollection<ProductoVendido> ProductoVendido { get; set; }
    }
}