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
    
    public partial class UsuarioSistema
    {
        public UsuarioSistema()
        {
            this.BoletaVendida = new HashSet<BoletaVendida>();
        }
    
        public int RowID { get; set; }
        public int RolID { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public string NombreUsuario { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public Nullable<bool> Activo { get; set; }
        public string CreadoPor { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public string ModificadoPor { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<bool> Sincronizado { get; set; }
        public Nullable<int> TeatroID { get; set; }
        public Nullable<int> RowIDCentral { get; set; }
        public string Foto_Empleado { get; set; }
    
        public virtual Teatro Teatro { get; set; }
        public virtual Rol Rol { get; set; }
        public virtual ICollection<BoletaVendida> BoletaVendida { get; set; }
    }
}
