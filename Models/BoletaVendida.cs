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
    
    public partial class BoletaVendida
    {
        public long RowID { get; set; }
        public Nullable<int> SillaID { get; set; }
        public Nullable<int> TarifaID { get; set; }
        public Nullable<int> TaquillaID { get; set; }
        public Nullable<int> FuncionID { get; set; }
        public Nullable<System.DateTime> FechaVenta { get; set; }
        public string MedioPago { get; set; }
        public Nullable<int> UsuarioID { get; set; }
    
        public virtual Funcion Funcion { get; set; }
        public virtual ListaDetalle ListaDetalle { get; set; }
        public virtual MapaSala MapaSala { get; set; }
        public virtual UsuarioSistema UsuarioSistema { get; set; }
        public virtual Taquilla Taquilla { get; set; }
    }
}
