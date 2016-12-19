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
            ViewBag.Servicios = db.Opcion.Where(s => s.Tipo.Codigo == "TIPOSERVICIO").ToList();
            ViewBag.Formatos = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.TipoTarifa = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOLISTADETALLE").ToList();
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
        public JsonResult Guardar_Lista_Precio(FormCollection formulario, int? RowID_Encabezado)
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
                ObjEncabezado.FechaInicial = ModelosPropios.Util.HoraInsertar(formulario["fechainicialvigencia"]);
                ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["fechafinalvigencia"]);
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
                ObjEncabezado.FechaInicial = ModelosPropios.Util.HoraInsertar(formulario["fechainicialvigencia"]);
                ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["fechafinalvigencia"]);
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
                List<ListaDetalle> obj_detalle_valida = objencabezado.ListaDetalle.Where(ld => ld.TipoFormatoID == RowID_Formato && ld.TipoServicioID == RowID_Servicio && ld.ListaEncabezadoID == RowID_Encabezado).ToList();
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
                            ObjDetalle.DiasAsignados = formulario["dias"];
                            ObjDetalle.Precio = int.Parse(formulario["precio"]);
                            ObjDetalle.PrecioDistribuidor = int.Parse(formulario["precio_distribuidor"]);
                            ObjDetalle.FechaInicial = ModelosPropios.Util.HoraInsertar(formulario["fecha_inicial"]);
                            ObjDetalle.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["fecha_final"]);
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
                        ObjDetalle.Precio = int.Parse(formulario["precio"]);
                        ObjDetalle.PrecioDistribuidor = int.Parse(formulario["precio_distribuidor"]);
                        ObjDetalle.FechaInicial = ModelosPropios.Util.HoraInsertar(formulario["fecha_inicial"]);
                        ObjDetalle.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["fecha_final"]);
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
            db.ListaDetalle.Where(lt => lt.RowID == RowID_Detalle);
            db.ListaDetalle.Remove(db.ListaDetalle.Where(lt => lt.RowID == RowID_Detalle).First());
            db.SaveChanges();
            return Json(RowID_Encabezado);
        }

        public TimeSpan Convertir_A_Hora_Militar(string hora)
        {
            DateTime localTime = ModelosPropios.Util.HoraInsertar(hora);
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
            ViewBag.TipoSillas = db.SalaObjeto.Where(so=>so.Estado==true).ToList();
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
                               TipoObjeto = st.Opcion.Nombre,
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
                //db.Eliminar_sillas_sala(RowID_Sala);
                //db.Database.ExecuteSqlCommand("exec Eliminar_sillas_sala"+RowID_Sala+"");
                //var posicionx = formulario["posicionx[]"].Split(',');
                //var posiciony = formulario["posiciony[]"].Split(',');
                int? filas = obj_sala.Cantidad_Filas;
                int? columnas = obj_sala.Cantidad_Columnas;
                for (int i = 0; i < columnas; i++)
                {
                    for (int j = 0; j < filas; j++)
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
                for (int i = 0; i < ObjSala.Cantidad_Columnas; i++)
                {
                    Data_Table = Data_Table + "<tr class='fila_" + i + "' style='padding:0px,0px,0px,0px;'>";
                    for (int j = 0; j < ObjSala.Cantidad_Filas; j++)
                    {

                        string clase = "posicion_objeto" + i + "_" + j + "";
                        string clase_posicionX = "posicion_x" + i + "_" + j + "";
                        string clase_posicionY = "posicion_y" + i + "_" + j + "";
                        //clase ="posicion_objeto"+i+"_"+j+"";
                        Data_Table = Data_Table + " <td style = 'padding:0px;border:solid 1px; WIDTH: 50PX; HEIGHT: 50PX;'  class=" + clase + " onclick = asignar_silla('" + clase + "') ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";


                        if (ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault() != null)
                        {
                            var tipoobjeto = "objeto  " + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().SalaObjeto.Opcion.Nombre;
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
            ViewBag.Formatos = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.Servicios = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSERVICIO").ToList();
            ViewBag.Estados = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOSALA").ToList();
            ViewBag.Audios = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOAUDIO").ToList();
            ViewBag.Teatros = db.Teatro.ToList();
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
            if (RowID_Sala == 0)
            {
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
                ObjSala.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOSALA" && e.Nombre == "EnFuncionamiento").Select(e => e.RowID).First().ToString());
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
            return Json(respuesta);
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
            ViewBag.TipoSillas = db.SalaObjeto.Where(so=>so.Estado==true).ToList();
            return View();
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarSilla(FormCollection formulario, HttpPostedFileBase documento, int? RowID_TipoSilla)
        {

            //formulario = DeSerialize(formulario);
            Boolean numeracion;
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
                    string adjunto = formulario["nombre"] + Path.GetExtension(documento.FileName);
                    documento.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto));
                    ObjSillaTipo.Nombre = formulario["nombre"];
                    ObjSillaTipo.TipoObjetoID = int.Parse(formulario["tipo_objeto"]);
                    ObjSillaTipo.Imagen = "Repositorio_Imagenes/Imagenes_Generales/" + adjunto;
                    ObjSillaTipo.CreadoPor = Session["usuario_creacion"].ToString();
                    ObjSillaTipo.FechaCreacion = DateTime.Now;
                    ObjSillaTipo.Estado = true;
                    ObjSillaTipo.Numeracion = numeracion;
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
                Silla_actualiza.Nombre = formulario["nombre"];
                Silla_actualiza.Imagen = adjunto;
                Silla_actualiza.TipoObjetoID = int.Parse(formulario["tipo_objeto"]);
                Silla_actualiza.CreadoPor = Session["usuario_creacion"].ToString();
                Silla_actualiza.FechaCreacion = DateTime.Now;
                Silla_actualiza.Numeracion = numeracion;
                db.SaveChanges();
            }


            return Json("Registro Exitoso");
        }

        public string EliminarObjeto(int RowID_Objeto)
        {
            SalaObjeto ObjetoElimina = db.SalaObjeto.Where(so => so.RowID == RowID_Objeto).FirstOrDefault();
            ObjetoElimina.Estado = false;
            db.SaveChanges();
            return "Eliminado";
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
            ObjTeatro.CentroOperacion = formulario["centro_costo"];
            ObjTeatro.IP = formulario["ip"];
            ObjTeatro.Nombre = formulario["nombre"].ToUpper();
            ObjTeatro.CiudadID = int.Parse(formulario["ciudad"]);
            ObjTeatro.EstadoID = int.Parse(formulario["Estado"]);
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
            ViewBag.TipoTerceroID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOTERCERO").ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO").ToList();
            ViewBag.CiudadID = db.Ciudad.ToList().OrderBy(f => f.Nombre);
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION").ToList();
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
        public JsonResult GuardarTercero(FormCollection formulario, int RowID_Tercero)
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
            ObjTercero.FechaNacimiento = ModelosPropios.Util.HoraInsertar(formulario["FechaNacimiento"].ToString());
            ObjTercero.Identificacion = formulario["identificacion"].ToString();
            int RowIdIdentificacion = Int32.Parse(formulario["tipo_tercero"].ToString());
            ObjTercero.Apellidos = "";
            //Si es natural le guardo el tipo de identificacion y el sexo
            if (db.Opcion.Where(f => f.RowID == RowIdIdentificacion).First().Codigo == "NATURAL")
            {
                ObjTercero.TipoIdentificacionID = Int32.Parse(formulario["tipo_identificacion"].ToString());
                ObjTercero.SexoID = Int32.Parse(formulario["sexo"].ToString());
                ObjTercero.Apellidos = formulario["apellido"];
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
            ViewBag.Pais = db.Pais.ToList();
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
                    db.Pais.Add(objPais);
                    db.SaveChanges();

                }
                else
                {
                    objPais = db.Pais.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objPais.Nombre = formulario["nombre"];
                    objPais.Descripcion = formulario["descripcion"];
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

        #endregion

        #region Taquilla

        [CheckSessionOutAttribute]
        public ActionResult Taquilla(int? RowID_Lista)
        {
            ViewBag.Taquilla = db.Taquilla.ToList();
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.Estado = db.Estado.ToList();

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
                    objTaq.Resoluciones = formulario["resoluciones"];
                    objTaq.TeatroID = Convert.ToInt32(formulario["teatro"]);
                    objTaq.EstadoID = Convert.ToInt32(formulario["estado"]);
                    objTaq.CreadoPor = Session["usuario_creacion"].ToString();
                    objTaq.FechaCreacion = DateTime.Now;
                    if (formulario["sincronizado"] == null)
                    {
                        objTaq.Sincronizado = false;

                    }
                    else
                    {
                        objTaq.Sincronizado = true;

                    }

                    db.Taquilla.Add(objTaq);
                    db.SaveChanges();

                }
                else
                {
                    objTaq = db.Taquilla.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objTaq.Nombre = formulario["nombre"];
                    objTaq.Resoluciones = formulario["resoluciones"];
                    objTaq.TeatroID = Convert.ToInt32(formulario["teatro"]);
                    objTaq.EstadoID = Convert.ToInt32(formulario["estado"]);
                    objTaq.ModificadoPor = Session["usuario_creacion"].ToString();
                    objTaq.FechaModificacion = DateTime.Now;
                    if (formulario["sincronizado"] == null)
                    {
                        objTaq.Sincronizado = false;

                    }
                    else
                    {
                        objTaq.Sincronizado = true;

                    }
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
    }
}