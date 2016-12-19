using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;

namespace CinemaPOS.Controllers
{
    public class POSController : Controller
    {
        //
        // GET: /POS/

        CinemaPOSEntities db = new CinemaPOSEntities();
        public ActionResult VistaPrincipal()
        {
            #region Comentarios

            //string hoy ="7/11/2016"/* DateTime.Now.ToString("dd/MM/yyyy")*/;
            //DateTime hoy1 = ModelosPropios.Util.HoraInsertar(hoy);
            ////ViewBag.FuncionesHoy = db.Funcion.Where(f => f.Fecha == hoy1).GroupBy(f => f.DetallePeliculaID).ToList();
            ////)
            //ViewBag.Funciones = new ModelosPropios.Model.Funcion_Vista();
            //ViewBag.Funciones = from dt in db.DetallePelicula.Where(f=>f.Funcion.Count()>0).ToList()
            //                    select new ModelosPropios.Model.Funcion_Vista
            //                    {
            //                        Detalle_Pelicula = dt,
            //                        Funciones = db.Funcion.Where(f => f.DetallePeliculaID == dt.RowID && f.Fecha==hoy1).ToList()
            //                    };
            //var datavista = from dt in db.DetallePelicula.Where(f => f.Funcion.Count() > 0).ToList()
            //                    select new ModelosPropios.Model.Funcion_Vista
            //                    {
            //                        Detalle_Pelicula = dt,
            //                        Funciones = db.Funcion.Where(f => f.DetallePeliculaID == dt.RowID && f.Fecha == hoy1).ToList()
            //                    };
            //string htmlvista = "";
            //short contadorpeliculas = 1;
            //short contadorfunciones = 1;
            //foreach (var peliculas_vista in datavista)
            //{
            //    if (contadorfunciones==1)
            //    {
            //        htmlvista = htmlvista + "<div class='row'>";
            //    }

            //            htmlvista = htmlvista + "<div class='col-sm-2'>";
            //                htmlvista = htmlvista + "<img style='width: 90 %; height: 240px; ' src='/" + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.MedioPelicula.Where(m => m.EncabezadoPeliculaID == peliculas_vista.Detalle_Pelicula.EncabezadoPeliculaID).OrderByDescending(mp => mp.RowID).FirstOrDefault().Afiche + "'>";
            //                 htmlvista = htmlvista + "<div class='text - primary text - center caja - horario'>";
            //                    htmlvista = htmlvista + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.TituloLocal + "<br />" + peliculas_vista.Detalle_Pelicula.Opcion.Nombre + peliculas_vista.Detalle_Pelicula.Opcion1.Nombre + " </div >";
            //    foreach (var item in peliculas_vista.Funciones)
            //    {
            //        htmlvista = htmlvista + "<a class='horarios' onclick='cargar_sala(" + item.RowID + "," + item.SalaID + ")'>" + item.HoraInicial.Value.ToString("HH:mm") + "</a>";
            //    }
            //            htmlvista = htmlvista + "</div >";
            //    contadorfunciones++;
            //    if (contadorfunciones == 6)
            //    {
            //        htmlvista = htmlvista + "</div>";
            //    }
            //}
            //ViewBag.htmlvista = htmlvista;
            #endregion

            //return Content(htmlvista());
            return View();
        }

        ////public string htmlvista()
        ////{
        ////    string hoy = "7/11/2016"/* DateTime.Now.ToString("dd/MM/yyyy")*/;
        ////    DateTime hoy1 = ModelosPropios.Util.HoraInsertar(hoy);
        ////    var hora_actual = (DateTime.Now.ToString("HH:mm")).Split(':');
        ////    int minutosactuales = int.Parse(hora_actual[0] + hora_actual[1]);
        ////    var datavista = from dt in db.DetallePelicula.Where(f => f.Funcion.Count() > 0).ToList()
        ////                    select new ModelosPropios.Model.Funcion_Vista
        ////                    {
        ////                        Detalle_Pelicula = dt,
        ////                        Funciones = db.Funcion.Where(f => f.DetallePeliculaID == dt.RowID && f.Fecha == hoy1).ToList()
        ////                    };
        ////    string htmlvista = "";
        ////    short contadorpeliculas = 1;
        ////    short contadorfunciones = 1;
        ////    foreach (var peliculas_vista in datavista)
        ////    {
        ////        if (contadorpeliculas == 1)
        ////        {
        ////            htmlvista = htmlvista + "<div class='row'>";
        ////        }
        ////        htmlvista = htmlvista + "<div class='col-sm-2'>";
        ////        htmlvista = htmlvista + "<img style='width: 90 %; height: 240px; ' src='/" + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.MedioPelicula.Where(m => m.EncabezadoPeliculaID == peliculas_vista.Detalle_Pelicula.EncabezadoPeliculaID).OrderByDescending(mp => mp.RowID).FirstOrDefault().Afiche + "'>";
        ////        htmlvista = htmlvista + "<div class='text-primary text-center caja-horario'>";
        ////        htmlvista = htmlvista + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.TituloLocal + "<br />" + peliculas_vista.Detalle_Pelicula.Opcion.Nombre + peliculas_vista.Detalle_Pelicula.Opcion1.Nombre + " </div >";
        ////        foreach (var item in peliculas_vista.Funciones)
        ////        {
        ////            TimeSpan diferenciahorario = DateTime.Now - item.HoraInicial.Value;
        ////            if ((diferenciahorario.Hours== 0 && diferenciahorario.Minutes >=20) || (diferenciahorario.Hours == 1 && diferenciahorario.Minutes <=20))
        ////            {
        ////                //la funcion proximanete sera cerrada para la venta verificar
        ////                htmlvista = htmlvista + "<div class='col-sm-4'>";
        ////                htmlvista = htmlvista + "<a class='horarios proximamente_iniciar' onclick='cargar_sala(" + item.RowID + "," + item.SalaID + ")'>" + item.HoraInicial.Value.ToString("HH:mm") + "</a>";
        ////                htmlvista = htmlvista + "</div>";
        ////            }
        ////            else if (diferenciahorario.Hours <= 0 && diferenciahorario.Minutes <=40)
        ////            {
        ////                htmlvista = htmlvista + "<div class='col-sm-4'>";
        ////                htmlvista = htmlvista + "<a class='horarios proximamente_cerrar ' onclick='cargar_sala(" + item.RowID + "," + item.SalaID + ")'>" + item.HoraInicial.Value.ToString("HH:mm") + "</a>";
        ////                htmlvista = htmlvista + "</div>";
        ////            }
        ////            else
        ////            {
        ////                htmlvista = htmlvista + "<div class='col-sm-4'>";
        ////                htmlvista = htmlvista + "<a class='horarios' onclick='cargar_sala(" + item.RowID + "," + item.SalaID + ")'>" + item.HoraInicial.Value.ToString("HH:mm") + "</a>";
        ////                htmlvista = htmlvista + "</div>";
        ////            }
        ////            //if (contadorfunciones==1)
        ////            //{
        ////            //    htmlvista = htmlvista + "<div class='row'>";
        ////            //}

        ////            //if (contadorfunciones==2)
        ////            //{
        ////            //    htmlvista = htmlvista + "</div>";
        ////            //    contadorfunciones = 1;
        ////            //}
        ////            //contadorfunciones++;
        ////        }
        ////        htmlvista = htmlvista + "</div >";
        ////        contadorpeliculas++;
        ////        if (contadorpeliculas == 7 )
        ////        {
        ////            htmlvista = htmlvista + "</div>";
        ////        }
        ////    }
        ////    return htmlvista;
        ////}
        public ActionResult MapaVenta(int? SalaID, int? FuncionID)
        {
            //MapaSala =db.MapaSala          
            return View();
        }

        public string GetFechaFiltro(int? accion, string fechavista)
        {
           
            //DateTime fecha = ModelosPropios.Util.HoraInsertar("11-15-2016");
            DateTime fecha = ModelosPropios.Util.HoraInsertar(fechavista);
            if (fecha != DateTime.Now.Date || accion == 1)
            {
                if (accion == 1)
                {
                    return fecha.AddDays(1).ToLongDateString();
                }
                else
                {
                    return fecha.AddDays(-1).ToLongDateString();
                }
            }
            else { return DateTime.Now.ToLongDateString(); }

            //return DateTime.Now.AddDays(1).ToLongDateString();


        }

        public string htmlvista(string fecha_programacion, int? multicineID)
        {
            string hoy = "12/12/2016"/* DateTime.Now.ToString("dd/MM/yyyy")*/;
            //cuadno la programacion sea creada quitar comentarios y enviar en el where conversionfecha
            DateTime hoy1 = ModelosPropios.Util.HoraInsertar(fecha_programacion);
            string conversionfecha = hoy1.ToString("dd/mm/yyyy");
            var hora_actual = (DateTime.Now.ToString("HH:mm")).Split(':');
            int minutosactuales = int.Parse(hora_actual[0] + hora_actual[1]);
            var datavista = from dt in db.DetallePelicula.ToList()
                            select new ModelosPropios.Model.Funcion_Vista
                            {
                                Detalle_Pelicula = dt,
                                Funciones = db.Funciones.Where(f => f.DetallePeliculaID == dt.RowID && f.FechaFuncion == hoy && f.RowIDTeatro == 23/*multicineID*/).ToList()
                            };
            string html = "";
            foreach (var peliculas_vista in datavista)
            {
                if (peliculas_vista.Funciones.Count() != 0)
                {


                    bool cerrar = true;
                    bool carrusel = true;
                    int contador_div = 1;
                    int contador_funciones_pelicula = 1;
                    html += "<tr>";
                    html += "<td>";
                    html += "<img class='poster-peliculas' src='/" + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.MedioPelicula.Where(m => m.EncabezadoPeliculaID == peliculas_vista.Detalle_Pelicula.EncabezadoPeliculaID).OrderByDescending(mp => mp.RowID).FirstOrDefault().Afiche + "' > ";
                    html += "</td>";
                    html += "<td  nombre-peliculas'>";
                    html += "<h5>" + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.TituloLocal + "</h5>";
                    html += "<h5>" + peliculas_vista.Detalle_Pelicula.Opcion.Nombre + peliculas_vista.Detalle_Pelicula.Opcion1.Nombre + "</h5>";
                    html += "<h5>" + peliculas_vista.Detalle_Pelicula.EncabezadoPelicula.Opcion1.Nombre + "</h5>";
                    html += "</td>";
                    html += "<td>";
                    html += "<div id = '" + peliculas_vista.Detalle_Pelicula.RowID + "' class='carousel slide' data-ride='carousel' data-interval='false'>";
                    html += "<div class='carousel-inner text-center' style='margin-left:40px;'>";
                    foreach (var funciones in peliculas_vista.Funciones)
                    {
                        if (peliculas_vista.Funciones.Count() > 3)
                        {

                            if (contador_div == 1)
                            {
                                html += "<div class='item active'>";
                                contador_div += 1;
                            }
                            else
                            {
                                if (peliculas_vista.Funciones.Count() >= 3 && contador_funciones_pelicula == 1)
                                {
                                    html += "<div class='item'>";
                                    contador_div += 1;
                                }
                            }
                        }
                        else { carrusel = false; }
                        //html += "<div class='col-sm-3 funcion mar-hor' onclick='javascrip:get_tarifas(" + funciones.RowID + ")'>";
                        html += "<h5 class='text-main'>" + funciones.HoraInicial.Value + "</h5>";
                        html += "<p>" + funciones.NombreSala + "<br />Disponible: 120</p>";
                        html += "</div>";
                        if (contador_funciones_pelicula == 3)
                        {
                            html += "</div>";
                            contador_funciones_pelicula = 0;
                            cerrar = false;
                        }
                        contador_funciones_pelicula++;

                    }
                    if (cerrar == false)
                    {
                        html += "</div>";
                    }
                    html += "</div>";
                    if (carrusel == true)
                    {
                        html += "<a class='carousel-control left' data-slide='prev' href='#" + peliculas_vista.Detalle_Pelicula.RowID + "' style='margin-left:10px;'><i class='demo-pli-arrow-left icon-3x'></i></a>";
                        html += "<a class='carousel-control right' data-slide='next' href='#" + peliculas_vista.Detalle_Pelicula.RowID + "' style='margin-right:-30px;'><i class='demo-pli-arrow-right icon-3x'></i></a>";
                    }

                    html += "</div>";

                    html += "</div>";
                    html += "</td>";
                    html += "</tr>";
                }
            }

            return html;
        }

        public string Get_Tarifas_Funcion(int RowID_Funcion)
        {
            string html_tarifas = "";
            List<ListaPrecioFuncion> Tarifas_de_funcion = db.ListaPrecioFuncion.Where(t => t.FuncionID == RowID_Funcion).OrderByDescending(t => t.RowID).ToList();
            int contador_row = 1;
            bool cerrar_row = true;
            try
            {
                if (Tarifas_de_funcion.FirstOrDefault().Funcion.SalaID != 0)
                {
                    var lol = Tarifas_de_funcion.FirstOrDefault().Funcion.SalaID;
                    html_tarifas += "<input type='hidden' id='SalaID' value='" + lol + "'>";
                }
            }
            catch (Exception)
            { }


            foreach (ListaPrecioFuncion tarifafuncion in Tarifas_de_funcion)
            {
                string clase = "panel-mint";
                if (tarifafuncion.ListaDetalle.Opcion2 != null)
                {
                    clase = "" + tarifafuncion.ListaDetalle.Opcion2.Codigo2 + "";
                }
                if (contador_row == 1)
                {
                    html_tarifas += "<div class='row'>";
                    cerrar_row = false;
                }
                html_tarifas += "<div class='col-sm-6 mar-hor' style='width:40%;' onclick='javascript:adicionar_item(" + tarifafuncion.RowID + ")'>";
                html_tarifas += "<div class='panel " + clase + " panel-colorful' style='height:50%'>";
                html_tarifas += "<div class='pad-all text-center'>";
                html_tarifas += "<span class='text-2x text-thin'>$" + tarifafuncion.ListaDetalle.Precio + "</span>";
                html_tarifas += "<p>" + tarifafuncion.ListaDetalle.Nombre + "</p>";
                html_tarifas += "</div>";
                html_tarifas += "</div>";
                html_tarifas += "</div>";
                if (contador_row == 2)
                {
                    html_tarifas += "</div>";
                    contador_row = 0;
                }
                contador_row++;
            }
            if (cerrar_row == false)
            {
                html_tarifas += "</div>";
            }

            return html_tarifas;
        }
        public string AdicionarItemVenta(int RowID_ListaFuncion, short? cantidad)
        {
            ListaPrecioFuncion ItemVenta = db.ListaPrecioFuncion.Where(lpf => lpf.RowID == RowID_ListaFuncion).FirstOrDefault();
            string html_item_venta = "";
            html_item_venta += "<div class='panel panel-primary panel-colorful item-venta item-elimina" + ItemVenta.RowID + "'  onclick='javascript:eliminar_item(" + ItemVenta.RowID + ")'>";
            html_item_venta += "<div class='pad-all media'>";
            html_item_venta += "<div class='media-left'>";
            html_item_venta += "<span class='text-2x text-bold' id='cantidad-total-" + ItemVenta.RowID + "'>" + cantidad + "</span>";
            html_item_venta += "</div>";
            html_item_venta += "<div class='media-body'>";
            html_item_venta += "<p class='h3 text-light mar-no media-heading'>" + ItemVenta.Funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + "&nbsp;" + ItemVenta.Funcion.DetallePelicula.Opcion.Nombre + "&nbsp;" + ItemVenta.Funcion.DetallePelicula.Opcion1.Nombre + "</p>";
            html_item_venta += "<span class='suma-costo-pedido' id='total-costo" + ItemVenta.RowID + "'>$ " + ItemVenta.ListaDetalle.Precio * cantidad + "</span>";
            html_item_venta += "<input type='hidden' class='precio-items-" + ItemVenta.RowID + "' value='" + ItemVenta.ListaDetalle.Precio + "'>";
            html_item_venta += "<input type='hidden' class='cantidad-boleta-" + ItemVenta.RowID + "' value='" + cantidad + "'></span>";
            html_item_venta += "<input type='hidden' class='tarifas_id' value='" + ItemVenta.ListaDetalle.RowID + "'></span>";
            html_item_venta += "</div>";
            html_item_venta += "</div>";

            //<div class='progress progress-xs progress-dark-base mar-no'>
            //	<div role = 'progressbar' aria-valuenow='75' aria-valuemin='0' aria-valuemax='100' class='progress-bar progress-bar-light' style='width: 75%'></div>
            //</div>
            html_item_venta += "<div class='pad-all text-sm bg-trans-dark'>";
            html_item_venta += "<span class='text-bold'>" + ItemVenta.ListaDetalle.Nombre + "</span>";
            html_item_venta += "<a href=\"javascript:cancelar_venta(\'item-elimina" + ItemVenta.RowID + "','" + ItemVenta.RowID + "\')\" class='close'><i class='ion-ios-close text-2x'></i></a>";
            html_item_venta += "</div>";
            html_item_venta += "</div>";
            html_item_venta += "<input type='hidden' class='item-ventas " + ItemVenta.RowID + "' >";
            return html_item_venta;
        }

        public ActionResult Mapa_Sala_funcion(int RowID_Sala, short? cantidad_sillas_venta,int RowIDFuncion,string RowIDTarifas)
        {
            Sala obj_sala = new Sala();
            obj_sala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            ViewBag.TipoSillas = db.SalaObjeto.ToList();
            int idtarifa = int.Parse(RowIDTarifas.Split(',')[0]);
            ViewBag.Tarifa = db.ListaDetalle.Where(t=>t.RowID== idtarifa).FirstOrDefault();
            ViewBag.CantidadSillasVenta = cantidad_sillas_venta;
            ViewBag.Funcion =db.Funcion.Where(f=>f.RowID== RowIDFuncion).FirstOrDefault();
            return View(obj_sala);
        }
        public string Get_Mapa_Sala(int RowID_Sala,int?RowID_Funcion)
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


                        MapaSala objmapa_sala = ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j && s.SalaID == ObjSala.RowID).FirstOrDefault();
                        ///BoletaVendida objBoleta = db.BoletaVendida.Where(bv=>bv.SillaID==objmapa_sala.RowID &&bv.FuncionID==RowID_Funcion).FirstOrDefault();
                        if (objmapa_sala != null)
                        {
                            if (objmapa_sala.SalaObjeto.Opcion.Codigo == "SILLA")
                            {
                                //if (objBoleta!=null)
                                //{
                                   Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class=" + clase + " style='background: #B0BEC5;' ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
                                //}
                                //else
                                //{
                                   //Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class=" + clase + " onclick = vender_silla(this) ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
                                //}
                                
                            }
                            else
                            {
                                Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class=" + clase + "><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
                            }
                            var tipoobjeto = "objeto  " + objmapa_sala.SalaObjeto.Opcion.Nombre;
                            if (objmapa_sala.SalaObjeto.Numeracion == true)
                            {
                                tipoobjeto = tipoobjeto + " tipo_silla";
                            }
                            Data_Table = Data_Table + "<input type ='hidden' class='" + tipoobjeto + "' name='" + clase + "' value=" + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().ObjetoID + ">";
                            Data_Table = Data_Table + "<strong><small></small ></strong >";
                            Data_Table = Data_Table + "<img style ='border:none;width:50px' src='/" + objmapa_sala.SalaObjeto.Imagen + "' />";

                        }
                        else
                        {
                            Data_Table = Data_Table + " <td style ='border:none; padding:0px; WIDTH: 50PX; HEIGHT: 50PX;'  class=" + clase + " ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
                        }
                        Data_Table = Data_Table + "</td>";
                    }
                    Data_Table = Data_Table + " </tr>";
                }

            }
            return Data_Table;
        }
        public ActionResult pruebabuscador()
        {
            return View();
        }
        public JsonResult TerminarVenta(string IDSillas, string IDTarifas,int RowIDFuncion)
        {
            BoletaVendida objBoleta = new BoletaVendida();
            var RowIDSillas = IDSillas.TrimEnd(',').Split(',');
            int RowIDTarifa = int.Parse(IDTarifas.Split(',')[0].ToString());
            int RowIDSilla = 0;
            int CantidadBoletasValidadas = 0;
            string Respuesta = "";
            List<BoletaVendida> ListaBoletas = new List<Models.BoletaVendida>(); 
            for (int i = 0; i < RowIDSillas.Length; i++)
            {
                RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                objBoleta = db.BoletaVendida.Where(BV=>BV.SillaID== RowIDSilla &&BV.FuncionID== RowIDFuncion).FirstOrDefault();
                if (objBoleta!=null)
                {
                    ListaBoletas.Add(objBoleta);
                }
            }
            if (ListaBoletas.Count()==0)
            {
                BoletaVendida objBoletaVenta = new BoletaVendida();
                for (int i = 0; i < RowIDSillas.Length; i++)
                {
                    RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                    objBoletaVenta.FuncionID = RowIDFuncion;
                    objBoletaVenta.TarifaID = RowIDTarifa;
                    RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                    objBoletaVenta.SillaID = RowIDSilla;
                    objBoletaVenta.FechaVenta = DateTime.Now;
                    objBoletaVenta.MedioPago = "Efectivo";
                    objBoletaVenta.TaquillaID = null;
                    db.BoletaVendida.Add(objBoletaVenta);
                    db.SaveChanges();
                }
                Respuesta = "Transacción exitosa";
            }
            else if (ListaBoletas.Count()!=0)
            {
                Respuesta = "Las siguientes boletas ya se encuentran vendidas";
            }
            return Json(new {Respuesta=Respuesta,data=ListaBoletas });
        }
    }
}
