using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;

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
        public ActionResult CreacionPQRS(int? RowID_pqrs,int? RowIdTercero)
        {
           
            //ViewBag.TipoTerceroID = new List<Opcion>();
            ViewBag.TipoTerceroID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOTERCERO").ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO").ToList();
            ViewBag.CiudadID = db.Ciudad.ToList().OrderBy(f => f.Nombre);
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION").ToList();
            
            if (RowID_pqrs==null|| RowID_pqrs==0)
            {
               
                return View(new ModelosPropios.Model.Modelos_Pqrs());
            }
            else
            {
                ModelosPropios.Model.Modelos_Pqrs ObjModelos = new ModelosPropios.Model.Modelos_Pqrs();
                return View(db.Pqrs.Where(p=>p.RowID==RowID_pqrs).FirstOrDefault());
            }
        }
        public ActionResult ModalTercero()
        {
            
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.TipoPQRS = db.TipoSolicitud.Where(ts => ts.Estado == true).ToList();
            return View();
        }
        public JsonResult Guardar_PQRS(FormCollection formulario,int? RowIDTercero)
        {
            Pqrs Obj_pqrs = new Pqrs();
            
            formulario = DeSerialize(formulario);
            Obj_pqrs.TeatroID = int.Parse(formulario["teatro"]);
            Obj_pqrs.TipoSolicitudID = int.Parse(formulario["tiposolicitud"]);
            Obj_pqrs.TerceroID = RowIDTercero;
            Obj_pqrs.EstadoID = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPQRS" && e.Nombre == "Recibida").FirstOrDefault().RowID;
            Obj_pqrs.Titulo = formulario["Titulo"];
            Obj_pqrs.Descripcion = formulario["descripcion-pqrs"];
            Obj_pqrs.FechaSuceso = ModelosPropios.Util.HoraInsertar( formulario["fecha_suceso"]);
            Obj_pqrs.FechaCreacion = DateTime.Now;
            Obj_pqrs.CreadoPor = Session["usuario_creacion"].ToString();
            db.Pqrs.Add(Obj_pqrs);
            db.SaveChanges();
            return Json(new { respuesta="Creado Exitosamente"});
        }
        public JsonResult CargarCLiente()
        {
            string term = Request.Params["term"];
            try
            {
                var data = (from Sucur in db.Tercero
                                                 .Where(f => f.Nombre.Contains(term)||f.Identificacion.Contains(term.Trim())).ToList()
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

        public ActionResult VistaPQRS()
        {
            return View(db.Pqrs.ToList());
        }
        public ActionResult Seguimiento(int? Rowid_Pqrs)
        {
            return View(db.Pqrs.Where(s=>s.RowID==Rowid_Pqrs).FirstOrDefault());
        }
    }
}
