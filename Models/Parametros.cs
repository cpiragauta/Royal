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
    
    public partial class Parametros
    {
        public int rowid { get; set; }
        public string cod_parametro { get; set; }
        public string nombre_paramatro { get; set; }
        public string valor_parametros { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
        public string CreadoPor { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
    }
}
