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
            DateTime FormatoHora;
            var FormatoPelicula = "";
            var ImgServicio = "";
            if (string.IsNullOrEmpty(FechaValidar))
            {
                FechaValidar = Convert.ToString(DateTime.Now.ToString("MM/dd/yyyy"));
            }
            List<Util.FuncionEncabezado> a = (from reg in db.FuncionesTvShow.Where(f => f.FechaFuncion == FechaValidar)
                                              select new Util.FuncionEncabezado
                                              {
                                                  titulo = reg.TituloLocal,
                                                  version = reg.PeliculaVersion,
                                                  Formato = reg.PeliculaIdioma,
                                                  pelicula = reg.RowID_EncabezadoPelicula,
                                                  peliculaDetalleID = reg.RowID_Detalle,
                                                  Poster = reg.Afiche,
                                                  Servicio = reg.Servicio,
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
                                                        Servicio = reg.Servicio,
                                                        Lista = db.FuncionesTvShow.Where(f => f.DetallePeliculaID == reg.peliculaDetalleID && f.FechaFuncion == FechaValidar).Distinct().ToList()

                                                    }).ToList();

            string resultad = "";
            int contador = 0;
            foreach (var item in Formato)
            {
                if (item.Poster == "")
                {
                    item.Poster = "../Repositorio_Imagenes/Poster_Peliculas/SING.jpg";
                }
                #region TIPO VERSION
                if (item.version.Contains(Util.Constantes.TIPO_VERSION_2D))
                {
                    FormatoPelicula = "<img src=\"../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Negro_Sin_Fondo/logo 2D-01-Funciones.png\" id='Icono2D'/>";
                }
                else
                {
                    FormatoPelicula = "<img src=\"../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Negro_Sin_Fondo/logo 3D-01-Funciones.png\" id='Icono3D'/>";
                }
                #endregion
                #region TIPO SERVICIOS
                switch (item.Servicio)
                {
                    case Util.Constantes.SERVICIO_4DX:
                        ImgServicio = "../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Fondo_Negro/LOGOS_SALAS_ROYAL_FILMS2-01.png";
                        break;
                    case Util.Constantes.SERVICIO_4TD:
                        ImgServicio = "";
                        break;
                    case Util.Constantes.SERVICIO_Atmos:
                        ImgServicio = "";
                        break;
                    case Util.Constantes.SERVICIO_Covan:
                        ImgServicio = "../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Fondo_Negro/LOGOS_SALAS_ROYAL_FILMS2-06.png";
                        break;
                    case Util.Constantes.SERVICIO_GENERAL:
                        ImgServicio = "";
                        break;
                    case Util.Constantes.SERVICIO_PLUS:
                        ImgServicio = "../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Fondo_Negro/LOGOS_SALAS_ROYAL_FILMS2-07.png";
                        break;
                    case Util.Constantes.SERVICIO_ULTRA:
                        ImgServicio = "../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Fondo_Negro/LOGOS_SALAS_ROYAL_FILMS2-02.png";
                        break;
                    case Util.Constantes.SERVICIO_VIP:
                        ImgServicio = "../../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Fondo_Negro/LOGOS_SALAS_ROYAL_FILMS2-05.png";
                        break;
                    default:
                        break;
                }
                #endregion
                resultad += "<li id=\"FuncionGlobal\">" +
                       "<div class='col-md-12'>" +
                            "<div id=\"IconosGeneral\">" +
                                "<img src='" + ImgServicio + "' id='IconosFunciones'/>" +
                            "</div>" +
                       "<div class=\"row\" id=\"General\">" +
                            "<div class=\"col-md-2 \" id=\"Afiche\">" +
                                "<img src=" + "../" + item.Poster + " alt=\"\" />" +
                                "</div>" +
                        "<div class=\"col-md-2 \" id=\"TituloFuncion\">" +
                         "<div id = \"Titulo\" ><h3 class='text-5x'> " + item.titulo + "</h3><div id='PrinFormarto'><div class='triangulo-equilatero-bottom-left '></div><h2 id='FormatoVersion'>" + FormatoPelicula + "<span>" + "</span></h2></div></div>" +
                         "</div>" +

                        "</div>" +
                "</div>";
                resultad += "<div class='row' id='CompleteContent'><div class='col-md-12' id='Horarios'>";
                //IEnumerable<Funciones> list = item.Lista.GroupBy(x => x.PeliculaIdioma).Select(y => y.First());
                resultad += item.Formato + ": ";
                foreach (var funcion in item.Lista)
                {
                    //if (contador==0)
                    //{

                    //}
                    FormatoHora = Convert.ToDateTime(funcion.HoraInicial.ToString());

                    resultad += " " + FormatoHora.ToString("hh:mm") + "  -";

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
                resultad = resultad.TrimEnd('-');
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
            var PeliculaTrailer = (from trailer in db.TVShowTrailers
                                   select new
                                   {
                                       Trailer = trailer.Trailer,
                                       Afiche = trailer.Afiche,
                                       Formato = trailer.Formato,
                                       Nombre = trailer.Nombre,
                                       Vers = trailer.Vers,
                                       Clasf = trailer.Clasf

                                   }).ToList();
            foreach (var item in PeliculaTrailer)
            {
                if (item.Trailer.Contains("www.youtube.com"))
                {
                }
                else
                {
                    ReplaceTrailer = "https://www.youtube.com/embed/" + item.Trailer + "?rel=0&amp;controls=0&amp;showinfo=0&autoplay=1;vq=hd720&enablejsapi=1";
                }
                if (item.Trailer.Contains("watch?v="))
                {
                    ReplaceTrailer = item.Trailer.Replace("watch?v=", "embed/");
                    ReplaceTrailer += "?rel=0&amp;controls=0&amp;showinfo=0&autoplay=1;vq=hd720&enablejsapi=1";
                }
                if (ContentTrailer.Contains("item active"))
                {
                    ItemActive = "item";
                }
                if (item.Afiche != "")
                {
                    ItemAfiches = "../" + item.Afiche;
                }
                else
                {
                    ItemAfiches = "../Repositorio_Imagenes/Imagenes_Generales/NoDisponibleAfiche.png";
                }
                ContentTrailer += "<div class='" + ItemActive + "'>" +
                 "<div class=\"col-sm-12\" id=\"Funciones\">" +
                         "<div class=\"row\">" +
                             "<div class=\"col-md-12\" style=\"margin-top:50px;\" id=\"Videos\">" +
                                        "<div id=\"video\">" +
                                            "<iframe width=\"800\" height=\"440\" src='" + ReplaceTrailer + "' frameborder =\"0\" allowfullscreen id=\"youtubeVideo\" volume=\"0\">" +
                                "</iframe>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                    "<div id=\"AficheEspacio\">" +
                        "<div class=\"center-block\">" +
                              "<img src='" + ItemAfiches + "'/>" +
                        "</div>" +
                    "</div>" +
                    "<div class=\"row\">" +
                        "<div class=\"col-md-6 col-md-offset-1\">" +
                                      "<div id=\"formato\">" +
                                          "<p>" + "FORMATO" + "</p>" +
                                          "<h2 style=\"display:block;\">" + item.Formato + "</h2>" +
                            "</div>" +
                            "<div class=\"col-md-12 col-md-offset-2\" id=\"NamePelicula\">" +
                                          "<div id=\"Pelicula\">" + item.Nombre + "</div>" +
                            "</div>" +
                        "</div>" +
                        "<div class=\"row\">" +
                            "<div class=\"col-md-4 col-md-offset-2\" id=\"NameLang\">" +
                                          "<div id=\"Idioma\">" +
                                              "<span>" + "VERSIÓN" + "</span>" +
                                              "<h2 style=\"display:block;\">" + item.Vers + "</h2>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"col-md-4 col-md-offset-1\" id=\"NameClasf\">" +
                                          "<div id=\"Clasf\">" +
                                              "<span>" + "CLASIFICACIÓN" + "</span>" +
                                    "<h2 style=\"display:block;\"> " + item.Clasf + "</h2>" +
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

        #region LISTA PRECIOS
        [CheckSessionOutAttribute]
        public ActionResult TvShowListaPrecios()
        {
            return View();
        }

        public string TVShowPrecios(string Teatro)
        {
            string ResultPrecios = "";
            string GrupoD = "";
            string ImgFormato = "";
            string ImagenF = "";
            string ItemActive = "item active";
            int i = 0;
            Teatro = "SAN MARTIN";
            var Teatros = db.Teatro.Where(f => f.Nombre.Contains(Teatro.ToUpper())).FirstOrDefault();
            List<TvShowListaPrecios> ListadoPrecios = new List<TvShowListaPrecios>();
            #region CONSULTAS
            var ListadoDias = (from Dias in db.GrupoDias
                               select new
                               {
                                   RowID = Dias.RowID,
                                   Nombre = Dias.Nombre,
                                   Dias = Dias.Dias,
                                   HoraInicio = Dias.HoraInicio,
                                   HoraFin = Dias.HoraFin
                               }).Distinct().Take(5).ToList();

            ListadoPrecios = (from Precios in db.TvShowListaPrecios.Where(f => f.Teatro == Teatros.Nombre.ToUpper()).ToList()
                              select new TvShowListaPrecios
                              {
                                  RowIDTarifa = Precios.RowIDTarifa,
                                  RowIDEncabezado = Precios.RowIDEncabezado,
                                  Dias = Precios.Dias,
                                  Precio = Precios.Precio,
                                  PrecioTCR=Precios.PrecioTCR,
                                  Formato = Precios.Formato,
                                  TipoTarifa = Precios.TipoTarifa,
                                  Teatro = Precios.Teatro,
                                  Servicio = Precios.Servicio,
                                  FechaInicial = Precios.FechaInicial,
                                  FechaFinal = Precios.FechaFinal,
                                  HoraInicial = Precios.HoraInicial,
                                  HoraFinal = Precios.HoraFinal,
                                  RowIDGrupoDias = Precios.RowIDGrupoDias,
                                  GrupoDias = Precios.GrupoDias
                              }).Distinct().ToList();
            #endregion
            string le = "";
            string grupo = "";
            var listavista = db.TvShowListaPrecios.Where(f=>f.Teatro==Teatro).OrderBy(f=>new{f.Formato,f.GrupoDias }).GroupBy(lol => new {lol.Formato,lol.GrupoDias } ).ToList();
            var lolol = listavista.GroupBy(lol => lol.GroupBy(lll => lll.Formato)).ToList();
            foreach (var item in listavista)
            {
                int cantidad = 1;
                
                //le += "div para grupos";
                foreach (var precio in item)
                {
                    if (precio.Formato == Util.Constantes.FORMATO_3D)
                    {
                        //ingresa al string de 3D
                    }
                    else
                    {
                        if (cantidad == 1)
                        {
                            le += "<li class='precios1'>";
                        }
                            if (precio.PrecioTCR==0)
                            {
                                le += "<span id=''>" + precio.Precio + "<span>";
                            }
                            else if (precio.PrecioTCR != 0)
                            {
                                le += "<span id='TCR'>" + precio.PrecioTCR + "<span>";
                            }

                        if (cantidad == item.Count())
                        {
                            le += "</li>";
                        }
                        //ingresa al string de 2D
                        cantidad++;
                    }

                    //le += "<div class='col-md-4'></div>";
                    //grupo = precio.GrupoDias;

                    //    if (precio.PrecioTCR == 0)
                    //    {
                    //        le += "<li class=\"precios1\">" + precio.Formato + precio.GrupoDias + "<span id =\"general\" >" + precio.Precio + "</span></li>";
                    //    }
                    //    else
                    //    {
                    //        le += "<li class=\"precios1\">" + precio.Formato + precio.GrupoDias + "<span id =\"TCR\" >" + precio.PrecioTCR + "</span></li>";
                    //    }

                }
                    //le += "</div>";
                    //le += "div que contiene tarifas de grupos";
            }
            #region CONTENIDOS
            foreach (var item in listavista)
            {
                if (i>=5)
                {
                }
                else
                {
                    GrupoD += "<li class='uno'>" + item.FirstOrDefault().GrupoDias + "</li>";
                }
                //IEnumerable<TvShowListaPrecios> list = item.Dias.GroupBy().Select(y => y.First());
                i++;
            }

            foreach (var item in ListadoPrecios)
            {
                //#region IMAGENES FORMATOS
                //switch (item.Formato)
                //{
                //    case Util.Constantes.FORMATO_2D:
                //        ImgFormato = "../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Blanco_Sin_Fondo/logo 2D-01.png";
                //        ImagenF += "<div class=\"col-md-4\">" +
                //            "<img src ='" + ImgFormato + "' />" +
                //        "</div>";
                //        break;
                //    case Util.Constantes.FORMATO_3D:
                //        ImgFormato = "../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Blanco_Sin_Fondo/logo 3D-01.png";
                //        ImagenF += "<div class=\"col-md-4\">" +
                //                    "<img src ='" + ImgFormato + "' />" +
                //                "</div>";
                //        break;
                //    case Util.Constantes.FORMATO_4D:
                //        ImgFormato = "../Repositorio_Imagenes/Imagenes_Generales/Iconos_TvShow/Blanco_Sin_Fondo/logo 4dx-01.png";
                //        ImagenF += "<div class=\"col-md-4\">" +
                //                    "<img src ='" + ImgFormato + "' />" +
                //                "</div>";
                //        break;
                //}
                //#endregion
                if (ResultPrecios.Contains("item active"))
                {
                    ItemActive = "item";
                }
                ResultPrecios += "<div class='" + ItemActive + "'><div id='ListadoPrecios'><div class=\"col-md-12\">" +
                            "<div id=\"TitulosPrecios\" >" +
                                 "<div class=\"row\">" +
                                "<div class=\"col-md-2\"></div>" +
                                "<div id=\"Logos\">" +
                                    ImagenF+
                                "</div>" +
                            "</div>" +
                        "</div>" +
                        "<br />" +
                        "<div id=\"GenAll\">" +
                            "<div class=\"row\" id=\"GenAll2\">" +
                                "<div class=\"col-md-2\">" +
                                    "<ul id=\"DiasPrecios\"> " +
                                        GrupoD +
                                    "</ul>" +
                                "</div>" +
                                "<div class=\"col-md-4\">" +
                                    "<ul id=\"ColorGeneral\"> " +
                                        "<li class=\"precios1\">" + item.Precio + "<span id =\"TCR\" >" + item.PrecioTCR + "</span></li>" +
                                        "<li class=\"precios1\">" + item.Precio + "<span id =\"TCR\" >" + item.PrecioTCR + "</span></li>" +
                                        "<li class=\"precios1\">" + item.Precio + "<span id =\"TCR\" >" + item.PrecioTCR + "</span></li>" +
                                        "<li class=\"precios1\">" + item.Precio + "<span id =\"TCR\" >" + item.PrecioTCR + "</span></li>" +
                                        "<li class=\"precios1\">" + item.Precio + "<span id =\"TCR\" >" + item.PrecioTCR + "</span></li>" +
                                    "</ul>" +
                                "</div>" +
                                "<div class=\"col-md-4\">" +
                                    "<ul id=\"ColorGeneral\" >" +
                                        "<li class=\"precios1\"><span id =\"TCR\" ></span></li>" +
                                        "<li class=\"precios1\"><span id =\"TCR\" ></span></li>" +
                                        "<li class=\"precios1\"><span id =\"TCR\" ></span></li>" +
                                        "<li class=\"precios1\"><span id =\"TCR\" ></span></li>" +
                                        "<li class=\"precios1\"><span id =\"TCR\" ></span></li>" +
                                    "</ul>" +
                                "</div> " +
                            "</div> " +
                       "</div>" +
                    "</div></div></div>";
            }
            #endregion
            return ResultPrecios;
        }
        #endregion
    }


}
