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
    public class ConveniosCuponesController : Controller
    {
        //
        // GET: /ConveniosCupones/

        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult VistaConveniosCupones()
        {
            return View(db.EncabezadoConvenio);
        }

        [CheckSessionOutAttribute]
        public ActionResult ConveniosCupones(int? RowId_Covenio)
        {
            ViewBag.TipoFormato = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.TipoConvenio = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCONVENIO").ToList();
            ViewBag.TipoMotivo = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOMOTIVO").ToList();
            ViewBag.TipoClasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLASIFICACIONPELICULA").ToList();
            ViewBag.ListaTeatro = db.Teatro;
            ViewBag.Clientes = db.Tercero.Where(f => f.Activo == true && f.Opcion2.Codigo == "EMPRESA").ToList();
            ViewBag.TipoEstado = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOCONVENIO").ToList();
            if (RowId_Covenio != null && RowId_Covenio != 0)
            {
                RowId_Covenio = int.Parse(Request.Params["RowId_Covenio"].ToString());
                ViewBag.listaDetalleConvenio = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowId_Covenio).OrderBy(f=> f.RowID).ToList();
                return View(db.EncabezadoConvenio.Where(t => t.RowID == RowId_Covenio).FirstOrDefault());
            }
            else
            {
                return View(new EncabezadoConvenio());
            }
        }


        [CheckSessionOutAttribute]
        public JsonResult GuardarConvenio(FormCollection formulario, int? RowID_EncabezadoConvenio)
        {
            List<DetalleConvenio> listaDetalle = null;
            String respuesta = "";
            int RowID = 0;
            EncabezadoConvenio ObjEncabezado = new EncabezadoConvenio();
            formulario = DeSerialize(formulario);
            if (RowID_EncabezadoConvenio == 0)
            {
                ObjEncabezado = CargarDatosEncabezadoConvenio(ObjEncabezado, formulario);
                try
                {
                    db.EncabezadoConvenio.Add(ObjEncabezado);
                    db.SaveChanges();
                    respuesta = "Guardado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            else if (RowID_EncabezadoConvenio != 0)//Para Actualiar
            {
                ObjEncabezado = db.EncabezadoConvenio.Where(t => t.RowID == RowID_EncabezadoConvenio).FirstOrDefault();
                ObjEncabezado = CargarDatosEncabezadoConvenio(ObjEncabezado, formulario);
                try
                {
                    db.SaveChanges();
                    listaDetalle = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == ObjEncabezado.RowID).ToList();
                    foreach (DetalleConvenio objD in listaDetalle)
                    {
                        objD.EstadoID = ObjEncabezado.EstadoID;
                        db.SaveChanges();
                    }
                    respuesta = "Actualizado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            return Json(new { respuesta = respuesta, RowID_EncabezadoConvenio = RowID });

        }

        public JsonResult cargarItemsApe(int RowID_Encabezado)
        {
            string tabla = "";
            int? idE = 0;
            int? cantidad = 0;

            List<DetalleConvenio> listaConvenio = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowID_Encabezado).ToList();
            if (listaConvenio.Count!=0)
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



        public EncabezadoConvenio CargarDatosEncabezadoConvenio(EncabezadoConvenio ObjEncabezado, FormCollection formulario)
        {
            ObjEncabezado.Nombre = formulario["Nombre"];
            ObjEncabezado.FechaInicio = ModelosPropios.Util.HoraInsertar(formulario["FechaInicial"]);
            ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinal"]);
            ObjEncabezado.EstadoID = db.Estado.FirstOrDefault().RowID; //Cambiar por el que es
            ObjEncabezado.Descripcion = ""; //Validar si toca meterla

            ObjEncabezado.TerceroID = Convert.ToInt32(formulario["Cliente"]);
            ObjEncabezado.TipoID = Convert.ToInt32(formulario["TipoConvenio"]);
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
        public JsonResult GuardarDetalleConvenio(FormCollection formulario, int RowID_EncabezadoConvenio, int Cantidad)
        {
            int con = 0;
            String respuesta = "";
            formulario = DeSerialize(formulario);
            DetalleConvenio objDetalle;
            if (RowID_EncabezadoConvenio != 0 && Cantidad != 0)
            {
                //DateTime FechaProgramacion = DateTime.Parse(formulario["FechaInicialFunciones"].ToString());
                for (int i = 0; i < Cantidad; i++)
                {
                    try
                    {
                        DetalleConvenio convenio;
                        objDetalle = new DetalleConvenio();
                        objDetalle.Nombre = formulario["NombreItem"];
                        objDetalle.TeatroAplicaID = db.Teatro.FirstOrDefault().RowID;//formulario["FechaInicialFunciones"];
                        objDetalle.Porcentaje = Convert.ToInt32(formulario["ValorItem"]);
                        objDetalle.EncabezadoConvenioID = RowID_EncabezadoConvenio;
                        objDetalle.EstadoID = db.EncabezadoConvenio.Where(f => f.RowID == RowID_EncabezadoConvenio).FirstOrDefault().EstadoID;
                        objDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                        objDetalle.FechaCreacion = DateTime.Now;
                        convenio = db.DetalleConvenio.Add(objDetalle);
                        db.SaveChanges();
                        convenio.Codigo = Hash(RowID_EncabezadoConvenio + "-" + convenio.RowID+"-"+convenio.FechaCreacion.Value.ToString("dd/MM/yyyy"));
                        con++;

                                       
                }catch (Exception ex)
                    { return Json(new { respuesta = "Error " + ex.Message }); }
                    respuesta = "Guardado Correctamente";
                }
                EncabezadoConvenio enca;
                enca = db.EncabezadoConvenio.Where(f => f.RowID == RowID_EncabezadoConvenio).FirstOrDefault();
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
                        DetalleConvenio convenio;
                        EncabezadoConvenio encabezado;
                        convenio = db.DetalleConvenio.Where(f => f.RowID == RowID_Detalle).FirstOrDefault();
                        encabezado = db.EncabezadoConvenio.Where(f => f.RowID == convenio.EncabezadoConvenioID).FirstOrDefault();
                        encabezado.Cantidad = (encabezado.Cantidad - 1);
                        db.DetalleConvenio.Remove(convenio);
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
