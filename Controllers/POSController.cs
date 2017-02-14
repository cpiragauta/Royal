using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Drawing.Printing;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using CinemaPOS.ModelosPropios;
using System.Data;
using System.Drawing.Printing;
using CinemaPOS.ModelosPropios;
namespace CinemaPOS.Controllers
{
    public class POSController : Controller
    {
        
        public void imprimi()
        {
            PrintDocument document = new PrintDocument();
        }
        // GET: /POS/

        CinemaPOSEntities db = new CinemaPOSEntities();
        [CheckSessionOutAttribute]
        public ActionResult VistaPrincipal()
        {

            int RowID_Taquilla = int.Parse(Session["RowID_Taquilla"].ToString());
            ViewBag.InfoTaquilla = db.Taquilla.Where(taq => taq.RowID == RowID_Taquilla).FirstOrDefault();
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
        [CheckSessionOutAttribute]
        public ActionResult MapaVenta(int? SalaID, int? FuncionID)
        {
            //MapaSala =db.MapaSala          
            return View();
        }
        [CheckSessionOutAttribute]
        public string GetFechaFiltro(int? accion, string fechavista)
        {

            //DateTime fecha = ModelosPropios.Util.HoraInsertar("11-15-2016");
            DateTime fecha = DateTime.Parse(fechavista);
            if (fecha != DateTime.Now.Date || accion == 1)
            {
                if (accion == 1)
                {
                    return fecha.AddDays(1).ToLongDateString();
                }
                else if (accion == 2)
                {
                    return fecha.AddDays(-1).ToLongDateString();
                }
                return DateTime.Now.ToLongDateString();
            }
            else { return DateTime.Now.ToLongDateString(); }

            //return DateTime.Now.AddDays(1).ToLongDateString();


        }
        [CheckSessionOutAttribute]
        public string htmlvista(string fecha_programacion, int? multicineID)
        {
            string hoy = "12/01/2016"/* DateTime.Now.ToString("dd/MM/yyyy")*/;
            //cuadno la programacion sea creada quitar comentarios y enviar en el where conversionfecha
            Taquilla objTaquillas = (Taquilla)(Session["Taquilla"]);
            int RowID_Teatro = objTaquillas.TeatroID;//int.Parse(Session["RowID_Teatro"].ToString());
            DateTime hoy1 = DateTime.Parse(fecha_programacion);
            string conversionfecha = hoy1.ToString("MM/dd/yyyy");
            var hora_actual = (DateTime.Now.ToString("HH:mm")).Split(':');
            int minutosactuales = int.Parse(hora_actual[0] + hora_actual[1]);
            //var datavista = from dt in db.DetallePelicula.ToList()
            //                select new ModelosPropios.Model.Funcion_Vista
            //                {
            //                    Detalle_Pelicula = dt,
            //                    Funciones = db.Funciones.Where(f => f.DetallePeliculaID == dt.RowID && f.FechaFuncion == conversionfecha && f.RowIDTeatro == RowID_Teatro /*multicineID*/).ToList()
            //                };
            var datavista = db.Funciones.Where(f => f.FechaFuncion == conversionfecha && f.RowIDTeatro == RowID_Teatro /*multicineID*/).GroupBy(f => f.DetallePeliculaID).ToList();
            var peliculas = datavista.ToList();
            string html = "";
            foreach (var peliculas_vista in datavista)
            {
                bool informacion_pelicula = false;

                int cantidad_funciones_consulta = peliculas_vista.Count();
                int cantidad_funciones_html = 0;
                bool cerrar = true;
                bool carrusel = true;
                int contador_div = 1;
                int contador_funciones_pelicula = 1;
                int contador_funciones_div = 0;
                int valida_cantidad_funciones = 0;
                string ruta = "";
                foreach (var funciones in peliculas_vista)
                {
                    
                    if (informacion_pelicula == false)
                    {
                        ruta = System.IO.Path.Combine(Server.MapPath("~/"+ funciones.Afiche+""));
                        ruta=ruta.Replace('/','\\');
                        if (System.IO.File.Exists(ruta))
                        {
                            ruta = funciones.Afiche;
                        }
                        else
                        {
                            ruta = @"Repositorio_Imagenes\Imagenes_Generales\nodisponible.png";
                        }
                        html += "<tr>";
                        html += "<td class='pad-no mar-no'>";
                        html += "<img class='poster-peliculas' onerror='imgError(this);' src='/" +ruta + "' > ";
                        html += "</td>";
                        html += "<td class='nombre-peliculas' id="+funciones.DetallePeliculaID+">";
                        html += "<h5>" + funciones.TituloLocal + "</h5>";
                        html += "<h5>" + funciones.PeliculaVersion + funciones.PeliculaIdioma + "</h5>";
                        html += "<button class=\"btn btn-xs btn-default col-sm-10\" id='Vendidas'>Vendidas</button></br>";
                        html += "<button class=\"btn btn-xs btn-default col-sm-10\" id='Reservadas'>Reservadas</button></br>";
                        html += "<button class=\"btn btn-xs btn-default col-sm-10\" id='Disponible'>Disponible</button></br>";
                        html += "<button class=\"btn btn-xs btn-default col-sm-10\" id='porcentaje_ocupacion'>Porcentaje ocupación</button>";
                        html += "</td>";
                        informacion_pelicula = true;
                    }
                    #region td funciones vista
                    if (cantidad_funciones_html == 0)
                    {
                        html += "<td class='col-sm-8'>";

                    }
                            if (cantidad_funciones_consulta > 3 && carrusel==true)
                            {
                                html += "<div id ='carousel_" + funciones.DetallePeliculaID + "' class='carousel slide' data-ride='carousel' data-interval='false'>";
                                html += " <div class='carousel-inner text-center'>";
                                
                                carrusel = false;

                            }
                                    #region item-funcion
                                    if (contador_funciones_div == 0 && cerrar==true)
                                    {
                                        html += "<div class='item active'>";
                                        html += "<div class='contenedor-funciones col-sm-12'>";
                                        cerrar = false;
                                    }
                                    else if (contador_funciones_div == 0 && cerrar == false)
                                    {
                                        html += "<div class='item'>";
                                         html += "<div class='contenedor-funciones col-sm-12'>";
                                    }
                                            html += "<div class='col-sm-3 funcion mar-hor text-center' id='"+funciones.RowID_Funcion+"' onmouseover='hoverfuncion(this)' onclick='javascrip:get_tarifas(" + funciones.RowID_Funcion + ")'>";
                                                string Hora_Funcion = DateTime.Parse(funciones.HoraInicial.ToString()).ToString("hh:mm tt", CultureInfo.InvariantCulture);
                                                html += "<h5 class='text-main hora_funcion'>" + Hora_Funcion + "</h5>";
                                                html += "<p>" + funciones.NombreSala + "<br />Disponible: 120</p>";
                                                
                                            html += "</div>";
                                            contador_funciones_div++;
                                            cantidad_funciones_html++;
                                            valida_cantidad_funciones++;
                                    if (contador_funciones_div == 3)
                                    {
                                        html += "</div>";
                                        html += "</div>";
                                        contador_funciones_div = 0;
                                    }
                                    #endregion
                    if (valida_cantidad_funciones == cantidad_funciones_consulta)
                    {
                        if (cantidad_funciones_consulta > 3)
                        {
                            html += "</div>";
                            
                            html += "</div>";
                            html += "<a class='carousel-control left control-izquierdo' data-slide='prev' href='#carousel_" + funciones.DetallePeliculaID + "' ><i class='demo-pli-arrow-left icon-3x'></i></a>";
                            html += "<a class='carousel-control right control-derecho' data-slide='next' href='#carousel_" + funciones.DetallePeliculaID + "' ><i class='demo-pli-arrow-right icon-3x'></i></a>";
                        }
                        html += "</td>";
                    }
                    #endregion
                }
                
            }
            return html;
        }

                //if (peliculas_vista.Count()!= 0)
                //{


                //    bool cerrar = true;
                //    bool carrusel = true;
                //    int contador_div = 1;
                //    int contador_funciones_pelicula = 1;
                //    html += "<tr>";
                //    html += "<td>";
                //    html += "<img class='poster-peliculas' src='/" + peliculas_vista.Afiche+"' > ";
                //    html += "</td>";
                //    html += "<td  nombre-peliculas'>";
                //    html += "<h5>" + peliculas_vista.TituloLocal + "</h5>";
                //    html += "<h5>" + peliculas_vista.PeliculaVersion + peliculas_vista.PeliculaIdioma + "</h5>";
                //    html += "</td>";
                //    html += "<td>";
                //    html += "<div id = '" + peliculas_vista.DetallePeliculaID+ "' class='carousel slide' data-ride='carousel' data-interval='false'>";
                //    html += "<div class='carousel-inner text-center' style='margin-left:40px;'>";
                //    var funciones=peliculas_vista
                //    foreach (var funciones in peliculas_vista.Funciones)
                //    {
                //        if (peliculas_vista.Funciones.Count() > 3)
                //        {

                //            if (contador_div == 1)
                //            {
                //                html += "<div class='item active'>";
                //                contador_div += 1;
                //            }
                //            else
                //            {
                //                if (peliculas_vista.Funciones.Count() >= 3 && contador_funciones_pelicula == 1)
                //                {
                //                    html += "<div class='item'>";
                //                    contador_div += 1;
                //                }
                //            }
                //        }
                //        else { carrusel = false; }
                //        html += "<div class='col-sm-3 funcion mar-hor' onclick='javascrip:get_tarifas(" + funciones.RowID_Funcion + ")'>";
                //        string Hora_Funcion = DateTime.Parse(funciones.HoraInicial.ToString()).ToString("hh:mm tt", CultureInfo.InvariantCulture);
                //        //string lol = funciones.HoraInicial;
                //        html += "<h5 class='text-main hora_funcion'>" + Hora_Funcion + "</h5>";
                //        html += "<p>" + funciones.NombreSala + "<br />Disponible: 120</p>";
                //        html += "</div>";
                //        if (contador_funciones_pelicula == 3)
                //        {
                //            html += "</div>";
                //            contador_funciones_pelicula = 0;
                //            cerrar = false;
                //        }
                //        contador_funciones_pelicula++;

                //    }
                //    if (cerrar == false)
                //    {
                //        html += "</div>";
                //    }
                //    html += "</div>";
                //    if (carrusel == true)
                //    {
                //        html += "<a class='carousel-control left' data-slide='prev' href='#" + peliculas_vista.Detalle_Pelicula.RowID + "' style='margin-left:10px;'><i class='demo-pli-arrow-left icon-3x'></i></a>";
                //        html += "<a class='carousel-control right' data-slide='next' href='#" + peliculas_vista.Detalle_Pelicula.RowID + "' style='margin-right:-30px;'><i class='demo-pli-arrow-right icon-3x'></i></a>";
                //    }

                //    html += "</div>";

                //    html += "</div>";
                //    html += "</td>";
                //    html += "</tr>";
                //}
            //}

            
        //}
        [CheckSessionOutAttribute]
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
                string clase = tarifafuncion.ListaDetalle.Opcion1.Codigo2;
                //if (tarifafuncion.ListaDetalle.Opcion1 != null)
                //{
                //    clase = "" + tarifafuncion.ListaDetalle.Opcion2.Codigo2 + "";
                //}
                if (contador_row == 1)
                {
                    html_tarifas += "<div class='row'>";
                }
                        html_tarifas += "<div class='panel media middle panel-tarifa-funcion col-sm-5 mar-all' data-quantity-ticket='0' data-validate='1' data-type-rate='" + tarifafuncion.ListaDetalle.Opcion1.Codigo+"' title='"+ tarifafuncion.ListaDetalle.Nombre + "' onclick='javascript:adicionar_item(" + tarifafuncion.RowID + ",1,this)'>";
                            html_tarifas += "<div class='media-left bg-mint pad-all'>";
                                html_tarifas += "<i class='demo-pli-coin icon-3x'></i><br />";
                            html_tarifas += "</div>";
                            html_tarifas +="<div class='media-body'>";
                                html_tarifas +="<p class='text-2x mar-no text-semibold'>$" + tarifafuncion.ListaDetalle.Precio + "</p>";
                                html_tarifas +="<p class='text-muted mar-no text-bold'>Nombre de la tarifa:</p>";
                                html_tarifas += "<p class='text-overflow'>" + tarifafuncion.ListaDetalle.Nombre + " </p>";
                                html_tarifas += "<p class='text-muted mar-no text-bold'>Servicio:</p>";
                                html_tarifas += "<p>"+tarifafuncion.ListaDetalle.Opcion1.Nombre+"</p>";
                            //  html_tarifas += "<div class='col-sm-6 mar-hor '  style='width:40%;' onclick='javascript:adicionar_item(" + tarifafuncion.RowID + ",1)'>";
                            //html_tarifas += "<div class='panel " + clase + " panel-colorful' title='" + tarifafuncion.ListaDetalle.Nombre + "' style='height:50%'>";
                            //html_tarifas += "<div class='pad-all text-center'>";
                            //html_tarifas += "<span class='text-2x text-thin'>$" + tarifafuncion.ListaDetalle.Precio + "</span>";
                            //html_tarifas += "<p class='text-overflow'>" + tarifafuncion.ListaDetalle.Nombre + "</p>";
                            //html_tarifas += "</div>";
                            html_tarifas += "</div>";
                        html_tarifas += "</div>";
                if (contador_row == 2)
                {
                    html_tarifas += "</div>";
                    contador_row = 0;
                }
                contador_row++;
            }
            

            return html_tarifas;
        }
        [CheckSessionOutAttribute]
        public string AdicionarItemVenta(int RowID_ListaFuncion, short? cantidadnueva,short?cantidad_anterior,string tipo_boleta)
        {
             ListaPrecioFuncion ItemVenta = db.ListaPrecioFuncion.Where(lpf => lpf.RowID == RowID_ListaFuncion).FirstOrDefault();
            string html_item_venta = "";
            if (cantidadnueva>4&& tipo_boleta=="TCR")
            {
                return html_item_venta = "Maximo redenciones alcanzada";
            }
            else
            {
                html_item_venta += "<div class='panel  panel-primary panel-colorful item-venta item-elimina" + ItemVenta.RowID + " mar-all' data-element-sale='boleta' data-type-rate='" + tipo_boleta + "' data-quantity-ticket='" + cantidadnueva + "' >";
                html_item_venta += "<div class='pad-all media'>";
                html_item_venta += "<div class='media-left'>";
                html_item_venta += "<span class='text-2x text-bold' id='cantidad-total-" + ItemVenta.RowID + "'>" + cantidadnueva + "</span>";
                html_item_venta += "</div>";
                html_item_venta += "<div class='media-body'>";
                html_item_venta += "<div class='col-sm-9'>";
                html_item_venta += "<p class='h3 text-light mar-no media-heading'>" + ItemVenta.Funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + "&nbsp;" + ItemVenta.Funcion.DetallePelicula.Opcion.Nombre + "&nbsp;" + ItemVenta.Funcion.DetallePelicula.Opcion1.Nombre + "</p>";
                html_item_venta += "<span class='suma-costo-pedido' id='total-costo" + ItemVenta.RowID + "'>$ " + ItemVenta.ListaDetalle.Precio * cantidadnueva + "</span>";
                html_item_venta += "<input type='hidden' class='precio-items-" + ItemVenta.RowID + "' value='" + ItemVenta.ListaDetalle.Precio + "'>";
                html_item_venta += "<input type='hidden' class='cantidad-boleta-" + ItemVenta.RowID + "' value='" + cantidadnueva + "'></span>";
                html_item_venta += "<input type='hidden' class='tarifas_id' value='" + ItemVenta.ListaDetalle.RowID + "'></span>";
                html_item_venta += "</div>";

                html_item_venta += "</div>";
                html_item_venta += "<div class='media-right mar-rgt'>";
                if (tipo_boleta == "TCR")
                {
                    html_item_venta += "<button class='btn btn-info btn-icon' data-validate='0'  data-type-rate='" + tipo_boleta + "' data-quantity-ticket='" + cantidadnueva + "' onclick=javascript:adicionar_item(" + RowID_ListaFuncion + ",3,this) style='background-color: rgba(97,208,255,1);width:100%;font-size: 10px;'><i class='ion-ios-cart icon-3x'></i><i class='icon-2x mar-rgt'>+3</i></button>";
                    html_item_venta += "<button class='btn btn-info btn-icon' data-validate='0' data-type-rate='" + tipo_boleta + "'   data-quantity-ticket='" + cantidadnueva + "' onclick=javascript:adicionar_item(" + RowID_ListaFuncion + ",1,this) style='background-color: rgba(97,208,255,1);width:100%;font-size: 10px;'><i class='ion-ios-cart icon-3x'></i><i class='icon-2x mar-rgt'>+1</i></button>";
                }
                else
                {
                    html_item_venta += "<button class='btn btn-info btn-icon' data-validate='0'  data-type-rate='" + tipo_boleta + "' data-quantity-ticket='" + cantidadnueva + "' onclick=javascript:adicionar_item(" + RowID_ListaFuncion + ",5,this) style='background-color: rgba(97,208,255,1);width:100%;font-size: 10px;'><i class='ion-ios-cart icon-3x'></i><i class='icon-2x mar-rgt'>+5</i></button>";
                    html_item_venta += "<button class='btn btn-info btn-icon mar-top' data-validate='0' data-type-rate='" + tipo_boleta + "' data-quantity-ticket='" + cantidadnueva + "' onclick=javascript:adicionar_item(" + RowID_ListaFuncion + ",10,this) style='background-color: rgba(97,208,255,1);width:100%;font-size: 10px;'><i class='ion-ios-cart icon-3x'></i><i class='icon-2x'>+10</i></button>";
                }

                html_item_venta += "<button class='btn btn-info btn-icon mar-top' data-type-rate='" + tipo_boleta + "' data-quantity-ticket='" + cantidadnueva + "' onclick='javascript:adicionar_item(" + ItemVenta.RowID + ",-1,this)' style='background-color: rgba(97,208,255,1);width:100%;font-size: 10px;'><i class='ion-ios-trash icon-3x'></i></button>";
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
                html_item_venta += "<input type='hidden' value='" + cantidadnueva + "' name='" + RowID_ListaFuncion + "' id='cantidad_boletas_tarifa_" + RowID_ListaFuncion + "' class='boletas_vender_ " + RowID_ListaFuncion + "'>";
            }
           
            return html_item_venta;
        }
        [CheckSessionOutAttribute]
        public ActionResult Mapa_Sala_funcion(int RowID_Sala, short? cantidad_sillas_venta, int RowIDFuncion, string RowIDTarifas)
        {
            Sala obj_sala = new Sala();
            obj_sala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            ViewBag.TipoSillas = db.SalaObjeto.ToList();
            int idtarifa = int.Parse(RowIDTarifas.Split(',')[0]);
            ViewBag.Tarifa = db.ListaDetalle.Where(t => t.RowID == idtarifa).FirstOrDefault();
            ViewBag.CantidadSillasVenta = cantidad_sillas_venta;
            ViewBag.Funcion = db.Funcion.Where(f => f.RowID == RowIDFuncion).FirstOrDefault();
            return View(obj_sala);
        }
        [CheckSessionOutAttribute]
        public string Get_Mapa_Sala(int RowID_Sala, int? RowID_Funcion)
        {
            string Data_Table = "";
            //VerMapaVenta_Result ObjVerMapaFuncion = new VerMapaVenta_Result();
            List<VerMapaVenta_Result> objsillas =db.VerMapaVenta(RowID_Funcion).ToList();
            int ContadorColumnas = 0;
            int CantidadColumnas = 0;
            int i = 0;
            foreach (var SillaMapa in objsillas)
            {
                CantidadColumnas = int.Parse(SillaMapa.SalaColumnas.ToString());
                if (ContadorColumnas== 0)
                {
                    Data_Table = Data_Table + "<tr class='fila_" + i + "' style='padding: 0px 0px 0px 0px;height:30px;'>";
                    i++;
                }
                if (SillaMapa.TipoObjeto=="SILLA")
                {
                    if (SillaMapa.SillaBloqueada==1)
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "' class='disabled' style='background: #FF8000; padding: 0px 0px 0px 0px;' >";
                    }
                    else if (SillaMapa.SillaVendida==1)
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "' class='disabled' style='background: #B0BEC5; padding: 0px 0px 0px 0px;' >";
                    }
                    else if (SillaMapa.SillaReservada == 1)
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "' class='disabled' style='background: #FFA726; padding: 0px 0px 0px 0px;' >";
                    }
                    else
                    {
                        Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "'  onclick = vender_silla(this) >";
                    }
                }
                else
                {
                    Data_Table = Data_Table + " <td id='" + SillaMapa.RowIDSillaMapa + "' style='padding: 0px 0px 0px 0px;'>";
                }
                ContadorColumnas++;
                Data_Table = Data_Table + "<strong><small>"+SillaMapa.SillaColumna+"&nbsp;"+(SillaMapa.SillaFila+1) +"</small ></strong >";
                Data_Table = Data_Table + "<img style ='border:none;width:40px' class='img-sm' src='/" + SillaMapa.ImagenObjeto + "' />";
                if (ContadorColumnas== CantidadColumnas)
                {
                    Data_Table = Data_Table + " </tr>";
                    ContadorColumnas = 0;
                }
            }
            //Sala ObjSala = db.Sala.Where(s => s.RowID == RowID_Sala).FirstOrDefault();
            
            //if (RowID_Sala != 0)
            //{
            //    for (int i = 0; i < ObjSala.Cantidad_Filas; i++)
            //    {
                    
            //        for (int j = 0; j < ObjSala.Cantidad_Columnas; j++)
            //        {

            //            string clase = "posicion_objeto" + i + "_" + j + "";
            //            string clase_posicionX = "posicion_x" + i + "_" + j + "";
            //            string clase_posicionY = "posicion_y" + i + "_" + j + "";
            //            //clase ="posicion_objeto"+i+"_"+j+"";


            //            MapaSala objmapa_sala = ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j && s.SalaID == ObjSala.RowID).FirstOrDefault();
            //            BoletaVendida objBoleta = db.BoletaVendida.Where(bv => bv.SillaID == objmapa_sala.RowID && bv.FuncionID == RowID_Funcion).FirstOrDefault();
            //            if (objmapa_sala != null)
            //            {
            //                if (objmapa_sala.SalaObjeto.Opcion1.Codigo == "SILLA")
            //                {
            //                    if (objBoleta != null)
            //                    {
            //                        Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class='" + clase + " disabled' style='background: #B0BEC5;' ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
            //                    }
            //                    else
            //                    {
            //                        Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class=" + clase + " onclick = vender_silla(this) ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
            //                    }

            //                }
            //                else
            //                {
            //                    Data_Table = Data_Table + " <td id='" + objmapa_sala.RowID + "'  class=" + clase + "><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
            //                }
            //                var tipoobjeto = "objeto  " + objmapa_sala.SalaObjeto.Opcion1.Nombre;
            //                if (objmapa_sala.SalaObjeto.Numeracion == true)
            //                {
            //                    tipoobjeto = tipoobjeto + " tipo_silla";
            //                }
            //                Data_Table = Data_Table + "<input type ='hidden' class='" + tipoobjeto + "' name='" + clase + "' value=" + ObjSala.MapaSala.Where(s => s.PosicionX == i && s.PosicionY == j).FirstOrDefault().ObjetoID + ">";
                            
            //                Data_Table = Data_Table + "<img style ='border:none;width:50px' src='/" + objmapa_sala.SalaObjeto.Imagen + "' />";

            //            }
            //            else
            //            {
            //                Data_Table = Data_Table + " <td style ='border:none; padding:0px; WIDTH: 50PX; HEIGHT: 50PX;'  class=" + clase + " ><input type='hidden' name='" + clase_posicionX + "' value=" + i + " /><input type = 'hidden' name='" + clase_posicionY + "' value=" + j + "  />";
            //            }
            //            Data_Table = Data_Table + "</td>";
            //        }
            //        Data_Table = Data_Table + " </tr>";
            //    }

            //}

            return Data_Table;
        }
        public ActionResult pruebabuscador()

        {
            //string s = "string to print";

            //PrintDocument p = new PrintDocument();
            //p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            //{
            //    e1.Graphics.DrawString(s, new Font("Times New Roman", 12), new SolidBrush(Color.Black), new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));

            //};
            //try
            //{
            //    p.Print();
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Exception Occured While Printing", ex);
            //}
            return View();
        }
        [CheckSessionOutAttribute]
        public JsonResult TerminarVenta(string IDSillas, string IDTarifas, int RowIDFuncion,int Efectivo,int Cambio,int Total,string IDProductos,string CantidadProductoID)
        {
            
            
            UsuarioSistema usuariotaquilla = (UsuarioSistema)(Session["usuario"]);
            Taquilla taquillas = (Taquilla)(Session["Taquilla"]);
            string Boletas = "";
            BoletaVendida objBoleta = new BoletaVendida();
            ControlIngreso objControlIngreso = new ControlIngreso();
            objControlIngreso.Cambio = Cambio;
            objControlIngreso.Efectivo = Efectivo;
            objControlIngreso.Total = Total;
            objControlIngreso.Fecha = DateTime.Now;
            objControlIngreso.Fecha = DateTime.Now;
            objControlIngreso.UsuarioID = usuariotaquilla.RowID;
            db.ControlIngreso.Add(objControlIngreso);
            db.SaveChanges();
            var RowIDSillas = IDSillas.TrimEnd(',').Split(',');
            int RowIDTarifa = int.Parse(IDTarifas.Split(',')[0].ToString());
            int RowIDSilla = 0;
            int CantidadBoletasValidadas = 0;
            int RowIDControl = objControlIngreso.RowID;
            string tipo_respuesta = "";
            string Respuesta = "";
            var RowidProducto = IDProductos.TrimEnd(',').Split(',');
            var CantidadxProducto = CantidadProductoID.TrimEnd(',').Split(',');
            ProductoVendido ObjProdVend = new ProductoVendido();
            if (RowidProducto.Length>0)
            {
                
                for (int i = 0; i < RowidProducto.Length; i++)
                {
                    if (RowidProducto[i] != "")
                    {
                        ObjProdVend.ProductoID = int.Parse(RowidProducto[i].ToString());
                        ObjProdVend.ControlIngresoID = objControlIngreso.RowID;
                        ObjProdVend.Cantidad = short.Parse(CantidadxProducto[i].ToString());
                        db.ProductoVendido.Add(ObjProdVend);
                        db.SaveChanges();
                    }
                   
                }
            }
            int[] rowid_sillasvendidas = new int[RowIDSillas.Length];
            List<BoletaVendida> ListaBoletas = new List<Models.BoletaVendida>();
            for (int i = 0; i < RowIDSillas.Length; i++)
            {

                RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                objBoleta = db.BoletaVendida.Where(BV => BV.SillaID == RowIDSilla && BV.FuncionID == RowIDFuncion).FirstOrDefault();
                if (objBoleta != null)
                {
                    ListaBoletas.Add(objBoleta);
                }
            }
            if (ListaBoletas.Count() == 0)
            {
                List<BoletaVendida> listaboletasimprime=new List<BoletaVendida> { };
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
                    objBoletaVenta.TaquillaID = taquillas.RowID;
                    objBoletaVenta.UsuarioID = usuariotaquilla.RowID;
                    objBoletaVenta.ControlIngresoID = RowIDControl;
                    db.BoletaVendida.Add(objBoletaVenta);
                    db.SaveChanges();
                    rowid_sillasvendidas[i] = int.Parse(objBoletaVenta.RowID.ToString());
                    BoletaVendida boletaimprimir = db.BoletaVendida.Where(f => f.RowID == objBoletaVenta.RowID).FirstOrDefault();
                    
                    //ListaBoletas.Add(boletaimprimir);   
                    Boletas += "";
                    tipo_respuesta = "IMPRIMIR";
                    boletaimprimir = new BoletaVendida();
                }
                List<BoletasVendidas> imprimiboletas= boletasimprimir(rowid_sillasvendidas);
                
                //var LOL = objBoletaVe2131nt31231a.Funcion.Fecha;
                foreach (var Boleta in imprimiboletas.ToList())
                {
                   Boletas+= "<div class='printableArea' style='width:300px;height:400px;display:table;margin-top:10px; margin-left:5px; margin-bottom:60px;'>";
                        Boletas += "<div>";
                            Boletas += "<img src ='/Repositorio_Imagenes/Imagenes_Generales/logo-royal.jpg' width='20%'/>";
                        Boletas += "</div>";
                        Boletas += "<div class='boleta' style='float:left;width:49%;height:100%;'>";
                            Boletas += "<p class='text-center pad-no mar-no'>Royal Films SAS<br />NIT: 890.105.652-3</p>";
                            Boletas += "<p class='text-center pad-no mar-no'><strong>Resolución:</strong><br /><strong>20000199088 de"+ DateTime.Now.ToShortDateString()+"</strong></p>";
                            Boletas += "<p class='text-center pad-no mar-no'><strong>Regimen simplificado:</strong></p>";
                                Boletas += "<p class='pad-no mar-no text-sm text-bold'>FACTURA DE VENTA N.<br/>45789</p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Funcion:</strong><br/> " + Boleta.FechaFuncion+"</p>";
                            Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.NombreSala+"</strong ></p>";
                            Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.HoraInicialFuncion+"</strong></p>";
                            Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.TituloLocal+ "</strong><br/><h3 class='pad-no mar-no'>" + Boleta.FormatoVersion+"</h3></p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Tarifa:<strong><br/> " + Boleta.NombreTarifa.Substring(0,20)+" </ p >";
                            Boletas += "<p class='pad-no mar-no'><strong> Precio: $" + Boleta.PrecioVenta+" </strong></p >";
                            Boletas += "<p class='pad-no mar-no'><strong>Iva: $0 </strong><br/></p>";
                            Boletas += "<p class'text-light'>Silla<br/>" + Boleta.SillaVendida + " </p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Cajero: </strong><br/>" + Boleta.InformacionTaquillero+" </p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Terminal </strong> " + Boleta.NombreTaquilla+" </p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Prefijo: </strong> " + Boleta.PrefijoTaquilla+" </p>";
                            Boletas += "<p class='pad-no mar-no'><strong> Fecha y hora de venta</strong><br/>" + Boleta.FechaVenta+ "</p>";
                    Boletas += "</div>";
                    // -----------------------------------------------------------
                    Boletas += "<div class='tirilla' style='float:right;width:49%;height:100%;'>";
                    Boletas += "<p class='text-center pad-no mar-no'>Royal Films SAS<br />NIT: 890.105.652-3</p>";
                    Boletas += "<p class='text-center pad-no mar-no'><strong>Resolución:</strong><br /><strong>20000199088 de" + DateTime.Now.ToShortDateString() + "</strong></p>";
                    Boletas += "<p class='text-center pad-no mar-no'><strong>Regimen simplificado:</strong></p>";
                    Boletas += "<p class='pad-no mar-no text-sm text-bold'>FACTURA DE VENTA N.<br/>45789</p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Funcion:</strong><br/> " + Boleta.FechaFuncion + "</p>";
                    Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.NombreSala + "</strong ></p>";
                    Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.HoraInicialFuncion + "</strong></p>";
                    Boletas += "<p class='pad-no mar-no'><strong>" + Boleta.TituloLocal + "</strong><br/><h3 class='pad-no mar-no'>" + Boleta.FormatoVersion + "</h3></p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Tarifa:<strong><br/> " + Boleta.NombreTarifa.Substring(0, 20) + " </ p >";
                    Boletas += "<p class='pad-no mar-no'><strong> Precio: $" + Boleta.PrecioVenta + " </strong></p >";
                    Boletas += "<p class='pad-no mar-no'><strong>Iva: $0 </strong><br/></p>";
                    Boletas += "<p class'text-light'>Silla<br/>" + Boleta.SillaVendida + " </p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Cajero: </strong><br/>" + Boleta.InformacionTaquillero + " </p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Terminal </strong> " + Boleta.NombreTaquilla + " </p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Prefijo: </strong> " + Boleta.PrefijoTaquilla + " </p>";
                    Boletas += "<p class='pad-no mar-no'><strong> Fecha y hora de venta</strong><br/>" + Boleta.FechaVenta + "</p>";
                    Boletas += "</div>";
            Boletas += "</div>";
            }
                Respuesta = "Transacción exitosa";
            }
            else if (ListaBoletas.Count() != 0)
            {
                Respuesta = "Las siguientes boletas ya se encuentran vendidas";
            }
            return Json(new { tipo_respuesta=tipo_respuesta, Respuesta = Respuesta, data = ListaBoletas,html=Boletas }, JsonRequestBehavior.AllowGet);
        }
        public string valida_estado_cupon(string codigo_barras)
        {
            //String des = "65B7A47635";
            string respuesta = "";
            //Encriptacion encrip = new Encriptacion(System.Text.Encoding.UTF8);
            //RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            //byte[] key = new byte[32]; 
            //byte[] iv = new byte[32];
            //rngCsp.GetBytes(key);
            //rngCsp.GetBytes(iv);
            //des = encrip.Decrypt(des, key, iv);

            //Busco el convenio en el local
            DetalleConvenio cupon_valida = db.DetalleConvenio.FirstOrDefault(dc => dc.Codigo == codigo_barras);


            //En caso de que no lo encuentre lo busco en el central
            if (cupon_valida == null)
            {
                String cadenaActual = db.Database.Connection.ConnectionString;
                db.Database.Connection.ConnectionString = (db.Teatro.Where(p => p.Nombre == "CENTRAL").FirstOrDefault().CadenaBD);
                try
                {
                    cupon_valida = db.DetalleConvenio.FirstOrDefault(dc => dc.Codigo == codigo_barras);
                }
                catch (Exception)
                {
                    cupon_valida = null;
                }
                if (cupon_valida != null)
                {
                    if (cupon_valida.Estado.Codigo.ToUpper() == ModelosPropios.Util.Constantes.ESTADO_CONVENIO_REDIMIDO)
                    {
                        respuesta = "EL convenio " + cupon_valida.EncabezadoConvenio.Nombre + " " + cupon_valida.Codigo + " ya ha sido redimido ";
                    }
                    else
                    {
                        //Si lo encuentro le cambio el estado a redimido
                        cupon_valida.EstadoID = db.Estado.Where(es => es.TipoEstado.Codigo == ModelosPropios.Util.Constantes.TIPO_ESTADO_CONVENIO && es.Codigo == ModelosPropios.Util.Constantes.ESTADO_CONVENIO_REDIMIDO).FirstOrDefault().RowID;
                        db.SaveChanges();
                        respuesta = "Convenio redimido";
                    }
                }
                
                //Le devuelvo la cadena que tenia
                db.Database.Connection.ConnectionString = cadenaActual;
            }
            else
            {
                if (cupon_valida.Estado.Codigo.ToUpper() == ModelosPropios.Util.Constantes.ESTADO_CONVENIO_REDIMIDO)
                {
                    respuesta = "EL convenio " + cupon_valida.EncabezadoConvenio.Nombre + " " + cupon_valida.Codigo + " ya ha sido redimido ";
                }
                else
                {
                    //Si lo encuentro le cambio el estado a redimido
                    cupon_valida.EstadoID = db.Estado.Where(es => es.TipoEstado.Codigo == ModelosPropios.Util.Constantes.TIPO_ESTADO_CONVENIO && es.Codigo == ModelosPropios.Util.Constantes.ESTADO_CONVENIO_REDIMIDO).FirstOrDefault().RowID;
                    db.SaveChanges();
                    respuesta = "Convenio redimido";
                }
            }
            if (cupon_valida == null)
            {
                respuesta = "EL convenio no existe";
            }
            return respuesta;
        }

        public List<BoletasVendidas> boletasimprimir(int[] rowids)
        {
            List<BoletasVendidas> boletas = new List <BoletasVendidas> { };
            for (int i = 0; i < rowids.Length; i++)
            {
                int rowid = rowids[i];
                BoletasVendidas objimprimir = db.BoletasVendidas.Where(bv => bv.RowIDBoleta == rowid).First();
                boletas.Add(objimprimir);
            }

            return boletas;
            
        }

        public ActionResult VenderProductos()
        {
            string info_table = "";
            UsuarioSistema obj_usuario = (UsuarioSistema)(Session["usuario"]);

            //List<Producto> list_product=db.Producto.Where(p=>p.)
            DateTime fecha = ModelosPropios.Util.FechaInsertar(DateTime.Now.ToShortDateString());
            //ControlCajaUsuarioEntrega ControlUsuario = db.ControlCajaUsuarioEntrega.Where(pr => pr.UsuarioID == obj_usuario.RowID && pr.FechaEntrega == fecha).FirstOrDefault();
            List<Producto> Producto = db.Producto.ToList();
            foreach (var Productos in Producto)
            {

                //info_table += "<div class='row'>";
                    info_table += "<div class='col-sm-6' data-tipo-producto='"+Productos.RowID+ "' data-cantidad-producto=0  data-ref-producto='"+Productos.Referencia+"' onclick='adicionar_producto(" + Productos.RowID+ ",this)'";
                        info_table += "<div class=\"panel panel-primary panel-colorful\">";
                            info_table += "<div class=\"pad-all text-center\">";
                                info_table += "<span class=\"text-3x text-thin\">"+Productos.Costo+"</span>";
                                    info_table += "<p>"+Productos.NombreProducto+"</p>";
                                    info_table+="<i class='demo-pli-shopping-bag icon-lg'></i>";
                            info_table += " </div>";
                    info_table += "</div>";
                info_table += "</div>";
               // info_table += "</div>";
                //info_table += "<td>";
                //info_table += "</td>";


            }
            return View((object)info_table);
        }

        public string AdicionarProducto(int RowID_Producto,int cantidad)
        {
            Producto producto_venta = db.Producto.Where(p => p.RowID == RowID_Producto).FirstOrDefault();
            string html_producto = "";
            html_producto += "<div id='" + producto_venta.RowID + "' data-product-ref='"+producto_venta.Referencia+"' data-product-id='" + producto_venta.RowID+ "' data-product-quantity-sale='"+cantidad+"' data-element-sale='producto' data-product-cost-total='" + cantidad * producto_venta.Costo + "' class='panel panel-success panel-colorful item-producto'>";
                html_producto += "<div class='pad-all media'>";
                html_producto += "<div class='<div class='media-left'>";
                    html_producto += "<i class='demo-pli-male icon-3x icon-fw'></i>";
                html_producto += "</div>";
            html_producto += "<div class='media-body'>";
            html_producto += "<p class='h3 text-light mar-no media-heading'>"+producto_venta.NombreProducto+"</p>";
            html_producto += "<p class='h3 text-light mar-no media-heading'>"+producto_venta.Costo+"</p>";
            html_producto += "<p class='h3 text-light mar-no media-heading' class='Cantidad_Producto'>"+ cantidad+"</p>";
            html_producto += "<p class='h3 text-light mar-no media-heading'>" + cantidad*producto_venta.Costo+"</p>";
            html_producto += "</div>";
            html_producto += "</div>";
            //html_producto += "<div class='progress progress-xs progress-success mar-no'>";
            html_producto += "<a href=\"javascript:eliminar_producto(\'" + producto_venta.RowID + "\')\"><i class='ion-ios-close text-2x'></i></a>";
            //html_producto += "</div>";
                 html_producto += "</div>";
            return html_producto;
        }
        public ActionResult AsignarTarjeta(string Cedula)
        {
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION").ToList();
            ViewBag.Ciudades = db.Ciudad.ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO" && f.Activo == true).ToList();
            ViewBag.Clasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLIENTE" && f.Activo == true).ToList();
            ViewBag.preferencias = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOGENEROPELICULA" && f.Activo == true).ToList();
            ClienteRoyal ObjCliente = new ClienteRoyal();
            if (Cedula!=null)
            {
                ObjCliente = db.ClienteRoyal.Where(cr=>cr.Documento==Cedula).FirstOrDefault();
                if (ObjCliente==null)
                {
                    return View(new ClienteRoyal());
                }
                return View( ObjCliente);
            }
            else
            {
                return View(new ClienteRoyal());
            }
        }
        public JsonResult ValidarTCR(string Ntarjeta)
        {
            string respuesta = "";
            string tipo_respuesta = "";
            TarjetaMembresiaClienteRoyal objTarjeta = db.TarjetaMembresiaClienteRoyal.Where(lol => lol.TarjetaMembresia.Codigo == Ntarjeta).FirstOrDefault();
            try
            {
                objTarjeta.NoRedencionesAprob = 4;
                tipo_respuesta = "success";
                respuesta = "Tarjeta encontrada";
            }
            catch (Exception)
            {
                tipo_respuesta = "warning";
                respuesta = "Respuesta tarjeta no se encuentra en el sistema ";
            }
            
            //if (objTarjeta!=null)
            //{
                
            //}
            //else
            //{
               
            //}
            objTarjeta = new TarjetaMembresiaClienteRoyal();
            return Json(new { respuesta=respuesta,objetotarjeta=objTarjeta,tipo_respuesta=tipo_respuesta },JsonRequestBehavior.AllowGet);
        }
        public JsonResult InformacionSala(int rowidfuncion)
        {
            EstadisticaFuncion_Result DataF= db.EstadisticaFuncion(rowidfuncion).FirstOrDefault();
            //var Data = (from func in DataF
            //        select new
            //        {
            //            Capacidad = func.Capacidad,
            //            SillasBloqueadas = func.SillasBloqueadas,
            //            SillasReservadas = func.SillasReservadas
            //        }).FirstOrDefault();
            return Json(DataF,JsonRequestBehavior.AllowGet);
        }
        #region CallCenter


        [CheckSessionOutAttribute]
        public ActionResult ConfirmaReserva(string RowIdCallCenter, FormCollection formulario, String DatosSillasSeleccionadas, int? RowIDTeatro)
        {
            /**/
            if(!String.IsNullOrEmpty(RowIdCallCenter)){
                RowIdCallCenter = RowIdCallCenter.TrimStart('\'').TrimEnd('\'');
            }
            ViewBag.DatosSillasSeleccionadas = DatosSillasSeleccionadas;
            if (!String.IsNullOrEmpty(RowIdCallCenter))
            {
                if (RowIDTeatro != null)
                {
                    String cadena = db.Teatro.FirstOrDefault(f => f.RowID == RowIDTeatro).CadenaBD;
                    db.Database.Connection.ConnectionString = cadena;
                }
                Reserva reserva;
                //Busco por Codigo Reserva 
                reserva = db.Reserva.FirstOrDefault(f => f.CodigoReserva.Equals(RowIdCallCenter));

                //Busco Por Telefono
                if (reserva == null)
                {
                    reserva = db.Reserva.FirstOrDefault(f => f.TelefonoCliente.Equals(RowIdCallCenter));
                }
                if (reserva == null)
                {
                    reserva = new Reserva();
                }
                else
                {
                    ViewBag.Boletas = db.BoletaReservada.Where(f => f.ReservaID == reserva.RowID).ToList();
                }
                return View(reserva);
            }
            return View(new Reserva());
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

        #endregion
    }
}