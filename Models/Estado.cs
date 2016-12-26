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
    
    public partial class Estado
    {
        public Estado()
        {
            this.DetalleConvenio = new HashSet<DetalleConvenio>();
            this.EncabezadoConvenio = new HashSet<EncabezadoConvenio>();
            this.Actividades = new HashSet<Actividades>();
            this.OportunidadVenta = new HashSet<OportunidadVenta>();
            this.TarjetaMembresia = new HashSet<TarjetaMembresia>();
            this.TarjetaMembresiaClienteRoyal = new HashSet<TarjetaMembresiaClienteRoyal>();
            this.DetallePelicula = new HashSet<DetallePelicula>();
            this.DetalleVentaEmpresarial = new HashSet<DetalleVentaEmpresarial>();
            this.EncabezadoVentaEmpresarial = new HashSet<EncabezadoVentaEmpresarial>();
            this.EncabezadoPelicula = new HashSet<EncabezadoPelicula>();
            this.EncabezadoProgramacion = new HashSet<EncabezadoProgramacion>();
            this.Funcion = new HashSet<Funcion>();
            this.GeneroPelicula = new HashSet<GeneroPelicula>();
            this.Impuesto = new HashSet<Impuesto>();
            this.ListaDetalle = new HashSet<ListaDetalle>();
            this.MedioPelicula = new HashSet<MedioPelicula>();
            this.PorcentajeParticipacion = new HashSet<PorcentajeParticipacion>();
            this.Pqrs = new HashSet<Pqrs>();
            this.Sala = new HashSet<Sala>();
            this.Seguimiento = new HashSet<Seguimiento>();
            this.MapaSala = new HashSet<MapaSala>();
            this.Taquilla = new HashSet<Taquilla>();
            this.Teatro = new HashSet<Teatro>();
        }
    
        public int RowID { get; set; }
        public Nullable<int> TipoEstadoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    
        public virtual ICollection<DetalleConvenio> DetalleConvenio { get; set; }
        public virtual ICollection<EncabezadoConvenio> EncabezadoConvenio { get; set; }
        public virtual ICollection<Actividades> Actividades { get; set; }
        public virtual ICollection<OportunidadVenta> OportunidadVenta { get; set; }
        public virtual ICollection<TarjetaMembresia> TarjetaMembresia { get; set; }
        public virtual ICollection<TarjetaMembresiaClienteRoyal> TarjetaMembresiaClienteRoyal { get; set; }
        public virtual ICollection<DetallePelicula> DetallePelicula { get; set; }
        public virtual ICollection<DetalleVentaEmpresarial> DetalleVentaEmpresarial { get; set; }
        public virtual ICollection<EncabezadoVentaEmpresarial> EncabezadoVentaEmpresarial { get; set; }
        public virtual ICollection<EncabezadoPelicula> EncabezadoPelicula { get; set; }
        public virtual ICollection<EncabezadoProgramacion> EncabezadoProgramacion { get; set; }
        public virtual ICollection<Funcion> Funcion { get; set; }
        public virtual ICollection<GeneroPelicula> GeneroPelicula { get; set; }
        public virtual ICollection<Impuesto> Impuesto { get; set; }
        public virtual ICollection<ListaDetalle> ListaDetalle { get; set; }
        public virtual ICollection<MedioPelicula> MedioPelicula { get; set; }
        public virtual ICollection<PorcentajeParticipacion> PorcentajeParticipacion { get; set; }
        public virtual ICollection<Pqrs> Pqrs { get; set; }
        public virtual ICollection<Sala> Sala { get; set; }
        public virtual ICollection<Seguimiento> Seguimiento { get; set; }
        public virtual ICollection<MapaSala> MapaSala { get; set; }
        public virtual ICollection<Taquilla> Taquilla { get; set; }
        public virtual ICollection<Teatro> Teatro { get; set; }
        public virtual TipoEstado TipoEstado { get; set; }
    }
}
