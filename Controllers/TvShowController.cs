using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Security.Cryptography;
using System.Text;
using CinemaPOS.ModelosPropios;

namespace CinemaPOS.Controllers
{
    public class TvShowController : Controller
    {
        //
        // GET: /ConveniosCupones/

        CinemaPOSEntities db = new CinemaPOSEntities();


        [CheckSessionOutAttribute]
        public ActionResult VistaTVShow()
        {
            ViewBag.Funcion = db.Funcion.Where(f => f.Fecha == DateTime.Now && f.EncabezadoProgramacion.TeatroID == 23);
            ViewBag.Teatros = db.Teatro.ToList();

            return View(db.Funciones);
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaProgramacion()
        {
            ViewBag.Funcion = db.Funcion.Where(f => f.Fecha == DateTime.Now && f.EncabezadoProgramacion.TeatroID == 23);
            ViewBag.Teatros = db.Teatro.ToList();

            return View(db.Funciones);
        }

        #region FUNCIONES
        public ActionResult TvShowFunciones(string FechaValidar)
        {
            //if (string.IsNullOrEmpty(FechaValidar))
            //{
            //    FechaValidar = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy"));
            //}
            //List<Util.FuncionEncabezado> a = (from reg in db.Funciones.Where(f => f.FechaFuncion == FechaValidar)
            //                                  select new Util.FuncionEncabezado
            //                                  {
            //                                      titulo = reg.TituloLocal,
            //                                      version = reg.PeliculaVersion,
            //                                      pelicula = reg.RowID_EncabezadoPelicula,
            //                                      //Lista = db.Funciones.Where(f=>f.RowID_EncabezadoPelicula==reg.RowID_EncabezadoPelicula).ToList()

            //                                  }).Distinct().ToList();
            //List<Util.FuncionEncabezado> Formato = (from reg in a.ToList()
            //                                        select new Util.FuncionEncabezado
            //                                        {
            //                                            titulo = reg.titulo,
            //                                            version = reg.version,
            //                                            pelicula = reg.pelicula,
            //                                            Lista = db.Funciones.Where(f => f.RowID_EncabezadoPelicula == reg.pelicula && f.PeliculaVersion == reg.version).ToList()

            //                                        }).ToList();

            //funcion.Select(m => new { m.TituloLocal,m.PeliculaVersion,m.RowID_EncabezadoPelicula }).Distinct().ToList();
            //var Lista = (from reg in data.ToList()
            //                                  select new 
            //                                  {
            //                                      Funcion=reg,
            //                                      Lista=db.Funciones.Where(f=>f.FechaFuncion == FechaValidar && f.RowID_EncabezadoPelicula==reg.RowID_EncabezadoPelicula).ToList()
            //                                  }).ToList();
            return View();
        }
        public string TvString(string FechaValidar)
        {
            if (string.IsNullOrEmpty(FechaValidar))
            {
                FechaValidar = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy"));
            }
            List<Util.FuncionEncabezado> a = (from reg in db.Funciones.Where(f => f.FechaFuncion == FechaValidar)
                                              select new Util.FuncionEncabezado
                                              {
                                                  titulo = reg.TituloLocal,
                                                  version = reg.PeliculaVersion,
                                                  Formato = reg.PeliculaIdioma,
                                                  pelicula = reg.RowID_EncabezadoPelicula,
                                                  peliculaDetalleID = reg.RowID_Detalle,
                                                  Poster = reg.Afiche,
                                                  //Lista = db.Funciones.Where(f=>f.RowID_EncabezadoPelicula==reg.RowID_EncabezadoPelicula).ToList()

                                              }).Distinct().ToList();
            List<Util.FuncionEncabezado> Formato = (from reg in a.ToList()
                                                    select new Util.FuncionEncabezado
                                                    {
                                                        titulo = reg.titulo,
                                                        version = reg.version,
                                                        Formato = reg.Formato,
                                                        pelicula = reg.pelicula,
                                                        Poster = reg.Poster,
                                                        Lista = db.Funciones.Where(f => f.DetallePeliculaID == reg.peliculaDetalleID).Distinct().ToList()

                                                    }).ToList();

            string resultad = "";
            int contador = 0;
            foreach (var item in Formato)
            {
                if (item.Poster == "")
                {
                    item.Poster = "../Repositorio_Imagenes/Poster_Peliculas/SING.jpg";
                }
                resultad += "<li id=\"FuncionGlobal\">" +
                       "<div class='col-md-12'>" +
                       "<div class=\"row\" id=\"General\">" +
                            "<div class=\"col-md-2 \" id=\"Afiche\">" +
                                "<img src=" + "../" + item.Poster + " alt=\"\" /></div>" +
                        "   <div class=\"col-md-2 \" id=\"TituloFuncion\">" +
                         "      <div id = \"Titulo\" > " + item.titulo + "<div id='PrinFormarto'><div class='triangulo-equilatero-bottom-left '></div><h2 id='FormatoVersion'>" + item.version + "<span>" + item.Formato + "</span></h2></div></div>" +
                         "  </div>" +

                        "</div>" +
                "</div>";
                resultad += "<div class='row' id='CompleteContent'><div class='col-md-12' id='Horarios'>";
                IEnumerable<Funciones> list = item.Lista.GroupBy(x => x.HoraInicial).Select(y => y.First());

                foreach (var funcion in list)
                {
                    //if (contador==0)
                    //{

                    //}
                    resultad += "   " + funcion.HoraInicial + "  |   ";

                    //if (contador == 5)
                    //{
                    //    resultad += "</div></div>";
                    //    contador = 0;
                    //}
                    //else
                    //{
                    //    contador++;
                    //}
                }
                //if (contador<7)
                //{
                resultad += "</div></div>";
                //    contador = 0;
                //}

                resultad += "</li>";

            }

            return resultad;
        }

        #endregion

        #region TRAILERS
        public ActionResult TVShowTrailers(int? rowid)
        {
            return View();
        }

        public string TvTrailers()
        {
            string ContentTrailer = "";
            string ReplaceTrailer = "";
            string ItemActive = "item active";
            string ItemAfiches = "";
            var PeliculaTrailer = (from trailer in db.EncabezadoPelicula
                                   join Clasificacion in db.Opcion on trailer.TipoClasificacionID equals Clasificacion.RowID
                                   join Idioma in db.Opcion on trailer.TipoIdiomaOriginalID equals Idioma.RowID
                                   join Detalle in db.DetallePelicula on trailer.RowID equals Detalle.EncabezadoPeliculaID
                                   select new
                                   {
                                       Trailer = trailer.Trailer,
                                       Afiche = trailer.Afiche,
                                       Formato = (Detalle.TipoFormatoID==0)?"":db.Opcion.Where(f=>f.RowID==Detalle.TipoFormatoID).FirstOrDefault().Nombre,
                                       Nombre = trailer.TituloLocal,
                                       Vers = (trailer.TipoIdiomaOriginalID==0)?"":db.Opcion.Where(f=>f.RowID==trailer.TipoIdiomaOriginalID).FirstOrDefault().Nombre,
                                       Clasf = (trailer.TipoClasificacionID==0)?"":db.Opcion.Where(f=>f.RowID==trailer.TipoClasificacionID).FirstOrDefault().Nombre

                                   }).ToList();
            foreach (var item in PeliculaTrailer)
            {
                if (item.Trailer.Contains("watch?v="))
                {
                    ReplaceTrailer = item.Trailer.Replace("watch?v=", "embed/");
                    ReplaceTrailer += "?rel=0&amp;controls=0&amp;showinfo=0&autoplay=1;vq=hd720&enablejsapi=1";
                }
                if (ContentTrailer.Contains("item active"))
                {
                    ItemActive = "item";
                }
                if (item.Afiche!="")
                {
                    ItemAfiches = "../"+item.Afiche;
                }else
                {
                    ItemAfiches = "../Repositorio_Imagenes/Imagenes_Generales/NoDisponibleAfiche.png";
                }
                ContentTrailer += "<div class='"+ItemActive+"'>" +
                 "<div class=\"col-sm-12\" id=\"Funciones\">" +
                         "<div class=\"row\">" +
                             "<div class=\"col-md-12\" style=\"margin-top:50px;\" id=\"Videos\">" +
                                        "<div id=\"video\">" +
                                            "<iframe width=\"800\" height=\"440\" src="+ReplaceTrailer+" frameborder =\"0\" allowfullscreen id=\"youtubeVideo\">"+
                                "</iframe>"+
                            "</div>"+
                        "</div>" +
                    "</div>" +
                    "<div id=\"AficheEspacio\">" +
                        "<div class=\"center-block\">" +
                              "<img src='"+ ItemAfiches + "'/>"+
                        "</div>" +
                    "</div>" +
                    "<div class=\"row\">" +
                        "<div class=\"col-md-6 col-md-offset-1\">" +
                                      "<div id=\"formato\">" +
                                          "<p>"+ "FORMATO" + "</p>"+
                                          "<h2 style=\"display:block;\">"+item.Formato+"</h2>"+
                            "</div>" +
                            "<div class=\"col-md-12 col-md-offset-2\" id=\"NamePelicula\">" +
                                          "<div id=\"Pelicula\">" +item.Nombre+"</div>" +
                            "</div>" +
                        "</div>" +
                        "<div class=\"row\">" +
                            "<div class=\"col-md-4 col-md-offset-2\" id=\"NameLang\">" +
                                          "<div id=\"Idioma\">" +
                                              "<span>"+ "VERSIÓN" + "</span>"+
                                              "<h2 style=\"display:block;\">"+item.Vers+"</h2>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"col-md-4 col-md-offset-1\" id=\"NameClasf\">" +
                                          "<div id=\"Clasf\">" +
                                              "<span>"+ "CLASIFICACIÓN" + "</span>" +
                                    "<h2 style=\"display:block;\"> "+item.Clasf+"</h2>"+
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
            "</div>";
            }
            return ContentTrailer;
        }
        #endregion


    }


}
