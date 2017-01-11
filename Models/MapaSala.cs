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
    
    public partial class MapaSala
    {
        public MapaSala()
        {
            this.BoletaVendida = new HashSet<BoletaVendida>();
        }
    
        public int RowID { get; set; }
        public Nullable<int> ObjetoID { get; set; }
        public Nullable<int> ZonaID { get; set; }
        public Nullable<int> SalaID { get; set; }
        public Nullable<int> EstadoID { get; set; }
        public string Columna { get; set; }
        public Nullable<short> Fila { get; set; }
        public Nullable<short> PosicionX { get; set; }
        public Nullable<short> PosicionY { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual Estado Estado { get; set; }
        public virtual SalaObjeto SalaObjeto { get; set; }
        public virtual Zona Zona { get; set; }
        public virtual ICollection<BoletaVendida> BoletaVendida { get; set; }
        public virtual Sala Sala { get; set; }
    }
}
