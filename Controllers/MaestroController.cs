using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.IO;
using System.Data.Entity;
using System.Net;
using System.Data;
using CinemaPOS.ModelosPropios;

namespace CinemaPOS.Controllers.Master
{
    public class MaestroController : Controller
    {
        ModelosPropios.Util lol = new ModelosPropios.Util();
        CinemaPOSEntities db = new CinemaPOSEntities();

        #region General
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Precio
        [CheckSessionOutAttribute]
        public ActionResult Precio(int? RowID_Lista)
        {
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.Servicios = db.Opcion.Where(s => s.Tipo.Codigo == "TIPOSERVICIO" && s.Activo == true).ToList();
            ViewBag.Formatos = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO" && f.Activo == true).ToList();
            ViewBag.TipoTarifa = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOLISTADETALLE" && f.Activo == true).ToList();
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOLISTAPRECIO").ToList();
            if (RowID_Lista != null)
            {
                return View(db.ListaEncabezado.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new ListaEncabezado());
            }
        }
        [CheckSessionOutAttribute]
        public ActionResult VistaPrecio()
        {
            ViewBag.ListasPrecio = db.ListaEncabezado.ToList();

            return View();
        }

        //public ActionResult Precio(int? rowid)
        //{
        //    ListaEncabezado encabezado = db.ListaEncabezado.Where(f => f.RowID == rowid).FirstOrDefault();
        //    ViewBag.Teatros = db.Teatro.ToList();
        //    ViewBag.Servicios = db.Opcion.Where(s => s.Tipo.Codigo == "TIPOSERVICIO").ToList();
        //    ViewBag.Formatos = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
        //    return View(encabezado);
        //}
        [CheckSessionOutAttribute]
        public JsonResult Guardar_Lista_Precio(FormCollection
            formulario, int? RowID_Encabezado)
        {

            ListaEncabezado ObjEncabezado = new ListaEncabezado();
            int estado = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOLISTAPRECIO" && e.Nombre == "EnElaboracion").FirstOrDefault().RowID;
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOLISTAPRECIO");

            if (RowID_Encabezado == 0)
            {
                formulario = DeSerialize(formulario);
                ObjEncabezado.Nombre = formulario["nombre"];
                ObjEncabezado.TeatroID = int.Parse(formulario["teatro"]);
                ObjEncabezado.Descripcion = formulario["descripcion"];
                ObjEncabezado.FechaInicial = ModelosPropios.Util.FechaInsertar(formulario["fechainicialvigencia"]);
                ObjEncabezado.FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["fechafinalvigencia"]);
                ObjEncabezado.CreadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.EstadoID = int.Parse(formulario["Estado"]);
                ObjEncabezado.FechaCreacion = DateTime.Now;
                db.ListaEncabezado.Add(ObjEncabezado);
                db.SaveChanges();
            }
            else
            {
                ObjEncabezado = db.ListaEncabezado.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                formulario = DeSerialize(formulario);
                ObjEncabezado.Nombre = formulario["nombre"];
                ObjEncabezado.TeatroID = int.Parse(formulario["teatro"]);
                ObjEncabezado.Descripcion = formulario["descripcion"];
                ObjEncabezado.FechaInicial = ModelosPropios.Util.FechaInsertar(formulario["fechainicialvigencia"]);
                ObjEncabezado.FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["fechafinalvigencia"]);
                ObjEncabezado.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.EstadoID = int.Parse(formulario["Estado"]);
                ObjEncabezado.FechaModificacion = DateTime.Now;
                db.SaveChanges();

            }
            int codigo_encabezado = ObjEncabezado.RowID;
            //var dias = formulario["dias[]"].Split('|');
            //var nombre_detalle = formulario["nombre_detalle[]"].Split('|');
            //var servicios = formulario["servicio[]"].Split(',');
            //var precio_distribuidor = formulario["precio_distribuidor[]"].Split(',');
            //var precio = formulario["precio[]"].Split(',');
            //var formato = formulario["formato[]"].Split(',');
            //var hora_inicial = formulario["hora_inicial[]"].Split(',');
            //var hora_final = formulario["hora_final[]"].Split(',');
            //var fecha_inicial = formulario["fecha_inicial[]"].Split(',');
            //var fecha_final = formulario["fecha_final[]"].Split(',');
            //ListaDetalle ObjDetalle = new ListaDetalle();
            //for (int i = 0; i < nombre_detalle.Count(); i++)
            //{

            //    ObjDetalle.ListaEncabezadoID = codigo_encabezado;
            //    ObjDetalle.TeatroID = int.Parse(formulario["teatro"]);
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];
            //    ObjDetalle.Nombre = nombre_detalle[i];

            //    ObjDetalle.Nombre = nombre_detalle[i];
            //}
            return Json(codigo_encabezado, JsonRequestBehavior.AllowGet);
        }
        [CheckSessionOutAttribute]
        public JsonResult Guardar_Precio(FormCollection formulario, int RowID_Encabezado, int? RowID_Detalle)
        {

            string respuesta = "";
            string tipoAlert = "2";
            ListaDetalle ObjDetalle = new ListaDetalle();
            ListaEncabezado objencabezado = db.ListaEncabezado.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
            int estado = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOLISTAPRECIO" && e.Nombre == "EnElaboracion").Select(s => s.RowID).First().ToString());
            // int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOSALA" && e.Nombre == "EnFuncionamiento").Select(e => e.RowID).First().ToString());
            formulario = DeSerialize(formulario);
            int tipo_tarifa = int.Parse(formulario["tipo_tarifa"]);
            if (RowID_Detalle != null)
            {
                ObjDetalle = db.ListaDetalle.Where(ld => ld.RowID == RowID_Detalle).FirstOrDefault();
                ObjDetalle.HoraInicial = Convertir_A_Hora_Militar(formulario["hora_inicial"]);
                ObjDetalle.HoraFinal = Convertir_A_Hora_Militar(formulario["hora_final"]);
                ObjDetalle.TipoListaDetalle = int.Parse(formulario["tipo_tarifa"]);
                ObjDetalle.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjDetalle.FechaModificacion = DateTime.Now;
                respuesta = "Información actualizada exitosamente";
            }

            if (RowID_Detalle == null)
            {
                int RowID_Servicio = int.Parse(formulario["servicios"]);
                int RowID_Formato = int.Parse(formulario["formato"]);
                TimeSpan horainicial = Convertir_A_Hora_Militar(formulario["hora_inicial"]);
                TimeSpan horafinal = Convertir_A_Hora_Militar(formulario["hora_final"]);
                List<ListaDetalle> obj_detalle_valida = objencabezado.ListaDetalle.Where(ld => ld.TipoFormatoID == RowID_Formato && ld.TipoServicioID == RowID_Servicio && ld.ListaEncabezadoID == RowID_Encabezado && ld.TipoListaDetalle == tipo_tarifa).ToList();
                if (horainicial >= horafinal)
                {
                    respuesta = "La hora inicial no puede ser mayor a la final ";
                }
                else
                {
                    Boolean Insertar = true;
                    int validahorario = 0;
                    int validadias = 0;
                    int validatipotarifa = 0;
                    if (obj_detalle_valida.Count != 0)
                    {
                        foreach (CinemaPOS.Models.ListaDetalle item in obj_detalle_valida)
                        {
                            validahorario = 0;
                            validadias = 0;
                            if (item.HoraInicial.Value >= horainicial && item.HoraFinal.Value <= horainicial)
                            {
                                respuesta = "El rango de tiempo seleccionado ya cuenta con una lista de precios asignada " + item.ListaEncabezado.Nombre + "  " + item.Nombre;
                                validahorario = validahorario + 1;
                            }
                            else
                            {
                                if (item.HoraInicial <= horafinal && item.HoraFinal >= horafinal)
                                {
                                    respuesta = "El rango de tiempo seleccionado ya cuenta con una lista de precios asignada " + item.ListaEncabezado.Nombre + "  " + item.Nombre;
                                    validahorario = validahorario + 1;
                                }
                            }
                            if (item.HoraInicial.Value <= horainicial && item.HoraFinal >= horainicial)
                            {
                                respuesta = "El rango de tiempo seleccionado ya cuenta con una lista de precios asignada " + item.ListaEncabezado.Nombre + "  " + item.Nombre;
                                validahorario = validahorario + 1;
                            }
                            var dias = item.DiasAsignados.Split(',');
                            string diassimilares = "";
                            for (int i = 0; i < dias.Length; i++)
                            {
                                var diasformulario = formulario["dias"].Split(',');
                                for (int j = 0; j < diasformulario.Length; j++)
                                {
                                    if (dias[i] == diasformulario[j])
                                    {
                                        diassimilares = diassimilares + "," + dias[i];
                                        validadias = validadias + 1;
                                    }
                                }
                            }
                            var lol = formulario["tipo_tarifa"];
                            var lol2 = item.TipoListaDetalle.ToString();
                            lol2 = (lol2 == "" ? lol2 = null : lol2);
                            if (lol2 == lol)
                            {
                                respuesta = "Ya existe una tarifa con la misma configuración";
                                validatipotarifa = 1;
                            }
                            //if (formulario["tipo_tarifa"] != null)
                            //{
                            //    if (item.TipoListaDetalle==int.Parse(formulario["tipo_tarifa"]))
                            //    {
                            //        validatipotarifa = 1;

                            //    }

                            //}
                            // respuesta = "" + diassimilares.TrimStart(',');

                        }
                        if (!(validadias != 0 && validahorario != 0 && validatipotarifa != 0))
                        {
                            ObjDetalle.ListaEncabezadoID = RowID_Encabezado;
                            ObjDetalle.TipoFormatoID = int.Parse(formulario["formato"]);
                            ObjDetalle.TipoServicioID = int.Parse(formulario["servicios"]);
                            ObjDetalle.HoraInicial = Convertir_A_Hora_Militar(formulario["hora_inicial"]);
                            ObjDetalle.HoraFinal = Convertir_A_Hora_Militar(formulario["hora_final"]);
                            if (formulario["tipo_tarifa"] != null)
                            {
                                ObjDetalle.TipoListaDetalle = int.Parse(formulario["tipo_tarifa"]);
                            }
                            ObjDetalle.DiasAsignados = formulario["dias"].TrimEnd(',');
                            ObjDetalle.Precio = int.Parse(formulario["precio"].Replace(".", ""));
                            ObjDetalle.PrecioDistribuidor = int.Parse(formulario["precio_distribuidor"].Replace(".", ""));
                            ObjDetalle.FechaInicial = ModelosPropios.Util.FechaInsertar(formulario["fecha_inicial"]);
                            ObjDetalle.FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["fecha_final"]);
                            ObjDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                            ObjDetalle.EstadoID = estado;
                            ObjDetalle.FechaCreacion = DateTime.Now;
                            ObjDetalle.Nombre = formulario["nombre_detalle"];
                            db.ListaDetalle.Add(ObjDetalle);
                            respuesta = "";
                        }
                    }

                    else
                    {
                        ObjDetalle.ListaEncabezadoID = RowID_Encabezado;
                        ObjDetalle.TipoFormatoID = int.Parse(formulario["formato"]);
                        ObjDetalle.TipoServicioID = int.Parse(formulario["servicios"]);
                        ObjDetalle.HoraInicial = Convertir_A_Hora_Militar(formulario["hora_inicial"]);
                        ObjDetalle.HoraFinal = Convertir_A_Hora_Militar(formulario["hora_final"]);
                        ObjDetalle.DiasAsignados = formulario["dias"];
                        if (formulario["tipo_tarifa"] != null)
                        {
                            ObjDetalle.TipoListaDetalle = int.Parse(formulario["tipo_tarifa"]);
                        }
                        ObjDetalle.Precio = int.Parse(formulario["precio"].Replace(".", ""));
                        ObjDetalle.PrecioDistribuidor = int.Parse(formulario["precio_distribuidor"].Replace(".", ""));
                        ObjDetalle.FechaInicial = ModelosPropios.Util.FechaInsertar(formulario["fecha_inicial"]);
                        ObjDetalle.FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["fecha_final"]);
                        ObjDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                        ObjDetalle.EstadoID = estado;
                        ObjDetalle.FechaCreacion = DateTime.Now;
                        ObjDetalle.Nombre = formulario["nombre_detalle"];
                        db.ListaDetalle.Add(ObjDetalle);
                    }
                }

            }

            db.SaveChanges();
            if (respuesta == "")
            {
                respuesta = "La información guardada";
                tipoAlert = "1";
            }
            return Json(new { respuesta = respuesta, tipoAlert = tipoAlert });
        }
        [CheckSessionOutAttribute]
        public string CrearTabla(int RowID_Encabezado)
        {
            string tabla = "";

            var registro = db.ListaDetalle.Where(ld => ld.ListaEncabezadoID == RowID_Encabezado);
            foreach (var item in registro)
            {
                tabla = tabla + "<tr>";
                tabla = tabla + "<td>" +
                "<div class=\"dropdown\">" +
                       "<a class=\"dropdown-toggle rowlink\" data-toggle=\"dropdown\" href=\"#\" title=\"Opciones función #" + item.RowID + "\">" +
                            "<i class=\"glyphicon glyphicon-th-list\"></i>" +
                        "</a>" +
                        "<ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"dLabel\">" +
                            "<li>" +
                                 "<a href=\'javascript:EliminarItem(" + @item.RowID + ")' class=\"ion-trash-a\">" +
                                         "Eliminar" +
                                    "</a>" +
                            "</li>" +
                            "<li>" +
                                "<a href=\'javascript:Bucar_Tarifa(" + @item.RowID + " ,1)' class=\"ion-edit\">" +
                                       "Editar" +
                                    "</a>" +
                            "</li>" +
                            "<li>" +
                                "<a href=\'javascript:Bucar_Tarifa(" + @item.RowID + " ,2)' class=\"ion-clipboard\">" +
                                       "Duplicar" +
                                    "</a>" +
                            "</li>" +
                        "</ul>" +
                    "</div>" +
                "</td>";
                tabla = tabla + "</td>";

                tabla = tabla + "<td>" + item.Nombre + "</td>";
                if (item.TipoServicioID != null)
                {
                    tabla = tabla + "<td>" + item.Opcion2.Nombre + "</td>";
                }
                else
                {
                    tabla = tabla + "<td>" + "</td>";
                }
                tabla = tabla + "<td>" + item.PrecioDistribuidor + "</td>";
                tabla = tabla + "<td>" + item.Precio + "</td>";
                tabla = tabla + "<td>" + item.Opcion.Nombre + "</td>";
                if (item.TipoListaDetalle != null)
                {
                    tabla = tabla + "<td>" + item.Opcion1.Nombre + "</td>";
                }
                else
                {
                    tabla = tabla + "<td>" + "</td>";
                }
                tabla = tabla + "<td>" + item.FechaInicial.Value.ToString("dd/MM/yyyy") + " a " + item.FechaFinal.Value.ToString("dd/MM/yyyy") + "</td>";
                tabla = tabla + "<td>" + item.HoraInicial + " a " + item.HoraFinal + "</td>";
                tabla = tabla + "<td>" + item.DiasAsignados + "</td>";
                tabla = tabla + "<td>" + item.Estado.Nombre + "</td>";
                tabla = tabla + "</tr>";
            }
            return tabla;
        }
        [CheckSessionOutAttribute]
        public JsonResult EliminarItem(int RowID_Detalle, int RowID_Encabezado)
        {
            ListaDetalle objdetalle = db.ListaDetalle.Where(lt => lt.RowID == RowID_Detalle).FirstOrDefault();
            if (objdetalle.ListaPrecioFuncion.Count()!=0)
            {
                return Json(new { RowID_Encabezado = RowID_Encabezado, tipo_respuesta = "warning", respuesta = "La tarifa no puede ser eliminada, ya se encuentra asociada a una función" });
            }
            else
            {
                db.ListaDetalle.Remove(db.ListaDetalle.Where(lt => lt.RowID == RowID_Detalle).First());
                db.SaveChanges();
            }
            return Json(new { RowID_Encabezado = RowID_Encabezado, tipo_respuesta = "success", respuesta = "Tarifa eliminada correctamente" });
        }

        public TimeSpan Convertir_A_Hora_Militar(string hora)
        {
            DateTime localTime = DateTime.Parse(hora);
            return TimeSpan.Parse(localTime.ToString("HH:mm"));
        }
        public JsonResult Buscar_Tarifa(int? RowID_Tarifa)
        {
            var objecto = (from o in db.ListaDetalle.Where(ld => ld.RowID == RowID_Tarifa).ToList()
                           select new
                           {
                               RowID = o.RowID,
                               TipoFormato = o.TipoFormatoID,
                               HoraInicial = o.HoraInicial.ToString(),
                               HoraFinal = o.HoraFinal.ToString(),
                               DiasAsignados = o.DiasAsignados,
                               Precio = o.Precio,
                               PrecioDistribuidor = o.PrecioDistribuidor,
                               Descripcion = o.Descripcion,
                               FechaInicial = o.FechaInicial.Value.ToString("dd/MM/yyyy"),
                               FechaFinal = o.FechaFinal.Value.ToString("dd/MM/yyyy"),
                               Nombre = o.Nombre,
                               TipoServicioID = o.TipoServicioID,
                           }).ToList();
            //List<ListaDetalle> obj = db.ListaDetalle.Where(ld => ld.RowID == RowID_Tarifa).ToList();
            //string lol = obj.RowID + "-" +obj.TeatroID + "-" + obj.TipoFormatoID + "-" + obj.HoraInicial + "-" + obj.HoraFinal + "-" + obj.DiasAsignados + "-" + obj.Precio + "-" + obj.PrecioDistribuidor + "-" + obj.FechaInicial + "-" + obj.FechaFinal+"-"+obj.TipoServicioID;
            var data = objecto;
            return Json(data);
        }
        #endregion

        #region Mapa
        [CheckSessionOutAttribute]
        public ActionResult MapaSala(int RowID_Sala, short? filas, short? columnas)
        {
            Sala obj_sala = new Sala();
            obj_sala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            if (filas != null && columnas != null)
            {
                obj_sala.Cantidad_Filas = filas;
                obj_sala.Cantidad_Columnas = columnas;
                db.SaveChanges();
            }
            ViewBag.TipoSillas = db.SalaObjeto.Where(so => so.Estado == true).ToList();
            //*Modificado por James Bernal/
            return View(obj_sala);
        }
        [CheckSessionOutAttribute]
        public JsonResult CargarImagenSilla(Int32 RowID_TipoSilla)
        {
            var objecto = (from st in db.SalaObjeto.Where(ld => ld.RowID == RowID_TipoSilla).ToList()
                           select new
                           {
                               RowID = st.RowID,
                               ruta = st.Imagen,
                               TipoObjeto = st.Opcion1.Nombre,
                               Numeracion = st.Numeracion
                           }).ToList();
            String ruta = db.SalaObjeto.Where(st => st.RowID == RowID_TipoSilla).Select(f => f.Imagen).FirstOrDefault();
            var data = objecto;
            return Json(data);
        }
        [CheckSessionOutAttribute]
        public JsonResult Guardar_Mapa_Sala(int RowID_Sala, FormCollection formulario)
        {
            formulario = DeSerialize(formulario);
            MapaSala Obj_MapaSala = new MapaSala();

            int contador = 0;
            Sala obj_sala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            if (RowID_Sala != 0)
            {
                db.Eliminar_sillas_sala(RowID_Sala);
                //db.Database.ExecuteSqlCommand("exec Eliminar_sillas_sala"+RowID_Sala+"");
                //var posicionx = formulario["posicionx[]"].Split(',');
                //var posiciony = formulario["posiciony[]"].Split(',');
                int? filas = obj_sala.Cantidad_Filas;
                int? columnas = obj_sala.Cantidad_Columnas;
                for (int i = 0; i < filas; i++)
                {
                    for (int j = 0; j < columnas; j++)
                    {
                        contador = contador + 1;

                        if (formulario["posicion_objeto" + i + "_" + j + ""] != null)
                        {
                            Obj_MapaSala.SalaID = RowID_Sala;
                            Obj_MapaSala.ObjetoID = int.Parse(formulario["posicion_objeto" + i + "_" + j + ""]);
                            Obj_MapaSala.PosicionX = short.Parse(formulario["posicion_x" + i + "_" + j + ""]);
                            Obj_MapaSala.PosicionY = short.Parse(formulario["posicion_y" + i + "_" + j + ""]);
                            Obj_MapaSala.Columna = formulario["posicion_columna" + i + "_" + j + ""];
                            Obj_MapaSala.Fila = short.Parse(j.ToString());
                            Obj_MapaSala.CreadoPor = Session["usuario_creacion"].ToString();
                            Obj_MapaSala.FechaCreacion = DateTime.Now;
                            db.MapaSala.Add(Obj_MapaSala);
                            db.SaveChanges();
                        }
                    }
                }
            }
            return Json("Guardado Exitosamente");
        }
        [CheckSessionOutAttribute]
        public string Get_Mapa_Sala(int RowID_Sala)
        {
            Sala ObjSala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            string Data_Table = "";
            if (RowID_Sala != 0)
            {
                for (int i = 0; i < ObjSala.Cantidad_Filas; i++)
                {
                    Data_Table = Data_Table + "<tr class='fila_" + i + "' style='padding:0px,0px,0px,0px;'>";
                    for (int j = 0; j < ObjSala.Cantidad_Columnas; j++)
                    {

                        string clase = "posicion_objeto" + i + "_" + j + "";
                        string clase_posicionX = "posicion_x" + i + "_" + j + "";
                        string clase_posicionY = "posicion_y" + i + "_" + j + "";
                        //clase ="posicion_objeto"+i+"_"+j+"";
                        Data_Table = Data_Table + " <td style = 'padding:0px;border:solid 1px; WIDTH: 50PX; HEIGHT: 50PX;'  class=" + clase + " onclick = asignar_silla('" + clase + "') ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";


                        if (ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault() != null)
                        {
                            var tipoobjeto = "objeto  " + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().SalaObjeto.Opcion1.Nombre;
                            if (ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().SalaObjeto.Numeracion == true)
                            {
                                tipoobjeto = tipoobjeto + " tipo_silla";
                            }
                            Data_Table = Data_Table + "<input type ='hidden' class='" + tipoobjeto + "' name='" + clase + "' value=" + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().ObjetoID + ">";
                            Data_Table = Data_Table + "<strong><small></small ></strong >";
                            Data_Table = Data_Table + "<img style ='width:50px' src='/" + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().SalaObjeto.Imagen + "' />";

                        }
                        Data_Table = Data_Table + "</td>";
                    }
                    Data_Table = Data_Table + " </tr>";
                }

            }
            return Data_Table;
        }
        #endregion

        #region Sala
        [CheckSessionOutAttribute]
        public ActionResult Sala(int? RowId_Sala)
        {
            ViewBag.Formatos = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO" && f.Activo == true).ToList();
            ViewBag.Servicios = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSERVICIO" && f.Activo == true).ToList();
            ViewBag.Estados = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOSALA").ToList();
            ViewBag.Audios = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOAUDIO" && f.Activo == true).ToList();
            ViewBag.Teatros = db.Teatro.Where(f=> f.Estado.Codigo == "ABIERTO").ToList();
            if (RowId_Sala != null && RowId_Sala != 0)
            {
                RowId_Sala = int.Parse(Request.Params["RowId_Sala"].ToString());
                return View(db.Sala.Where(t => t.RowID == RowId_Sala).FirstOrDefault());
            }
            else
            {
                return View(new Sala());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarSala(FormCollection formulario, int? RowID_Sala)
        {
            string respuesta = "";
            formulario = DeSerialize(formulario);
            Sala ObjSala = new Sala();
            int estadoid= int.Parse(formulario["estado"]);
            if (RowID_Sala == 0)
            {
                Estado objestado = db.Estado.Where(es => es.RowID == estadoid).FirstOrDefault();
                if (objestado.Codigo== "ENFUNCIONAMIENTO")
                {
                    if (objestado.MapaSala.Count()==0)
                    {
                        return Json(new { tipo_respuesta = "warning",respuesta= "La sala no cuenta con mapa" });
                    }
                }
                ObjSala.Nombre = formulario["nombre"];
                ObjSala.TipoAudioID = int.Parse(formulario["tipo_audio"]);
                //db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOSALA" && e.Nombre == "EnFuncionamiento").Select(e => e.RowID).First().ToString()
                ObjSala.EstadoID = int.Parse(formulario["estado"]);
                ObjSala.TeatroID = int.Parse(formulario["teatro"].ToString());
                ObjSala.Capacidad = int.Parse(formulario["capacidad"]);
                ObjSala.CreadoPor = Session["usuario_creacion"].ToString();
                ObjSala.FechaCreacion = DateTime.Now;
                ObjSala.Sincronizado = false;
                db.Sala.Add(ObjSala);
                db.SaveChanges();
                int codigo_sala = int.Parse(ObjSala.RowID.ToString());
                if (formulario["formato"] != null)
                {
                    guardar_formatos_sala(formulario["formato"], codigo_sala);
                }
                if (formulario["servicio"] != null)
                {
                    guardar_servicio_sala(formulario["servicio"], codigo_sala);
                }
                respuesta = "Guardado Correctamente";

            }
            else if (RowID_Sala != null)
            {
                Estado objestado = db.Estado.Where(es => es.RowID == estadoid).FirstOrDefault();
                if (objestado.Codigo == "ENFUNCIONAMIENTO")
                {
                    List<MapaSala> ObjetosSala = db.MapaSala.Where(f => f.SalaID == RowID_Sala).ToList();
                    if (ObjetosSala.Count() == 0)
                    {                        
                        return Json(new { tipo_respuesta = "warning", respuesta = "La sala no cuenta con mapa" });
                    }
                }
                int cantidad = db.FormatoSala.Where(fs => fs.SalaID == RowID_Sala).Count();
                for (int i = 0; i < cantidad; i++)
                {
                    FormatoSala obj_formato_sala = db.FormatoSala.Where(fs => fs.SalaID == RowID_Sala).FirstOrDefault();
                    db.FormatoSala.Remove(obj_formato_sala);
                    db.SaveChanges();
                }
                cantidad = db.ServicioSala.Where(fs => fs.SalaID == RowID_Sala).Count();
                for (int i = 0; i < cantidad; i++)
                {
                    ServicioSala obj_servicio_sala = db.ServicioSala.Where(fs => fs.SalaID == RowID_Sala).FirstOrDefault();
                    db.ServicioSala.Remove(obj_servicio_sala);
                    db.SaveChanges();
                }
                ObjSala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
                ObjSala.Nombre = formulario["nombre"];
                ObjSala.TipoAudioID = int.Parse(formulario["tipo_audio"]);
                ObjSala.EstadoID = int.Parse(formulario["estado"]);
                ObjSala.TeatroID = int.Parse(formulario["teatro"].ToString());
                ObjSala.Capacidad = int.Parse(formulario["capacidad"]);
                ObjSala.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjSala.FechaModificacion = DateTime.Now;
                ObjSala.Sincronizado = false;
                db.SaveChanges();
                /*Eliminar los formatos asignados y servicios asignados a la peliculas*/
                if (formulario["formato"] != null)
                {
                    guardar_formatos_sala(formulario["formato"], int.Parse(RowID_Sala.ToString()));
                }
                if (formulario["servicio"] != null)
                {
                    guardar_servicio_sala(formulario["servicio"], int.Parse(RowID_Sala.ToString()));
                }
                respuesta = "Actualizado Correctamente";
            }
            return Json(new {tipo_respuesta="success", respuesta = respuesta });
        }

        [CheckSessionOutAttribute]
        public Sala CargarDatosSala(Sala ObjSala, FormCollection formulario)
        {
            ObjSala.Nombre = formulario["nombre"];
            ObjSala.TipoAudioID = int.Parse(formulario["tipo_audio"]);
            ObjSala.EstadoID = int.Parse(formulario["estado"]);
            ObjSala.TeatroID = int.Parse(formulario["teatro"].ToString());
            ObjSala.Capacidad = int.Parse(formulario["capacidad"]);
            ObjSala.EstadoID = int.Parse(formulario["estado"].ToString());

            if (ObjSala.RowID == 0)
            {
                ObjSala.CreadoPor = Session["usuario_creacion"].ToString();
                ObjSala.FechaCreacion = DateTime.Now;
            }
            else
            {
                //Elminar todos los formatos que tenga asignados la sala
                for (int i = 0; i < db.FormatoSala.Where(fs => fs.SalaID == ObjSala.RowID).Count(); i++)
                {
                    FormatoSala lol = db.FormatoSala.Where(fs => fs.SalaID == ObjSala.RowID).FirstOrDefault();
                    db.FormatoSala.Remove(lol);
                    db.SaveChanges();
                }
                //Elminar todos los Servicios que tenga asignados la sala
                for (int i = 0; i < db.ServicioSala.Where(fs => fs.SalaID == ObjSala.RowID).Count(); i++)
                {
                    ServicioSala lol = db.ServicioSala.Where(fs => fs.SalaID == ObjSala.RowID).FirstOrDefault();
                    db.ServicioSala.Remove(lol);
                    db.SaveChanges();
                }
                ObjSala.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjSala.FechaModificacion = DateTime.Now;
            }
            return ObjSala;
        }

        public void guardar_servicio_sala(string cadena, int codigo_sala)
        {
            string[] servicios = cadena.Split(',');
            ServicioSala ObjServicioSala = new ServicioSala();
            for (int i = 0; i < servicios.Length; i++)
            {
                ObjServicioSala.SalaID = codigo_sala;
                ObjServicioSala.TipoServicioID = int.Parse(servicios[i].ToString());
                ObjServicioSala.CreadoPor = Session["usuario_creacion"].ToString();
                ObjServicioSala.FechaCreacion = DateTime.Now;
                db.ServicioSala.Add(ObjServicioSala);
                db.SaveChanges();
            }
        }

        public void guardar_formatos_sala(string cadena, int codigo_sala)
        {
            string[] formatos = cadena.Split(',');
            FormatoSala ObjFormatoSala = new FormatoSala();
            for (int i = 0; i < formatos.Length; i++)
            {
                ObjFormatoSala.SalaID = codigo_sala;
                ObjFormatoSala.TipoFormatoID = int.Parse(formatos[i].ToString());
                ObjFormatoSala.CreadoPor = Session["usuario_creacion"].ToString();
                ObjFormatoSala.FechaCreacion = DateTime.Now;
                db.FormatoSala.Add(ObjFormatoSala);
                db.SaveChanges();
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaSala()
        {
            ViewBag.Salas = db.Sala.ToList();
            return View();
        }


        #endregion

        #region Silla

        [CheckSessionOutAttribute]
        public ActionResult Silla(int? RowID_Silla)
        {
            ViewBag.servicios = db.Opcion.Where(f => f.TipoID == 3);
            ViewBag.TipoObjeto = db.Opcion.Where(o => o.Tipo.Codigo == "TIPOOBJETO").ToList();
            if (RowID_Silla != null)
            {
                return View(db.SalaObjeto.Where(t => t.RowID == RowID_Silla).FirstOrDefault());
            }
            else
            {
                return View(new SalaObjeto());
            }


        }

        [CheckSessionOutAttribute]
        public ActionResult VistaSilla()
        {
            ViewBag.TipoSillas = db.SalaObjeto.ToList();
          
            return View();
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarSilla(FormCollection formulario, HttpPostedFileBase documento, int? RowID_TipoSilla)
        {

            Boolean numeracion;
            Boolean estado;
            if (RowID_TipoSilla == 0)
            {
                SalaObjeto ObjSillaTipo = new SalaObjeto();

                try
                {
                    if (formulario["numeracion"] == "on")
                    {
                        numeracion = true;
                    }
                    else
                    {
                        numeracion = false;
                    }
                    if (formulario["estado"] == "on")
                    {
                        estado = true;
                    }
                    else
                    {
                        estado = false;
                    }
                    

                    string adjunto = formulario["nombre"] + Path.GetExtension(documento.FileName);
                    documento.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto));
                    ObjSillaTipo.Nombre = formulario["nombre"];
                    ObjSillaTipo.Imagen = Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto);
                    ObjSillaTipo.CreadoPor = Session["usuario_creacion"].ToString();
                    ObjSillaTipo.FechaCreacion = DateTime.Now;
                    ObjSillaTipo.Numeracion = numeracion;
                    ObjSillaTipo.ServicioID = int.Parse(formulario["servicio"]);
                    ObjSillaTipo.TipoObjetoID = int.Parse(formulario["tipo_objeto"]);
                    ObjSillaTipo.Estado = estado;
                    db.SalaObjeto.Add(ObjSillaTipo);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json("Error obteniendo la informacion del formulario " + ex.Message);
                    throw;
                }
            }
            else
            {
                string adjunto = "";

                if (documento != null)
                {
                    adjunto = formulario["nombre"] + Path.GetExtension(documento.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales"), adjunto);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(adjunto);
                        documento.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto));
                        adjunto = "Repositorio_Imagenes/Imagenes_Generales/" + adjunto;
                    }
                    else
                    {
                        documento.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto));
                        adjunto = "Repositorio_Imagenes/Imagenes_Generales/" + adjunto;
                    }

                }
                if (documento == null)
                {
                    adjunto = formulario["imagen_actualiza"];
                }
                SalaObjeto Silla_actualiza = db.SalaObjeto.Where(ts => ts.RowID == RowID_TipoSilla).FirstOrDefault();
                if (formulario["numeracion"] == "on")
                {
                    numeracion = true;
                }
                else
                {
                    numeracion = false;
                }
                if (formulario["estado"] == "on")
                {
                    estado = true;
                }
                else
                {
                    estado = false;
                }
                Silla_actualiza.Nombre = formulario["nombre"];
                Silla_actualiza.Estado = estado;
                Silla_actualiza.Imagen = adjunto;
                Silla_actualiza.ModificadoPor = Session["usuario_creacion"].ToString();
                Silla_actualiza.FechaModificacion = DateTime.Now;
                Silla_actualiza.ServicioID = int.Parse(formulario["servicio"]);
                Silla_actualiza.TipoObjetoID = int.Parse(formulario["tipo_objeto"]);
                Silla_actualiza.Numeracion = numeracion;
                db.SaveChanges();
            }


            return Json("Registro Exitoso");
        }


        [CheckSessionOutAttribute]
        public JsonResult EliminarObjeto(int RowID_TipoSilla)
        {
            String Respuesta = "";
            String TipoRespuesta = "";
            if (RowID_TipoSilla != null)
            {
                //Valido que no existan mapas con este objeto
                int CantidadObjetos = db.MapaSala.Where(f => f.ObjetoID == RowID_TipoSilla).ToList().Count();
                if (CantidadObjetos>0)
                {
                    Respuesta = "No se puede eliminar este objeto, se encuentra asociado a un mapa";
                    TipoRespuesta = "warning";
                }
                else
                {
                    try
                    {
                        SalaObjeto objeto = db.SalaObjeto.FirstOrDefault(f => f.RowID == RowID_TipoSilla);
                        db.SalaObjeto.Remove(objeto);
                        db.SaveChanges();
                        Respuesta = "Objeto Eliminado";
                        TipoRespuesta = "success";
                    }
                    catch (Exception)
                    {
                        Respuesta = "No se puede eliminar este objeto";
                        TipoRespuesta = "error";
                    }
                }

            }

            return Json(new { Respuesta = Respuesta, TipoRespuesta = TipoRespuesta });
        }



        #endregion

        #region Teatro

        [CheckSessionOutAttribute]
        public ActionResult VistaTeatro()
        {
            ViewBag.Teatros = db.Teatro.OrderByDescending(f => f.RowID);
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult Teatro(int? RowId_Teatro)
        {
            ViewBag.Companias = db.Tercero.Where(t => t.Opcion2.Codigo == "EMPRESA").ToList();
            ViewBag.ListaCentrosOperacion = db.CentroOperacion.ToList();
            ViewBag.Ciudades = db.Ciudad.ToList().OrderBy(f => f.Nombre);
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOTEATRO");
            if (RowId_Teatro != null && RowId_Teatro != 0)
            {
                RowId_Teatro = int.Parse(Request.Params["RowId_Teatro"].ToString());
                return View(db.Teatro.Where(t => t.RowID == RowId_Teatro).FirstOrDefault());
            }
            else
            {
                return View(new Teatro());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarTeatro(FormCollection formulario, int RowID_Teatro)
        {
            String respuesta = "";
            Teatro ObjTeatro = new Teatro();
            formulario = DeSerialize(formulario);
            if (RowID_Teatro == 0)
            {
                ObjTeatro = CargarDatosTeatro(ObjTeatro, formulario);
                try
                {
                    db.Teatro.Add(ObjTeatro);
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Guardado Correctamente";

            }
            if (RowID_Teatro != 0)//Para Actualiar
            {
                ObjTeatro = db.Teatro.Where(t => t.RowID == RowID_Teatro).FirstOrDefault();
                ObjTeatro = CargarDatosTeatro(ObjTeatro, formulario);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Actualizado Correctamente";
            }
            return Json(respuesta);
        }

        public Teatro CargarDatosTeatro(Teatro ObjTeatro, FormCollection formulario)
        {

            // ObjTeatro.CompaniaID = int.Parse(formulario["empresa"]);
            ObjTeatro.CentroOperacionID = int.Parse(formulario["centro_costo"]);
            ObjTeatro.IP = formulario["ip"];
            ObjTeatro.Nombre = formulario["nombre"].ToUpper();
            ObjTeatro.CiudadID = int.Parse(formulario["ciudad"]);
            ObjTeatro.EstadoID = int.Parse(formulario["Estado"]);
            ObjTeatro.CadenaBD = formulario["CadenaBD"];
            ObjTeatro.Sincronizado = false;
            if (ObjTeatro.RowID == 0)
            {
                ObjTeatro.CreadoPor = Session["usuario_creacion"].ToString();
                ObjTeatro.FechaCreacion = DateTime.Now;
            }
            else
            {
                ObjTeatro.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjTeatro.FechaModificacion = DateTime.Now;
            }
            return ObjTeatro;
        }
        public string Getip_Teatro(int? RowIDTeatro)
        {
            Teatro objteatro = db.Teatro.Where(t => t.RowID == RowIDTeatro).FirstOrDefault();
            return objteatro.IP.ToString();
        }
        #endregion

        #region Tercero

        [CheckSessionOutAttribute]
        public ActionResult VistaTercero()
        {
            ViewBag.Tercero = db.Tercero.OrderByDescending(f => f.RowID);
            return View();
        }

        // GET: Terceroes/Create
        [CheckSessionOutAttribute]
        public ActionResult Tercero(int? RowID_Tercero)
        {
            ViewBag.TipoTerceroID = new List<Opcion>();
            ViewBag.TipoTerceroID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOTERCERO" && f.Activo == true).ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO" && f.Activo == true).ToList();
            ViewBag.CiudadID = db.Ciudad.ToList().OrderBy(f => f.Nombre);
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION" && f.Activo == true).ToList();
            ViewBag.listaContactos = db.Contacto.Where(f => f.EmpresaID == RowID_Tercero).OrderBy(f => f.RowID).ToList();
            ViewBag.TipoIdentificacionDistribuidor = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACIONDISTRIBUIDOR" && f.Activo == true).ToList();
            var selloT = (from sellos in db.Sello_Distribuidor.Where(f => f.DistribuidorID == RowID_Tercero)
                          select new
                          {
                              rowID = sellos.RowID,
                              SelloID = sellos.SelloID,
                              DistribuidorID = sellos.DistribuidorID
                          }).ToList();
            ViewBag.SellosT = selloT;
            if (RowID_Tercero != null && RowID_Tercero != 0)
            {
                RowID_Tercero = int.Parse(Request.Params["RowID_Tercero"].ToString());
                return View(db.Tercero.Where(t => t.RowID == RowID_Tercero).FirstOrDefault());

            }
            else
            {
                return View(new Tercero());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarTercero(FormCollection formulario, int? RowID_Tercero)
        {
            String respuesta = "";
            Tercero ObjTercero = new Tercero();
            formulario = DeSerialize(formulario);
            if (RowID_Tercero == 0)
            {

                ObjTercero = CargarDatosTercero(ObjTercero, formulario);
                try
                {
                    db.Tercero.Add(ObjTercero);
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Guardado Correctamente";
                if (formulario["tipo_sello"]!=null)
                {
                    var rowid_Sello = formulario["tipo_sello"].Split(',');
                    foreach (var item in rowid_Sello)
                    {
                        int CSello = Convert.ToInt32(item);
                        Sello_Distribuidor sello = db.Sello_Distribuidor.Where(f => f.SelloID == CSello && f.DistribuidorID == ObjTercero.RowID).FirstOrDefault();
                        if (sello == null)
                            sello = new Sello_Distribuidor();
                        sello.DistribuidorID = ObjTercero.RowID;
                        sello.SelloID = CSello;
                        if (sello.RowID == 0)
                            db.Sello_Distribuidor.Add(sello);
                        db.SaveChanges();
                    }
                }
                


            }
            if (RowID_Tercero != 0)//Para Actualiar
            {
                ObjTercero = db.Tercero.Where(t => t.RowID == RowID_Tercero).FirstOrDefault();
                ObjTercero = CargarDatosTercero(ObjTercero, formulario);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Actualizado Correctamente";

            }
            return Json(new { respuesta = respuesta, rowid_tercero = ObjTercero.RowID });
        }

        public Tercero CargarDatosTercero(Tercero ObjTercero, FormCollection formulario)
        {
            ObjTercero.TipoTerceroID = int.Parse(formulario["tipo_tercero"].ToString());
            
            ObjTercero.Identificacion = formulario["identificacion"].ToString();
            int RowIdIdentificacion = Int32.Parse(formulario["tipo_tercero"].ToString());
            ObjTercero.Apellidos = "";
            //Si es natural le guardo el tipo de identificacion y el sexo
            if (db.Opcion.Where(f => f.RowID == RowIdIdentificacion).First().Codigo == "NATURAL")
            {
                ObjTercero.TipoIdentificacionID = Int32.Parse(formulario["tipo_identificacion"].ToString());
                ObjTercero.SexoID = Int32.Parse(formulario["sexo"].ToString());
                ObjTercero.Apellidos = formulario["apellido"];
                ObjTercero.FechaNacimiento = ModelosPropios.Util.FechaInsertar(formulario["FechaNacimiento"].ToString());
            }
            ObjTercero.Nombre = formulario["nombre"];
            ObjTercero.Telefono = formulario["telefono"];
            ObjTercero.Direccion = formulario["direccion"];
            ObjTercero.CiudadID = int.Parse(formulario["ciudad"].ToString());
            ObjTercero.Descripcion = formulario["descripcion"];
            ObjTercero.Correo = formulario["correo"];
            ObjTercero.Activo = true;
            ObjTercero.Sincronizado = false;
            if (ObjTercero.RowID == 0)
            {
                ObjTercero.CreadoPor = Session["usuario_creacion"].ToString();
                ObjTercero.FechaCreacion = DateTime.Now;
            }
            else
            {
                ObjTercero.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjTercero.FechaModificacion = DateTime.Now;
            }
            return ObjTercero;
        }
        [CheckSessionOutAttribute]
        public JsonResult ValidarExistenciaTercero(string Identificacion)
        {
            try
            {
                var data = (from terceroexiste in db.Tercero
                                                 .Where(f => f.Identificacion == Identificacion).ToList()
                            select new
                            {
                                rowid = terceroexiste.RowID,
                                identificacion = terceroexiste.Identificacion,
                                tipotercero = terceroexiste.TipoTerceroID,
                                nombre = terceroexiste.Nombre,
                                apellido = terceroexiste.Apellidos,
                                telefono = terceroexiste.Telefono,
                                ciudad = terceroexiste.CiudadID,
                                direccion = terceroexiste.Direccion,
                                correo = terceroexiste.Correo,
                                tipoidentificacion = terceroexiste.TipoIdentificacionID,
                                fechanacimiento = terceroexiste.FechaNacimiento.Value.ToString("dd/MM/yyyy"),
                                genero = terceroexiste.SexoID,
                            }).Distinct().OrderBy(f => f.rowid).Take(1);
                //data = data.Where(f => f.label.Contains(term));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarContacto(FormCollection formulario, int RowID_Tercero)
        {
            String respuesta = "";
            formulario = DeSerialize(formulario);
            Contacto contacto = new Contacto();
            if (RowID_Tercero != 0)
            {
                try
                {
                    contacto.Identificacion = formulario["Identificacion"];
                    contacto.Nombre = formulario["Nombre"];
                    contacto.Apellido = formulario["Apellido"];
                    contacto.CorreoElectronico = formulario["correoContacto"];
                    contacto.Telefono = formulario["telefonoContacto"];
                    contacto.EmpresaID = RowID_Tercero;
                    db.Contacto.Add(contacto);
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
                respuesta = "Guardado Correctamente";
            }
            return Json(respuesta);
        }

        [CheckSessionOutAttribute]
        public JsonResult ActualizarContacto(FormCollection formulario, int RowID_Contacto)
        {
            String respuesta = "";
            formulario = DeSerialize(formulario);
            Contacto contacto = new Contacto();
            if (RowID_Contacto != 0)
            {
                try
                {
                    contacto = db.Contacto.Where(f => f.RowID == RowID_Contacto).FirstOrDefault();
                    contacto.Identificacion = formulario["Identificacion"];
                    contacto.Nombre = formulario["Nombre"];
                    contacto.Apellido = formulario["Apellido"];
                    contacto.CorreoElectronico = formulario["correoContacto"];
                    contacto.Telefono = formulario["telefonoContacto"];
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
                respuesta = "Guardado Correctamente";
            }
            return Json(respuesta);
        }


        [CheckSessionOutAttribute]
        public JsonResult ConsultarContacto(int RowID_contacto)
        {
            String respuesta = "";
            Contacto contacto = new Contacto();

            try
            {
                contacto = db.Contacto.Where(f => f.RowID == RowID_contacto).FirstOrDefault();
                respuesta = "ok";
            }
            catch (Exception ex)
            { return Json(new { contacto = contacto.Nombre }); }
            return Json(new { row_id_c = contacto.RowID, identi = contacto.Identificacion, nom = contacto.Nombre, ape = contacto.Apellido, corr = contacto.CorreoElectronico, tele = contacto.Telefono });
        }

        public JsonResult RefrescarContactos(int? rowID)
        {
            var data = (from contactoTercero in db.Contacto.Where(f => f.EmpresaID == rowID)
                        select new
                        {
                            rowid = contactoTercero.RowID,
                            Identificacion = contactoTercero.Identificacion,
                            Nombre = contactoTercero.Nombre,
                            Apellido = contactoTercero.Apellido,
                            Correo = contactoTercero.CorreoElectronico,
                            Telefono = contactoTercero.Telefono
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CargarSellos()
        {
            var TipoTerceros = db.Opcion.Where(f => f.Codigo == Util.Constantes.TIPO_SELLO).FirstOrDefault();
            var data = (from sello in db.Tercero.Where(f => f.TipoTerceroID == TipoTerceros.RowID)
                            //join distribuidor in db.Sello_Distribuidor on sello.TipoTerceroID equals distribuidor.SelloID
                        select new
                        {
                            rowid = sello.RowID,
                            name = sello.Identificacion + " - " + sello.Nombre + " " + sello.Apellidos,
                            label = sello.Identificacion + " - " + sello.Nombre + " " + sello.Apellidos
                        }).Distinct().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public string CargarSellosV2(int rowID)
        {
            var TipoTerceros = db.Opcion.Where(f => f.Codigo == Util.Constantes.TIPO_SELLO).FirstOrDefault();
            List<Tercero> tercero = db.Tercero.Where(f => f.TipoTerceroID == TipoTerceros.RowID).Distinct().ToList();
            string result = "<option value='' disabled>Seleccione una Opción</option>";
            List<Sello_Distribuidor> diss = db.Sello_Distribuidor.Where(f => f.DistribuidorID == rowID).Distinct().ToList();
            bool duplicate = false;
            foreach (Tercero item in tercero)
            {
                duplicate = false;

                foreach (Sello_Distribuidor sello in diss)
                {
                    if (item.RowID == sello.SelloID)
                    {
                        duplicate = true;
                    }

                }
                if (duplicate)
                {
                    result += "<option value =" + item.RowID + " selected='selected'>" + item.Identificacion + " - " + item.Nombre + " " + item.Apellidos + "</option>";
                }
                else
                {
                    result += "<option value =" + item.RowID + ">" + item.Identificacion + " - " + item.Nombre + " " + item.Apellidos + "</option>";
                }
            }
            return result;
        }   

        #endregion

        #region Opcion
        [CheckSessionOutAttribute]
        public ActionResult Opcion(int? RowID_Lista)
        {
            //Se llena la lista del select
            ViewBag.Tipos = db.Tipo.ToList();

            if (RowID_Lista != null)
            {
                return View(db.Opcion.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Opcion());
            }
        }
        [CheckSessionOutAttribute]
        public ActionResult VistaOpcion()
        {
            ViewBag.Opcion = db.Opcion.ToList();
            return View();
        }

        [CheckSessionOutAttribute]
        public Boolean Guardar_Opcion(FormCollection formulario, int RowID_Encabezado)
        {
            Opcion ObjOpcion = new Opcion();
            Boolean accionRealizada = true;
            try
            {
                if (RowID_Encabezado == 0)
                {
                    formulario = DeSerialize(formulario);

                    ObjOpcion.TipoID = Convert.ToInt32(formulario["tipo"]);
                    ObjOpcion.Nombre = formulario["nombre"];
                    ObjOpcion.Codigo = formulario["codigo"];
                    ObjOpcion.Codigo2 = formulario["codigo2"];
                    ObjOpcion.Descripcion = formulario["descripcion"];
                    ObjOpcion.ValorDefecto = formulario["valorDefecto"];
                    ObjOpcion.NumOrden = Convert.ToInt16(formulario["numOrden"]);
                    ObjOpcion.FechaCreacion = DateTime.Now;
                    if (formulario["activo"] == null)
                    {
                        ObjOpcion.Activo = false;
                    }
                    else
                    {
                        ObjOpcion.Activo = true;
                    }
                    db.Opcion.Add(ObjOpcion);
                    db.SaveChanges();
                }
                else
                {
                    ObjOpcion = db.Opcion.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    string a = formulario["tipo"];
                    ObjOpcion.TipoID = Convert.ToInt32(formulario["tipo"]);
                    ObjOpcion.Nombre = formulario["nombre"];
                    ObjOpcion.Codigo = formulario["codigo"];
                    ObjOpcion.Codigo2 = formulario["codigo2"];
                    ObjOpcion.Descripcion = formulario["descripcion"];
                    ObjOpcion.ValorDefecto = formulario["valorDefecto"];
                    ObjOpcion.NumOrden = Convert.ToInt16(formulario["numOrden"]);
                    ObjOpcion.FechaModificacion = DateTime.Now;
                    if (formulario["activo"] == null)
                    {
                        ObjOpcion.Activo = false;
                    }
                    else
                    {
                        ObjOpcion.Activo = true;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                accionRealizada = false;
            }

            int codigo_encabezado = ObjOpcion.RowID;
            return accionRealizada;
        }


        #endregion

        #region Pais

        [CheckSessionOutAttribute]
        public ActionResult Pais(int? RowID_Lista)
        {
            if (RowID_Lista != null)
            {
                return View(db.Pais.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Pais());
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaPais()
        {
            ViewBag.Pais = db.Pais.OrderByDescending(f => f.RowID).ToList();
            return View();
        }

        [CheckSessionOutAttribute]
        public Boolean Guardar_Pais(FormCollection formulario, int? RowID_Encabezado)
        {
            Boolean tipoAlert = true;
            try
            {
                Pais objPais = new Pais();
                if (RowID_Encabezado == 0)
                {
                    formulario = DeSerialize(formulario);
                    objPais.Nombre = formulario["nombre"];
                    objPais.Descripcion = formulario["descripcion"];
                    objPais.FechaCreacion = DateTime.Now;
                    db.Pais.Add(objPais);
                    db.SaveChanges();

                }
                else
                {
                    objPais = db.Pais.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objPais.Nombre = formulario["nombre"];
                    objPais.Descripcion = formulario["descripcion"];
                    objPais.FechaModificacion = DateTime.Now;
                    db.SaveChanges();
                }
                int codigo_encabezado = objPais.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;
        }
        #endregion

        #region Departamento

        [CheckSessionOutAttribute]
        public ActionResult Departamento(int? RowID_Lista)
        {
            ViewBag.Pais = db.Pais.ToList();

            if (RowID_Lista != null)
            {
                return View(db.Departamento.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Departamento());
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaDepartamento()
        {
            ViewBag.Departamento = db.Departamento.ToList();
            return View();
        }


        [CheckSessionOutAttribute]
        public Boolean Guardar_Departamento(FormCollection formulario, int? RowID_Encabezado)
        {
            Boolean tipoAlert = true;
            try
            {
                Departamento objDep = new Departamento();
                if (RowID_Encabezado == 0)
                {
                    formulario = DeSerialize(formulario);
                    objDep.Nombre = formulario["nombre"];
                    objDep.Descripcion = formulario["descripcion"];
                    objDep.PaisID = Convert.ToInt32(formulario["pais"]);
                    db.Departamento.Add(objDep);
                    db.SaveChanges();

                }
                else
                {
                    objDep = db.Departamento.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objDep.Nombre = formulario["nombre"];
                    objDep.Descripcion = formulario["descripcion"];
                    objDep.PaisID = Convert.ToInt32(formulario["pais"]);
                    db.SaveChanges();

                }
                int codigo_encabezado = objDep.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;

        }
        [CheckSessionOutAttribute]
        public JsonResult CargarDepartamento(Int32 rowid)
        {
            List<Departamento> departamentos = db.Departamento.Where(f => f.PaisID == rowid).ToList();
            ///Para formar el Json		
            var query = (from Departamento in departamentos
                         select new
                         {
                             RowId = Departamento.RowID,
                             Nombre = Departamento.Nombre
                         }
            ).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Taquilla

        [CheckSessionOutAttribute]
        public ActionResult Taquilla(int? RowID_Lista)
        {
            ViewBag.Taquilla = db.Taquilla.ToList();
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.Estado = db.Estado.Where(e=>e.TipoEstado.Codigo== "TIPOTAQUILLA").ToList();
            if (RowID_Lista != null)
            {
                return View(db.Taquilla.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Taquilla());
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaTaquilla()
        {
            ViewBag.Taquilla = db.Taquilla.ToList();



            return View();
        }


        [CheckSessionOutAttribute]
        public Boolean Guardar_Taquilla(FormCollection formulario, int? RowID_Encabezado)
        {
            Boolean tipoAlert = true;
            try
            {
                Taquilla objTaq = new Taquilla();
                if (RowID_Encabezado == 0)
                {
                    formulario = DeSerialize(formulario);
                    objTaq.Nombre = formulario["nombre"];
                    objTaq.ConsecutivoInicial = formulario["consecutivoinicial"];
                    objTaq.ConsecutivoFinal = formulario["consecutivofinal"];
                    objTaq.FechaInicial = DateTime.Parse(formulario["fechainicial"]);
                    objTaq.FechaFinal = DateTime.Parse(formulario["fechafinal"]);
                    objTaq.IP = formulario["ip"];
                    //objTaq.Resoluciones = formulario["resoluciones"];
                    objTaq.TeatroID = Convert.ToInt32(formulario["teatro"]);
                    objTaq.EstadoID = Convert.ToInt32(formulario["estado"]);
                    objTaq.CreadoPor = Session["usuario_creacion"].ToString();
                    objTaq.FechaCreacion = DateTime.Now;

                    db.Taquilla.Add(objTaq);
                    db.SaveChanges();

                }
                else
                {
                    objTaq = db.Taquilla.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objTaq.Nombre = formulario["nombre"];
                    objTaq.ConsecutivoInicial = formulario["consecutivoinicial"];
                    objTaq.ConsecutivoFinal = formulario["consecutivofinal"];
                    objTaq.FechaInicial = DateTime.Parse(formulario["fechainicial"]);
                    objTaq.FechaFinal = DateTime.Parse(formulario["fechafinal"]);
                    objTaq.IP = formulario["ip"];
                    //objTaq.Resoluciones = formulario["resoluciones"];
                    objTaq.TeatroID = Convert.ToInt32(formulario["teatro"]);
                    objTaq.EstadoID = Convert.ToInt32(formulario["estado"]);
                    objTaq.ModificadoPor = Session["usuario_creacion"].ToString();
                    objTaq.FechaModificacion = DateTime.Now;
                    
                    db.SaveChanges();
                }
                int codigo_encabezado = objTaq.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;

        }

        #endregion

        #region Ciudad

        [CheckSessionOutAttribute]
        public ActionResult Ciudad(int? RowID_Lista)
        {
            ViewBag.Ciudad = db.Ciudad.ToList();
            ViewBag.Pais = db.Pais.ToList();

            ViewBag.Departamento = db.Departamento.ToList();

            if (RowID_Lista != null)
            {
                return View(db.Ciudad.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Ciudad());
            }
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaCiudad()
        {
            ViewBag.Ciudad = db.Ciudad.ToList();
            return View();
        }


        [CheckSessionOutAttribute]
        public Boolean Guardar_Ciudad(FormCollection formulario, int? RowID_Encabezado)
        {
            Boolean tipoAlert = true;
            try
            {
                Ciudad objCiudad = new Ciudad();
                if (RowID_Encabezado == 0)
                {
                    formulario = DeSerialize(formulario);
                    objCiudad.Nombre = formulario["nombre"];
                    objCiudad.Descripcion = formulario["descripcion"];
                    objCiudad.DepartamentoID = Convert.ToInt32(formulario["departamento"]);
                    objCiudad.FechaCreacion = DateTime.Now;
                    db.Ciudad.Add(objCiudad);
                    db.SaveChanges();

                }
                else
                {
                    objCiudad = db.Ciudad.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objCiudad.Nombre = formulario["nombre"];
                    objCiudad.Descripcion = formulario["descripcion"];
                    objCiudad.DepartamentoID = Convert.ToInt32(formulario["departamento"]);
                    objCiudad.FechaModificacion = DateTime.Now;
                    db.SaveChanges();

                }
                int codigo_encabezado = objCiudad.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;

        }


        #endregion

        #region zona

        [CheckSessionOutAttribute]
        public ActionResult Zona()
        {
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaZona()
        {
            ViewBag.Zonas = db.Zona.ToList();
            return View();
        }


        #region Ajax
        [ValidateAntiForgeryToken]
        public JsonResult Guardar(FormCollection formulario)
        {

            Zona Zona = new Zona();
            formulario = DeSerialize(formulario);
            Zona.Nombre = formulario["Nombre"];
            Zona.Prioridad = int.Parse(formulario["prioridad"]);
            Zona.FechaCreacion = DateTime.Now;
            db.Zona.Add(Zona);
            db.SaveChanges();
            return Json("Realizado");
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListaZonas()
        {
            ViewBag.Zonas = db.Zona.ToList();
            return View();
        }
        #endregion

        [ValidateAntiForgeryToken]
        public ActionResult DetailsZona(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zona zona = db.Zona.Find(id);
            if (zona == null)
            {
                return HttpNotFound();
            }
            return View(zona);
        }


        [CheckSessionOutAttribute]
        public ActionResult Create([Bind(Include = "RowID,Nombre,Prioridad,CreadoPor,FechaCreacion,ModificadoPor,FechaModificacion,Sincronizado")] Zona zona)
        {
            if (ModelState.IsValid)
            {
                db.Zona.Add(zona);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(zona);
        }

        [CheckSessionOutAttribute]
        public ActionResult EditZona(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zona zona = db.Zona.Find(id);
            if (zona == null)
            {
                return HttpNotFound();
            }
            return View(zona);
        }

        [CheckSessionOutAttribute]
        public ActionResult Edit([Bind(Include = "RowID,Nombre,Prioridad,CreadoPor,FechaCreacion,ModificadoPor,FechaModificacion,Sincronizado")] Zona zona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(zona);
        }


        [CheckSessionOutAttribute]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zona zona = db.Zona.Find(id);
            if (zona == null)
            {
                return HttpNotFound();
            }
            return View(zona);
        }


        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedZona(int id)
        {
            Zona zona = db.Zona.Find(id);
            db.Zona.Remove(zona);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region CRM CLIENTE ROYAL
        [CheckSessionOutAttribute]
        public ActionResult ClienteRoyal(int? rowid)
        {
            List<Util.ClientesRoyal> cliente = new List<Util.ClientesRoyal>();
            cliente = (from clientes in db.ClienteRoyal
                       join tarjetaM in db.TarjetaMembresiaClienteRoyal
                       on clientes.RowID equals tarjetaM.ClienteRoyalID
                       join membresia in db.TarjetaMembresia
                       on tarjetaM.TarjetaMembresiaID equals membresia.RowID
                       select new ModelosPropios.Util.ClientesRoyal
                       {
                           rowid = clientes.RowID,
                           documento = clientes.Documento,
                           correo = clientes.CorreoElectronico,
                           nombreCompleto = clientes.Nombre + " " + clientes.Apellido,
                           telefono = clientes.Telefono,
                           ciudades = (clientes.CiudadResidenciaID == null) ? "" : clientes.Ciudad.Nombre + " " + clientes.Ciudad.Departamento.Nombre,
                           genero = (clientes.GeneroID == null) ? "" : db.Opcion.Where(f => f.RowID == clientes.GeneroID).FirstOrDefault().Nombre,
                           fechaNac = clientes.FechaNacimiento,
                           tarjetaID = (membresia.Codigo == null) ? "" : membresia.Codigo,
                           info = clientes.RecibirInformacion,
                           clasificacion = (clientes.Opcion == null) ? "" : db.Opcion.Where(f => f.RowID == clientes.ClasificacionID).FirstOrDefault().Nombre,
                           pref = (clientes.preferenciasID == null) ? "" : db.Opcion.Where(f => f.RowID == clientes.preferenciasID).FirstOrDefault().Nombre
                       }).ToList();
            ViewBag.Listado = cliente;
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult NuevoClienteRoyal(int? rowID)
        {
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION" && f.Activo == true).ToList();
            ViewBag.Ciudades = db.Ciudad.ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO" && f.Activo == true).ToList();
            ViewBag.Clasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLIENTE" && f.Activo == true).ToList();
            ViewBag.preferencias = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOGENEROPELICULA" && f.Activo == true).ToList();
            TarjetaMembresiaClienteRoyal tmembresia = new TarjetaMembresiaClienteRoyal();
            try
            { tmembresia = db.TarjetaMembresiaClienteRoyal.Where(f => f.ClienteRoyalID == rowID).FirstOrDefault(); }
            catch { }
            TarjetaMembresia membresia = new TarjetaMembresia();
            try
            { membresia = db.TarjetaMembresia.Where(f => f.RowID == tmembresia.TarjetaMembresiaID).FirstOrDefault(); }
            catch { }
            ViewBag.Tarjeta = membresia.Codigo;
            ClienteRoyal royal = db.ClienteRoyal.Where(f => f.RowID == rowID).FirstOrDefault();
            if (royal == null)
                royal = new Models.ClienteRoyal();

            return View(royal);
        }

        [CheckSessionOutAttribute]
        [HttpPost]
        public JsonResult GuardarClienteRoyal(int? rowID, FormCollection formulario)
        {
            formulario = DeSerialize(formulario);
            ClienteRoyal cliente = db.ClienteRoyal.Where(f => f.RowID == rowID).FirstOrDefault();
            var TipoEstado = db.Estado.Where(f => f.Codigo == Util.Constantes.ESTADO_ACTIVO).FirstOrDefault();
            if (cliente == null || rowID == 0)
            { cliente = new Models.ClienteRoyal(); }

            try
            {
                cliente.Apellido = formulario["Apellido"];
                cliente.Nombre = formulario["Nombre"];
                cliente.Documento = formulario["Documento"];
                cliente.CorreoElectronico = formulario["CorreoElectronico"];
                cliente.Telefono = formulario["Telefono"];
                string info = formulario["RecibirInformacion"];
                if (info == "on")
                { cliente.RecibirInformacion = true; }
                else { cliente.RecibirInformacion = false; }
                cliente.FechaNacimiento = Convert.ToDateTime(formulario["FechaNacimiento"]);
                cliente.CiudadResidenciaID = Convert.ToInt32(formulario["CiudadResidenciaID"]);
                cliente.GeneroID = Convert.ToInt32(formulario["GeneroID"]);
                cliente.ClasificacionID = Convert.ToInt32(formulario["ClasificacionID"]);
                cliente.preferenciasID = Convert.ToInt32(formulario["preferenciasID"]);
                cliente.CreadoPor = Session["usuario_creacion"].ToString();
                cliente.FechaNacimiento = DateTime.Now;
                string cofd = formulario["TarjetaMembresiaClienteRoyal"].ToString();
                TarjetaMembresia membresia = db.TarjetaMembresia.Where(f => f.Codigo == cofd).FirstOrDefault();
                if (membresia == null)
                { membresia = new TarjetaMembresia(); }
                string codigo = formulario["TarjetaMembresiaClienteRoyal"];
                membresia.Codigo = codigo;
                membresia.CreadoPor = Session["usuario_creacion"].ToString();
                membresia.FechaCreacion = DateTime.Now;
                membresia.EstadoID = TipoEstado.RowID;
                membresia.NoRedenciones = Util.Constantes.REDENCIONES_No4;
                if (membresia == null || rowID == 0)
                { db.TarjetaMembresia.Add(membresia); }
                db.SaveChanges();
                if (rowID == null || rowID == 0)
                { db.ClienteRoyal.Add(cliente); }
                db.SaveChanges();

                TarjetaMembresiaClienteRoyal tarjetaID = db.TarjetaMembresiaClienteRoyal.Where(f => f.ClienteRoyalID == rowID).FirstOrDefault();
                if (tarjetaID == null || rowID == 0)
                { tarjetaID = new TarjetaMembresiaClienteRoyal(); }
                tarjetaID.TarjetaMembresiaID = membresia.RowID;
                tarjetaID.ClienteRoyalID = cliente.RowID;
                tarjetaID.FechaActivacion = DateTime.Now;
                tarjetaID.EstadoID = TipoEstado.RowID; 
                tarjetaID.NoRedencionesAprob = Util.Constantes.REDENCIONES_No4;
                if (rowID == null || rowID == 0)
                { db.TarjetaMembresiaClienteRoyal.Add(tarjetaID); }
                db.SaveChanges();
                return Json(new { respuesta = "ok", clienteRoyal = cliente.RowID });
            }
            catch (Exception e)
            {
                return Json("error" + e);
            }
        }
        #endregion

        #region CRM COTIZACIONES
        [CheckSessionOutAttribute]
        public ActionResult ListadoCotizaciones(int? rowid)
        {
            List<Util.Cotizaciones> cotizaciones = new List<Util.Cotizaciones>();

            cotizaciones = (from cotizacion in db.OportunidadVenta
                            join tercero in db.Tercero
                            on cotizacion.ProspectoID equals tercero.RowID
                            join teatros in db.Teatro
                            on cotizacion.TeatroID equals teatros.RowID
                            join lista in db.ListaEncabezado
                            on cotizacion.ListaPrecioID equals lista.RowID
                            join contac in db.Contacto
                            on cotizacion.ContactoID equals contac.RowID
                            select new Util.Cotizaciones
                            {
                                rowid = cotizacion.RowID,
                                prospecto = tercero.Nombre,
                                teatro = teatros.Nombre,
                                contacto = contac.Nombre + " " + contac.Apellido,
                                titulo = cotizacion.Titulo,
                                valor = cotizacion.Valor,
                                fechaApertura = cotizacion.FechaApertura,
                                fechaCierre = cotizacion.FechaCierra,
                                fechaEntrega = cotizacion.FechaEntrega,
                                listaPrecios = lista.Nombre,
                                descripcion = cotizacion.Descripcion,
                                prospectoID = cotizacion.ProspectoID,
                                teatroID = teatros.RowID
                            }).ToList();

            ViewBag.Listado = cotizaciones;
            return View();
        }
        [CheckSessionOutAttribute]
        public ActionResult Cotizaciones(int? rowid)
        {
            ViewBag.prospectos = db.Tercero.ToList();
            ViewBag.teatros = db.Teatro.ToList();
            var TipoEstados = db.Estado.Where(f => f.Codigo == Util.Constantes.TIPO_ESTADO_COTIZACION).FirstOrDefault();
            ViewBag.estado = db.Estado.Where(f => f.TipoEstadoID == TipoEstados.RowID).ToList();
            ViewBag.ListaPrecios = db.ListaEncabezado.ToList();
            OportunidadVenta cotizacion = db.OportunidadVenta.Where(f => f.RowID == rowid).FirstOrDefault();
            if (cotizacion == null || rowid == null)
            { cotizacion = new OportunidadVenta(); }
            return View(cotizacion);
        }
        [CheckSessionOutAttribute]
        public JsonResult ContactoAsignado(int? prospectoID)
        {
            var data = (from contacto in db.Contacto.Where(f => f.EmpresaID == prospectoID)
                        select new
                        {
                            rowidContacto = contacto.RowID,
                            nombreContacto = contacto.Nombre + " " + contacto.Apellido,
                            celular = contacto.Telefono
                        }).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [CheckSessionOutAttribute]
        [HttpPost]
        public JsonResult GuardarCotizacion(int? rowID, FormCollection formulario)
        {
            formulario = DeSerialize(formulario);
            try
            {
                OportunidadVenta venta = new OportunidadVenta();
                if ((rowID != null))
                { venta = db.OportunidadVenta.Where(f => f.RowID == rowID).FirstOrDefault(); }

                venta.ProspectoID = Convert.ToInt32(formulario["ProspectoIDx"]);
                venta.TeatroID = Convert.ToInt32(formulario["TeatroID"]);
                venta.ContactoID = Convert.ToInt32(formulario["ContactoHiden"]);
                venta.Titulo = formulario["Titulo"];
                venta.Valor = formulario["Valor"];
                venta.FechaApertura = Convert.ToDateTime(formulario["FechaApertura"]);
                venta.FechaCierra = Convert.ToDateTime(formulario["FechaCierra"]);
                venta.FechaEntrega = Convert.ToDateTime(formulario["FechaEntrega"]);
                venta.ListaPrecioID = Convert.ToInt32(formulario["ListaPrecioID"]);
                venta.Descripcion = formulario["Descripcion"];
                venta.EstadoID = Convert.ToInt32(formulario["EstadoID"]);
                if (rowID == 0 || rowID == null)
                { db.OportunidadVenta.Add(venta); }
                db.SaveChanges();
                return Json(new { respuesta = "ok", cotizaciones = venta.RowID });
            }
            catch (Exception ex)
            { return Json("error" + ex); }

        }
        #endregion

        #region ACTIVIDADES
        [CheckSessionOutAttribute]
        public ActionResult ListadoActividades()
        {
            List<Util.Actividades> actividades = new List<Util.Actividades>();

            actividades = (from actividad in db.Actividades
                           select new Util.Actividades
                           {
                               rowid = actividad.rowID,
                               asunto = actividad.asunto,
                               tipoAct = (actividad.tipoActividadID == 0) ? "" : db.Opcion.Where(f => f.RowID == actividad.tipoActividadID).FirstOrDefault().Nombre,
                               fechaInicio = actividad.fechaInicio,
                               FechaFin = actividad.fechaFin,
                               tipoRef = (actividad.tipoReferenciaID == 0) ? "" : db.Opcion.Where(f => f.RowID == actividad.tipoReferenciaID).FirstOrDefault().Nombre,
                               referenciado_a = actividad.referenciado_a,
                               prioridad = (actividad.prioridadID == 0) ? "" : db.Opcion.Where(f => f.RowID == actividad.prioridadID).FirstOrDefault().Nombre,
                               estado = (actividad.estadoID == 0) ? "" : db.Estado.Where(f => f.RowID == actividad.estadoID).FirstOrDefault().Nombre,
                               descripcion = actividad.descripcion

                           }).ToList();
            ViewBag.Listado = actividades;
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult Actividades(int? rowid)
        {
            Actividades actividad = db.Actividades.Where(f => f.rowID == rowid).FirstOrDefault();
            if (actividad == null)
            { actividad = new Models.Actividades(); }
            var TipoActivdades = db.Tipo.Where(f => f.Codigo == Util.Constantes.TIPO_ACTIVIDAD).FirstOrDefault();
            var TipoReferencias = db.Tipo.Where(f => f.Codigo == Util.Constantes.TIPO_REFERENCIA).FirstOrDefault();
            var TipoPrioridades = db.Tipo.Where(f => f.Codigo == Util.Constantes.PRIORIDAD).FirstOrDefault();
            var TipoEstados = db.Estado.Where(f => f.Codigo == Util.Constantes.ESTADOS_ACTIVIDAD).FirstOrDefault();

            ViewBag.TipoAct = db.Opcion.Where(f => f.TipoID == TipoActivdades.RowID).ToList();
            ViewBag.tipoRef = db.Opcion.Where(f => f.TipoID == TipoReferencias.RowID).ToList();
            ViewBag.Prioridad = db.Opcion.Where(f => f.TipoID == TipoPrioridades.RowID).OrderBy(f => f.NumOrden).ToList();
            ViewBag.Estados = db.Estado.Where(f => f.TipoEstadoID == TipoEstados.RowID).ToList();
            return View(actividad);
        }

        [CheckSessionOutAttribute]
        public ActionResult ModalReferenciado(int t_referencia)
        {
            ViewBag.titulo = "";
            ViewBag.columna1 = "";
            ViewBag.columna2 = "";
            ViewBag.columna3 = "";
            ViewBag.columna4 = "";
            ViewBag.columna5 = "";
            var TipoReferencias = db.Opcion.Where(f => f.RowID == t_referencia).FirstOrDefault().Codigo;
            List<Util.ModalReferencias> lista = new List<Util.ModalReferencias>();
            switch (TipoReferencias)
            {
                case Util.Constantes.ACTIVIDAD_TIPO_RELACION_Cliente:
                    ViewBag.titulo = "CLIENTES";
                    ViewBag.columna1 = "Identificación";
                    ViewBag.columna2 = "Nombre Completo";
                    ViewBag.columna3 = "Teléfono";
                    ViewBag.columna4 = "Correo";

                    lista = (from item in db.Tercero.OrderByDescending(f => f.RowID).ToList()
                             select new Util.ModalReferencias()
                             {
                                 rowid = item.RowID,
                                 valor = item.Identificacion + " - " + item.Nombre + " " + item.Apellidos,
                                 columna1 = item.Identificacion,
                                 columna2 = item.Nombre + " " + item.Apellidos,
                                 columna3 = item.Telefono,
                                 columna4 = item.Correo
                             }).ToList();

                    break;
                case Util.Constantes.ACTIVIDAD_TIPO_RELACION_Oportunidad:
                    ViewBag.titulo = "OPORTUNIDADES";
                    ViewBag.columna1 = "Prospecto";
                    ViewBag.columna2 = "Vendedor";
                    ViewBag.columna3 = "Fecha Solicitud";
                    ViewBag.columna4 = "Estado";
                    var TipoEstado = db.Estado.Where(f => f.Codigo == Util.Constantes.OPORTUNIDAD_VENTA).FirstOrDefault();
                    lista = (from item in db.OportunidadVenta.Where(f => f.EstadoID != TipoEstado.RowID).OrderByDescending(f => f.RowID).ToList()
                             select new Util.ModalReferencias()
                             {
                                 rowid = item.RowID,
                                 valor = (item.TeatroID == null) ? "" : item.Titulo + " - " + db.Teatro.Where(f => f.RowID == item.TeatroID).FirstOrDefault().Nombre,
                                 columna1 = (item.TeatroID == null) ? "" : db.Teatro.Where(f => f.RowID == item.TeatroID).FirstOrDefault().Nombre,
                                 columna2 = item.CreadoPor,
                                 columna3 = item.fechaCreacion.ToString(),
                                 columna4 = (item.EstadoID == 0) ? "" : item.Estado.Nombre
                             }).ToList();
                    break;
                case Util.Constantes.ACTIVIDAD_TIPO_RELACION_Contacto:
                    ViewBag.titulo = "CONTACTOS";
                    ViewBag.columna1 = "Identificación";
                    ViewBag.columna2 = "Nombre";
                    ViewBag.columna3 = "Tercero";
                    ViewBag.columna4 = "Teléfono";
                    lista = (from item in db.Contacto.ToList()
                             select new Util.ModalReferencias()
                             {
                                 rowid = item.RowID,
                                 valor = item.Identificacion + " - " + item.Nombre + " " + item.Apellido,
                                 columna1 = item.Identificacion,
                                 columna2 = item.Nombre + " " + item.Apellido,
                                 columna3 = (item.EmpresaID == null) ? "" : db.Tercero.Where(f => f.RowID == item.EmpresaID).FirstOrDefault().Nombre + " " + db.Tercero.Where(f => f.RowID == item.EmpresaID).FirstOrDefault().Apellidos,
                                 columna4 = item.Telefono
                             }).ToList();
                    break;
                case Util.Constantes.ACTIVIDAD_TIPO_RELACION_pqrs:
                    ViewBag.titulo = "PQRS";
                    ViewBag.columna1 = "Título";
                    ViewBag.columna2 = "Tercero";
                    ViewBag.columna3 = "Tipo Solicitud";
                    ViewBag.columna4 = "Estado";

                    lista = (from item in db.Pqrs.ToList()
                             select new Util.ModalReferencias()
                             {
                                 rowid = item.RowID,
                                 valor = item.RowID + " - " + item.Titulo,
                                 columna1 = item.Titulo,
                                 columna2 = (item.TerceroID == null) ? "" : db.Tercero.Where(f => f.RowID == item.TerceroID).FirstOrDefault().Nombre + " " + db.Tercero.Where(f => f.RowID == item.TerceroID).FirstOrDefault().Apellidos,
                                 columna3 = (item.TipoSolicitudID == null) ? "" : db.Opcion.Where(f => f.TipoID == item.TipoSolicitudID).FirstOrDefault().Nombre,
                                 columna4 = (item.EstadoID == null) ? "" : db.Estado.Where(f => f.RowID == item.EstadoID).FirstOrDefault().Nombre
                             }).ToList();
                    break;
            }
            return View(lista.ToList());
        }

        [CheckSessionOutAttribute]
        public JsonResult GetEvents()
        {
            //LISTADO DE EVENTOS PARA CARGAR EL CALENDARIO
            List<Util.fullCalendar> calendario = (from evento in (db.Actividades.OrderByDescending(f => f.fechaCreacion).ToList())
                                                  select new Util.fullCalendar()
                                                  {
                                                      title = evento.creadoPor.Trim() + ": " + evento.asunto.Trim() + " " + evento.descripcion,
                                                      start = evento.fechaInicio.ToString("s"),
                                                      end = Convert.ToString(evento.fechaFin),
                                                      backgroundColor = "#546E7A",
                                                      borderColor = "#333"
                                                  }).ToList();

            var rows = calendario.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        [CheckSessionOutAttribute]
        [HttpPost]
        public JsonResult GuardarActividad(FormCollection formulario, int? rowid)
        {
            formulario = DeSerialize(formulario);
            Actividades actividad = db.Actividades.Where(f => f.rowID == rowid).FirstOrDefault();
            if (rowid == null || rowid == 0)
            { actividad = new Models.Actividades(); }
            try
            {
                actividad.asunto = formulario["asunto"];
                actividad.tipoActividadID = Convert.ToInt32(formulario["tipoActividadID"]);
                actividad.fechaInicio = Convert.ToDateTime(formulario["fechaInicio"]);
                actividad.tipoReferenciaID = Convert.ToInt32(formulario["tipoReferenciaID"]);
                actividad.referenciado_a = formulario["referenciado_a"];
                actividad.prioridadID = Convert.ToInt32(formulario["prioridadID"]);
                actividad.estadoID = Convert.ToInt32(formulario["estadoID"]);
                actividad.descripcion = formulario["descripcion"];
                actividad.fechaCreacion = DateTime.Now;
                actividad.creadoPor = Session["usuario_creacion"].ToString();
                if (rowid == 0 || rowid == null)
                { db.Actividades.Add(actividad); }
                else
                {
                    actividad.fechaMod = DateTime.Now;
                    actividad.usuarioMod = Session["usuario_creacion"].ToString();
                }
                db.SaveChanges();
                if (rowid == null || rowid == 0)
                { MailSender.Enviar_Actividad(actividad, "PLANTILLA_ACTIVIDAD", Session["usuario_creacion"].ToString(), "", ""); }
                return Json(new { respuesta = "ok", Act = actividad.rowID });
            }
            catch (Exception e)
            { return Json(new { respuesta = "error", Act = actividad.rowID }); }
        }
        #endregion

        #region HABEAS DATA
        [CheckSessionOutAttribute]
        public ActionResult HabeasData()
        {
            Parametros Data = db.Parametros.Where(f => f.cod_parametro == "HABEASDATA").FirstOrDefault();
            return View(Data);
        }
        [CheckSessionOutAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public string GuardarHabeasData(string habeas)
        {
            Parametros habeasData = db.Parametros.Where(f => f.cod_parametro == "HABEASDATA").FirstOrDefault();
            try
            {
                habeasData.valor_parametros = habeas;
                db.SaveChanges();
                return "ok";
            }
            catch
            {
                return "error";
            }
        }
        [CheckSessionOutAttribute]
        public string CargarHabeasData()
        {
            var data = db.Parametros.Where(f => f.cod_parametro == "HABEASDATA").FirstOrDefault().valor_parametros;
            return data;
        }
        #endregion


        #region Plantillas Correos

        [CheckSessionOutAttribute]
        public ActionResult VistaPlantillasCorreo()
        {
            ViewBag.Plantillas = db.Plantillas.ToList();
            return View();
        }




        public ActionResult FormatoCorreo(int? RowID_Lista)
        {
            //ViewBag.Planillas = _db.m_plantillas_clientes.ToList();
            if (RowID_Lista != null)
            {
                Plantillas plantilla = db.Plantillas.Where(le => le.RowID == RowID_Lista).FirstOrDefault();
                //ViewBag.formato = plantilla.formato_correo;
                return View(plantilla);
            }
            else
            {
                return View(new Plantillas());
            }

        }



        #endregion
        public string CodigoEstado()
        {
            List<Estado> objestado = db.Estado.ToList();
            Estado estadoactualiza = new Estado();
            foreach (Estado item in objestado)
            {
                estadoactualiza = db.Estado.Where(e => e.RowID == item.RowID).FirstOrDefault();
                estadoactualiza.Codigo = item.Nombre.Replace(" ","").ToUpper();
                db.SaveChanges();
            }
            return "Actualizacion exitosa";
        }
        public string CodigoRol()
        {
            List<Rol> objrol = db.Rol.ToList();
            Rol rolactualiza = new Rol();
            foreach (Rol item in objrol)
            {
                rolactualiza = db.Rol.Where(e => e.RowID == item.RowID).FirstOrDefault();
                rolactualiza.Codigo = item.Nombre.Replace(" ", "").ToUpper();
                db.SaveChanges();
            }
            return "Actualizacion exitosa";
        }
    }
}