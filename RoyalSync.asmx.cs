using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;
using CinemaPOS.Models;

namespace CinemaPOS
{
    /// <summary>
    /// Descripción breve de RoyalSync
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class RoyalSync : System.Web.Services.WebService
    {
        CinemaPOSEntities db = new CinemaPOSEntities();
        CinemaPOSEntities dbCentral = new CinemaPOSEntities();
        CinemaPOSEntities dbLocal = new CinemaPOSEntities();
        int cont = 0;

        public void InicializarConexiones(String ConnCentral, String ConnLocal)
        {
            dbCentral.Database.Connection.ConnectionString = db.Parametros.FirstOrDefault(f => f.cod_parametro == ConnCentral).valor_parametros;
            Parametros asjd = db.Parametros.FirstOrDefault(f => f.cod_parametro == ConnLocal);
            dbLocal.Database.Connection.ConnectionString = asjd.valor_parametros;
        }

        #region Usuarios

        [WebMethod]
        public void SincronizarUsuariosSistema()
        {
            InicializarConexiones("CONNCENTRAL", "CONNLOCAL");
            List<UsuarioSistema> UsuariosCentral = dbCentral.UsuarioSistema.Where(f => f.Sincronizado == false).ToList();

            foreach (UsuarioSistema item in UsuariosCentral)
            {
                UsuarioSistema usuarioReferencia = new UsuarioSistema();
                //Valido si existe el usuario
                usuarioReferencia = dbLocal.UsuarioSistema.FirstOrDefault(f => f.RowIDCentral == item.RowID);
                //Si ya existe lo Actualizo
                if (usuarioReferencia != null)
                {
                    usuarioReferencia = CargarDatos(usuarioReferencia, item);
                    dbLocal.SaveChanges();
                    GuardarHistorico(item.RowID, "UsuarioSistema", usuarioReferencia.RowID, "UsuarioSistema", "Actualizacion" );
                    //Actualizo el central
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();
                    usuarioReferencia = null;
                    usuarioReferencia = new UsuarioSistema();
                }
                else//Si no existe lo creo
                {
                    usuarioReferencia = new UsuarioSistema();
                    usuarioReferencia = CargarDatos(usuarioReferencia, item);
                    usuarioReferencia.RowIDCentral = item.RowID;
                    dbLocal.UsuarioSistema.Add(usuarioReferencia);
                    dbLocal.SaveChanges();
                    GuardarHistorico(item.RowID, "UsuarioSistema", usuarioReferencia.RowID, "UsuarioSistema", "Creación");
                    //Actualizo el central
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();        
                }
            }
        }

        public UsuarioSistema CargarDatos(UsuarioSistema usuarioReferencia, UsuarioSistema UsuarioCentral)
        {
            usuarioReferencia.RowID = usuarioReferencia.RowID;
            usuarioReferencia.RolID = UsuarioCentral.RolID;
            usuarioReferencia.Nombre = UsuarioCentral.Nombre;
            usuarioReferencia.Apellido = UsuarioCentral.Apellido;
            usuarioReferencia.Contrasena = UsuarioCentral.Contrasena;
            usuarioReferencia.NombreUsuario = UsuarioCentral.NombreUsuario;
            usuarioReferencia.Telefono = UsuarioCentral.Telefono;
            usuarioReferencia.Correo = UsuarioCentral.Correo;
            usuarioReferencia.Activo = UsuarioCentral.Activo;
            usuarioReferencia.TeatroID = UsuarioCentral.TeatroID;
            usuarioReferencia.Sincronizado = true;
            usuarioReferencia.CreadoPor = UsuarioCentral.CreadoPor;
            usuarioReferencia.FechaCreacion = UsuarioCentral.FechaCreacion;

            return usuarioReferencia;
        }

        #endregion

        #region Sala 

        public void SincronizarSalasSistema()
        {
            InicializarConexiones("CONNCENTRAL", "CONNLOCAL");
            List<Sala> SalaCentral = dbCentral.Sala.Where(f => f.Sincronizado == false).ToList();
            if (SalaCentral.Count > 0)
            {

                Int32 rowid = SalaCentral[cont].RowID;
                Sala Salacentral = dbCentral.Sala.FirstOrDefault(f => f.RowID == rowid);

                Sala SalaReferencia = dbLocal.Sala.FirstOrDefault(f => f.RowIDCentral == rowid);
                cont = cont + 1;
                //Si ya existe lo Actualizo
                if (SalaReferencia != null)
                {

                    SalaReferencia.Cantidad_Columnas = Salacentral.Cantidad_Columnas;
                    SalaReferencia.Cantidad_Filas = Salacentral.Cantidad_Filas;
                    SalaReferencia.Capacidad = Salacentral.Capacidad;
                    SalaReferencia.CreadoPor = Salacentral.CreadoPor;
                    SalaReferencia.FechaCreacion = Salacentral.FechaCreacion;
                    SalaReferencia.Nombre = Salacentral.Nombre;
                    SalaReferencia.Sincronizado = true;
                    SalaReferencia.ModificadoPor = Session["usuario_creacion"].ToString();
                    SalaReferencia.FechaModificacion = DateTime.Now;
                    try
                    {
                        dbLocal.SaveChanges();
                    }
                    catch { SincronizarSalasSistema(); }
                    GuardarHistorico(Salacentral.RowID, "Sala", SalaReferencia.RowID, "Sala", "Actualizacion");
                    //Actualizo el central
                    Salacentral.Sincronizado = true;
                    dbCentral.SaveChanges();
                }
                else//Si no existe lo creo
                {
                    SalaReferencia = new Sala();
                    SalaReferencia.Cantidad_Columnas = Salacentral.Cantidad_Columnas;
                    SalaReferencia.Cantidad_Filas = Salacentral.Cantidad_Filas;
                    SalaReferencia.Capacidad = Salacentral.Capacidad;
                    SalaReferencia.CreadoPor = Salacentral.CreadoPor;
                    SalaReferencia.EstadoID = Salacentral.EstadoID;
                    SalaReferencia.FechaCreacion = Salacentral.FechaCreacion;
                    SalaReferencia.Nombre = Salacentral.Nombre;
                    SalaReferencia.RowIDCentral = Salacentral.RowID;
                    SalaReferencia.Sincronizado = true;
                    SalaReferencia.TeatroID = Salacentral.TeatroID;
                    SalaReferencia.TipoAudioID = Salacentral.TipoAudioID;
                    dbLocal.Sala.Add(SalaReferencia);
                    dbLocal.SaveChanges();
                    GuardarHistorico(Salacentral.RowID, "Sala", SalaReferencia.RowID, "Sala", "Creacion");
                    Salacentral.Sincronizado = true;
                    dbCentral.SaveChanges();
                }
                if (cont == SalaCentral.Count)
                {
                    return;
                }
                else
                {
                    cont = cont - 1;
                    SincronizarSalasSistema();
                }
            }
        }

        #endregion

        #region Teatro
        public void SincronizarTeatros()
        {
            InicializarConexiones("CONNCENTRAL", "CONNLOCAL");

            List<Teatro> teatroscentral = dbCentral.Teatro.Where(f => f.Sincronizado == false).ToList();
            foreach (Teatro item in teatroscentral)
            {
                Teatro sincronizacion = new Teatro();
                sincronizacion = dbLocal.Teatro.FirstOrDefault(f => f.RowIDCentral == item.RowID);

                cont = cont + 1;

                if (sincronizacion != null)
                {
                    sincronizacion = Cargardatosteatro(sincronizacion, item);
                    try
                    {
                        dbLocal.SaveChanges();
                    }
                    catch
                    {
                        return;
                    }

                    GuardarHistorico(item.RowID, "teatros", sincronizacion.RowID, "teatros", "Actualizacion");
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();
                    sincronizacion = null;

                    sincronizacion = new Teatro();
                }
                else
                {
                    sincronizacion = new Teatro();
                    sincronizacion = Cargardatosteatro(sincronizacion, item);
                    sincronizacion.RowIDCentral = item.RowID;
                    dbLocal.Teatro.Add(sincronizacion);
                    dbLocal.SaveChanges();
                    GuardarHistorico(item.RowID, "teatros", sincronizacion.RowID, "teatros", "Creación");
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();
                }

            }
        }
        public Teatro Cargardatosteatro(Teatro teatrosReferencia, Teatro teatroscentral)
        {

            teatrosReferencia.CompaniaID = teatroscentral.CompaniaID;
            teatrosReferencia.CentroOperacion = teatroscentral.CentroOperacion;
            teatrosReferencia.IP = teatroscentral.IP;
            teatrosReferencia.Nombre = teatroscentral.Nombre;
            teatrosReferencia.CiudadID = teatroscentral.CiudadID;
            teatrosReferencia.CreadoPor = teatroscentral.CreadoPor;
            teatrosReferencia.FechaCreacion = teatroscentral.FechaCreacion;
            teatrosReferencia.FechaModificacion = teatroscentral.FechaModificacion;
            teatrosReferencia.ModificadoPor = teatroscentral.ModificadoPor;
            teatrosReferencia.EstadoID = teatroscentral.EstadoID;
            teatrosReferencia.Sincronizado = true;
            return teatrosReferencia;
        }
        #endregion

        #region Terceros

        public void SincronizarTerceros()
        {
            InicializarConexiones("CONNCENTRAL", "CONNLOCAL");
            List<Tercero> sincronizacionterceros = dbCentral.Tercero.Where(f => f.Sincronizado == false).ToList();

            foreach (Tercero item in sincronizacionterceros)
            {
                Tercero Terceros = new Tercero();
                //Valido si existe el usuario
                Terceros = dbLocal.Tercero.FirstOrDefault(f => f.RowIDCentral == item.RowID);
                //Si ya existe lo Actualizo
                if (Terceros != null)
                {
                    Terceros = CargarDatosTerceros(Terceros, item);
                    dbLocal.SaveChanges();
                    GuardarHistorico(item.RowID, "Terceros", Terceros.RowID, "Terceros", "Actualizacion");
                    //Actualizo el central
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();
                    Terceros = null;
                    Terceros = new Tercero();
                }
                else//Si no existe lo creo
                {
                    Terceros = new Tercero();
                    Terceros = CargarDatosTerceros(Terceros, item);
                    Terceros.RowIDCentral = item.RowID;
                    dbLocal.Tercero.Add(Terceros);
                    try
                    {
                        dbLocal.SaveChanges();
                    }
                    catch { return; }
                    
                    GuardarHistorico(item.RowID, "Terceros", Terceros.RowID, "Terceros", "Creación");
                    //Actualizo el central
                    item.Sincronizado = true;
                    dbCentral.SaveChanges();
                }
            }
        }

        public Tercero CargarDatosTerceros(Tercero TerceroReferencia, Tercero TerceroCentral)
        {
            TerceroReferencia.RowID = TerceroReferencia.RowID;
            TerceroReferencia.TipoTerceroID = TerceroCentral.TipoTerceroID;
            TerceroReferencia.Identificacion = TerceroCentral.Identificacion;
            TerceroReferencia.Nombre = TerceroCentral.Nombre;
            TerceroReferencia.Apellidos = TerceroCentral.Apellidos;
            TerceroReferencia.Telefono = TerceroCentral.Telefono;
            TerceroReferencia.CiudadID = TerceroCentral.CiudadID;
            TerceroReferencia.Descripcion = TerceroCentral.Descripcion;
            TerceroReferencia.Direccion = TerceroCentral.Direccion;
            TerceroReferencia.Correo = TerceroCentral.Correo;
            TerceroReferencia.Activo = TerceroCentral.Activo;
            TerceroReferencia.CreadoPor = TerceroCentral.CreadoPor;
            TerceroReferencia.FechaCreacion = TerceroCentral.FechaCreacion;
            TerceroReferencia.ModificadoPor = TerceroCentral.ModificadoPor;
            TerceroReferencia.FechaModificacion = TerceroCentral.FechaModificacion;
            TerceroReferencia.Sincronizado = true;
            TerceroReferencia.TipoIdentificacionID = TerceroCentral.TipoIdentificacionID;
            TerceroReferencia.FechaNacimiento = TerceroCentral.FechaNacimiento;
            TerceroReferencia.SexoID = TerceroCentral.SexoID;

            return TerceroReferencia;
        }

        public void GuardarHistorico(int RowIDCentral, String EntidadCentral, int RowIDLocal, String EntidadLocal, String Descripcion)
        {
            SincronizacionMaestros Historico = new SincronizacionMaestros();
            Historico.RowIDCentral = RowIDCentral;
            Historico.EntidadCentral = EntidadCentral;
            Historico.RowIDLocal = RowIDLocal;
            Historico.EntidadLocal = EntidadLocal;
            Historico.SincronizadoPor = Session["usuario_creacion"].ToString();
            Historico.FechaSincronizacion = DateTime.Now;
            //Historico.Descripcion = Descripcion;
            dbLocal.SincronizacionMaestros.Add(Historico);
            dbLocal.SaveChanges();
            dbCentral.SincronizacionMaestros.Add(Historico);
            dbCentral.SaveChanges();
        }
    }
        #endregion

       

}

