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
    
    public partial class Actividades
    {
        public int rowID { get; set; }
        public string asunto { get; set; }
        public int tipoActividadID { get; set; }
        public System.DateTime fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFin { get; set; }
        public int tipoReferenciaID { get; set; }
        public string referenciado_a { get; set; }
        public int prioridadID { get; set; }
        public int estadoID { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public string creadoPor { get; set; }
        public Nullable<System.DateTime> fechaMod { get; set; }
        public string usuarioMod { get; set; }
        public string Adjunto { get; set; }
        public string CopiaCorreo { get; set; }
    
        public virtual Estado Estado { get; set; }
        public virtual Opcion Opcion { get; set; }
        public virtual Opcion Opcion1 { get; set; }
        public virtual Opcion Opcion2 { get; set; }
    }
}
