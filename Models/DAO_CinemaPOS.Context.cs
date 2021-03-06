﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CinemaPOSEntities : DbContext
    {
        public CinemaPOSEntities()
            : base("name=CinemaPOSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DetalleConvenio> DetalleConvenio { get; set; }
        public virtual DbSet<EncabezadoConvenio> EncabezadoConvenio { get; set; }
        public virtual DbSet<Actividades> Actividades { get; set; }
        public virtual DbSet<ClienteRoyal> ClienteRoyal { get; set; }
        public virtual DbSet<TarjetaMembresia> TarjetaMembresia { get; set; }
        public virtual DbSet<TarjetaMembresiaClienteRoyal> TarjetaMembresiaClienteRoyal { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<CentroOperacion> CentroOperacion { get; set; }
        public virtual DbSet<Ciudad> Ciudad { get; set; }
        public virtual DbSet<Contacto> Contacto { get; set; }
        public virtual DbSet<Departamento> Departamento { get; set; }
        public virtual DbSet<EnvioMail> EnvioMail { get; set; }
        public virtual DbSet<Estado> Estado { get; set; }
        public virtual DbSet<Impuesto> Impuesto { get; set; }
        public virtual DbSet<Opcion> Opcion { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Parametros> Parametros { get; set; }
        public virtual DbSet<Plantillas> Plantillas { get; set; }
        public virtual DbSet<SalaObjeto> SalaObjeto { get; set; }
        public virtual DbSet<Sello_Distribuidor> Sello_Distribuidor { get; set; }
        public virtual DbSet<Tercero> Tercero { get; set; }
        public virtual DbSet<Tipo> Tipo { get; set; }
        public virtual DbSet<TipoEstado> TipoEstado { get; set; }
        public virtual DbSet<Zona> Zona { get; set; }
        public virtual DbSet<DetallePelicula> DetallePelicula { get; set; }
        public virtual DbSet<Elenco> Elenco { get; set; }
        public virtual DbSet<EncabezadoPelicula> EncabezadoPelicula { get; set; }
        public virtual DbSet<GeneroPelicula> GeneroPelicula { get; set; }
        public virtual DbSet<ImpuestoPelicula> ImpuestoPelicula { get; set; }
        public virtual DbSet<MedioPelicula> MedioPelicula { get; set; }
        public virtual DbSet<PorcentajeParticipacion> PorcentajeParticipacion { get; set; }
        public virtual DbSet<TeatroPelicula> TeatroPelicula { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<RolMenu> RolMenu { get; set; }
        public virtual DbSet<TipoMenu> TipoMenu { get; set; }
        public virtual DbSet<EvidenciaPqrs> EvidenciaPqrs { get; set; }
        public virtual DbSet<Pqrs> Pqrs { get; set; }
        public virtual DbSet<SeguientoEvidencia> SeguientoEvidencia { get; set; }
        public virtual DbSet<TipoSolicitud> TipoSolicitud { get; set; }
        public virtual DbSet<EncabezadoProgramacion> EncabezadoProgramacion { get; set; }
        public virtual DbSet<Funcion> Funcion { get; set; }
        public virtual DbSet<ListaPrecioFuncion> ListaPrecioFuncion { get; set; }
        public virtual DbSet<ListaDetalle> ListaDetalle { get; set; }
        public virtual DbSet<ListaEncabezado> ListaEncabezado { get; set; }
        public virtual DbSet<FormatoSala> FormatoSala { get; set; }
        public virtual DbSet<Sala> Sala { get; set; }
        public virtual DbSet<ServicioSala> ServicioSala { get; set; }
        public virtual DbSet<DetalleVentaEmpresarial> DetalleVentaEmpresarial { get; set; }
        public virtual DbSet<EncabezadoVentaEmpresarial> EncabezadoVentaEmpresarial { get; set; }
        public virtual DbSet<TeatroVentaEmpresarial> TeatroVentaEmpresarial { get; set; }
        public virtual DbSet<Funciones> Funciones { get; set; }
        public virtual DbSet<Taquilla> Taquilla { get; set; }
        public virtual DbSet<Seguimiento> Seguimiento { get; set; }
        public virtual DbSet<Rol> Rol { get; set; }
        public virtual DbSet<HistoricolLog> HistoricolLog { get; set; }
        public virtual DbSet<OportunidadVenta> OportunidadVenta { get; set; }
        public virtual DbSet<Teatro> Teatro { get; set; }
        public virtual DbSet<UsuarioSistema> UsuarioSistema { get; set; }
        public virtual DbSet<BoletaVendida> BoletaVendida { get; set; }
        public virtual DbSet<MapaSala> MapaSala { get; set; }
        public virtual DbSet<BoletasVendidas> BoletasVendidas { get; set; }
        public virtual DbSet<ControlIngreso> ControlIngreso { get; set; }
        public virtual DbSet<ControlCajaUsuarioEntrega> ControlCajaUsuarioEntrega { get; set; }
        public virtual DbSet<ControlCajaUsuarioRecibe> ControlCajaUsuarioRecibe { get; set; }
        public virtual DbSet<VistaCierreCaja> VistaCierreCaja { get; set; }
        public virtual DbSet<FuncionesTvShow> FuncionesTvShow { get; set; }
        public virtual DbSet<SillaBloqueo> SillaBloqueo { get; set; }
        public virtual DbSet<Producto> Producto { get; set; }
        public virtual DbSet<GrupoDias> GrupoDias { get; set; }
        public virtual DbSet<ProductoVendido> ProductoVendido { get; set; }
        public virtual DbSet<TvShowListaPrecios> TvShowListaPrecios { get; set; }
        public virtual DbSet<BloqueoSillaslol> BloqueoSillaslol { get; set; }
        public virtual DbSet<BoletaReservada> BoletaReservada { get; set; }
        public virtual DbSet<Reserva> Reserva { get; set; }
        public virtual DbSet<TVShowTrailers> TVShowTrailers { get; set; }
    
        public virtual int Eliminar_sillas_sala(Nullable<int> rowIDSala)
        {
            var rowIDSalaParameter = rowIDSala.HasValue ?
                new ObjectParameter("RowIDSala", rowIDSala) :
                new ObjectParameter("RowIDSala", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Eliminar_sillas_sala", rowIDSalaParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual int Eliminar_Sillas_Sala_sp(Nullable<int> rowIDSala)
        {
            var rowIDSalaParameter = rowIDSala.HasValue ?
                new ObjectParameter("RowIDSala", rowIDSala) :
                new ObjectParameter("RowIDSala", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Eliminar_Sillas_Sala_sp", rowIDSalaParameter);
        }
    
        public virtual ObjectResult<ValoreCierreCaja_Result> ValoreCierreCaja()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ValoreCierreCaja_Result>("ValoreCierreCaja");
        }
    
        public virtual ObjectResult<BloqueoSillas_Result> BloqueoSillas()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<BloqueoSillas_Result>("BloqueoSillas");
        }
    
        public virtual int Sp_Integracion_Crear_Tercero(string tipoTercero, string identificacion)
        {
            var tipoTerceroParameter = tipoTercero != null ?
                new ObjectParameter("TipoTercero", tipoTercero) :
                new ObjectParameter("TipoTercero", typeof(string));
    
            var identificacionParameter = identificacion != null ?
                new ObjectParameter("Identificacion", identificacion) :
                new ObjectParameter("Identificacion", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Sp_Integracion_Crear_Tercero", tipoTerceroParameter, identificacionParameter);
        }
    
        public virtual ObjectResult<VerMapaVenta_Result1> VerMapaVenta(Nullable<int> funcion)
        {
            var funcionParameter = funcion.HasValue ?
                new ObjectParameter("funcion", funcion) :
                new ObjectParameter("funcion", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VerMapaVenta_Result1>("VerMapaVenta", funcionParameter);
        }
    
        public virtual ObjectResult<EstadisticaFuncion_Result> EstadisticaFuncion(Nullable<int> rowIDFuncion)
        {
            var rowIDFuncionParameter = rowIDFuncion.HasValue ?
                new ObjectParameter("RowIDFuncion", rowIDFuncion) :
                new ObjectParameter("RowIDFuncion", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<EstadisticaFuncion_Result>("EstadisticaFuncion", rowIDFuncionParameter);
        }
    }
}
