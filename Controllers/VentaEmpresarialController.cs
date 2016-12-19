using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Security.Cryptography;
using System.Text;

namespace CinemaPOS.Controllers
{
    public class VentaEmpresarialController : Controller
    {
        //
        // GET: /ConveniosCupones/

        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult VistaVentaEmpresarial()
        {
            return View(db.EncabezadoVentaEmpresarial);
        }

        [CheckSessionOutAttribute]
        public ActionResult VentaEmpresarial(int? RowId_Empresarial)
        {
            ViewBag.TipoFormato = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.TipoConvenio = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCONVENIO").ToList();
            ViewBag.TeatrosDisp = db.Teatro.ToList();
            ViewBag.TipoMotivo = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOMOTIVO").ToList();
            ViewBag.TipoClasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLASIFICACIONPELICULA").ToList();
            ViewBag.Teatros = db.TeatroVentaEmpresarial.ToList();
            ViewBag.ListaPrecios = db.ListaEncabezado.ToList();
            ViewBag.Clientes = db.Tercero.Where(f => f.Activo == true && f.Opcion2.Codigo == "EMPRESA").ToList();
            ViewBag.TipoEstado = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOCONVENIO").ToList();
            var TeatroNoDisp = db.TeatroVentaEmpresarial.Where(f => f.VentaEncabezadoID == RowId_Empresarial).ToList();
            List<Teatro> Teatros = new List<Teatro>();
            List<Teatro> TeatrosV2 = db.Teatro.ToList();
            foreach (TeatroVentaEmpresarial item in TeatroNoDisp)
            {
                foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
                {
                    TeatrosV2.Remove(item2);
                }

            }

            ViewBag.TeatrosDisp = TeatrosV2;
            if (RowId_Empresarial != null && RowId_Empresarial != 0)
            {
                RowId_Empresarial = int.Parse(Request.Params["RowId_Empresarial"].ToString());
                ViewBag.listaDetalleEmpresarial = db.DetalleVentaEmpresarial.Where(f => f.VentaEncabezadoID == RowId_Empresarial).OrderBy(f => f.RowID).ToList();
                return View(db.EncabezadoVentaEmpresarial.Where(t => t.RowID == RowId_Empresarial).FirstOrDefault());
            }
            else
            {
                return View(new EncabezadoVentaEmpresarial());
            }
        }

        public ActionResult ModalTeatroVentaEmpresarial(int? rowid)
        {
            var TeatroNoDisp = db.TeatroVentaEmpresarial.Where(f => f.VentaEncabezadoID == rowid).ToList();
            List<Teatro> Teatros = new List<Teatro>();
            List<Teatro> TeatrosV2 = db.Teatro.ToList();
            foreach (TeatroVentaEmpresarial item in TeatroNoDisp)
            {
                foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
                {
                    TeatrosV2.Remove(item2);
                }

            }

            ViewBag.TeatrosDisp = TeatrosV2;
            var id = db.EncabezadoVentaEmpresarial.Where(f => f.RowID == rowid).FirstOrDefault();
            ViewBag.Empresarial = "Venta Empresarial: "+id.Nombre;
            ViewBag.RowEmpresa = id.RowID;
            return View();
        }

        public JsonResult GuardarTeatro(int rowidEmpresarial, int rowidTeatro)
        {
            TeatroVentaEmpresarial teatro_empresarial = new TeatroVentaEmpresarial();
            string validate = "";
            if (rowidTeatro != 0)
            {
                try
                {
                    if (db.TeatroVentaEmpresarial.Where(f => f.TeatroID == rowidTeatro && f.VentaEncabezadoID == rowidEmpresarial).FirstOrDefault() == null)
                    {
                        teatro_empresarial.TeatroID = rowidTeatro;
                        teatro_empresarial.VentaEncabezadoID = rowidEmpresarial;
                        teatro_empresarial.CreadoPor = Session["usuario_creacion"].ToString();
                        teatro_empresarial.CreadoPor = Session["usuario_creacion"].ToString();
                        teatro_empresarial.FechaCreacion = DateTime.Now;
                        db.TeatroVentaEmpresarial.Add(teatro_empresarial);
                        db.SaveChanges();
                        validate = "ok";
                        ModalTeatroVentaEmpresarial(rowidEmpresarial);
                    }else{
                      validate = "El teatro ya se encuentra asociado a la venta empresarial";

                    }
                }
                catch (Exception e)
                { validate = "error"+e.Message; }
            }
            return Json(new { rowidEmpresarial = rowidEmpresarial, mensaje = validate }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult RefrescarTabla(int? rowid)
        {
            var data = (from teatroEmpresarial in db.TeatroVentaEmpresarial.Where(f => f.VentaEncabezadoID == rowid)
                        select new
                        {
                            rowid = teatroEmpresarial.RowID,
                            teatro = (teatroEmpresarial.TeatroID == 0) ? "" : teatroEmpresarial.Teatro.Nombre,
                            ubicacion = (teatroEmpresarial.Teatro.CiudadID == null) ? "" : teatroEmpresarial.Teatro.Ciudad.Nombre
                        }).ToList();

            //var TeatroNoDisp = db.TeatroVentaEmpresarial.Where(f => f.VentaEncabezadoID == rowid).ToList();
            //List<Teatro> Teatros = new List<Teatro>();
            //List<Teatro> TeatrosV2 = db.Teatro.ToList();
            //foreach (TeatroVentaEmpresarial item in TeatroNoDisp)
            //{
            //    foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
            //    {
            //        TeatrosV2.Remove(item2);
            //    }

            //}
            // foreach (Teatro item in TeatrosV2)
            //{
            //    lista+="<option value="+item.RowID+">"+item.Nombre+"-"+item.Ciudad.Nombre+"</option>";
            //}
              
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarEmpresarial(FormCollection formulario, int? RowID_EncabezadoConvenio)
        {
            String respuesta = "";
            int RowID = 0;
            EncabezadoVentaEmpresarial ObjEncabezado = new EncabezadoVentaEmpresarial();
            List<DetalleVentaEmpresarial> listaDetalle = null;
            formulario = DeSerialize(formulario);
            if (RowID_EncabezadoConvenio == 0)
            {
                ObjEncabezado = CargarDatosEncabezadoEmpresarial(ObjEncabezado, formulario);
                try
                {
                    db.EncabezadoVentaEmpresarial.Add(ObjEncabezado);
                    db.SaveChanges();
                    respuesta = "Guardado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            else if (RowID_EncabezadoConvenio != 0)//Para Actualiar
            {
                ObjEncabezado = db.EncabezadoVentaEmpresarial.Where(t => t.RowID == RowID_EncabezadoConvenio).FirstOrDefault();
                ObjEncabezado = CargarDatosEncabezadoEmpresarial(ObjEncabezado, formulario);
                try
                {
                    db.SaveChanges();
                    respuesta = "Actualizado Correctamente";
                    RowID = ObjEncabezado.RowID;
                    listaDetalle = db.DetalleVentaEmpresarial.Where(f => f.VentaEncabezadoID == RowID).ToList();
                    foreach (DetalleVentaEmpresarial objD in listaDetalle)
                    {
                        objD.EstadoID = ObjEncabezado.EstadoID;
                        db.SaveChanges();
                   }
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            return Json(new { respuesta = respuesta, RowID_EncabezadoConvenio = RowID });

        }

         [HttpPost]
        public JsonResult EliminarTeatroEmpresarial(int? RowID)
        {
            string proccess = "";
            if (RowID != null)
            {
                TeatroVentaEmpresarial teatro = db.TeatroVentaEmpresarial.Where(f => f.RowID == RowID).FirstOrDefault();
                if (teatro != null)
                {
                    db.TeatroVentaEmpresarial.Remove(teatro);
                    db.SaveChanges();
                    proccess = "ok";
                }
                else
                {proccess = "error";}
            }
            return Json(proccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cargarItemsApe(int RowID_Encabezado)
        {
            string tabla = "";
            int? idE = 0;
            int? cantidad = 0;

            List<DetalleConvenio> listaConvenio = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowID_Encabezado).ToList();
            if (listaConvenio.Count != 0)
            {
                foreach (DetalleConvenio convenio in listaConvenio)
                {
                    tabla = tabla + "<tr>";
                    tabla += "<td>" + convenio.RowID + "</td>";
                    tabla += "<td>" + convenio.Nombre + "</td>";
                    tabla += "<td>" + convenio.Codigo + "</td>";
                    tabla += "<td>" + convenio.Estado.Nombre + "</td>";
                    tabla += "<td>" + convenio.Teatro.Nombre + "</td>";
                    tabla += "<td><center>" + convenio.Porcentaje + "%</center></td>";
                    tabla += "  <td><a href='javascript:EliminarItems(" + convenio.RowID + ");' class=\"ion-trash-a\"></a></td>";
                    tabla = tabla + "</tr>";
                }
                idE = listaConvenio[0].EncabezadoConvenioID;
                cantidad = (db.EncabezadoConvenio.Where(f => f.RowID == idE).FirstOrDefault().Cantidad);
            }

            var data = new { tabla = tabla, cantidad = cantidad };

            return Json(data);
        }



        public EncabezadoVentaEmpresarial CargarDatosEncabezadoEmpresarial(EncabezadoVentaEmpresarial ObjEncabezado, FormCollection formulario)
        {
            ObjEncabezado.Nombre = formulario["Nombre"];
            ObjEncabezado.FechaInicio = ModelosPropios.Util.HoraInsertar(formulario["FechaInicial"]);
            ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinal"]);
            ObjEncabezado.EstadoID = db.Estado.FirstOrDefault().RowID; //Cambiar por el que es

            ObjEncabezado.TerceroID = Convert.ToInt32(formulario["Cliente"]);
            ObjEncabezado.FormatoID = Convert.ToInt32(formulario["Formato"]);
            ObjEncabezado.ClasificacionID = Convert.ToInt32(formulario["Clasificacion"]);
            ObjEncabezado.EstadoID = Convert.ToInt32(formulario["EstadoConvenio"]);

            if (ObjEncabezado.RowID == 0)
            {
                ObjEncabezado.CreadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaCreacion = DateTime.Now;
                ObjEncabezado.Cantidad = 0;
            }
            else
            {
                ObjEncabezado.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaModificacion = DateTime.Now;
            }
            return ObjEncabezado;
        }

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


        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarDetalleEmpresarial(FormCollection formulario, int RowID_EncabezadoEmpresarial, int Cantidad)
        
{
            int con = 0;
            String respuesta = "";
            formulario = DeSerialize(formulario);
            DetalleVentaEmpresarial objDetalle;
            if (RowID_EncabezadoEmpresarial != 0 && Cantidad != 0)
            {
                //DateTime FechaProgramacion = ModelosPropios.Util.HoraInsertar(formulario["FechaInicialFunciones"].ToString());
                for (int i = 0; i < Cantidad; i++)
                {
                    try
                    {
                        DetalleVentaEmpresarial empresarial;
                        objDetalle = new DetalleVentaEmpresarial();
                        objDetalle.VentaEncabezadoID = RowID_EncabezadoEmpresarial;
                        objDetalle.EstadoID = db.EncabezadoVentaEmpresarial.Where(f => f.RowID == RowID_EncabezadoEmpresarial).FirstOrDefault().EstadoID;
                        objDetalle.Nombre = formulario["NombreItem"];
                        objDetalle.ListaID = Convert.ToInt32(83);
                        objDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                        objDetalle.FechaCreacion = DateTime.Now;
                        empresarial = db.DetalleVentaEmpresarial.Add(objDetalle);
                        db.SaveChanges();
                        empresarial.Codigo = Hash(RowID_EncabezadoEmpresarial + "-" + empresarial.RowID + "-" + empresarial.FechaCreacion.Value.ToString("dd/MM/yyyy"));
                        con++;
                    }
                    catch (Exception ex)
                    { return Json(new { respuesta = "Error " + ex.Message }); }
                    respuesta = "Guardado Correctamente";
                }
                EncabezadoVentaEmpresarial enca;
                enca = db.EncabezadoVentaEmpresarial .Where(f => f.RowID == RowID_EncabezadoEmpresarial).FirstOrDefault();
                enca.Cantidad = con;
                db.SaveChanges();

            }

            return Json(respuesta);
        }

        [CheckSessionOutAttribute]
        public JsonResult EliminarItems(int RowID_Detalle)
        {
            String respuesta = "";
            if (RowID_Detalle != 0)
            {
                try
                {
                    DetalleVentaEmpresarial empresarial;
                    EncabezadoVentaEmpresarial encabezado;
                    empresarial = db.DetalleVentaEmpresarial.Where(f => f.RowID == RowID_Detalle).FirstOrDefault();
                    encabezado = db.EncabezadoVentaEmpresarial.Where(f => f.RowID == empresarial.VentaEncabezadoID).FirstOrDefault();
                    encabezado.Cantidad = (encabezado.Cantidad - 1);
                    db.DetalleVentaEmpresarial.Remove(empresarial);
                    db.SaveChanges();

                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
                respuesta = "Guardado Correctamente";
            }
            return Json(respuesta);

        }



    }

}
