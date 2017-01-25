using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.ModelosPropios;

namespace CinemaPOS.Controllers
{
    public class AdminTeatroController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();
        //
        // GET: /AdminTeatro/
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

        public ActionResult ControlCajaUsuario(int? RowIDControl)
        {
            ViewBag.Taquilleros = db.UsuarioSistema.Where(us => us.Activo == true && us.Rol.Nombre == "CAJERO TAQUILLERO").ToList();
            if (RowIDControl!=null)
            {
                return View(db.ControlCajaUsuarioEntrega.Where(ccu => ccu.RowID == RowIDControl).FirstOrDefault());
            }
            else
            {
                return View(new ControlCajaUsuarioEntrega());
            }
            
           // return View();
        }

        public JsonResult GuardarControl(FormCollection formulario,int? rowid_control)
        {
            ControlCajaUsuarioEntrega objcontrol_ingresa = new ControlCajaUsuarioEntrega();
            //formulario = DeSerialize(formulario);
            int CodigoEstado = db.Estado.Where(E => E.Codigo == Util.Constantes.ESTADO_CONTROL_CAJA_USUARIO).FirstOrDefault().RowID;
            string tipo_respuesta = "";
            string respuesta = "";
            if (rowid_control!=null ||rowid_control!=0)
            {
                try
                {
                    objcontrol_ingresa.UsuarioID = int.Parse(formulario["taquillero"]);
                    objcontrol_ingresa.ValorEntrega = int.Parse(formulario["valor_entrega"]);
                    objcontrol_ingresa.CantidadTarjetas = int.Parse(formulario["cantidad_tcr"]);
                    objcontrol_ingresa.CantidadGafas = int.Parse(formulario["cantidad_gafas_adulto"]);
                    objcontrol_ingresa.CantidadGafasNin = int.Parse(formulario["cantidad_gafas_nino"]);
                    objcontrol_ingresa.CantidadBonoRegalo = int.Parse(formulario["cantidad_bono_regalo"]);
                    objcontrol_ingresa.FechaEntrega = DateTime.Now;
                    objcontrol_ingresa.EstadoID = CodigoEstado;
                    db.ControlCajaUsuarioEntrega.Add(objcontrol_ingresa);
                    db.SaveChanges();
                    tipo_respuesta = "success";
                    respuesta = "Información guardada exitosamente";
                }
                catch (Exception ex)
                {
                    tipo_respuesta = "error";
                    respuesta = ex.Message;
                }
               
            }
            return Json(new { tipo_respuesta=tipo_respuesta,respuesta=respuesta},JsonRequestBehavior.AllowGet);
        }

        public ActionResult VistaControlCaja()
        {
            ViewBag.Controles = db.ControlCajaUsuarioEntrega.ToList();
            return View();
        }

        public ActionResult RegistrarIngreso(int? RowIDControlentrega)
        {
            ControlCajaUsuarioEntrega objControlUsuario = new ControlCajaUsuarioEntrega();
            if (RowIDControlentrega!=null)
            {   
                objControlUsuario = db.ControlCajaUsuarioEntrega.Where(cue => cue.RowID == RowIDControlentrega).FirstOrDefault();
                string fecha = objControlUsuario.FechaEntrega.Value.ToShortDateString();
                //var lolol = db.ValoreCierreCaja();
                ViewBag.Valores = db.ValoreCierreCaja().Where(ciu =>ciu.RowidUsuario == objControlUsuario.UsuarioID && ciu.Fecha == fecha).ToList();

                //var lol = db.VistaCierreCaja.ToList();
                //ViewBag.Valores = db.VistaCierreCaja.ToList();/* Where(ciu =>/*ciu.RowidUsuario == objControlUsuario.UsuarioID && ciu.Fecha == fecha).ToList();*/
                    return View(objControlUsuario);
            }
            return View(objControlUsuario);
        }
        public JsonResult GuardarControlIngreso(FormCollection formulario)
        {
            //formulario = DeSerialize(formulario);
            ControlCajaUsuarioRecibe objControlRecibe = new ControlCajaUsuarioRecibe();
            objControlRecibe.ValorEntrega = int.Parse(formulario["valor_entrega"].ToString().Replace(".",""));
            objControlRecibe.CantidadTarjetas = int.Parse(formulario["cantidad_tarjetas"]);
            objControlRecibe.CantidadBonoRegalo = int.Parse(formulario["cantidad_bono_regalo"]);
            objControlRecibe.CantidadGafasAd = int.Parse(formulario["cantidad_gafas_adulto"]);
            objControlRecibe.CantidadGafasNin = int.Parse(formulario["cantidad_gafas_nino"]);
            objControlRecibe.FechaEntrega = DateTime.Now;
            objControlRecibe.ControlCajaEntregaID = int.Parse(formulario["RowID_ControlUsuario"]);
            db.ControlCajaUsuarioRecibe.Add(objControlRecibe);
            db.SaveChanges();
            return Json("");
        }
    }
}