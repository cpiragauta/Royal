using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.ModelosPropios;
using System.Globalization;

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
        [CheckSessionOutAttribute]
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
        [CheckSessionOutAttribute]
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
        [CheckSessionOutAttribute]
        public ActionResult VistaControlCaja()
        {
            ViewBag.Controles = db.ControlCajaUsuarioEntrega.ToList();
            return View();
        }
        [CheckSessionOutAttribute]
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
        [CheckSessionOutAttribute]
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
        [CheckSessionOutAttribute]
        [ValidateInput(false)]
        public ActionResult BloquearSilla(int SalaID)
        {
            string Fecha = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime fechafiltro = DateTime.ParseExact(Fecha,"yyyy-MM-dd",CultureInfo.InvariantCulture);
            ViewBag.Funciones = db.Funcion.Where(fun => fun.SalaID == SalaID && fun.Fecha> fechafiltro).ToList();
            string Data_Table = "";
            //VerMapaVenta_Result ObjVerMapaFuncion = new VerMapaVenta_Result();
            List<BloqueoSillas_Result> objsillas = db.BloqueoSillas().Where(bs=>bs.RowIDSala==SalaID).OrderBy(bs=>bs.RowIDSillaMapa).ToList();
            int ContadorColumnas = 0;
            int CantidadColumnas = 0;
            int i = 0;
            foreach (var SillaMapa in objsillas)
            {
                CantidadColumnas = int.Parse(SillaMapa.SalaColumnas.ToString());
                if (ContadorColumnas == 0)
                {
                    Data_Table = Data_Table + "<tr class='fila_" + i + "' style='padding:0px,0px,0px,0px;'>";
                    i++;
                }
                if (SillaMapa.TipoObjeto == "SILLA")
                {
                    if (SillaMapa.PermitirBloqueo != 0)
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "' class='disabled' style='background: #B0BEC5;' >";
                    }
                    else
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "'  onclick = bloquear_silla(this) >";
                    }
                }
                else
                {
                    Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "'>";
                }
                ContadorColumnas++;
                Data_Table = Data_Table + "<strong><small>" + SillaMapa.SillaColumna + "&nbsp;" + (SillaMapa.SillaFila + 1) + "</small ></strong >";
                Data_Table = Data_Table + "<img class='img-xs' style ='border:none;' src='/" + SillaMapa.ImagenObjeto + "' />";
                if (ContadorColumnas == CantidadColumnas)
                {
                    Data_Table = Data_Table + " </tr>";
                    ContadorColumnas = 0;
                }
            }

            return View((object)Data_Table);
        }
        [CheckSessionOutAttribute]
        public ActionResult VistaSalasTeatro()
        {
            int TeatroID = int.Parse(Session["RowID_Teatro"].ToString());
            ViewBag.Salas = db.Sala.Where(s => s.TeatroID == TeatroID).ToList();
            return View();
        }
        [CheckSessionOutAttribute]
        public JsonResult BloquearSillaFuncion(int RowIDObjeto, FormCollection formulario/* string FuncionesID,string Observaciones*/)
        {
            SillaBloqueo objSillaBloqueo = new SillaBloqueo();
            string respuesta = "La sillas han sido bloqueadas";
            string tipo_respuesta = "success";
            UsuarioSistema usuario = (UsuarioSistema)(Session["usuario"]);
            formulario = DeSerialize(formulario);

            var RowIDFunciones = formulario["list_funciones_sala"].Split(',');
            try
            {
                for (int i = 0; i < RowIDFunciones.Length; i++)
                {
                    objSillaBloqueo.SillaID = RowIDObjeto;
                    objSillaBloqueo.FuncionId = int.Parse(RowIDFunciones[i]);
                    objSillaBloqueo.Observaciones = formulario["observacion"];
                    objSillaBloqueo.FechaBloqueo = DateTime.Now;
                    objSillaBloqueo.UsuarioBloqueoID = usuario.RowID;
                    objSillaBloqueo.Estado = true;
                    db.SillaBloqueo.Add(objSillaBloqueo);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                tipo_respuesta = "warning";
                respuesta = ex.Message;
            }

            return Json(new {tipo_respuesta=tipo_respuesta,respuesta=respuesta },JsonRequestBehavior.AllowGet);
        }
        [CheckSessionOutAttribute]
        public string DataTable(int SalaID)
        {
            string data_table = "";
            string Fecha = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime fechafiltro = DateTime.ParseExact(Fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            List<SillaBloqueo> ListaSillasBloqueadas = db.SillaBloqueo.Where(sb => sb.Funcion.SalaID == SalaID /*&& sb.Funcion.Fecha>fechafiltro*/).ToList(); ;
            foreach (var item in ListaSillasBloqueadas)
            {
                data_table += "<tr>";
                    data_table += "<td>" + item.Funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + "</td>";
                    data_table += "<td>" + DateTime.Parse( item.Funcion.HoraInicial.ToString()).ToString("hh:mm tt",CultureInfo.InvariantCulture) + "</td>"; 
                    data_table += "<td>" + item.Funcion.Fecha + "</td>";
                    data_table += "<td>" + item.MapaSala.Columna + "-" + item.MapaSala.Fila + "</td>";
                    data_table += "<td>" + item.Estado + "</td>";
                    data_table += "<td>" + item.UsuarioSistema.Nombre+"-"+item.UsuarioSistema.Apellido + "</td>";
                    if (item.Estado==true)
                    {
                        data_table += "<td><a href='javascript:liberar_silla(" + item.RowID + ",false)'><i class='icon ion-trash-a'></i></a></td>";
                    }
                    else {
                        data_table += "<td style=color><a href='javascript:liberar_silla(" + item.RowID + ",true)'><i class='ion-checkmark-circled'></i></a></td>";
                    }
                data_table += "</tr>";
            }
            return data_table;
        }

        public string EstadoBloqueoSala(int rowid_bloqueo,bool estado)
        {
            SillaBloqueo objSillaBloqueo = db.SillaBloqueo.Where(sb => sb.RowID == rowid_bloqueo).FirstOrDefault();
            objSillaBloqueo.Estado = estado;
            db.SaveChanges();
            return "Cambio  realizado";
        }
    }
}