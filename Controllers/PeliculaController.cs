using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.IO;

namespace CinemaPOS.Controllers.Pelicula
{
    public class PeliculaController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();
        // GET: Pelicula
        [CheckSessionOutAttribute]
        public ActionResult Pelicula(int? RowID_Pelicula)
        {
            ViewBag.Distribuidor = db.Tercero.Where(t=>t.Opcion2.Codigo=="DISTRIBUIDOR").ToList();
            ViewBag.Pais = db.Pais.ToList();
            ViewBag.Clasificacion = db.Opcion.Where(c => c.Tipo.Codigo == "TIPOCLASIFICACIONPELICULA").ToList();
            ViewBag.Version = db.Opcion.Where(c => c.Tipo.Codigo == "TIPOVERSION").ToList();
            ViewBag.Formato = db.Opcion.Where(c => c.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.Idioma = db.Opcion.Where(i => i.Tipo.Codigo == "TIPOIDIOMA").ToList();
            ViewBag.GeneroPelicula = db.Opcion.Where(i => i.Tipo.Codigo == "TIPOGENEROPELICULA").ToList();
            ViewBag.Teatros = db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == RowID_Pelicula).ToList();
            var TeatroNoDisp = db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == RowID_Pelicula).ToList();
            List<Teatro> Teatros = new List<Teatro>();
            List<Teatro> TeatrosV2 = db.Teatro.ToList();
            foreach (TeatroPelicula item in TeatroNoDisp)
            {
                foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
                {
                    TeatrosV2.Remove(item2);
                }

            }

            ViewBag.TeatrosDisp = TeatrosV2;
            if (RowID_Pelicula==null)
            {
              
                return View(new EncabezadoPelicula());
            }
            else
            {
                return View(db.EncabezadoPelicula.Where(ep => ep.RowID == RowID_Pelicula).FirstOrDefault());
            }
            
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarPelicula(FormCollection formulario, HttpPostedFileBase afiche, HttpPostedFileBase thumbnail, int? RowID_Pelicula)
        {

            #region Encabezado 
            DetallePelicula ObjDetalle = new DetallePelicula();
            Boolean derecho_corto;
            EncabezadoPelicula ObjPelicula = new EncabezadoPelicula();
            var l = formulario["derecho_corto"];
            if (formulario["derecho_corto"] == "on")
            {
                derecho_corto = true;
            }
            else
            {
                derecho_corto = false;
            }
            if (RowID_Pelicula==0)
            {
                ObjPelicula.DerechoCorto = derecho_corto;
                ObjPelicula.Duracion = ConvertirHoraMinutos(formulario["duracion"]);
                ObjPelicula.DistribuidorID = int.Parse(formulario["distribuidor"]);
                ObjPelicula.PaisID = int.Parse(formulario["pais"]);
                ObjPelicula.TipoClasificacionID = int.Parse(formulario["clasificacion"]);
                ObjPelicula.TipoIdiomaOriginalID = int.Parse(formulario["idioma"]);
                ObjPelicula.TituloLocal = formulario["titulo_local"].ToUpper();
                ObjPelicula.TituloOriginal = formulario["titulo_original"].ToUpper();
                ObjPelicula.NumeroActa = formulario["numero_acta"];
                ObjPelicula.Sinopsis = formulario["sinopsis"];
                ObjPelicula.WebOficial = formulario["web_oficial"];
                ObjPelicula.FechaEstreno = ModelosPropios.Util.HoraInsertar(formulario["fecha_estreno"]);
                ObjPelicula.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "Confirmada").Select(e => e.RowID).First().ToString());
                ObjPelicula.CreadoPor =  Session["usuario_creacion"].ToString(); 
                ObjPelicula.FechaCreacion = DateTime.Now;
                db.EncabezadoPelicula.Add(ObjPelicula);
                db.SaveChanges();
            }
            else
            {
                ObjPelicula = db.EncabezadoPelicula.Where(ep => ep.RowID == RowID_Pelicula).FirstOrDefault();
                ObjPelicula.DerechoCorto = derecho_corto;
                ObjPelicula.Duracion = int.Parse(formulario["duracion"]);
                ObjPelicula.DistribuidorID = int.Parse(formulario["distribuidor"]);
                ObjPelicula.PaisID = int.Parse(formulario["pais"]);
                ObjPelicula.TipoClasificacionID = int.Parse(formulario["clasificacion"]);
                ObjPelicula.TipoIdiomaOriginalID = int.Parse(formulario["idioma"]);
                ObjPelicula.TituloLocal = formulario["titulo_local"];
                ObjPelicula.TituloOriginal = formulario["titulo_original"];
                ObjPelicula.NumeroActa = formulario["numero_acta"];
                ObjPelicula.Sinopsis = formulario["sinopsis"];
                ObjPelicula.WebOficial = formulario["web_oficial"];
                ObjPelicula.FechaEstreno = ModelosPropios.Util.HoraInsertar(formulario["fecha_estreno"]);
                ObjPelicula.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "Confirmada").Select(e => e.RowID).First().ToString());
                ObjPelicula.CreadoPor = Session["usuario_creacion"].ToString();
                ObjPelicula.FechaCreacion = DateTime.Now;
                db.SaveChanges();
            }
            #endregion
            int codigo_pelicula = ObjPelicula.RowID;
            #region Genero Pelicula
            var generos = formulario["genero_pelicula"].Split(',');
            if (generos.Count()!=0)
            {
                if (RowID_Pelicula != 0 && ObjPelicula.GeneroPelicula.Count() != 0)
                {
                    foreach (GeneroPelicula item in ObjPelicula.GeneroPelicula.Where(e => e.EncabezadoPelicula.RowID == ObjPelicula.RowID).ToList())
                    {
                        db.GeneroPelicula.Remove(item);
                        db.SaveChanges();
                    }
                }
                GeneroPelicula obj_genero = new GeneroPelicula();
                for (int i = 0; i < generos.Count(); i++)
                {
                    obj_genero.EncabezadoPeliculaID = codigo_pelicula;
                    obj_genero.TipoGeneroID = int.Parse(generos[i].ToString());
                    obj_genero.CreadoPor= Session["usuario_creacion"].ToString();
                    obj_genero.FechaCreacion = DateTime.Now;
                    db.GeneroPelicula.Add(obj_genero);
                    db.SaveChanges();
                }
            }
            #endregion
            #region Detalle 
            if (formulario["formato[]"]!=null)
            {
                var formatos = formulario["formato[]"].Split(',');
                var version = formulario["version[]"].Split(',');
                if (formatos.Length != ObjPelicula.DetallePelicula.Count())
                {
                    int cantidad_detalles = ObjPelicula.DetallePelicula.Count();
                                        int estadoId = db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "EnCreacion").FirstOrDefault().RowID;
                    for (int i = cantidad_detalles; i < formatos.Length; i++)
                    {
                        int FormatoID = int.Parse(formatos[i].ToString());
                        int VersionID = int.Parse(version[i].ToString());
                           // = db.DetallePelicula.Where(dt => dt.TipoFormatoID ==lol1  && dt.TipoIdiomaID ==  && dt.EncabezadoPeliculaID == ObjPelicula.RowID).Count();
                        if (db.DetallePelicula.Where(dt=>dt.TipoFormatoID== FormatoID && dt.TipoIdiomaID == VersionID && dt.EncabezadoPeliculaID==ObjPelicula.RowID).Count()==0)
                        {
                            ObjDetalle.EncabezadoPeliculaID = codigo_pelicula;
                            ObjDetalle.TipoFormatoID = FormatoID;
                            ObjDetalle.TipoIdiomaID = VersionID;
                            ObjDetalle.EstadoID = estadoId;
                            ObjDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                            ObjDetalle.FechaCreacion = DateTime.Now;
                            db.DetallePelicula.Add(ObjDetalle);
                            db.SaveChanges();
                        }
                    }
                }

            }

            #endregion

            #region Elenco 
            var actores = formulario["actores"].Split(',');
            Elenco ObjElenco = new Elenco();
            if (actores.Count() != 0)
            {
                if (RowID_Pelicula!=0 && ObjPelicula.Elenco.Count()!=0)
                {
                    foreach (Elenco item in ObjPelicula.Elenco.Where(e=>e.EncabezadoPelicula.RowID==ObjPelicula.RowID && e.Opcion.Codigo== "ACTOR").ToList())
                    {
                        db.Elenco.Remove(item);
                        db.SaveChanges();
                    }
                }
                foreach (var actor in actores)
                {
                    ObjElenco.Nombre = actor;
                    ObjElenco.TipoElencoID = int.Parse(db.Opcion.Where(o => o.Codigo == "ACTOR").FirstOrDefault().RowID.ToString());
                    ObjElenco.EncabezadoPeliculaID = codigo_pelicula;
                    db.Elenco.Add(ObjElenco);
                    db.SaveChanges();
                }

            }
            var directores = formulario["directores"].Split(',');
            if (directores.Count() != 0)
            {
                if (RowID_Pelicula != 0 && ObjPelicula.Elenco.Count() != 0)
                {
                    foreach (Elenco item in ObjPelicula.Elenco.Where(e => e.EncabezadoPelicula.RowID == ObjPelicula.RowID && e.Opcion.Codigo == "DIRECTORES").ToList())
                    {
                        db.Elenco.Remove(item);
                        db.SaveChanges();
                    }
                }
                foreach (var director in directores)
                {
                    ObjElenco.Nombre = director;
                    ObjElenco.TipoElencoID = int.Parse(db.Opcion.Where(o => o.Codigo == "DIRECTORES").FirstOrDefault().RowID.ToString());
                    ObjElenco.EncabezadoPeliculaID = codigo_pelicula;
                    db.Elenco.Add(ObjElenco);
                    db.SaveChanges();
                }
            }

            #endregion

            #region Medio
            MedioPelicula ObjMedio = new MedioPelicula();
            string ruta_fiche = "";
            string ruta_thumbnail = "";
            if (RowID_Pelicula == 0 ||ObjPelicula.MedioPelicula.Count()==0)
            {
                //inserta solamente el afiche
                if (afiche != null)
                {
                    ruta_fiche = ObjPelicula.TituloLocal + Path.GetExtension(afiche.FileName);
                    afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche));
                    ruta_fiche = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche;
                }
                // inserta solamente el afiche en miniatura
                if (thumbnail != null)
                {
                    ruta_thumbnail = ObjPelicula.TituloLocal + "_thumbnail" + Path.GetExtension(thumbnail.FileName);
                    afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail));
                    ruta_thumbnail = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail;
                }

                ObjMedio.Afiche = ruta_fiche;
                ObjMedio.AficheMiniatura = ruta_thumbnail;
                ObjMedio.Trailer = formulario["trailer"];
                ObjMedio.Teaser = formulario["teaser"];
                ObjMedio.EncabezadoPeliculaID = codigo_pelicula;
                ObjMedio.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "Confirmada").Select(e => e.RowID).First().ToString());
                ObjMedio.CreadoPor = Session["usuario_creacion"].ToString();
                ObjMedio.FechaCreacion = DateTime.Now;
                db.MedioPelicula.Add(ObjMedio);
                db.SaveChanges();
            }
            else
            {
                ObjMedio = db.MedioPelicula.Where(mp => mp.EncabezadoPeliculaID == RowID_Pelicula).OrderByDescending(mp => mp.RowID).First();
                ruta_fiche = ObjMedio.Afiche;
                ruta_thumbnail = ObjMedio.AficheMiniatura;
                //Actualiza solamente el afiche
                if (afiche != null)
                {
                    ruta_fiche = ObjPelicula.TituloLocal + Path.GetExtension(afiche.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales"), ruta_fiche);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche));
                    ruta_fiche = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche;
                    
                }
                // Actualiza solamente el afiche en miniatura
                if (thumbnail != null)
                {
                    ruta_thumbnail = ObjPelicula.TituloLocal + "_thumbnail" + Path.GetExtension(thumbnail.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales"), ruta_thumbnail);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    
                    thumbnail.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail));
                    ruta_thumbnail = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail;
                }

                ObjMedio.Afiche = ruta_fiche;
                ObjMedio.AficheMiniatura = ruta_thumbnail;
                ObjMedio.Trailer = formulario["trailer"];
                ObjMedio.Teaser = formulario["teaser"];
                ObjMedio.EncabezadoPeliculaID = codigo_pelicula;
                ObjMedio.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "Confirmada").Select(e => e.RowID).First().ToString());
                ObjMedio.CreadoPor = Session["usuario_creacion"].ToString();
                ObjMedio.FechaCreacion = DateTime.Now;
                
                db.MedioPelicula.Add(ObjMedio);
                db.SaveChanges();
            }
            //if (afiche != null)
            //{


            //    /* ObjPelicula.TituloOriginal + "_thumbnail" + Path.GetExtension(thumbnail.FileName);*/

            //    if (RowID_Pelicula != 0 && ObjPelicula.MedioPelicula.Count() != 0)
            //    {
            //        ObjMedio = db.MedioPelicula.Where(mp => mp.EncabezadoPeliculaID == RowID_Pelicula).OrderBy(mp => mp.RowID).First();
            //        ruta_fiche = ObjPelicula.TituloOriginal + Path.GetExtension(afiche.FileName);
            //        ruta_thumbnail = "" /*ObjPelicula.TituloOriginal + "_thumbnail" + Path.GetExtension(thumbnail.FileName)*/;
            //        var path_afiche = System.IO.Path.Combine(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales"), ruta_fiche);


            //        if (System.IO.File.Exists(path_afiche))
            //        {
            //            System.IO.File.Delete(path_afiche);
            //            afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche));
            //            ruta_fiche = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche;
            //            //thumbnail.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + thumbnail));
            //            //ruta_thumbnail = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail;
            //        }
            //        else
            //        {
            //            try
            //            {
            //                afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche));
            //                ruta_fiche = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche;
            //            }
            //            catch (Exception)
            //            {

            //                throw;
            //            }

            //        }
            //        ObjMedio.Afiche = ruta_fiche;
            //        ObjMedio.AficheMiniatura = ruta_thumbnail;
            //        ObjMedio.Trailer = formulario["trailer"];
            //        ObjMedio.Teaser = formulario["teaser"];
            //        ObjMedio.EncabezadoPeliculaID = codigo_pelicula;
            //        ObjMedio.EstadoID = int.Parse(db.Estado.Where(e => e.TipoEstado.Codigo == "TIPOPELICULA" && e.Nombre == "Confirmada").Select(e => e.RowID).First().ToString());
            //        ObjMedio.CreadoPor = "Admin";
            //        ObjMedio.FechaCreacion = DateTime.Now;
            //        db.SaveChanges();
            //    }

            //    afiche.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche));
            //    ruta_fiche = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_fiche;
            //    thumbnail.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Poster_Peliculas/" + thumbnail));
            //    ruta_thumbnail = "Repositorio_Imagenes/Poster_Peliculas/" + ruta_thumbnail;


            //}
            #endregion

            #region Porcentaje
            PorcentajeParticipacion ObjParticipacion = new PorcentajeParticipacion();
            if (RowID_Pelicula != 0)
            {
                if (ObjPelicula.PorcentajeParticipacion.Count() == 0)
                {
                    if (formulario["porcentaje[]"] != null)
                    {
                        var porcentajes = formulario["porcentaje[]"].Split(',');
                        var fecha_inicio = formulario["fecha_inicial_porcentaje[]"].Split(',');
                        var fecha_final = formulario["fecha_final_porcentaje[]"].Split(',');
                        var nombre = formulario["nombre_porcentaje[]"].Split(',');
                        if (porcentajes.Count() != 0)
                        {

                            for (int i = 0; i < porcentajes.Length; i++)
                            {
                                ObjParticipacion.EncabezadoPeliculaID = codigo_pelicula;
                                ObjParticipacion.Porcentaje = int.Parse(porcentajes[i]);
                                ObjParticipacion.FechaInicial = ModelosPropios.Util.HoraInsertar(fecha_inicio[i]);
                                ObjParticipacion.FechaFinal = ModelosPropios.Util.HoraInsertar(fecha_final[i]);
                                ObjParticipacion.Nombre = nombre[i];
                                ObjParticipacion.CreadoPor = Session["usuario_creacion"].ToString();
                                ObjParticipacion.FechaCreacion = DateTime.Now;
                                db.PorcentajeParticipacion.Add(ObjParticipacion);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    var porcentajes = formulario["porcentaje[]"].Split(',');
                    var fecha_inicio = formulario["fecha_inicial_porcentaje[]"].Split(',');
                    var fecha_final = formulario["fecha_final_porcentaje[]"].Split(',');
                    var nombre = formulario["nombre_porcentaje[]"].Split(',');
                    int i = 0;
                    foreach (PorcentajeParticipacion item in ObjPelicula.PorcentajeParticipacion)
                    {
                        item.EncabezadoPeliculaID = codigo_pelicula;
                        item.Porcentaje = int.Parse(porcentajes[i]);
                        item.FechaInicial = ModelosPropios.Util.HoraInsertar(fecha_inicio[i]);
                        item.FechaFinal = ModelosPropios.Util.HoraInsertar(fecha_final[i]);
                        item.Nombre = nombre[i];
                        item.CreadoPor = Session["usuario_creacion"].ToString();
                        item.FechaCreacion = DateTime.Now;
                        db.SaveChanges();
                        i = i + 1;
                    }
                    if (i+1!=ObjPelicula.PorcentajeParticipacion.Count())
                    {
                        for (int j=i; j < porcentajes.Length; j++)
                        {
                            ObjParticipacion.EncabezadoPeliculaID = codigo_pelicula;
                            ObjParticipacion.Porcentaje = int.Parse(porcentajes[i]);
                            ObjParticipacion.FechaInicial = ModelosPropios.Util.HoraInsertar(fecha_inicio[i]);
                            ObjParticipacion.FechaFinal = ModelosPropios.Util.HoraInsertar(fecha_final[i]);
                            ObjParticipacion.Nombre = nombre[i];
                            ObjParticipacion.CreadoPor = Session["usuario_creacion"].ToString();
                            ObjParticipacion.FechaCreacion = DateTime.Now;
                            db.PorcentajeParticipacion.Add(ObjParticipacion);
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                //////revisa 
                if (formulario["porcentaje[]"] != null)
                {
                    var porcentajes = formulario["porcentaje[]"].Split(',');
                    var fecha_inicio = formulario["fecha_inicial_porcentaje[]"].Split(',');
                    var fecha_final = formulario["fecha_final_porcentaje[]"].Split(',');
                    var nombre = formulario["nombre_porcentaje[]"].Split(',');
                    if (porcentajes.Count() != 0)
                    {
                        for (int i = 0; i < porcentajes.Length; i++)
                        {
                            ObjParticipacion.EncabezadoPeliculaID = codigo_pelicula;
                            ObjParticipacion.Porcentaje = int.Parse(porcentajes[i]);
                            ObjParticipacion.FechaInicial = ModelosPropios.Util.HoraInsertar(fecha_inicio[i]);
                            ObjParticipacion.FechaFinal = ModelosPropios.Util.HoraInsertar(fecha_final[i]);
                            ObjParticipacion.Nombre = nombre[i];
                            ObjParticipacion.CreadoPor = Session["usuario_creacion"].ToString();
                            ObjParticipacion.FechaCreacion = DateTime.Now;
                            db.PorcentajeParticipacion.Add(ObjParticipacion);
                            db.SaveChanges();
                        }
                    }
                }
            }
            #endregion
            return Json("Realizado");
        }

        public int ConvertirHoraMinutos(string Hora)
        {
            var Minutos = int.Parse(Hora.Split(':')[0]) * 60 + int.Parse(Hora.Split(':')[1]);
            return int.Parse(Minutos.ToString());
        }

        [CheckSessionOutAttribute]
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
        public ActionResult VistaPelicula()
        {
            ViewBag.Peliculas = db.EncabezadoPelicula.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult EliminarTeatroPelicula(int? RowID)
        {
            string proccess = "";
            if (RowID != null)
            {
                TeatroPelicula teatro = db.TeatroPelicula.Where(f => f.RowID == RowID).FirstOrDefault();
                if (teatro != null)
                {
                    db.TeatroPelicula.Remove(teatro);
                    db.SaveChanges();
                    proccess = "ok";
                }
                else
                {proccess = "error";}
            }
            return Json(proccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ModalTeatroPelicula(int? rowid)
        {
            var TeatroNoDisp = db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == rowid).ToList();
            List<Teatro> Teatros = new List<Teatro>();
            List<Teatro> TeatrosV2 = db.Teatro.ToList();
            foreach (TeatroPelicula item in TeatroNoDisp)
            {
                foreach (var item2 in db.Teatro.Where(f=>f.RowID==item.TeatroID))
                {
                    TeatrosV2.Remove(item2);
                }

            }

            ViewBag.TeatrosDisp = TeatrosV2;
            var id = db.EncabezadoPelicula.Where(f => f.RowID == rowid).FirstOrDefault();
            ViewBag.Pelicula = id.TituloLocal;
            ViewBag.PeliculaV2 = id.RowID;
            return View();
        }

        public JsonResult GuardarTeatro(int rowidPelicula,int rowidTeatro)
        {
            TeatroPelicula teatro_pelicula = new TeatroPelicula();
            string validate = "";
            if (rowidTeatro != 0)
            {
                try
                {
                    teatro_pelicula.TeatroID = rowidTeatro;
                    teatro_pelicula.EncabezadoPeliculaID = rowidPelicula;
                    teatro_pelicula.CreadoPor = Session["usuario_creacion"].ToString();
                    teatro_pelicula.FechaCreacion = DateTime.Now;
                    db.TeatroPelicula.Add(teatro_pelicula);
                    db.SaveChanges();
                    validate = "ok";
                }
                catch (Exception)
                {validate = "error";}
            }
            return Json(validate,JsonRequestBehavior.AllowGet);
        }




        public JsonResult GuardarTodos(int rowidPelicula)
        {
            TeatroPelicula teatro_pelicula = new TeatroPelicula();
            string validate = "";
           try{
               var TeatroNoDisp = db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == rowidPelicula).ToList();
               List<Teatro> Teatros = new List<Teatro>();
               List<Teatro> TeatrosV2 = db.Teatro.ToList();
               foreach (TeatroPelicula item in TeatroNoDisp)
               {
                   foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
                   {
                       TeatrosV2.Remove(item2);
                   }

               }


               foreach (var item3 in TeatrosV2)
               {
                   teatro_pelicula.TeatroID = item3.RowID;
                   teatro_pelicula.EncabezadoPeliculaID = rowidPelicula;
                   teatro_pelicula.CreadoPor = Session["usuario_creacion"].ToString();
                   teatro_pelicula.FechaCreacion = DateTime.Now;
                   db.TeatroPelicula.Add(teatro_pelicula);
                   db.SaveChanges();
                   validate = "ok";

               }
                  
                }
                catch (Exception)
                { validate = "error"; }
            
            return Json(validate, JsonRequestBehavior.AllowGet);
        }



        [CheckSessionOutAttribute]
        public JsonResult CargarTeatro(Int32 rowidPelicula)
        {            
          ////llenar 
            var TeatroNoDisp = db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == rowidPelicula).ToList();
            List<Teatro> Teatros = new List<Teatro>();
            List<Teatro> TeatrosV2 = db.Teatro.ToList();
            foreach (TeatroPelicula item in TeatroNoDisp)
            {
                foreach (var item2 in db.Teatro.Where(f => f.RowID == item.TeatroID))
                {
                     TeatrosV2.Remove(item2);
                }

            }


            ///Para formar el Json
            var query = (from Teatro  in TeatrosV2
                         select new
                         {
                             RowId = Teatro.RowID,
                             Nombre = Teatro.Nombre,
                             Ciudad = Teatro.Ciudad.Nombre
                         }
            ).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }



        public JsonResult RefrescarTabla(int? rowid)
        {
            var data = (from teatroPelicula in db.TeatroPelicula.Where(f => f.EncabezadoPeliculaID == rowid)
                        select new
                        {
                            rowid = teatroPelicula.RowID,
                            teatro = (teatroPelicula.TeatroID == 0) ? "" : teatroPelicula.Teatro.Nombre,
                            ubicacion = (teatroPelicula.Teatro.CiudadID == null) ? "" : teatroPelicula.Teatro.Ciudad.Nombre
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

      

    }


}