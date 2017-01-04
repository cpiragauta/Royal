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
    
    public partial class Opcion
    {
        public Opcion()
        {
            this.EncabezadoConvenio = new HashSet<EncabezadoConvenio>();
            this.EncabezadoConvenio1 = new HashSet<EncabezadoConvenio>();
            this.EncabezadoConvenio2 = new HashSet<EncabezadoConvenio>();
            this.Actividades = new HashSet<Actividades>();
            this.Actividades1 = new HashSet<Actividades>();
            this.Actividades2 = new HashSet<Actividades>();
            this.ClienteRoyal = new HashSet<ClienteRoyal>();
            this.ClienteRoyal1 = new HashSet<ClienteRoyal>();
            this.ClienteRoyal2 = new HashSet<ClienteRoyal>();
            this.Impuesto = new HashSet<Impuesto>();
            this.DetallePelicula = new HashSet<DetallePelicula>();
            this.DetallePelicula1 = new HashSet<DetallePelicula>();
            this.Elenco = new HashSet<Elenco>();
            this.EncabezadoVentaEmpresarial = new HashSet<EncabezadoVentaEmpresarial>();
            this.EncabezadoVentaEmpresarial1 = new HashSet<EncabezadoVentaEmpresarial>();
            this.EncabezadoPelicula = new HashSet<EncabezadoPelicula>();
            this.EncabezadoPelicula1 = new HashSet<EncabezadoPelicula>();
            this.FormatoSala = new HashSet<FormatoSala>();
            this.GeneroPelicula = new HashSet<GeneroPelicula>();
            this.ListaDetalle = new HashSet<ListaDetalle>();
            this.ListaDetalle1 = new HashSet<ListaDetalle>();
            this.ListaDetalle2 = new HashSet<ListaDetalle>();
            this.ListaEncabezado = new HashSet<ListaEncabezado>();
            this.Sala = new HashSet<Sala>();
            this.SalaObjeto = new HashSet<SalaObjeto>();
            this.SalaObjeto1 = new HashSet<SalaObjeto>();
            this.ServicioSala = new HashSet<ServicioSala>();
            this.Tercero = new HashSet<Tercero>();
            this.Tercero1 = new HashSet<Tercero>();
            this.Tercero2 = new HashSet<Tercero>();
            this.TipoSolicitud = new HashSet<TipoSolicitud>();
            this.Taquilla = new HashSet<Taquilla>();
            this.Seguimiento = new HashSet<Seguimiento>();
            this.Seguimiento1 = new HashSet<Seguimiento>();
            this.ClienteRoyal3 = new HashSet<ClienteRoyal>();
        }
    
        public int RowID { get; set; }
        public int TipoID { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Codigo2 { get; set; }
        public string Descripcion { get; set; }
        public string ValorDefecto { get; set; }
        public Nullable<short> NumOrden { get; set; }
        public Nullable<bool> Activo { get; set; }
        public System.DateTime FechaCreacion { get; set; }
        public Nullable<System.DateTime> FechaModificacion { get; set; }
        public Nullable<int> RowIDCreacion { get; set; }
        public Nullable<int> TeatroCreacion { get; set; }
    
        public virtual ICollection<EncabezadoConvenio> EncabezadoConvenio { get; set; }
        public virtual ICollection<EncabezadoConvenio> EncabezadoConvenio1 { get; set; }
        public virtual ICollection<EncabezadoConvenio> EncabezadoConvenio2 { get; set; }
        public virtual ICollection<Actividades> Actividades { get; set; }
        public virtual ICollection<Actividades> Actividades1 { get; set; }
        public virtual ICollection<Actividades> Actividades2 { get; set; }
        public virtual ICollection<ClienteRoyal> ClienteRoyal { get; set; }
        public virtual ICollection<ClienteRoyal> ClienteRoyal1 { get; set; }
        public virtual ICollection<ClienteRoyal> ClienteRoyal2 { get; set; }
        public virtual ICollection<Impuesto> Impuesto { get; set; }
        public virtual ICollection<DetallePelicula> DetallePelicula { get; set; }
        public virtual ICollection<DetallePelicula> DetallePelicula1 { get; set; }
        public virtual ICollection<Elenco> Elenco { get; set; }
        public virtual ICollection<EncabezadoVentaEmpresarial> EncabezadoVentaEmpresarial { get; set; }
        public virtual ICollection<EncabezadoVentaEmpresarial> EncabezadoVentaEmpresarial1 { get; set; }
        public virtual ICollection<EncabezadoPelicula> EncabezadoPelicula { get; set; }
        public virtual ICollection<EncabezadoPelicula> EncabezadoPelicula1 { get; set; }
        public virtual ICollection<FormatoSala> FormatoSala { get; set; }
        public virtual ICollection<GeneroPelicula> GeneroPelicula { get; set; }
        public virtual ICollection<ListaDetalle> ListaDetalle { get; set; }
        public virtual ICollection<ListaDetalle> ListaDetalle1 { get; set; }
        public virtual ICollection<ListaDetalle> ListaDetalle2 { get; set; }
        public virtual ICollection<ListaEncabezado> ListaEncabezado { get; set; }
        public virtual ICollection<Sala> Sala { get; set; }
        public virtual ICollection<SalaObjeto> SalaObjeto { get; set; }
        public virtual ICollection<SalaObjeto> SalaObjeto1 { get; set; }
        public virtual ICollection<ServicioSala> ServicioSala { get; set; }
        public virtual ICollection<Tercero> Tercero { get; set; }
        public virtual ICollection<Tercero> Tercero1 { get; set; }
        public virtual ICollection<Tercero> Tercero2 { get; set; }
        public virtual ICollection<TipoSolicitud> TipoSolicitud { get; set; }
        public virtual Tipo Tipo { get; set; }
        public virtual ICollection<Taquilla> Taquilla { get; set; }
        public virtual ICollection<Seguimiento> Seguimiento { get; set; }
        public virtual ICollection<Seguimiento> Seguimiento1 { get; set; }
        public virtual ICollection<ClienteRoyal> ClienteRoyal3 { get; set; }
    }
}
