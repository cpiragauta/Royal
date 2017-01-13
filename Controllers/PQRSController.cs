using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.ModelosPropios;
namespace CinemaPOS.Controllers.Inicio
{
    public class PQRSController : Controller
    {
        //
        // GET: /PQRS/
    CinemaPOSEntities db = new CinemaPOSEntities();
        private FormCollection DeSerialize(FormCollection formulario)
        {
            FormCollection collection = new FormCollection();
            //un-encode, and add spaces back in
            string querystring = Uri.UnescapeDataString(formulario["formulario"]).Replace("+", " ");
            var split = querystring.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (string s in split)
            {
                string text = s.Substring(0, s.IndexOf("="));
                string value = s.Substring(s.IndexOf("=") + 1);

                if (items.Keys.Contains(text))
                    items[text] = items[text] + "," + value;
                else
                    items.Add(text, value);
            }
            foreach (var i in items)
            {
                collection.Add(i.Key, i.Value);
            }
            return collection;
        }

        #region PQRS
        [CheckSessionOutAttribute]
        public ActionResult CreacionPQRS(int? RowID_pqrs, int? RowIdTercero)
        {

            //ViewBag.TipoTerceroID = new List<Opcion>();
            ViewBag.TipoTerceroID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOTERCERO").ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO").ToList();
            ViewBag.CiudadID = db.Ciudad.ToList().OrderBy(f => f.Nombre);
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION").ToList();

            if (RowID_pqrs == null || RowID_pqrs == 0)
            {

                return View(new ModelosPropios.Model.Modelos_Pqrs());
            }
            else
            {

                ModelosPropios.Model.Modelos_Pqrs ObjModelos = new ModelosPropios.Model.Modelos_Pqrs();
                ObjModelos.Pqrs_Terceros = db.Pqrs.Where(p => p.RowID == RowID_pqrs).ToList();
                ObjModelos.Pqrs_Seguimiento = db.Seguimiento.Where(p => p.PQRS_ID == RowID_pqrs).ToList();
                return View(ObjModelos);

            }
        }

        [CheckSessionOutAttribute]
        public ActionResult ModalTercero()
        {

            var TipoAreas = db.Tipo.Where(f => f.Codigo == Util.Constantes.TIPO_AREA).FirstOrDefault();
            var TipoEstados = db.TipoEstado.Where(f => f.Codigo == Util.Constantes.TIPO_ESTADO_PQRS).FirstOrDefault();
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.TipoPQRS = db.TipoSolicitud.Where(ts => ts.Estado == true).ToList();
            ViewBag.CambioArea = db.Opcion.Where(f => f.TipoID == TipoAreas.RowID).ToList();
            ViewBag.Estados = db.Estado.Where(f => f.TipoEstadoID == TipoEstados.RowID).ToList();
            return View();
        }

        [CheckSessionOutAttribute]
        public JsonResult Guardar_PQRS(FormCollection formulario, int? RowIDTercero)
        {
            Pqrs Obj_pqrs = new Pqrs();

            formulario = DeSerialize(formulario);
            Obj_pqrs.TeatroID = int.Parse(formulario["TeatroID"]);
            Obj_pqrs.TipoSolicitudID = int.Parse(formulario["tiposolicitud"]);
            Obj_pqrs.TerceroID = RowIDTercero;
            Obj_pqrs.EstadoID = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPQRS" && e.Nombre == "Recibida").FirstOrDefault().RowID;
            Obj_pqrs.Titulo = formulario["Titulo"];
            Obj_pqrs.Descripcion = formulario["descripcion-pqrs"];
            Obj_pqrs.FechaSuceso = Convert.ToDateTime(formulario["fecha_suceso"]);
            Obj_pqrs.FechaCreacion = DateTime.Now;
            Obj_pqrs.CreadoPor = Session["usuario_creacion"].ToString();
            db.Pqrs.Add(Obj_pqrs);
            db.SaveChanges();
            return Json(Obj_pqrs.RowID);
        }

        [CheckSessionOutAttribute]
        public JsonResult CargarCLiente()
        {
            string term = Request.Params["term"];
            try
            {
                var data = (from Sucur in db.Tercero
                                                 .Where(f => f.Nombre.Contains(term) || f.Identificacion.Contains(term.Trim())).ToList()
                            select new
                            {
                                rowid = Sucur.RowID,
                                nombre = Sucur.Nombre,
                                label = Sucur.Nombre + " " + Sucur.Apellidos


                            }).Distinct().OrderBy(f => f.label).Take(15);
                //data = data.Where(f => f.label.Contains(term));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        [CheckSessionOutAttribute]
        public ActionResult VistaPQRS()
        {
            return View(db.Pqrs.ToList());
        }

        [CheckSessionOutAttribute]
        public void EnviarCorreoRecepcion(Int32 PQRS_ID)
        {
            Pqrs pqrsCorreo = db.Pqrs.FirstOrDefault(f => f.RowID == PQRS_ID);
            MailSender.Enviar_RecepcionPQRS(pqrsCorreo, "PLANTILLA_RECEPCION_PQRS", Session["usuario_creacion"].ToString(), null, pqrsCorreo.Tercero.Correo, "camilop@pangea.com.co");
        }



        #endregion

        #region PQRS - Tipo Solicitud
        [CheckSessionOutAttribute]
        public ActionResult VistaTipoSolicitud()
        {
            return View(db.TipoSolicitud.ToList());
        }

        [CheckSessionOutAttribute]
        public ActionResult TipoSolicitud(int? TipoSolicitud_pqrs)
        {
            var TipoAreas = db.Tipo.Where(f => f.Codigo == Util.Constantes.TIPO_AREA).FirstOrDefault();
            ViewBag.ListaAreas = db.Opcion.Where(f => f.TipoID == TipoAreas.RowID && f.Activo == true).ToList();
            TipoSolicitud  solicitud= new TipoSolicitud();
            if (TipoSolicitud_pqrs != null)
            {
                solicitud = db.TipoSolicitud.Where(f => f.RowID == TipoSolicitud_pqrs).FirstOrDefault();
            }
           
            return View(solicitud);
        }

        

        [CheckSessionOutAttribute]
        public JsonResult guardar_TipoSolicitud(FormCollection formulario)
        {
            formulario = DeSerialize(formulario);
            String respuesta = "";
            int RowidSeguimiento = 0;
            TipoSolicitud tipoSolicitud = new TipoSolicitud();
            if (formulario["RowID_TipoSolicitud"] != null)
            {
                RowidSeguimiento = Int32.Parse(formulario["RowID_TipoSolicitud"].ToString());
                if (RowidSeguimiento !=0 )
                {
                    tipoSolicitud = db.TipoSolicitud.FirstOrDefault(f => f.RowID == RowidSeguimiento);
                }
            }
            
            try
            {
                tipoSolicitud.Nombre = formulario["nombre"];
                tipoSolicitud.AreaID = Convert.ToInt32(formulario["Area"]);
                if (formulario["activo"] == null)
                {
                    tipoSolicitud.Estado = false;
                }
                else
                {
                    tipoSolicitud.Estado = true;
                }
                tipoSolicitud.Descripcion = formulario["descripcion"];                
                if (RowidSeguimiento == 0) {
                    tipoSolicitud.FechaCreacion = DateTime.Now;
                    tipoSolicitud.CreadoPor = Session["usuario_creacion"].ToString();
                    db.TipoSolicitud.Add(tipoSolicitud);
                    respuesta = "Guardado Correctamente";
                }
                else
                {
                    respuesta = "Actualizado Correctamente";
                }               
                db.SaveChanges();
                return Json(new { respuesta = respuesta, tipoRespuesta = "success" });
            }
            catch (Exception)
            {
                return Json(new { respuesta = "Error", tipoRespuesta = "error"});
            }
        }


        #endregion

        #region Seguimiento 
        [CheckSessionOutAttribute]
        public ActionResult Seguimiento(int? Rowid_Pqrs)
        {
            var TipoAreas = db.Tipo.Where(f => f.Codigo == Util.Constantes.TIPO_AREA).FirstOrDefault();
            var TipoEstados = db.TipoEstado.Where(f => f.Codigo == Util.Constantes.TIPO_ESTADO_PQRS).FirstOrDefault();
            ViewBag.CambioArea = db.Opcion.Where(f => f.TipoID == TipoAreas.RowID && f.Activo == true).ToList();
            ViewBag.Estados = db.Estado.Where(f => f.TipoEstadoID == TipoEstados.RowID).ToList();
            return View(db.Pqrs.Where(s => s.RowID == Rowid_Pqrs).FirstOrDefault());
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarSeguimiento(FormCollection formulario)
        {
            formulario = DeSerialize(formulario);

            Seguimiento seguimiento = new Seguimiento();
            try
            {
                seguimiento.AreaAnteriorID = Convert.ToInt32(formulario["AreaAnteriorID"]);
                seguimiento.AreaActualID = Convert.ToInt32(formulario["AreaActualID"]);
                seguimiento.EstadoID = Convert.ToInt32(formulario["EstadoID"]);
                seguimiento.Observaciones = formulario["Observaciones"];
                seguimiento.FechaCreacion = DateTime.Now;
                seguimiento.CreadoPor = Session["usuario_creacion"].ToString();
                seguimiento.PQRS_ID = Convert.ToInt32(formulario["PQRS_ID"]);
                db.Seguimiento.Add(seguimiento);
                db.SaveChanges();
                return Json(seguimiento.RowID);
            }
            catch (Exception)
            {
                return Json("error");
            }
        }


        [CheckSessionOutAttribute]
        public String CargarListaSeguimiento(Int32 PQRS_ID)
        {
            String tabla = "";
            List<Seguimiento> ListaSeguimiento = db.Seguimiento.Where(ld => ld.PQRS_ID == PQRS_ID).ToList();
            foreach (Seguimiento seguimiento in ListaSeguimiento)
            {
                tabla = tabla + "<tr>";
                tabla += "<td>" + seguimiento.Opcion.Nombre + "</td>";
                tabla += "<td>" + seguimiento.Opcion1.Nombre + "</td>";
                tabla += "<td>" + seguimiento.Estado.Nombre + "</td>";
                tabla += "<td>" + seguimiento.Observaciones + "</td>";
                tabla += "<td>" + seguimiento.CreadoPor + "</td>";
                tabla = tabla + "</tr>";

            }
            return tabla;
        }

        #endregion
     
    }
}
