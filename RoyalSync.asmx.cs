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
}
