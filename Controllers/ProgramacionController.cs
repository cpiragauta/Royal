using CinemaPOS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.ModelosPropios;

namespace CinemaPOS.Controllers
{
    public class ProgramacionController : Controller
    {

        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult VistaProgramacion(int? RowId)
        {
            return View(db.EncabezadoProgramacion);
        }

        [CheckSessionOutAttribute]
        public ActionResult Programacion(int? RowID_EncabezadoProgramacion)
        {
            ViewBag.ListaTeatro = db.Teatro.Where(f => f.Estado.Codigo == "ABIERTO").ToList();
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOFUNCION");
            if (RowID_EncabezadoProgramacion != null && RowID_EncabezadoProgramacion != 0)
            {
                RowID_EncabezadoProgramacion = int.Parse(Request.Params["RowID_EncabezadoProgramacion"].ToString());
                ViewBag.listaFunciones = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                EncabezadoProgramacion programacion = db.EncabezadoProgramacion.FirstOrDefault(t => t.RowID == RowID_EncabezadoProgramacion);
                if (programacion != null)
                {
                    ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO").OrderBy(f => f.RowID);
                    ViewBag.ListaPeliculas = db.TeatroPelicula.Where(f => f.TeatroID == programacion.TeatroID);
                    ViewBag.ListasPrecios = db.ListaEncabezado.Where(f => f.TeatroID == programacion.TeatroID && f.FechaInicial >= programacion.FechaInicial);
                }
                return View(programacion);
            }
            else
            {
                return View(new EncabezadoProgramacion());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult TraerFunciones(int? RowID_Encabezado)
        {
            var query = (from funcion in db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Encabezado)
                         select new
                         {
                             titulo = funcion.DetallePelicula.EncabezadoPelicula.TituloLocal,
                             version = funcion.DetallePelicula.Opcion.Codigo,
                             FechaInicialAno = funcion.Fecha.Value.Year,
                             FechaInicialMes = funcion.Fecha.Value.Month,
                             FechaInicialDia = funcion.Fecha.Value.Day,
                             FechaInicialHora = funcion.HoraInicial.Value.Hours,
                             FechaInicialMinuto = funcion.HoraInicial.Value.Minutes,

                             FechaFinalHora = funcion.HoraFinal.Value.Hours,
                             FechaFinalMinuto = funcion.HoraFinal.Value.Minutes,

                         }
           ).ToList();
            return Json(query);

        }
        // '2016-11-05T16:45:00',
        public JsonResult Buscar_Funcion(int? RowID_Funcion)
        {
            var objecto = (from o in db.Funcion.Where(ld => ld.RowID == RowID_Funcion).ToList()
                           select new
                           {
                               RowID = o.RowID,
                               SalaID = o.SalaID,
                               PeliculaID = o.DetallePeliculaID,
                               TiempoCorto = o.TiempoCorto,
                               TiempoAseo = o.TiempoAseo,
                               FechaFuncion = o.Fecha.Value.ToString("dd/MM/yyyy")
                           }).ToList();
            return Json(objecto);
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarProgramacionEncabezado(FormCollection formulario, int? RowID_EncabezadoProgramacion)
        {
            String respuesta = "";
            int RowID = 0;
            EncabezadoProgramacion ObjEncabezado = new EncabezadoProgramacion();
            formulario = DeSerialize(formulario);

            IFormatProvider culture = new CultureInfo("es-ES", true);
            DateTime NuevaFechaInicial = ModelosPropios.Util.FechaInsertar(formulario["FechaInicial"]);

            respuesta = formulario["FechaInicial"] + "   " + NuevaFechaInicial.ToString();


            DateTime NuevaFechaFinal = ModelosPropios.Util.FechaInsertar(formulario["FechaFinal"]);
            int TeatroID = Convert.ToInt32(formulario["Teatro"]);
            List<EncabezadoProgramacion> Programaciones = db.EncabezadoProgramacion.Where(f => f.TeatroID == TeatroID
                && (
                (NuevaFechaInicial >= f.FechaInicial && NuevaFechaInicial <= f.FechaFinal) ||
                (NuevaFechaFinal >= f.FechaInicial && NuevaFechaFinal <= f.FechaFinal) ||
                (NuevaFechaInicial >= f.FechaInicial && NuevaFechaFinal <= f.FechaFinal))).ToList();
            if (NuevaFechaInicial > NuevaFechaFinal)
            {
                respuesta = "la fecha Inicial debe ser menor a la Final";
                return Json(new { respuesta = "Error " + respuesta });
            }
            if (RowID_EncabezadoProgramacion == 0)
            {
                if (Programaciones.Count > 0)
                {
                    respuesta = "Ya existe una programacion con la misma vigencia " + Programaciones.First().Titulo;
                    return Json(new { respuesta = "Error " + respuesta });
                }
                ObjEncabezado = CargarDatosEncabezadoProgramacion(ObjEncabezado, formulario);
                try
                {
                    db.EncabezadoProgramacion.Add(ObjEncabezado);

                    //List<Funcion> listaFuncion = new List<Funcion>();
                    //listaFuncion = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                    //foreach (Funcion item in listaFuncion)
                    //{
                    //    item.EstadoID = ObjEncabezado.EstadoID;
                    //}
                    db.SaveChanges();
                    respuesta = respuesta + "Guardado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = respuesta + " Error " + ex.Message }); }
            }
            if (RowID_EncabezadoProgramacion != 0)//Para Actualiar
            {
                foreach (EncabezadoProgramacion item in Programaciones)
                {
                    if (item.RowID != RowID_EncabezadoProgramacion)
                    {
                        respuesta = "Ya existe una programacion con la misma vigencia " + Programaciones.First().Titulo;
                        return Json(new { respuesta = "Error " + respuesta });
                    }
                }

                ObjEncabezado = db.EncabezadoProgramacion.Where(t => t.RowID == RowID_EncabezadoProgramacion).FirstOrDefault();
                ObjEncabezado = CargarDatosEncabezadoProgramacion(ObjEncabezado, formulario);
                using (var context = new CinemaPOSEntities())
                {
                    context.Database.ExecuteSqlCommand(
                        "UPDATE [Programacion].[Funcion] set [EstadoID] =" + ObjEncabezado.EstadoID + " where [EncabezadoProgramacionID] = " + RowID_EncabezadoProgramacion);
                }
  

                //List<Funcion> listaFuncion = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                //listaFuncion.ForEach(a => a.EstadoID = ObjEncabezado.EstadoID);
                //foreach (Funcion item in listaFuncion)
                //{
                //    item.EstadoID = ObjEncabezado.EstadoID;
                //}
                try
                {
                    db.SaveChanges();
                    respuesta = "Actualizado Correctamente";  


                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            return Json(new { respuesta = respuesta, RowID_EncabezadoProgramacion = RowID });
        }


        public EncabezadoProgramacion CargarDatosEncabezadoProgramacion(EncabezadoProgramacion ObjEncabezado, FormCollection formulario)
        {
            IFormatProvider culture = new CultureInfo("es-ES", true);
            ObjEncabezado.Titulo = formulario["Titulo"].ToUpper();
            ObjEncabezado.FechaInicial = ModelosPropios.Util.FechaInsertar(formulario["FechaInicial"]);
            ObjEncabezado.FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["FechaFinal"]);
            ObjEncabezado.EstadoID = Convert.ToInt32(formulario["Estado"]);
            ObjEncabezado.TeatroID = Convert.ToInt32(formulario["Teatro"]);
            if (ObjEncabezado.RowID == 0)
            {
                ObjEncabezado.CreadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaCreacion = DateTime.Now;
            }
            else
            {
                ObjEncabezado.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaModificacion = DateTime.Now;
            }
            return ObjEncabezado;
        }

        [CheckSessionOutAttribute]
        public JsonResult Guardar_Funciones(FormCollection formulario, int RowID_EncabezadoProgramacion)
        {
            String respuesta1 = "", respuesta2 = "";
            Funcion ObjFuncion = new Funcion();
            formulario = DeSerialize(formulario);
            var HorasFunciones = formulario["HorasFunciones"].Split(',');
            int contadorFunciones = 0;

            //DateTime fechaInicialVigencia = DateTime.Parse(formulario["FechaInicial"]);
            //DateTime fechaFinalVigencia = DateTime.Parse(formulario["FechaFinal"]);
            //DateTime fechaInicialFunciones = DateTime.Parse(formulario["FechaInicialFunciones"]);
            //DateTime fechaFinalFunciones = DateTime.Parse(formulario["FechaFinalFunciones"]);

            //if (fechaInicialFunciones < fechaInicialVigencia || fechaInicialFunciones > fechaFinalVigencia)
            //{
            //    respuesta = "Las fechas seleccionadas estan fuera de la vigencia.";
            //}
            //else if (fechaFinalFunciones < fechaInicialVigencia || fechaFinalFunciones > fechaFinalVigencia)
            //{
            //    respuesta = "Las fechas seleccionadas estan fuera de la vigencia.";
            //}
            //else if (fechaInicialFunciones < fechaInicialVigencia && fechaFinalFunciones > fechaFinalVigencia)
            //{
            //    respuesta = "Las fechas seleccionadas estan fuera de la vigencia.";
            //}


            if (RowID_EncabezadoProgramacion != 0)
            {
                DateTime FechaProgramacion = ModelosPropios.Util.FechaInsertar(formulario["FechaInicialFunciones"].ToString());
                List<Funcion> FuncionesExistentes = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                //Recorro Uno a uno los dias del rango que selecciono
                TimeSpan diferencia = ModelosPropios.Util.FechaInsertar(formulario["FechaFinalFunciones"].ToString()) - ModelosPropios.Util.FechaInsertar(formulario["FechaInicialFunciones"].ToString());
                DateTime FechaFinal = ModelosPropios.Util.FechaInsertar(formulario["FechaFinalFunciones"].ToString());
                TimeSpan HoraInicial = TimeSpan.MinValue;
                TimeSpan HoraFinal;
                for (int i = 0; i <= diferencia.Days; i++)
                {
                    //Recorro una a una las funciones que ingresaron
                    foreach (String horaFuncion in HorasFunciones)
                    {
                        try
                        {
                            HoraInicial = TimeSpan.Parse(horaFuncion); //Inicial
                            HoraFinal = HoraInicial.Add(TimeSpan.FromMinutes(Convert.ToDouble(formulario["TTotal"]))); //Inicial mas Total
                            ObjFuncion = CargarDatosFuncion(ObjFuncion, formulario, RowID_EncabezadoProgramacion, HoraInicial, HoraFinal, FechaProgramacion);
                            String validacion = ValidarFuncion(ObjFuncion, FuncionesExistentes);
                            if (String.IsNullOrEmpty(validacion))
                            {
                                db.Funcion.Add(ObjFuncion);
                                db.SaveChanges();
                                if (i <= FechaFinal.Day)//Para adicionar la nueva que se creo
                                {
                                    FuncionesExistentes.Add(ObjFuncion);
                                    ObjFuncion = new Funcion();//Limpio el Objeto
                                    contadorFunciones++;
                                }
                            }
                            else
                            {
                                respuesta2 += validacion;
                                ObjFuncion = new Funcion();//Limpio el Objeto
                            }
                        }
                        catch (Exception ex)
                        { return Json(new { respuesta = "Error " + ex.Message }); }
                    }
                    FechaProgramacion = FechaProgramacion.AddDays(1);
                }

            }
            if (!String.IsNullOrEmpty(respuesta2))
            {
                respuesta2 = "No se puede crear " + respuesta2 + " Porque entra en conflicto con otra función.";
            }
            respuesta1 = contadorFunciones + "";
            return Json(new { respuesta1 = respuesta1, respuesta2 = respuesta2 });
        }

        public Funcion CargarDatosFuncion(Funcion ObjFuncion, FormCollection formulario, int RowID_EncabezadoProgramacion, TimeSpan horaInicial, TimeSpan HoraFinal, DateTime FechaProgramacion)
        {
            ObjFuncion.EncabezadoProgramacionID = RowID_EncabezadoProgramacion;
            ObjFuncion.SalaID = Int32.Parse(formulario["Sala"]);
            ObjFuncion.Fecha = FechaProgramacion;
            ObjFuncion.TiempoCorto = Int16.Parse(formulario["Tadicional"]);
            ObjFuncion.TiempoAseo = Int16.Parse(formulario["TadicionalAseo"]);
            ObjFuncion.HoraInicial = horaInicial;
            ObjFuncion.HoraFinal = HoraFinal;
            ObjFuncion.DetallePeliculaID = Int32.Parse(formulario["DetallePelicula"]);
            ObjFuncion.Estado = db.Estado.FirstOrDefault(f => f.Nombre == "Abierta" && f.TipoEstado.Codigo == "TIPOFUNCION");
            ObjFuncion.Sincronizado = false;
            if (ObjFuncion.RowID == 0)
            {
                ObjFuncion.CreadoPor = Session["usuario_creacion"].ToString();
                ObjFuncion.FechaCreacion = DateTime.Now;
            }
            return ObjFuncion;
        }

        [CheckSessionOutAttribute]
        public JsonResult CargarSalas(Int32 IdTeatro)
        {
            var query = (from salas in db.Sala
                         where salas.TeatroID == IdTeatro && salas.Estado.Codigo == "ENFUNCIONAMIENTO"
                         select new
                         {
                             rowid = salas.RowID,
                             Nombre = salas.Nombre,
                         }
            ).ToList();
            return Json(query);
        }

        [CheckSessionOutAttribute]
        public JsonResult CargarPeliculas(Int32 IdSala)
        {
            Opcion Formato2D = db.Opcion.FirstOrDefault(f => f.Codigo == "2D");
            int iDFormato2D = Formato2D.RowID;
            int iDFormato3D = Formato2D.RowID;
            Sala Sala = db.Sala.FirstOrDefault(f => f.RowID == IdSala);
            foreach (FormatoSala item in Sala.FormatoSala) // Para Saber si tiene formato 2D y 3D
            {
                if (item.Opcion == null)
                {
                    Opcion Formato = db.Opcion.FirstOrDefault(f => f.RowID == item.TipoFormatoID);
                    if (Formato.Codigo == "3D")
                    {
                        iDFormato3D = Formato.RowID;
                    }
                }
                else if (item.Opcion.Codigo == "3D")
                {
                    iDFormato3D = item.Opcion.RowID;
                }
            }
            List<TeatroPelicula> peliculas = db.TeatroPelicula.Where(f => f.TeatroID == Sala.TeatroID).ToList();
            List<DetallePelicula> detallePelicula2 = new List<DetallePelicula>();
            foreach (TeatroPelicula item in peliculas)
            {

                foreach (DetallePelicula item2 in item.EncabezadoPelicula.DetallePelicula)
                {
                    if (item2.TipoFormatoID == iDFormato2D || item2.TipoFormatoID == iDFormato3D)
                    { //Si no tiene 3D, busco por 2D y 2D
                        detallePelicula2.Add(item2);
                    }
                }
            }
            ///Para formar el Json
            var query = (from detallePelicula in detallePelicula2
                         select new
                         {
                             RowId = detallePelicula.RowID,
                             Nombre = detallePelicula.EncabezadoPelicula.TituloLocal,
                             Formato = detallePelicula.Opcion.Codigo,
                             Version = detallePelicula.Opcion1.Codigo
                         }
            ).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }


        [CheckSessionOutAttribute]
        public string CargarFunciones(int RowID_Encabezado)
        {
            string tabla = "";
            List<Funcion> ListaFunciones = db.Funcion.Where(ld => ld.EncabezadoProgramacionID == RowID_Encabezado).ToList();
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("Es-Es");
            tabla = tabla + " <thead>";
tabla = tabla + "                        <tr>";
tabla = tabla + "                            <th></th>";
tabla = tabla + "                            <th style=\"width:10px;display:none;\"></th>";
tabla = tabla + "                            <th>Sala</th>";
tabla = tabla + "                            <th>Servicio</th>";
tabla = tabla + "                            <th>Película</th>";
tabla = tabla + "                            <th>Formato</th>";
tabla = tabla + "                            <th>Fecha</th>";
tabla = tabla + "                            <th>Hora Inicial - Final</th>";
tabla = tabla + "                            <th>T. Total</th>";
tabla = tabla + "                            <th>Precio</th>";
tabla = tabla + "                            <th>Estado</th>";
tabla = tabla + "                        </tr>";
tabla = tabla + "                    </thead>";
tabla = tabla + "                    <tfoot>";
tabla = tabla + "                        <tr>";
tabla = tabla + "                            <th></th>";
tabla = tabla + "                            <th style=\"width:10px;display:none;\"><input type=\"text\" id=\"ID\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Sala\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Servicio\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Película\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Formato\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Fecha\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Hora Inicial - Final\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar T. Total\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Precio\" /></th>";
tabla = tabla + "                            <th style=\"padding: 5px 3px 0px 3px; font-size: small;\"><input type=\"text\" style=\"width: 100%;\" placeholder=\"Buscar Estado\" /></th>";
tabla = tabla + "                        </tr>";
tabla = tabla + "                    </tfoot>";
tabla = tabla + "                    <tbody>";
            foreach (Funcion funcion in ListaFunciones)
            {
                tabla = tabla + "<tr>";
                tabla = tabla + "<td>" +
                        "<div class=\"dropdown\">" +
                               "<a class=\"dropdown-toggle rowlink\" data-toggle=\"dropdown\" href=\"#\" title=\"Opciones función #" + funcion.RowID + "\">" +
                                    "<i class=\"glyphicon glyphicon-th-list\"></i>" +
                                "</a>" +
                                "<ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"dLabel\">" +
                                    "<li>" +
                                        "<a href=\"javascript:EliminarFuncion(" + funcion.RowID + ")\" style=\"color:black;\" data-toggle=\"tooltip\" data-placement=\"bottom\" title=\"Eliminar función No." + funcion.RowID + "\">" +
                                            "<i class=\"ion-trash-a\"></i> Eliminar" +
                                        "</a>" +
                                    "</li>" +
                                    "<li>" +
                                        "<a href=\"javascript:Duplicar_Funcion(" + funcion.RowID + ")\" style=\"color:black;\" data-toggle=\"tooltip\" data-placement=\"bottom\" title=\"Duplicar función No." + funcion.RowID + "\">" +
                                            "<i class=\"ion-edit\"></i> Duplicar" +
                                        "</a>" +
                                    "</li>" +
                                "</ul>" +
                            "</div>" +
                        "</td>";
                tabla += "<td  style=\"width:10px;display:none;\">" +  funcion.RowID+ "</td>";
                tabla += "<td style=\"width: 9%;\">" + funcion.Sala.Nombre+ "</td>";
                tabla += "<td style=\"width: 7%;\">" + funcion.Sala.ServicioSala.First().Opcion.Codigo+ "</td>";
                if (funcion.DetallePelicula.EncabezadoPelicula.TituloLocal.Length >= 15)
                {
                    tabla += "<td title=\""+funcion.DetallePelicula.EncabezadoPelicula.TituloLocal+"\">" + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal.Substring(0, 15) + "...</td>";
                }
                else
                {
                    tabla += "<td>" + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + "</td>";
                }
                tabla += "<td>" + funcion.DetallePelicula.Opcion.Codigo + " " + funcion.DetallePelicula.Opcion1.Codigo2 + "</td>";
                if (funcion.Fecha != null)
                {
                    tabla += "<td>" + ci.DateTimeFormat.GetDayName(funcion.Fecha.Value.DayOfWeek) + " - " + funcion.Fecha.Value.ToString("dd/MM/yyyy") + "</td>";
                }
                else
                {
                    tabla += "<td>" + funcion.Fecha + "</td>";
                }
                tabla += "<td>";
                if (funcion.HoraInicial != null)
                {
                    tabla += funcion.HoraInicial.Value;
                }
                else
                {
                    tabla += funcion.HoraInicial;
                }
                tabla += " - ";
                if (funcion.HoraFinal != null)
                {
                    tabla += funcion.HoraFinal.Value;
                }
                else
                {
                    tabla += @funcion.HoraFinal;
                }
                tabla += "</td>";
                tabla += "<td  style=\"width: 8%\">" + (Convert.ToInt32(funcion.TiempoCorto) + funcion.DetallePelicula.EncabezadoPelicula.Duracion + Convert.ToInt32(funcion.TiempoAseo)) + "</td>";

                tabla += "<td style=\"padding: 2px;\">";
                tabla += "<table  style=\" font-size: smaller; \">";
                foreach (CinemaPOS.Models.ListaPrecioFuncion precio in funcion.ListaPrecioFuncion)
                {
                    if (precio.ListaDetalle.TipoListaDetalle != null)
                    {
                        tabla += "<tr style=\"padding: 2px;\">";
                        tabla += "<td>" + precio.ListaDetalle.Precio + " - " + funcion.Sala.ServicioSala.First().Opcion.Codigo + "  " + @precio.ListaDetalle.TipoListaDetalle.Value + "</td>";
                        tabla += "</tr>";
                    }
                    else
                    {
                        tabla += "<tr style=\"padding: 2px;\">";
                        tabla += "<td>" + precio.ListaDetalle.Precio + " - " + funcion.Sala.ServicioSala.First().Opcion.Codigo + "</td>";
                        tabla += "</tr>";
                    }
                }
                if (funcion.ListaPrecioFuncion.Count == 0)
                {

                    tabla += "<tr style=\"padding: 2px;\">";
                    tabla += "<td>Por Asignar</td>";
                    tabla += "</tr>";
                }
                tabla += "</table>";
                tabla += "</td>";
                if (funcion.Estado != null)
                {
                    tabla += "<td>" + funcion.Estado.Descripcion + " </td>";
                }
                else
                {
                    tabla += "<td></td>";
                }
                tabla = tabla + "</tr>";
            }
            tabla = tabla + "                    </tbody>";
            return tabla;
        }


        [CheckSessionOutAttribute]
        public string CargarFuncionesGraficas(int RowID_Encabezado, int DiasACorrer)
        {
            string tabla = "";
            List<Funcion> ListaFunciones = db.Funcion.Where(ld => ld.EncabezadoProgramacionID == RowID_Encabezado).ToList();
            List<Sala> listaSalas = null;
            if (ListaFunciones.Count == 0)
            {
                return tabla;
            }
            EncabezadoProgramacion programacion = ListaFunciones.First().EncabezadoProgramacion;
            if (ListaFunciones.Count == 0)
            {

            }
            if (programacion == null)
            {
                programacion = db.EncabezadoProgramacion.FirstOrDefault(f => f.RowID == ListaFunciones.First().EncabezadoProgramacionID);
                listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO").OrderBy(f => f.RowID).ToList();
            }
            else
            {
                listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO").OrderBy(f => f.RowID).ToList();
            }
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("Es-Es");
            int cantDias = DiasACorrer + 7;

            tabla += "  <thead>";
            tabla += "<tr>";
            tabla += "<td class=\"day Bordes\" id=\"table-header-days\">";
            tabla += "<p>Sala/Día</p>";
            tabla += "</td>";
            for (int i = DiasACorrer; i < cantDias; i++)
            {
                if ((programacion.FechaInicial.Value.AddDays(i)) <= programacion.FechaFinal.Value)
                {
                   String fechaEncabezado = programacion.FechaInicial.Value.AddDays(i).ToString("dd/MM/yyyy");

                    tabla += "<td class=\"day Bordes\" id=\"table-header-days\">";
                    tabla += "<p>";
                    tabla += "<a class=\"ion-clipboard\"  href=\"javascript:ModalReplicarFuncionxDia('"+fechaEncabezado+"')\" title=\"Replicar Funciones por Día\"></a>";
                    tabla += ci.DateTimeFormat.GetDayName(programacion.FechaInicial.Value.AddDays(i).DayOfWeek).ToUpper() + "  " + programacion.FechaInicial.Value.AddDays(i).ToString("dd/MM/yyyy") + "</p>";
                    tabla += "</td>";
                }
            }
            tabla += "      </tr>";
            tabla += "   </thead>";
            tabla += "<tbody>";

            foreach (CinemaPOS.Models.Sala itemSala in listaSalas)
            {
                tabla += "<tr>";
                tabla += "<td class=\"Sala Bordes\" id=\"SalaTitle\">";
                tabla += "<p>";
                tabla += "<a class=\"ion-clipboard\" href=\"javascript:ModalReplicarFuncionxSala(" + itemSala.RowID + ")\" title=\"Replicar Funciones por Sala\"></a>";
                tabla += itemSala.Nombre + "</p>";
                tabla += "</td>";
                for (int i = DiasACorrer; i < cantDias; i++)
                {
                    tabla += "<td class=\"Bordes\">";
                    foreach (CinemaPOS.Models.Funcion funcion in ListaFunciones)
                    {
                        if (funcion.SalaID == itemSala.RowID)
                        {
                            if (programacion.FechaInicial.Value.AddDays(i) <= programacion.FechaFinal.Value)//Para que no se pase
                            {
                                if (programacion.FechaInicial.Value.AddDays(i) == funcion.Fecha.Value)
                                {
                                    tabla += "<p class=\"Funcion\" onclick=\"javascript:ModalEditarFun(" + funcion.RowID + ")\" style=\"cursor:pointer \">";
                                    tabla += funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + " ";
                                    tabla += funcion.DetallePelicula.Opcion.Codigo + " ";
                                    tabla += funcion.DetallePelicula.Opcion1.Codigo2;
                                    tabla += "<br /> ";
                                    if (funcion.HoraInicial.Value != null)
                                    {
                                        tabla += funcion.HoraInicial.Value.ToString().Substring(0, 5) + "  ";
                                    } 

                                    if (funcion.HoraFinal.Value != null)
                                    {
                                        tabla += funcion.HoraFinal.Value.ToString().Substring(0, 5);
                                    }

                                    tabla += "<br />";
                                    if (funcion.ListaPrecioFuncion.Count > 0)
                                    {
                                        tabla += "<i class=\"ion-cash icon-identificador\" style=\"font-size: x-large;\">&nbsp;</i>";
                                    }
                                    if (funcion.BoletaVendida.Count > 0)
                                    {
                                        tabla += "<i class=\"ion-ios-film-outline icon-identificador\" style=\"font-size: x-large;\">&nbsp;</i>";
                                    }
                                    if (funcion.Estado != null)
                                    {
                                        if (funcion.Estado.Nombre == "Abierta")
                                        {
                                            tabla += "<i class=\"ion-unlocked \" style=\"font-size: large;\"></i>";
                                        }
                                        else if (funcion.Estado.Nombre == "Cerrada")
                                        {
                                            tabla += "<i class=\"ion-locked\" style=\"font-size: large;\"></i>";
                                        }
                                        else if (funcion.Estado.Nombre == "Planeada")
                                        {
                                            tabla += "<i class=\"ion-calendar\" style=\"font-size: large;\"></i>";
                                        }
                                    }
                                    else
                                    {
                                        tabla += "<i class=\"ion-alert-circled\" style=\"font-size: large;\"></i>";
                                    }
                                    tabla += "</p>";

                                }
                            }
                        }
                    }
                    tabla += "</td>";
                }
                tabla += "</tr>";
            }
            tabla += "</tbody>";
            return tabla;
        }


        [CheckSessionOutAttribute]
        public JsonResult CargarDuracion(Int32 IdPeliculaDetalle)
        {
            DetallePelicula detalle = db.DetallePelicula.Where(f => f.RowID == IdPeliculaDetalle).FirstOrDefault();
            var duracion = "";
            try
            {
                duracion = detalle.EncabezadoPelicula.Duracion.ToString();
            }
            catch { }
            return Json(new { duracion = duracion });
        }

        [CheckSessionOutAttribute]
        public JsonResult AsignarListaPrecios(Int32 RowID_ListaPrecios, Int32 RowID_EncabezadoProgramacion)
        {
            string respuesta = "";
            List<Funcion> Funciones = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
            List<ListaDetalle> Tarifas = db.ListaDetalle.Where(f => f.ListaEncabezadoID == RowID_ListaPrecios).ToList();
            if (Tarifas.Count >= 1)
            {
                foreach (Funcion funcion in Funciones)
                {
                    foreach (ListaDetalle tarifa in Tarifas)
                    {
                        if (tarifa.ListaEncabezado.TeatroID == funcion.Sala.TeatroID)// Mismo teatro
                        {
                            if (tarifa.TipoFormatoID == funcion.DetallePelicula.TipoFormatoID) //Mismo Formato
                            {
                                if (funcion.HoraInicial.Value >= tarifa.HoraInicial.Value && funcion.HoraInicial.Value <= tarifa.HoraFinal.Value)
                                { //Esta en el rango
                                    if (funcion.Fecha >= tarifa.FechaInicial && funcion.Fecha <= tarifa.FechaFinal)
                                    { //Esta en el rango de fechas
                                        if (ValidarMismoDia(funcion, tarifa))
                                        {
                                            foreach (ServicioSala servicioSala in funcion.Sala.ServicioSala)
                                            {
                                                if (servicioSala.TipoServicioID == tarifa.TipoServicioID) //Mismo servicio
                                                {
                                                    ListaPrecioFuncion PrecioExiste = db.ListaPrecioFuncion.FirstOrDefault(f => f.FuncionID == funcion.RowID && f.ListaDetalleID == tarifa.RowID);
                                                    if (PrecioExiste == null) //Validacion para que no se le asigne el mismo precio 2 veces
                                                    {
                                                        ListaPrecioFuncion Precio = new ListaPrecioFuncion();
                                                        Precio.FuncionID = funcion.RowID;
                                                        Precio.ListaDetalleID = tarifa.RowID;
                                                        Precio.CreadoPor = Session["usuario_creacion"].ToString();
                                                        Precio.FechaCreacion = DateTime.Now;
                                                        db.ListaPrecioFuncion.Add(Precio);
                                                        db.SaveChanges();
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }

                }
            }
            else
            {
                respuesta = "Error: Lista de precios no contiene detalles.";
            }

            return Json(respuesta);
        }


        [CheckSessionOutAttribute]
        public JsonResult AsignarListaPrecioFuncion(Int32 RowID_ListaPrecios, Int32 RowID_Funcion)
        {
            string respuesta = "", tipoRespuesta = "success";
            Funcion Funcion = db.Funcion.FirstOrDefault(f => f.RowID == RowID_Funcion);
            ListaDetalle Tarifa = db.ListaDetalle.FirstOrDefault(f => f.RowID == RowID_ListaPrecios);

            ListaPrecioFuncion PrecioExiste = db.ListaPrecioFuncion.FirstOrDefault(f => f.FuncionID == Funcion.RowID && f.ListaDetalleID == Tarifa.RowID);
            if (PrecioExiste == null) //Validacion para que no se le asigne el mismo precio 2 veces
            {
                ListaPrecioFuncion Precio = new ListaPrecioFuncion();
                Precio.FuncionID = Funcion.RowID;
                Precio.ListaDetalleID = Tarifa.RowID;
                Precio.CreadoPor = Session["usuario_creacion"].ToString();
                Precio.FechaCreacion = DateTime.Now;
                db.ListaPrecioFuncion.Add(Precio);
                db.SaveChanges();
                respuesta = "Asignado Correctamente";
            }
            else
            {
                respuesta = "Error: Esta tarifa ya esta asignada.";
                tipoRespuesta = "warning";
            }
            return Json(new { respuesta = respuesta, tipoRespuesta = tipoRespuesta });
        }

        public bool ValidarMismoDia(Funcion funcion, ListaDetalle tarifa)
        {
            bool respuesta = false;
            List<String> dias = tarifa.DiasAsignados.Split(',').ToList();


            CultureInfo ci = new CultureInfo("Es-Es");
            var diaFuncion = ci.DateTimeFormat.GetDayName(funcion.Fecha.Value.DayOfWeek);
            foreach (String diaTarifa in dias)
            {
                if (diaTarifa.ToUpper() == diaFuncion.ToUpper().Replace("É", "E").Replace("Á", "A"))
                {
                    respuesta = true;
                    return respuesta;
                }
            }
            return respuesta;
        }

        [CheckSessionOutAttribute]
        public JsonResult EliminarFuncion(int RowID_Funcion)
        {
            String tipoMensaje = "", Respuesta = "";
            if (db.Funcion.FirstOrDefault(f => f.RowID == RowID_Funcion) != null) //Valido si existe la funcion
            {
                //Valido si esta funcion tiene boletas Vendidas
                List<BoletaVendida> BoletasVendidas = db.BoletaVendida.Where(f => f.FuncionID == RowID_Funcion).ToList();
                if (BoletasVendidas.Count > 0)
                {
                    return Json(new { tipoMensaje = "warning", Respuesta = "No puede eliminar esta Función, tiene una boleta Vendida." });
                }

                List<ListaPrecioFuncion> PreciosFuncion = db.ListaPrecioFuncion.Where(f => f.FuncionID == RowID_Funcion).ToList();

                foreach (ListaPrecioFuncion item in PreciosFuncion)
                {
                    db.ListaPrecioFuncion.Remove(item);
                    db.SaveChanges();
                }

                db.Funcion.Remove(db.Funcion.Where(lt => lt.RowID == RowID_Funcion).First());
                db.SaveChanges();
                Respuesta = "Función eliminada correctamente";
                tipoMensaje = "success";
            }
            return Json(new { tipoMensaje = tipoMensaje, Respuesta = Respuesta});
        }

        public String ValidarFuncion(Funcion funcion, List<Funcion> FuncionesExistentes)
        {
            String respuesta = "";
            foreach (Funcion item in FuncionesExistentes)
            {
                if (item.SalaID == funcion.SalaID)//Misma Sala
                {
                    if (item.Fecha.Value == funcion.Fecha.Value)// Mismo dia
                    {
                        if (item.DetallePelicula == null)
                        {
                            item.DetallePelicula = db.DetallePelicula.FirstOrDefault(f => f.RowID == item.DetallePeliculaID);
                        }
                        if (funcion.DetallePelicula == null)
                        {
                            funcion.DetallePelicula = db.DetallePelicula.FirstOrDefault(f => f.RowID == funcion.DetallePeliculaID);
                        }

                        if (item.RowID != funcion.RowID) //Que no sea la misma funcion
                        {
                            //Menor a 0 La hora esta antes
                            //0 es la misma
                            //Mayor a 0 la hora esta despues
                            if (funcion.HoraInicial.Value >= item.HoraInicial.Value && funcion.HoraInicial.Value <= item.HoraFinal.Value)
                            {
                                respuesta += " * " + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + " " + funcion.DetallePelicula.Opcion.Nombre + " el día " + funcion.Fecha.Value.ToString("dd/MM/yyyy") + " " + funcion.HoraInicial + "\n";
                                return respuesta;
                            }
                            else if (funcion.HoraFinal.Value >= item.HoraInicial.Value && funcion.HoraFinal.Value <= item.HoraFinal.Value)
                            {
                                respuesta += " * " + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + " " + funcion.DetallePelicula.Opcion.Nombre + " el día " + funcion.Fecha.Value.ToString("dd/MM/yyyy") + " " + funcion.HoraInicial + "\n\n";
                                return respuesta;
                            }
                            else if (funcion.HoraInicial.Value >= item.HoraInicial.Value && funcion.HoraFinal.Value <= item.HoraFinal.Value)
                            {
                                respuesta += " * " + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + " " + funcion.DetallePelicula.Opcion.Nombre + " el día " + funcion.Fecha.Value.ToString("dd/MM/yyyy") + " " + funcion.HoraInicial + "\n";
                                return respuesta;
                            }

                        }
                    }
                }

            }


            return respuesta;
        }

        public String ValidarRango()
        {
            //Si las funciones no coinciden 
            return "";
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

#region modal editarFuncion

        [CheckSessionOutAttribute]
        public ActionResult ModalEditarFuncion(int? rowid)
        {
            Funcion funcion = db.Funcion.FirstOrDefault(f => f.RowID == rowid);
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOFUNCION");
            //Y el tipo de formato(2D 3D) es el mismo
            ViewBag.ListasPreciosAsignados = db.ListaPrecioFuncion.Where(f => f.FuncionID == rowid);


            //Listas de precio donde la fecha de la funcion esta en el rango de fechas Y la hora de la funcion esta en el rango de horas, y donde el servicio de la sala es el mismo
            List<ListaDetalle> tarifasDisponibles = db.ListaDetalle.Where(f => (f.ListaEncabezado.TeatroID == funcion.Sala.TeatroID) && ((f.FechaInicial.Value <= funcion.Fecha.Value) &&
             (f.FechaFinal.Value >= funcion.Fecha.Value)) && (f.HoraInicial.Value <= funcion.HoraInicial.Value) && (f.HoraFinal.Value >= funcion.HoraInicial.Value)).ToList();

            List<ListaDetalle> tarifasDisponiblesParaEnviar = new List<ListaDetalle>();

            foreach (ListaDetalle item in tarifasDisponibles)
            {
                if (funcion.DetallePelicula == null)
                {
                    funcion.DetallePelicula = db.DetallePelicula.FirstOrDefault(f => f.RowID == funcion.DetallePeliculaID);
                }
                if ((funcion.DetallePelicula.TipoFormatoID == item.TipoFormatoID)) // Mismo formato (2D, 3D)
                {
                    foreach (ServicioSala item2 in funcion.Sala.ServicioSala)
                    {
                        if (item2.TipoServicioID == item.TipoServicioID)//Si tiene el mismo servicio
                        {
                            String[] Dias = item.DiasAsignados.Split(',');
                            CultureInfo ci = new CultureInfo("Es-Es");
                            var diaFuncion = ci.DateTimeFormat.GetDayName(funcion.Fecha.Value.DayOfWeek);
                            var algo = diaFuncion.ToUpper().Replace("É", "E").Replace("Á", "A");
                            foreach (var diaTarifa in Dias)
                            {
                                if (diaTarifa.ToUpper() == diaFuncion.ToUpper().Replace("É", "E").Replace("Á", "A"))
                                {
                                    tarifasDisponiblesParaEnviar.Add(item);
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            ViewBag.ListasPreciosDisponibles = tarifasDisponiblesParaEnviar;


            Funcion programacion = db.Funcion.FirstOrDefault(t => t.RowID == rowid);
            if (programacion != null)
            {
                ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.Sala.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO");
                ViewBag.ListaPeliculas = db.TeatroPelicula.Where(f => f.TeatroID == programacion.Sala.TeatroID);
            }

            return View(funcion);
        }

        [CheckSessionOutAttribute]
        public JsonResult EditarFuncion(int? rowIdFuncion, FormCollection formulario)
        {
            String respuesta = "", tipoRespuesta = "success";
            formulario = DeSerialize(formulario);
            Funcion funcion = db.Funcion.FirstOrDefault(f => f.RowID == rowIdFuncion);
            try
            {
                List<Funcion> FuncionesExistentes = db.Funcion.Where(f => f.EncabezadoProgramacionID == funcion.EncabezadoProgramacionID).ToList();
                funcion.EstadoID = Convert.ToInt32(formulario["Estado"].ToString());
                funcion.SalaID = Int32.Parse(formulario["Sala"]);
                funcion.Fecha = Util.FechaInsertar(formulario["FechaFuncion"]);
                funcion.DetallePeliculaID = Int32.Parse(formulario["DetallePelicula"]);
                funcion.Sincronizado = false;
                String validacion = ValidarFuncion(funcion, FuncionesExistentes);
                if (String.IsNullOrEmpty(validacion))
                {
                    funcion.FechaModificacion = DateTime.Now;
                    funcion.ModificadoPor = Session["usuario_creacion"].ToString();
                    db.SaveChanges();
                    respuesta = "Actualizado Correctamente";
                }
                else
                {
                    return Json(new { respuesta = "NO se puede editar, Entra en conflicto con otra Función", tipoRespuesta = "warning" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = "Error " + ex.Message, tipoRespuesta = "danger" });
            }
            return Json(new { respuesta = respuesta, tipoRespuesta = tipoRespuesta });
        }




        #region ModalEliminarFuncion Masivo

        [CheckSessionOutAttribute]
        public ActionResult ModalEliminarFuncionMasivo(int rowid)
        {
            EncabezadoProgramacion programacion = db.EncabezadoProgramacion.FirstOrDefault(f => f.RowID == rowid);
            if (programacion == null)
            {
                return View(new EncabezadoProgramacion { });
            }
            else
            {
                ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO");
                ViewBag.ListaPeliculas = db.TeatroPelicula.Where(f => f.TeatroID == programacion.TeatroID);
                return View(programacion);
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult EliminarTodasFunciones(int RowID_Programacion)
        {
            String tipoMensaje = "", Respuesta = "";
            int contFunciones = 0;
            List<Funcion> ListaFunciones = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion).ToList();
            foreach (var funcion in ListaFunciones)
            {
                //Valido si esta funcion tiene boletas Vendidas
                List<BoletaVendida> BoletasVendidas = db.BoletaVendida.Where(f => f.FuncionID == funcion.RowID).ToList();
                if (BoletasVendidas.Count > 0)
                {
                    Respuesta += " Funcion "+funcion.Fecha+" "+funcion.HoraInicial+" \n\n";
                }
                else
                {
                    List<ListaPrecioFuncion> PreciosFuncion = db.ListaPrecioFuncion.Where(f => f.FuncionID == funcion.RowID).ToList();
                    foreach (ListaPrecioFuncion item in PreciosFuncion)
                    {
                        db.ListaPrecioFuncion.Remove(item);
                        db.SaveChanges();
                    }
                    db.Funcion.Remove(db.Funcion.Where(lt => lt.RowID == funcion.RowID).First());
                    db.SaveChanges();
                    contFunciones++;
                }
            }
            Respuesta = String.IsNullOrEmpty(Respuesta) ? "" : " Menos: "+Respuesta;
            Respuesta = contFunciones+ " Funciones eliminadas correctamente "+ Respuesta;
            tipoMensaje = "success";
            return Json(new { tipoMensaje = tipoMensaje, Respuesta = Respuesta });
        }

        [CheckSessionOutAttribute]
        public JsonResult EliminarFuncionesMasivo(int RowID_Programacion, FormCollection formulario)
        {
            formulario = DeSerialize(formulario);

            String tipoMensaje = "", Respuesta = "";

            int contFunciones = 0;
            List<Funcion> ListaFunciones = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion).ToList();

            ListaFunciones = formulario["DetallePelicula"] == null ? ListaFunciones : ListaFunciones.Where(f => f.DetallePeliculaID == Convert.ToInt32(formulario["DetallePelicula"].ToString())).ToList();
            ListaFunciones = formulario["Sala"] == null ? ListaFunciones : ListaFunciones.Where(f => f.SalaID == Convert.ToInt32(formulario["Sala"].ToString())).ToList();
            ListaFunciones = String.IsNullOrEmpty(formulario["fechaEliminar"].ToString()) ? ListaFunciones: ListaFunciones.Where(f =>  f.Fecha == ModelosPropios.Util.FechaInsertar(formulario["fechaEliminar"].ToString())).ToList();

            //TimeSpan Horainicial = formulario["hora_inicial"] != null ? TimeSpan.Parse(formulario["hora_inicial"].ToString()) : new TimeSpan();
            //TimeSpan HoraFinal = formulario["hora_final"] != null ? TimeSpan.Parse(formulario["hora_final"].ToString()) : new TimeSpan();

            //if (Horainicial != TimeSpan.Zero && HoraFinal != TimeSpan.Zero)
            //{
                
            //}
            //else if (Horainicial != TimeSpan.Zero)
            //{
                
            //}
            //else if (HoraFinal != TimeSpan.Zero)
            //{

            //}

            foreach (var funcion in ListaFunciones)
            {
                //Valido si esta funcion tiene boletas Vendidas
                List<BoletaVendida> BoletasVendidas = db.BoletaVendida.Where(f => f.FuncionID == funcion.RowID).ToList();
                if (BoletasVendidas.Count > 0)
                {
                    Respuesta += " Funcion " + funcion.Fecha + " " + funcion.HoraInicial + " \n\n";
                }
                else
                {
                    List<ListaPrecioFuncion> PreciosFuncion = db.ListaPrecioFuncion.Where(f => f.FuncionID == funcion.RowID).ToList();
                    foreach (ListaPrecioFuncion item in PreciosFuncion)
                    {
                        db.ListaPrecioFuncion.Remove(item);
                        db.SaveChanges();
                    }
                    db.Funcion.Remove(db.Funcion.Where(lt => lt.RowID == funcion.RowID).First());
                    db.SaveChanges();
                    contFunciones++;
                }
            }
            Respuesta = String.IsNullOrEmpty(Respuesta) ? "" : " Menos: " + Respuesta;
            Respuesta = contFunciones + " Funciones eliminadas correctamente " + Respuesta;
            tipoMensaje = "success";
            return Json(new { tipoMensaje = tipoMensaje, Respuesta = Respuesta });
        }
        

        #endregion


        #region ReplicarFuncionPorDia

        [CheckSessionOutAttribute]
        public ActionResult ModalReplicarFuncionxDia(int rowid, String FechaFuncion)
        {
            EncabezadoProgramacion programacion = db.EncabezadoProgramacion.FirstOrDefault(f => f.RowID == rowid);
            DateTime FechaFuncion1 = ModelosPropios.Util.FechaInsertar(FechaFuncion);
            if (programacion == null)
            {
                return View(new EncabezadoProgramacion { });
            }
            else
            {
                List<DateTime> FechasFunciones = new List<DateTime>();
                DateTime FechaReferencia = programacion.FechaInicial.Value;
                while (FechaReferencia <= programacion.FechaFinal)
                {
                    FechasFunciones.Add(FechaReferencia);
                    FechaReferencia = FechaReferencia.AddDays(1);
                }
                ViewBag.FechasFunciones = FechasFunciones;
                ViewBag.FechaReplicar = FechaFuncion1;
                return View(programacion);
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult ReplicarFuncionesxDia(int RowID_Programacion, DateTime FechaReplicar, FormCollection formulario)
        {
            String tipoMensaje = "", respuesta = "";
            formulario = DeSerialize(formulario);
            int contFunciones = 0;
            List<Funcion> ListaFuncionesAInsertar = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion && f.Fecha == FechaReplicar).ToList();
            //Recorro las fechas seleccionadas e intento insertar las funciones del dia que selecciono
            List<Funcion> FuncionesExistentes = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion).ToList();
            if (ListaFuncionesAInsertar.Count == 0)
            {
                return Json(new { tipoMensaje = "warning", Respuesta = "Este día no tiene funciones para replicar" });
            }
            foreach (var NuevaFecha in formulario)
            {
                DateTime FechaFuncion;
                try
                { FechaFuncion = Convert.ToDateTime(NuevaFecha.ToString()); }
                catch (Exception)
                { continue; }

                foreach (var funcion in ListaFuncionesAInsertar)
                {
                    try
                    {
                        Funcion ObjFuncion = new Funcion();
                        ObjFuncion.DetallePeliculaID = funcion.DetallePeliculaID;
                        ObjFuncion.EncabezadoProgramacionID = funcion.EncabezadoProgramacionID;
                        ObjFuncion.SalaID = funcion.SalaID;
                        ObjFuncion.Fecha = FechaFuncion;
                        ObjFuncion.HoraInicial = funcion.HoraInicial;
                        ObjFuncion.HoraFinal = funcion.HoraFinal;
                        ObjFuncion.TiempoAseo = funcion.TiempoAseo;
                        ObjFuncion.TiempoCorto = funcion.TiempoCorto;
                        ObjFuncion.Sincronizado = false;
                        ObjFuncion.CreadoPor = Session["usuario_creacion"].ToString();
                        ObjFuncion.FechaCreacion = DateTime.Now;
                        ObjFuncion.EstadoID = funcion.EstadoID;

                        String validacion = ValidarFuncion(ObjFuncion, FuncionesExistentes);
                        if (String.IsNullOrEmpty(validacion))
                        {
                            db.Funcion.Add(ObjFuncion);
                            db.SaveChanges();
                            FuncionesExistentes.Add(ObjFuncion);
                            ObjFuncion = new Funcion();//Limpio el Objeto
                            contFunciones++;
                        }
                        else
                        {
                            respuesta += validacion;
                            ObjFuncion = new Funcion();//Limpio el Objeto
                        }
                    }
                    catch (Exception ex)
                    { return Json(new { respuesta = "Error " + ex.Message }); }
                }
            }
            respuesta = contFunciones + " Funciones Replicadas correctamente " + respuesta;
            tipoMensaje = "success";
            return Json(new { tipoMensaje = tipoMensaje, Respuesta = respuesta });
        }

        

        #endregion

        #region Replicar Funcion x Sala
        [CheckSessionOutAttribute]
        public ActionResult ModalReplicarFuncionxSala(int rowid, int RowIdSala)
        {
            EncabezadoProgramacion programacion = db.EncabezadoProgramacion.FirstOrDefault(f => f.RowID == rowid);
            if (programacion == null)
            {
                return View(new EncabezadoProgramacion { });
            }
            else
            {
                ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID && f.Estado.Codigo == "ENFUNCIONAMIENTO").OrderBy(f => f.RowID);
                ViewBag.SalaReplicar = db.Sala.FirstOrDefault(f=>f.RowID == RowIdSala);
                return View(programacion);
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult ReplicarFuncionesxSala(int RowID_Programacion, int RowIdSala, FormCollection formulario)
        {
            String tipoMensaje = "", respuesta = "";
            formulario = DeSerialize(formulario);
            int contFunciones = 0;
            List<Funcion> ListaFuncionesAInsertar = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion && f.SalaID == RowIdSala).ToList();
            //Recorro las fechas seleccionadas e intento insertar las funciones del dia que selecciono
            List<Funcion> FuncionesExistentes = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_Programacion).ToList();
            if (ListaFuncionesAInsertar.Count == 0)
            {
                return Json(new { tipoMensaje = "warning", Respuesta = "Esta Sala no tiene funciones para replicar" });
            }
            foreach (var NuevaSala in formulario)
            {
                int RowIdNuevaSala = 0;
                try
                { RowIdNuevaSala = Convert.ToInt32(NuevaSala.ToString()); }
                catch (Exception)
                { continue; }

                foreach (var funcion in ListaFuncionesAInsertar)
                {
                    try
                    {
                        if (RowIdNuevaSala > 0)
                        {
                            Funcion ObjFuncion = new Funcion();
                            ObjFuncion.DetallePeliculaID = funcion.DetallePeliculaID;
                            ObjFuncion.EncabezadoProgramacionID = funcion.EncabezadoProgramacionID;
                            ObjFuncion.SalaID = RowIdNuevaSala;
                            ObjFuncion.Fecha = funcion.Fecha;
                            ObjFuncion.HoraInicial = funcion.HoraInicial;
                            ObjFuncion.HoraFinal = funcion.HoraFinal;
                            ObjFuncion.TiempoAseo = funcion.TiempoAseo;
                            ObjFuncion.TiempoCorto = funcion.TiempoCorto;
                            ObjFuncion.Sincronizado = false;
                            ObjFuncion.CreadoPor = Session["usuario_creacion"].ToString();
                            ObjFuncion.FechaCreacion = DateTime.Now;
                            ObjFuncion.EstadoID = funcion.EstadoID;

                            String validacion = ValidarFuncion(ObjFuncion, FuncionesExistentes);
                            if (String.IsNullOrEmpty(validacion))
                            {
                                db.Funcion.Add(ObjFuncion);
                                db.SaveChanges();
                                FuncionesExistentes.Add(ObjFuncion);
                                ObjFuncion = new Funcion();//Limpio el Objeto
                                contFunciones++;
                            }
                            else
                            {
                                respuesta += validacion;
                                ObjFuncion = new Funcion();//Limpio el Objeto
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    { return Json(new { respuesta = "Error " + ex.Message }); }
                }
            }
            respuesta = contFunciones + " Funciones Replicadas correctamente " + respuesta;
            tipoMensaje = "success";
            return Json(new { tipoMensaje = tipoMensaje, Respuesta = respuesta });
        }
        #endregion
    }



#endregion

   

}
