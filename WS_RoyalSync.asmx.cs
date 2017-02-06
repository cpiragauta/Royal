using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CinemaPOS.Models;

namespace CinemaPOS
{
    /// <summary>
    /// Descripción breve de WS_RoyalSync
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class WS_RoyalSinc : System.Web.Services.WebService
    {
        CinemaPOSEntities db = new CinemaPOSEntities();
        CinemaPOSEntities dbCentral = new CinemaPOSEntities();
        CinemaPOSEntities dbLocal = new CinemaPOSEntities();


        public string ValidarConexion(int RowID)
        {
            String respuestaConexion = "";
            try
            {
                dbLocal.Database.Connection.ConnectionString = db.Teatro.FirstOrDefault(f => f.RowID == RowID).CadenaBD;
                List<Taquilla> taquillas = dbLocal.Taquilla.ToList();
                respuestaConexion = "Conexion Exitosa";
            }
            catch
            {
                respuestaConexion = "";
            }
            return respuestaConexion;
        }

        public void InicializarConexiones(String ConnCentral, Int32? RowID)
        {
            //
            dbCentral.Database.Connection.ConnectionString = db.Teatro.FirstOrDefault(f => f.Nombre == ConnCentral).CadenaBD;
            dbLocal.Database.Connection.ConnectionString = db.Teatro.FirstOrDefault(f => f.RowID == RowID).CadenaBD;
        }

        public void SincronizarOpciones(Int32? RowID)
        {
            SincronizarTipo(RowID);
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "OPCION" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Opcion> Opcionsave = new List<Opcion>();
                List<Opcion> OpcionUpdate = new List<Opcion>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Opcionsave = dbCentral.Opcion.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    OpcionUpdate = dbCentral.Opcion.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Opcionsave = dbCentral.Opcion.ToList();
                }
                foreach (Opcion Item in Opcionsave)
                {
                    Opcion ObjOpcion = new Opcion();
                    ObjOpcion = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjOpcion == null)
                    {
                        ObjOpcion = new Opcion();
                        ObjOpcion.Activo = Item.Activo;
                        ObjOpcion.Codigo = Item.Codigo;
                        ObjOpcion.Codigo2 = Item.Codigo2;
                        ObjOpcion.Nombre = Item.Nombre;
                        ObjOpcion.ValorDefecto = Item.ValorDefecto;
                        ObjOpcion.Descripcion = Item.Descripcion;
                        ObjOpcion.NumOrden = Item.NumOrden;
                        Tipo Tipolocal = dbLocal.Tipo.Where(f => f.RowIDCreacion == Item.TipoID).FirstOrDefault();
                        ObjOpcion.TipoID = Tipolocal.RowID;
                        ObjOpcion.FechaCreacion = DateTime.Now;
                        ObjOpcion.RowIDCreacion = Item.RowID;
                        dbLocal.Opcion.Add(ObjOpcion);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Opcion Item in OpcionUpdate)
                {
                    Opcion ObjOpcion = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjOpcion != null)
                    {
                        ObjOpcion.Activo = Item.Activo;
                        ObjOpcion.Codigo = Item.Codigo;
                        ObjOpcion.Codigo2 = Item.Codigo2;
                        ObjOpcion.Nombre = Item.Nombre;
                        ObjOpcion.ValorDefecto = Item.ValorDefecto;
                        ObjOpcion.Descripcion = Item.Descripcion;
                        ObjOpcion.NumOrden = Item.NumOrden;
                        Tipo Tipolocal = dbLocal.Tipo.Where(f => f.RowIDCreacion == Item.TipoID).FirstOrDefault();
                        ObjOpcion.TipoID = Tipolocal.RowID;
                        ObjOpcion.FechaModificacion = DateTime.Now;
                        ObjOpcion.RowIDCreacion = Item.RowID;
                        dbLocal.Opcion.Add(ObjOpcion);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }

                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "OPCION";
            dbLocal.HistoricolLog.Add(LOG);
            LOG.Registros = Sincronizados;
            dbLocal.SaveChanges();
        }
        public void SincronizarTipo(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TIPO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Tipo> Tiposave = new List<Tipo>();
                List<Tipo> TipoUpdate = new List<Tipo>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Tiposave = dbCentral.Tipo.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    TipoUpdate = dbCentral.Tipo.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Tiposave = dbCentral.Tipo.ToList();
                }
                foreach (Tipo Item in Tiposave)
                {
                    Tipo ObjTipo = dbLocal.Tipo.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTipo == null)
                    {
                        ObjTipo = new Tipo();
                        ObjTipo.Codigo = Item.Codigo;
                        ObjTipo.Nombre = Item.Nombre;
                        ObjTipo.FechaCreacion = DateTime.Now;
                        ObjTipo.RowIDCreacion = Item.RowID;
                        dbLocal.Tipo.Add(ObjTipo);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Tipo Item in TipoUpdate)
                {
                    Tipo ObjTipo = dbLocal.Tipo.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTipo != null)
                    {
                        ObjTipo.Codigo = Item.Codigo;
                        ObjTipo.Nombre = Item.Nombre;
                        ObjTipo.FechaModificacion = DateTime.Now;
                        ObjTipo.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TIPO";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarEstado(Int32? RowID)
        {
            SincronizarTipoEstado(RowID);
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "ESTADO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Estado> Estadosave = new List<Estado>();
                List<Estado> EstadoUpdate = new List<Estado>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Estadosave = dbCentral.Estado.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    EstadoUpdate = dbCentral.Estado.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Estadosave = dbCentral.Estado.ToList();
                }
                foreach (Estado Item in Estadosave)
                {
                    Estado ObjOpcion = new Estado();
                    ObjOpcion = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjOpcion == null)
                    {
                        ObjOpcion = new Estado();
                        ObjOpcion.Codigo = Item.Codigo;
                        ObjOpcion.Descripcion = Item.Descripcion;
                        ObjOpcion.FechaCreacion = DateTime.Now;
                        ObjOpcion.Nombre = Item.Nombre;
                        ObjOpcion.RowIDCreacion = Item.RowID;
                        TipoEstado tipoE = dbLocal.TipoEstado.Where(f => f.RowIDCreacion == Item.TipoEstadoID).FirstOrDefault();
                        ObjOpcion.TipoEstadoID = tipoE.RowID;
                        dbLocal.Estado.Add(ObjOpcion);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Estado Item in EstadoUpdate)
                {
                    Estado ObjOpcion = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjOpcion != null)
                    {
                        ObjOpcion.Codigo = Item.Codigo;
                        ObjOpcion.Descripcion = Item.Descripcion;
                        ObjOpcion.FechaModificacion = DateTime.Now;
                        ObjOpcion.Nombre = Item.Nombre;
                        ObjOpcion.RowIDCreacion = Item.RowID;
                        TipoEstado tipoE = dbLocal.TipoEstado.Where(f => f.RowIDCreacion == Item.TipoEstadoID).FirstOrDefault();
                        ObjOpcion.TipoEstadoID = tipoE.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "ESTADO";
            dbLocal.HistoricolLog.Add(LOG);
            LOG.Registros = Sincronizados;
            dbLocal.SaveChanges();
        }
        public void SincronizarTipoEstado(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TIPOESTADO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<TipoEstado> TipoEstadosave = new List<TipoEstado>();
                List<TipoEstado> TipoEstadoUpdate = new List<TipoEstado>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    TipoEstadosave = dbCentral.TipoEstado.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    TipoEstadoUpdate = dbCentral.TipoEstado.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    TipoEstadosave = dbCentral.TipoEstado.ToList();
                }
                foreach (TipoEstado Item in TipoEstadosave)
                {
                    TipoEstado ObjTipo = new TipoEstado();
                    ObjTipo = dbLocal.TipoEstado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTipo == null)
                    {
                        ObjTipo = new TipoEstado();
                        ObjTipo.Codigo = Item.Codigo;
                        ObjTipo.Nombre = Item.Nombre;
                        ObjTipo.FechaCreacion = DateTime.Now;
                        ObjTipo.RowIDCreacion = Item.RowID;
                        dbLocal.TipoEstado.Add(ObjTipo);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (TipoEstado Item in TipoEstadoUpdate)
                {
                    TipoEstado ObjTipo = dbLocal.TipoEstado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTipo != null)
                    {
                        ObjTipo.Codigo = Item.Codigo;
                        ObjTipo.Nombre = Item.Nombre;
                        ObjTipo.FechaModificacion = DateTime.Now;
                        ObjTipo.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TIPOESTADO";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarPais(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "PAIS" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Pais> PaisSave = new List<Pais>();
                List<Pais> PaisUpdate = new List<Pais>();

                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    PaisSave = dbCentral.Pais.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    PaisUpdate = dbCentral.Pais.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();

                }
                else
                {
                    PaisSave = dbCentral.Pais.ToList();
                }
                foreach (Pais Item in PaisSave)
                {
                    Pais ObjPais = new Pais();
                    ObjPais = dbLocal.Pais.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjPais == null)
                    {
                        ObjPais = new Pais();
                        ObjPais.Nombre = Item.Nombre;
                        ObjPais.Descripcion = Item.Descripcion;
                        ObjPais.FechaCreacion = DateTime.Now;
                        ObjPais.RowIDCreacion = Item.RowID;
                        ObjPais.Descripcion = Item.Descripcion;
                        ObjPais.ErpID = Item.ErpID;
                        dbLocal.Pais.Add(ObjPais);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Pais Item in PaisUpdate)
                {
                    Pais ObjPais = dbLocal.Pais.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjPais != null)
                    {
                        ObjPais.Nombre = Item.Nombre;
                        ObjPais.Descripcion = Item.Descripcion;
                        ObjPais.FechaModificacion = DateTime.Now;
                        ObjPais.RowIDCreacion = Item.RowID;
                        ObjPais.Descripcion = Item.Descripcion;
                        ObjPais.ErpID = Item.ErpID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";
            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "PAIS";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarDepartamento(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "DEPARTAMENTO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Departamento> DepartamentoSave = new List<Departamento>();
                List<Departamento> DepartamentoUpdate = new List<Departamento>();

                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    DepartamentoSave = dbCentral.Departamento.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    DepartamentoUpdate = dbCentral.Departamento.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();

                }
                else
                {
                    DepartamentoSave = dbCentral.Departamento.ToList();
                }
                foreach (Departamento Item in DepartamentoSave)
                {
                    Departamento ObjDepartamento = new Departamento();
                    ObjDepartamento = dbLocal.Departamento.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjDepartamento == null)
                    {
                        ObjDepartamento = new Departamento();
                        Pais ps = dbLocal.Pais.Where(f => f.RowIDCreacion == Item.PaisID).FirstOrDefault();
                        ObjDepartamento.PaisID = ps.RowID;
                        ObjDepartamento.Nombre = Item.Nombre;
                        ObjDepartamento.Descripcion = Item.Descripcion;
                        ObjDepartamento.FechaCreacion = DateTime.Now;
                        ObjDepartamento.RowIDCreacion = Item.RowID;
                        ObjDepartamento.ErpID = Item.ErpID;
                        dbLocal.Departamento.Add(ObjDepartamento);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Departamento Item in DepartamentoUpdate)
                {
                    Departamento ObjDepartamento = dbLocal.Departamento.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjDepartamento != null)
                    {
                        Pais ps = dbLocal.Pais.Where(f => f.RowIDCreacion == Item.PaisID).FirstOrDefault();
                        ObjDepartamento.PaisID = ps.RowID;
                        ObjDepartamento.Nombre = Item.Nombre;
                        ObjDepartamento.Descripcion = Item.Descripcion;
                        ObjDepartamento.FechaModificacion = DateTime.Now;
                        ObjDepartamento.RowIDCreacion = Item.RowID;
                        ObjDepartamento.ErpID = Item.ErpID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "DEPARTAMENTO";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarCiudad(Int32? RowID)
        {
            SincronizarPais(RowID);
            SincronizarDepartamento(RowID);
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "CIUDAD" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Ciudad> CiudadSave = new List<Ciudad>();
                List<Ciudad> CiudadUpdate = new List<Ciudad>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    CiudadSave = dbCentral.Ciudad.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    CiudadUpdate = dbCentral.Ciudad.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    CiudadSave = dbCentral.Ciudad.ToList();
                }
                foreach (Ciudad Item in CiudadSave)
                {
                    Ciudad ObjCiudad = new Ciudad();
                    ObjCiudad = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjCiudad == null)
                    {
                        ObjCiudad = new Ciudad();
                        Departamento depto = dbLocal.Departamento.Where(f => f.RowIDCreacion == Item.DepartamentoID).FirstOrDefault();
                        ObjCiudad.DepartamentoID = depto.RowID;
                        ObjCiudad.Descripcion = Item.Descripcion;
                        ObjCiudad.Nombre = Item.Nombre;
                        ObjCiudad.FechaCreacion = DateTime.Now;
                        ObjCiudad.RowIDCreacion = Item.RowID;
                        ObjCiudad.ErpID = Item.ErpID;
                        dbLocal.Ciudad.Add(ObjCiudad);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Ciudad Item in CiudadUpdate)
                {
                    Ciudad ObjCiudad = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjCiudad != null)
                    {
                        Departamento depto = dbLocal.Departamento.Where(f => f.RowIDCreacion == Item.DepartamentoID).FirstOrDefault();
                        ObjCiudad.DepartamentoID = depto.RowID;
                        ObjCiudad.Descripcion = Item.Descripcion;
                        ObjCiudad.Nombre = Item.Nombre;
                        ObjCiudad.FechaModificacion = DateTime.Now;
                        ObjCiudad.RowIDCreacion = Item.RowID;
                        ObjCiudad.ErpID = Item.ErpID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "CIUDAD";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarTerceros(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TERCERO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Tercero> TerceroSave = new List<Tercero>();
                List<Tercero> TerceroUpdate = new List<Tercero>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    TerceroSave = dbCentral.Tercero.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    TerceroUpdate = dbCentral.Tercero.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    TerceroSave = dbCentral.Tercero.ToList();
                }
                foreach (Tercero Item in TerceroSave)
                {
                    Tercero ObjTercero = new Tercero();
                    ObjTercero = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTercero == null)
                    {
                        ObjTercero = new Tercero();
                        ObjTercero.Activo = Item.Activo;
                        ObjTercero.Apellidos = Item.Apellidos;
                        Ciudad ciud = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.CiudadID).FirstOrDefault();
                        ObjTercero.CiudadID = ciud.RowID;
                        ObjTercero.Correo = Item.Correo;
                        ObjTercero.CreadoPor = Item.CreadoPor;
                        ObjTercero.Descripcion = Item.Descripcion;
                        ObjTercero.FechaCreacion = DateTime.Now;
                        ObjTercero.FechaNacimiento = Item.FechaNacimiento;
                        ObjTercero.Identificacion = Item.Identificacion;
                        ObjTercero.Nombre = Item.Nombre;
                        ObjTercero.ind_accionista = Item.ind_accionista;
                        ObjTercero.ind_cliente = Item.ind_cliente;
                        ObjTercero.ind_empleado = Item.ind_empleado;
                        ObjTercero.ind_interno = Item.ind_interno;
                        ObjTercero.ind_otros = Item.ind_otros;
                        ObjTercero.ind_proveedor = Item.ind_proveedor;
                        ObjTercero.RowIDCreacion = Item.RowID;
                        if (Item.SexoID != null)
                        {
                            Opcion sex = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.SexoID).FirstOrDefault();
                            ObjTercero.SexoID = Item.SexoID;
                        }
                        if (Item.TipoIdentificacionID != null)
                        {
                            Opcion tipI = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoIdentificacionID).FirstOrDefault();
                            ObjTercero.TipoIdentificacionID = tipI.RowID;
                        }
                        if (Item.TipoTerceroID != null)
                        {
                            Opcion tipT = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoTerceroID).FirstOrDefault();
                            ObjTercero.TipoTerceroID = tipT.RowID;
                        }
                        dbLocal.Tercero.Add(ObjTercero);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Tercero Item in TerceroUpdate)
                {
                    Tercero ObjTercero = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTercero.FechaModificacion.Value < Item.FechaModificacion.Value)
                    {
                        ObjTercero.Activo = Item.Activo;
                        ObjTercero.Apellidos = Item.Apellidos;
                        Ciudad ciud = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.CiudadID).FirstOrDefault();
                        ObjTercero.CiudadID = ciud.RowID;
                        ObjTercero.Correo = Item.Correo;
                        ObjTercero.CreadoPor = Item.CreadoPor;
                        ObjTercero.Descripcion = Item.Descripcion;
                        ObjTercero.FechaModificacion = DateTime.Now;
                        ObjTercero.FechaNacimiento = Item.FechaNacimiento;
                        ObjTercero.Identificacion = Item.Identificacion;
                        ObjTercero.Nombre = Item.Nombre;
                        ObjTercero.ind_accionista = Item.ind_accionista;
                        ObjTercero.ind_cliente = Item.ind_cliente;
                        ObjTercero.ind_empleado = Item.ind_empleado;
                        ObjTercero.ind_interno = Item.ind_interno;
                        ObjTercero.ind_otros = Item.ind_otros;
                        ObjTercero.ind_proveedor = Item.ind_proveedor;
                        ObjTercero.RowIDCreacion = Item.RowID;
                        if (Item.SexoID != null)
                        {
                            Opcion sex = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.SexoID).FirstOrDefault();
                            ObjTercero.SexoID = Item.SexoID;
                        }
                        if (Item.TipoIdentificacionID != null)
                        {
                            Opcion tipI = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoIdentificacionID).FirstOrDefault();
                            ObjTercero.TipoIdentificacionID = tipI.RowID;
                        }
                        if (Item.TipoTerceroID != null)
                        {
                            Opcion tipT = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoTerceroID).FirstOrDefault();
                            ObjTercero.TipoTerceroID = tipT.RowID;
                        }
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";
            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            dbLocal.HistoricolLog.Add(LOG);
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TERCERO";
            LOG.Registros = Sincronizados;
            dbLocal.SaveChanges();
            SincronizarContactos(RowID);
        }
        public void SincronizarTeatros(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                SincronizarCentroOperacion(RowID);
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TEATROS" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Teatro> TeatroSave = new List<Teatro>();
                List<Teatro> TeatroUpdate = new List<Teatro>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    TeatroSave = dbCentral.Teatro.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    TeatroUpdate = dbCentral.Teatro.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    TeatroSave = dbCentral.Teatro.ToList();
                }
                foreach (Teatro Item in TeatroSave)
                {
                    Teatro objTeatro = new Teatro();
                    objTeatro = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objTeatro == null)
                    {
                        objTeatro = new Teatro();
                        objTeatro.CadenaBD = Item.CadenaBD;
                        if (Item.CentroOperacion != null)
                        {
                            CentroOperacion co = dbLocal.CentroOperacion.Where(f => f.RowIDCreacion == Item.CentroOperacionID).FirstOrDefault();
                            objTeatro.CentroOperacionID = co.RowID;
                        }
                        objTeatro.CreadoPor = Item.CreadoPor;
                        Ciudad CD = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.CiudadID).FirstOrDefault();
                        objTeatro.CiudadID = CD.RowID;
                        if (objTeatro.CompaniaID != null)
                        {

                            Tercero ter = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.CompaniaID).FirstOrDefault();
                            objTeatro.CompaniaID = ter.RowID;
                        }
                        Estado est = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        objTeatro.EstadoID = est.RowID;
                        objTeatro.IP = Item.IP;
                        objTeatro.FechaCreacion = DateTime.Now;
                        objTeatro.Nombre = Item.Nombre;
                        objTeatro.RowIDCreacion = Item.RowID;
                        dbLocal.Teatro.Add(objTeatro);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Teatro Item in TeatroUpdate)
                {
                    Teatro objTeatro = new Teatro();
                    objTeatro = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objTeatro != null)
                    {
                        objTeatro.CadenaBD = Item.CadenaBD;
                        if (Item.CentroOperacion != null)
                        {
                            CentroOperacion co = dbLocal.CentroOperacion.Where(f => f.RowIDCreacion == Item.CentroOperacionID).FirstOrDefault();
                            objTeatro.CentroOperacionID = co.RowID;
                        }
                        Ciudad CD = dbLocal.Ciudad.Where(f => f.RowIDCreacion == Item.CiudadID).FirstOrDefault();
                        objTeatro.CiudadID = CD.RowID;
                        Tercero ter = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.CompaniaID).FirstOrDefault();
                        objTeatro.CompaniaID = ter.RowID;
                        Estado est = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        objTeatro.EstadoID = est.RowID;
                        objTeatro.IP = Item.IP;
                        objTeatro.FechaModificacion = DateTime.Now;
                        objTeatro.Nombre = Item.Nombre;
                        objTeatro.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            SincronizarTaquilla(RowID);
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TEATROS";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarCentroOperacion(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "CENTROOPERACION" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<CentroOperacion> CentroOperacionSave = new List<CentroOperacion>();
                List<CentroOperacion> CentroOperacionUpdate = new List<CentroOperacion>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    CentroOperacionSave = dbCentral.CentroOperacion.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    CentroOperacionUpdate = dbCentral.CentroOperacion.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    CentroOperacionSave = dbCentral.CentroOperacion.ToList();
                }
                foreach (CentroOperacion Item in CentroOperacionSave)
                {
                    CentroOperacion objCentroOperacion = new CentroOperacion();
                    objCentroOperacion = dbLocal.CentroOperacion.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objCentroOperacion == null)
                    {
                        objCentroOperacion = new CentroOperacion();
                        objCentroOperacion.CompaniaID = Item.CompaniaID;
                        objCentroOperacion.FechaCreacion = DateTime.Now;
                        objCentroOperacion.Nombre = Item.Nombre;
                        objCentroOperacion.Region = Item.Region;
                        objCentroOperacion.RowIDCreacion = Item.RowID;
                        dbLocal.CentroOperacion.Add(objCentroOperacion);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (CentroOperacion Item in CentroOperacionUpdate)
                {
                    CentroOperacion objCentroOperacion = new CentroOperacion();
                    objCentroOperacion = dbLocal.CentroOperacion.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objCentroOperacion != null)
                    {
                        objCentroOperacion.CompaniaID = Item.CompaniaID;
                        objCentroOperacion.FechaCreacion = DateTime.Now;
                        objCentroOperacion.Nombre = Item.Nombre;
                        objCentroOperacion.Region = Item.Region;
                        objCentroOperacion.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "CENTROOPERACION";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarUsuarios(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                SincronizarRol(RowID);
                SincronizarTipoMenu(RowID);
                SincronizarMenu(RowID);
                SincronizarRolMenu(RowID);
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "USUARIOS" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<UsuarioSistema> UsuarioSave = new List<UsuarioSistema>();
                List<UsuarioSistema> UsuarioUpdate = new List<UsuarioSistema>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    UsuarioSave = dbCentral.UsuarioSistema.Where(f => f.FechaCreacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                    UsuarioUpdate = dbCentral.UsuarioSistema.Where(f => f.FechaModificacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                }
                else
                {
                    UsuarioSave = dbCentral.UsuarioSistema.Where(f => f.TeatroID == RowID).ToList();
                }
                foreach (UsuarioSistema Item in UsuarioSave)
                {
                    UsuarioSistema objUsuario = new UsuarioSistema();
                    objUsuario = dbLocal.UsuarioSistema.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objUsuario == null)
                    {
                        objUsuario = new UsuarioSistema();
                        objUsuario.Activo = Item.Activo;
                        objUsuario.Apellido = Item.Apellido;
                        objUsuario.Contrasena = Item.Contrasena;
                        objUsuario.Correo = Item.Correo;
                        objUsuario.FechaCreacion = DateTime.Now;
                        objUsuario.Foto_Empleado = Item.Foto_Empleado;
                        objUsuario.Nombre = Item.Nombre;
                        objUsuario.NombreUsuario = Item.NombreUsuario;
                        objUsuario.RowIDCreacion = Item.RowID;
                        Rol rl = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RolID).FirstOrDefault();
                        objUsuario.RolID = rl.RowID;
                        Teatro te = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                        objUsuario.TeatroID = te.RowID;
                        objUsuario.Telefono = Item.Telefono;
                        objUsuario.CreadoPor = Item.CreadoPor;
                        dbLocal.UsuarioSistema.Add(objUsuario);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (UsuarioSistema Item in UsuarioUpdate)
                {
                    UsuarioSistema objUsuario = new UsuarioSistema();
                    objUsuario = dbLocal.UsuarioSistema.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (objUsuario != null)
                    {
                        objUsuario.Activo = Item.Activo;
                        objUsuario.Apellido = Item.Apellido;
                        objUsuario.Contrasena = Item.Contrasena;
                        objUsuario.Correo = Item.Correo;
                        objUsuario.FechaModificacion = DateTime.Now;
                        objUsuario.Foto_Empleado = Item.Foto_Empleado;
                        objUsuario.Nombre = Item.Nombre;
                        objUsuario.NombreUsuario = Item.NombreUsuario;
                        Rol rl = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RolID).FirstOrDefault();
                        objUsuario.RolID = rl.RowID;
                        objUsuario.RowIDCreacion = Item.RowID;
                        objUsuario.CreadoPor = Item.CreadoPor;
                        Teatro te = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                        objUsuario.TeatroID = te.RowID;
                        objUsuario.Telefono = Item.Telefono;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                    else
                    {
                        objUsuario = new UsuarioSistema();
                        objUsuario = dbLocal.UsuarioSistema.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                        if (objUsuario == null)
                        {
                            objUsuario = new UsuarioSistema();
                            objUsuario.Activo = Item.Activo;
                            objUsuario.Apellido = Item.Apellido;
                            objUsuario.Contrasena = Item.Contrasena;
                            objUsuario.Correo = Item.Correo;
                            objUsuario.FechaCreacion = DateTime.Now;
                            objUsuario.Foto_Empleado = Item.Foto_Empleado;
                            objUsuario.Nombre = Item.Nombre;
                            objUsuario.NombreUsuario = Item.NombreUsuario;
                            objUsuario.RowIDCreacion = Item.RowID;
                            Rol rl = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RolID).FirstOrDefault();
                            objUsuario.RolID = rl.RowID;
                            Teatro te = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                            objUsuario.TeatroID = te.RowID;
                            objUsuario.Telefono = Item.Telefono;
                            objUsuario.CreadoPor = Item.CreadoPor;
                            dbLocal.UsuarioSistema.Add(objUsuario);
                            dbLocal.SaveChanges();
                            Sincronizados = Sincronizados + 1;
                        }
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "USUARIOS";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarRol(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "ROL" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Rol> Rolsave = new List<Rol>();
                List<Rol> RolUpdate = new List<Rol>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Rolsave = dbCentral.Rol.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    RolUpdate = dbCentral.Rol.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Rolsave = dbCentral.Rol.ToList();
                }
                foreach (Rol Item in Rolsave)
                {
                    Rol Objrol = new Rol();
                    Objrol = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (Objrol == null)
                    {
                        Objrol = new Rol();
                        Objrol.Codigo = Item.Codigo;
                        Objrol.Nombre = Item.Nombre;
                        Objrol.RowIDCreacion = Item.RowID;
                        Objrol.FechaCreacion = DateTime.Now;
                        dbLocal.Rol.Add(Objrol);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Rol Item in RolUpdate)
                {
                    Rol Objrol = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (Objrol != null)
                    {
                        Objrol.Codigo = Item.Codigo;
                        Objrol.Nombre = Item.Nombre;
                        Objrol.RowIDCreacion = Item.RowID;
                        Objrol.FechaModificacion = DateTime.Now;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "ROL";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarContactos(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "CONTACTO" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Contacto> Contactosave = new List<Contacto>();
                List<Contacto> ContactoUpdate = new List<Contacto>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Contactosave = dbCentral.Contacto.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    ContactoUpdate = dbCentral.Contacto.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Contactosave = dbCentral.Contacto.ToList();
                }
                foreach (Contacto Item in Contactosave)
                {
                    Contacto ObjContacto = new Contacto();
                    ObjContacto = dbLocal.Contacto.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjContacto == null)
                    {
                        ObjContacto = new Contacto();
                        ObjContacto.Apellido = Item.Apellido;
                        ObjContacto.CorreoElectronico = Item.CorreoElectronico;
                        ObjContacto.EmpresaID = Item.EmpresaID;
                        ObjContacto.FechaCreacion = DateTime.Now;
                        ObjContacto.Identificacion = Item.Identificacion;
                        ObjContacto.RowIDCreacion = Item.RowID;
                        if (Item.EmpresaID != null)
                        {
                            Tercero ter = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.EmpresaID).FirstOrDefault();
                            ObjContacto.EmpresaID = ter.RowID;
                        }
                        ObjContacto.Nombre = Item.Nombre;
                        ObjContacto.RowIDCreacion = Item.RowID;
                        dbLocal.Contacto.Add(ObjContacto);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Contacto Item in ContactoUpdate)
                {
                    Contacto ObjContacto = dbLocal.Contacto.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjContacto != null)
                    {
                        ObjContacto.Apellido = Item.Apellido;
                        ObjContacto.CorreoElectronico = Item.CorreoElectronico;
                        ObjContacto.EmpresaID = Item.EmpresaID;
                        ObjContacto.FechaCreacion = DateTime.Now;
                        ObjContacto.Identificacion = Item.Identificacion;
                        ObjContacto.RowIDCreacion = Item.RowID;
                        if (Item.EmpresaID != null)
                        {
                            Tercero ter = dbLocal.Tercero.Where(f => f.RowIDCreacion == Item.EmpresaID).FirstOrDefault();
                            ObjContacto.EmpresaID = ter.RowID;
                        }
                        ObjContacto.Nombre = Item.Nombre;
                        ObjContacto.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "CONTACTO";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarMenu(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "MENU" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Menu> Menusave = new List<Menu>();
                List<Menu> MenuUpdate = new List<Menu>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Menusave = dbCentral.Menu.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    MenuUpdate = dbCentral.Menu.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    Menusave = dbCentral.Menu.ToList();
                }
                foreach (Menu Item in Menusave)
                {
                    Menu ObjMenu = new Menu();
                    ObjMenu = dbLocal.Menu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu == null)
                    {
                        ObjMenu = new Menu();
                        ObjMenu.Activo = Item.Activo;
                        ObjMenu.Descripcion = Item.Descripcion;
                        ObjMenu.FechaCreacion = DateTime.Now;
                        ObjMenu.Imagen = Item.Imagen;
                        ObjMenu.Nombre = Item.Nombre;
                        ObjMenu.Orden = Item.Orden;
                        ObjMenu.RowIDCreacion = Item.RowID;
                        TipoMenu tm = dbLocal.TipoMenu.Where(f => f.RowIDCreacion == Item.TipoMenuID).FirstOrDefault();
                        ObjMenu.TipoMenuID = tm.RowID;
                        ObjMenu.URL = Item.URL;
                        dbLocal.Menu.Add(ObjMenu);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Menu Item in MenuUpdate)
                {

                    Menu ObjMenu = dbLocal.Menu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu != null)
                    {
                        ObjMenu.Activo = Item.Activo;
                        ObjMenu.Descripcion = Item.Descripcion;
                        ObjMenu.FechaModificacion = DateTime.Now;
                        ObjMenu.Imagen = Item.Imagen;
                        ObjMenu.Nombre = Item.Nombre;
                        ObjMenu.Orden = Item.Orden;
                        ObjMenu.RowIDCreacion = Item.RowID;
                        ObjMenu.URL = Item.URL;
                        TipoMenu tm = dbLocal.TipoMenu.Where(f => f.RowIDCreacion == Item.TipoMenuID).FirstOrDefault();
                        ObjMenu.TipoMenuID = tm.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "MENU";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarTipoMenu(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TIPOMENU" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<TipoMenu> TipoMenusave = new List<TipoMenu>();
                List<TipoMenu> TipoMenuUpdate = new List<TipoMenu>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    TipoMenusave = dbCentral.TipoMenu.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    TipoMenuUpdate = dbCentral.TipoMenu.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    TipoMenusave = dbCentral.TipoMenu.ToList();
                }
                foreach (TipoMenu Item in TipoMenusave)
                {
                    TipoMenu ObjMenu = new TipoMenu();
                    ObjMenu = dbLocal.TipoMenu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu == null)
                    {
                        ObjMenu = new TipoMenu();
                        ObjMenu.Activo = Item.Activo;
                        ObjMenu.FechaCreacion = DateTime.Now;
                        ObjMenu.Nombre = Item.Nombre;
                        ObjMenu.RowIDCreacion = Item.RowID;
                        dbLocal.TipoMenu.Add(ObjMenu);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (TipoMenu Item in TipoMenuUpdate)
                {

                    TipoMenu ObjMenu = dbLocal.TipoMenu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu != null)
                    {
                        ObjMenu.Activo = Item.Activo;
                        ObjMenu.FechaCreacion = DateTime.Now;
                        ObjMenu.Nombre = Item.Nombre;
                        ObjMenu.RowIDCreacion = Item.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TIPOMENU";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarRolMenu(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "ROLMENU" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<RolMenu> RolMenusave = new List<RolMenu>();
                List<RolMenu> RolMenuUpdate = new List<RolMenu>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    RolMenusave = dbCentral.RolMenu.Where(f => f.FechaCreacion > Consulta.FechaInicio).ToList();
                    RolMenuUpdate = dbCentral.RolMenu.Where(f => f.FechaModificacion > Consulta.FechaInicio).ToList();
                }
                else
                {
                    RolMenusave = dbCentral.RolMenu.ToList();
                }
                foreach (RolMenu Item in RolMenusave)
                {
                    RolMenu ObjMenu = new RolMenu();
                    ObjMenu = dbLocal.RolMenu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu == null)
                    {
                        ObjMenu = new RolMenu();
                        ObjMenu.Estado = Item.Estado;
                        ObjMenu.FechaCreacion = DateTime.Now;
                        Menu mn = dbLocal.Menu.Where(f => f.RowIDCreacion == Item.MenuID).FirstOrDefault();
                        Rol rl = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RolID).FirstOrDefault();
                        ObjMenu.MenuID = mn.RowID;
                        ObjMenu.RolID = rl.RowID;
                        dbLocal.RolMenu.Add(ObjMenu);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (RolMenu Item in RolMenuUpdate)
                {

                    RolMenu ObjMenu = dbLocal.RolMenu.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjMenu != null)
                    {
                        ObjMenu.Estado = Item.Estado;
                        ObjMenu.FechaCreacion = DateTime.Now;
                        Menu mn = dbLocal.Menu.Where(f => f.RowIDCreacion == Item.MenuID).FirstOrDefault();
                        Rol rl = dbLocal.Rol.Where(f => f.RowIDCreacion == Item.RowIDCreacion).FirstOrDefault();
                        ObjMenu.MenuID = mn.RowID;
                        ObjMenu.RolID = rl.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "ROLMENU";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarTaquilla(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "TAQUILLA" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<Taquilla> Taquillasave = new List<Taquilla>();
                List<Taquilla> TaquillaUpdate = new List<Taquilla>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    Taquillasave = dbCentral.Taquilla.Where(f => f.FechaCreacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                    TaquillaUpdate = dbCentral.Taquilla.Where(f => f.FechaModificacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                }
                else
                {
                    Taquillasave = dbCentral.Taquilla.Where(f => f.TeatroID == RowID).ToList();
                }
                foreach (Taquilla Item in Taquillasave)
                {
                    Taquilla ObjTaquilla = new Taquilla();
                    ObjTaquilla = dbLocal.Taquilla.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTaquilla == null)
                    {
                        ObjTaquilla = new Taquilla();
                        ObjTaquilla.ConsecutivoFinal = Item.ConsecutivoFinal;
                        ObjTaquilla.ConsecutivoInicial = Item.ConsecutivoInicial;
                        ObjTaquilla.CreadoPor = Item.CreadoPor;
                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        ObjTaquilla.EstadoID = es.RowID;
                        ObjTaquilla.FechaCreacion = DateTime.Now;
                        ObjTaquilla.FechaInicial = Item.FechaInicial;
                        ObjTaquilla.FechaFinal = Item.FechaFinal;
                        ObjTaquilla.IP = Item.IP;
                        ObjTaquilla.Nombre = Item.Nombre;
                        ObjTaquilla.Prefijo = Item.Prefijo;
                        ObjTaquilla.RowIDCreacion = Item.RowID;
                        if (ObjTaquilla.TipoTaquillaID != null)
                        {
                            Opcion op = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoTaquillaID).FirstOrDefault();
                            ObjTaquilla.TipoTaquillaID = op.RowID;
                        }
                        if (ObjTaquilla.TeatroID != null)
                        {
                            Teatro tr = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                            ObjTaquilla.TeatroID = tr.RowID;
                        }
                        dbLocal.Taquilla.Add(ObjTaquilla);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (Taquilla Item in TaquillaUpdate)
                {

                    Taquilla ObjTaquilla = new Taquilla();
                    ObjTaquilla = dbLocal.Taquilla.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjTaquilla != null)
                    {
                        ObjTaquilla.ConsecutivoFinal = Item.ConsecutivoFinal;
                        ObjTaquilla.ConsecutivoInicial = Item.ConsecutivoInicial;
                        ObjTaquilla.CreadoPor = Item.CreadoPor;
                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        ObjTaquilla.EstadoID = es.RowID;
                        ObjTaquilla.FechaCreacion = DateTime.Now;
                        ObjTaquilla.FechaInicial = Item.FechaInicial;
                        ObjTaquilla.FechaFinal = Item.FechaFinal;
                        ObjTaquilla.IP = Item.IP;
                        ObjTaquilla.Nombre = Item.Nombre;
                        ObjTaquilla.Prefijo = Item.Prefijo;
                        ObjTaquilla.RowIDCreacion = Item.RowID;
                        Opcion op = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoTaquillaID).FirstOrDefault();
                        ObjTaquilla.TipoTaquillaID = op.RowID;
                        if (ObjTaquilla.TeatroID != null)
                        {
                            Teatro tr = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                            ObjTaquilla.TeatroID = tr.RowID;
                        }
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "TAQUILLA";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarListaDetalle(Int32? RowIDEncabezado, Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "LISTADETALLE" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<ListaDetalle> ListaDetallesave = new List<ListaDetalle>();
                List<ListaDetalle> ListaDetalleUpdate = new List<ListaDetalle>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    ListaDetallesave = dbCentral.ListaDetalle.Where(f => f.FechaCreacion > Consulta.FechaInicio && f.ListaEncabezadoID == RowIDEncabezado).ToList();
                    ListaDetalleUpdate = dbCentral.ListaDetalle.Where(f => f.FechaModificacion > Consulta.FechaInicio && f.ListaEncabezadoID == RowIDEncabezado).ToList();
                }
                else
                {
                    ListaDetallesave = dbCentral.ListaDetalle.Where(f => f.ListaEncabezadoID == RowIDEncabezado).ToList();
                }
                foreach (ListaDetalle Item in ListaDetallesave)
                {
                    ListaDetalle ObjListaDetalle = new ListaDetalle();
                    ObjListaDetalle = dbLocal.ListaDetalle.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjListaDetalle == null)
                    {
                        ObjListaDetalle = new ListaDetalle();
                        ObjListaDetalle.Descripcion = Item.Descripcion;
                        ObjListaDetalle.DiasAsignados = Item.DiasAsignados;
                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        ObjListaDetalle.EstadoID = es.RowID;
                        ObjListaDetalle.FechaCreacion = DateTime.Now;
                        ObjListaDetalle.FechaFinal = Item.FechaFinal;
                        ObjListaDetalle.FechaInicial = Item.FechaInicial;
                        ObjListaDetalle.HoraFinal = Item.HoraFinal;
                        ObjListaDetalle.HoraInicial = Item.HoraInicial;
                        ListaEncabezado le = dbLocal.ListaEncabezado.Where(f => f.RowIDCreacion == Item.ListaEncabezadoID).FirstOrDefault();
                        ObjListaDetalle.CreadoPor = Item.CreadoPor;
                        ObjListaDetalle.ListaEncabezadoID = le.RowID;
                        ObjListaDetalle.Nombre = Item.Nombre;
                        ObjListaDetalle.Precio = Item.Precio;
                        ObjListaDetalle.PrecioDistribuidor = Item.PrecioDistribuidor;
                        ObjListaDetalle.RowIDCreacion = Item.RowID;
                        Opcion op = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoFormatoID).FirstOrDefault();
                        ObjListaDetalle.TipoFormatoID = op.RowID;
                        Opcion opt = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoListaDetalle).FirstOrDefault();
                        ObjListaDetalle.TipoListaDetalle = opt.RowID;
                        Opcion opts = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoServicioID).FirstOrDefault();
                        ObjListaDetalle.TipoServicioID = opts.RowID;
                        dbLocal.ListaDetalle.Add(ObjListaDetalle);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                foreach (ListaDetalle Item in ListaDetalleUpdate)
                {

                    ListaDetalle ObjListaDetalle = new ListaDetalle();
                    ObjListaDetalle = dbLocal.ListaDetalle.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjListaDetalle != null)
                    {
                        ObjListaDetalle.Descripcion = Item.Descripcion;
                        ObjListaDetalle.DiasAsignados = Item.Descripcion;

                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        int est = es.RowID;
                        ObjListaDetalle.EstadoID = est;
                        ObjListaDetalle.FechaModificacion = DateTime.Now;
                        ObjListaDetalle.FechaFinal = Item.FechaFinal;
                        ObjListaDetalle.FechaInicial = Item.FechaInicial;
                        ObjListaDetalle.HoraFinal = Item.HoraFinal;
                        ObjListaDetalle.HoraInicial = Item.HoraInicial;
                        ListaEncabezado le = dbLocal.ListaEncabezado.Where(f => f.RowIDCreacion == Item.ListaEncabezadoID).FirstOrDefault();
                        ObjListaDetalle.CreadoPor = Item.CreadoPor;
                        ObjListaDetalle.ListaEncabezadoID = le.RowID;
                        ObjListaDetalle.Nombre = Item.Nombre;
                        ObjListaDetalle.Precio = Item.Precio;
                        ObjListaDetalle.PrecioDistribuidor = Item.PrecioDistribuidor;
                        ObjListaDetalle.RowIDCreacion = Item.RowID;
                        Opcion op = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoFormatoID).FirstOrDefault();
                        ObjListaDetalle.TipoFormatoID = op.RowID;
                        Opcion opt = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoListaDetalle).FirstOrDefault();
                        ObjListaDetalle.TipoListaDetalle = opt.RowID;
                        Opcion opts = dbLocal.Opcion.Where(f => f.RowIDCreacion == Item.TipoServicioID).FirstOrDefault();
                        ObjListaDetalle.TipoServicioID = opts.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "LISTADETALLE";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();
        }
        public void SincronizarListaEncabezado(Int32? RowID)
        {
            int Sincronizados = 0;
            HistoricolLog LOG = new HistoricolLog();
            try
            {
                InicializarConexiones("CENTRAL", RowID);
                LOG.FechaInicio = DateTime.Now;
                IList<HistoricolLog> Validacion = dbLocal.HistoricolLog.Where(f => f.Sincronizo == "LISTA" && f.Respuesta == "OK").ToList();
                HistoricolLog Consulta = new HistoricolLog();
                List<ListaEncabezado> ListaEncabezadosave = new List<ListaEncabezado>();
                List<ListaEncabezado> ListaEncabezadoUpdate = new List<ListaEncabezado>();
                if (Validacion.Count > 0)
                {
                    Consulta = Validacion.OrderByDescending(f => f.RowID).FirstOrDefault();
                    ListaEncabezadosave = dbCentral.ListaEncabezado.Where(f => f.FechaCreacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                    ListaEncabezadoUpdate = dbCentral.ListaEncabezado.Where(f => f.FechaModificacion > Consulta.FechaInicio && f.TeatroID == RowID).ToList();
                }
                else
                {
                    ListaEncabezadosave = dbCentral.ListaEncabezado.Where(f => f.TeatroID == RowID).ToList();
                }
                foreach (ListaEncabezado Item in ListaEncabezadosave)
                {
                    ListaEncabezado ObjListaEncabezado = new ListaEncabezado();
                    ObjListaEncabezado = dbLocal.ListaEncabezado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjListaEncabezado == null)
                    {
                        ObjListaEncabezado = new ListaEncabezado();
                        ObjListaEncabezado.Descripcion = Item.Descripcion;
                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        int est = es.RowID;
                        ObjListaEncabezado.EstadoID = est;
                        ObjListaEncabezado.FechaCreacion = DateTime.Now;
                        ObjListaEncabezado.FechaFinal = Item.FechaFinal;
                        ObjListaEncabezado.FechaInicial = Item.FechaInicial;
                        ObjListaEncabezado.Nombre = Item.Nombre;
                        ObjListaEncabezado.RowIDCreacion = Item.RowID;
                        ObjListaEncabezado.CreadoPor = Item.CreadoPor;
                        Teatro tr = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                        ObjListaEncabezado.TeatroID = tr.RowID;
                        dbLocal.ListaEncabezado.Add(ObjListaEncabezado);
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                        SincronizarListaDetalle(Item.RowID, RowID);
                    }
                }
                foreach (ListaEncabezado Item in ListaEncabezadoUpdate)
                {

                    ListaEncabezado ObjListaEncabezado = new ListaEncabezado();
                    ObjListaEncabezado = dbLocal.ListaEncabezado.Where(f => f.RowIDCreacion == Item.RowID).FirstOrDefault();
                    if (ObjListaEncabezado != null)
                    {
                        ObjListaEncabezado.Descripcion = Item.Descripcion;
                        Estado es = dbLocal.Estado.Where(f => f.RowIDCreacion == Item.EstadoID).FirstOrDefault();
                        int est = es.RowID;
                        ObjListaEncabezado.EstadoID = est;
                        ObjListaEncabezado.FechaCreacion = DateTime.Now;
                        ObjListaEncabezado.FechaFinal = Item.FechaFinal;
                        ObjListaEncabezado.FechaInicial = Item.FechaInicial;
                        ObjListaEncabezado.Nombre = Item.Nombre;
                        ObjListaEncabezado.RowIDCreacion = Item.RowID;
                        Teatro tr = dbLocal.Teatro.Where(f => f.RowIDCreacion == Item.TeatroID).FirstOrDefault();
                        ObjListaEncabezado.TeatroID = tr.RowID;
                        dbLocal.SaveChanges();
                        Sincronizados = Sincronizados + 1;
                        SincronizarListaDetalle(Item.RowID, RowID);
                    }
                }
                LOG.Respuesta = "OK";

            }
            catch (Exception ex)
            {
                LOG.Respuesta = ex.ToString();
            }
            LOG.FechaFin = DateTime.Now;
            LOG.Sincronizo = "LISTA";
            LOG.Registros = Sincronizados;
            dbLocal.HistoricolLog.Add(LOG);
            dbLocal.SaveChanges();

        }

    }
}
