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
    
    public partial class VerMapaVenta_Result
    {
        public int RowIDObjeto { get; set; }
        public string NombreObjeto { get; set; }
        public string ImagenObjeto { get; set; }
        public Nullable<bool> NumeracionObjeto { get; set; }
        public string TipoObjeto { get; set; }
        public Nullable<int> RowIDServicioObjeto { get; set; }
        public string NombreServicioObjeto { get; set; }
        public int RowIDSala { get; set; }
        public string NombreSala { get; set; }
        public int RowIDSillaMapa { get; set; }
        public int RowIDFuncion { get; set; }
        public Nullable<int> SalaFilas { get; set; }
        public Nullable<int> SalaColumnas { get; set; }
        public string SillaColumna { get; set; }
        public Nullable<short> SillaFila { get; set; }
        public int SillaVendida { get; set; }
        public int SillaBloqueada { get; set; }
        public int SillaReservada { get; set; }
    }
}
