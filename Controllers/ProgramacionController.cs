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
            ViewBag.ListaTeatro = db.Teatro;
            ViewBag.ListasPrecios = db.ListaEncabezado.ToList();
            ViewBag.ListaEstados = db.Estado.Where(f => f.TipoEstado.Codigo== "TIPOPROGRAMACIONENCABEZADO");
            if (RowID_EncabezadoProgramacion != null && RowID_EncabezadoProgramacion != 0)
            {
                RowID_EncabezadoProgramacion = int.Parse(Request.Params["RowID_EncabezadoProgramacion"].ToString());
                ViewBag.listaFunciones = db.Funcion.Where(f => f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                EncabezadoProgramacion programacion = db.EncabezadoProgramacion.FirstOrDefault(t => t.RowID == RowID_EncabezadoProgramacion);
                if (programacion != null)
                {
                    ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID);
                    ViewBag.ListaPeliculas = db.TeatroPelicula.Where(f=> f.TeatroID == programacion.TeatroID);
                    ViewBag.ListasPrecios = db.ListaEncabezado.Where(f => f.TeatroID == programacion.TeatroID);
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
            var query = (from funcion in db.Funcion.Where(f=>f.EncabezadoProgramacionID==RowID_Encabezado)
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

            IFormatProvider culture = new CultureInfo("es-ES",true);
            DateTime NuevaFechaInicial = ModelosPropios.Util.HoraInsertar(formulario["FechaInicial"]);

            respuesta = formulario["FechaInicial"] + "   " + NuevaFechaInicial.ToString();


            DateTime NuevaFechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinal"]);
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
                    db.SaveChanges();
                    respuesta = respuesta+ "Guardado Correctamente";
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
            ObjEncabezado.FechaInicial = ModelosPropios.Util.HoraInsertar(formulario["FechaInicial"]);
            ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinal"]);
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
            String respuesta = "";
            Funcion ObjFuncion = new Funcion();
            formulario = DeSerialize(formulario);
            var HorasFunciones = formulario["HorasFunciones"].Split(',');


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
                DateTime FechaProgramacion = ModelosPropios.Util.HoraInsertar(formulario["FechaInicialFunciones"].ToString());
                List<Funcion> FuncionesExistentes = db.Funcion.Where(f=> f.EncabezadoProgramacionID == RowID_EncabezadoProgramacion).ToList();
                //Recorro Uno a uno los dias del rango que selecciono
                TimeSpan diferencia = ModelosPropios.Util.HoraInsertar(formulario["FechaFinalFunciones"].ToString()) - ModelosPropios.Util.HoraInsertar(formulario["FechaInicialFunciones"].ToString());
                DateTime FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinalFunciones"].ToString());
                TimeSpan HoraInicial = TimeSpan.MinValue;
                TimeSpan HoraFinal;
                for (int i =0; i <= diferencia.Days; i++)
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
                            if(String.IsNullOrEmpty(validacion)){
                                db.Funcion.Add(ObjFuncion);
                                db.SaveChanges();
                                if (i <= FechaFinal.Day)//Para adicionar la nueva que se creo
                                {
                                    FuncionesExistentes.Add(ObjFuncion);
                                    ObjFuncion = new Funcion();//Limpio el Objeto
                                }
                            }
                            else{
                                respuesta += validacion;
                                ObjFuncion = new Funcion();//Limpio el Objeto
                            }
                        }
                        catch (Exception ex)
                        { return Json(new { respuesta = "Error " + ex.Message }); }
                    }
                    FechaProgramacion = FechaProgramacion.AddDays(1);
                }

            }
            if (!String.IsNullOrEmpty(respuesta))
            {
                respuesta = "No se puede crear " + respuesta + " Porque entra en conflicto con otra función.";
            }

            return Json(respuesta);
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
                         where salas.TeatroID == IdTeatro
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
		        else if (item.Opcion.Codigo == "3D"){
                    iDFormato3D = item.Opcion.RowID;
                }
	        }
            List<TeatroPelicula> peliculas = db.TeatroPelicula.Where( f => f.TeatroID == Sala.TeatroID).ToList();
            List<DetallePelicula>  detallePelicula2 = new List<DetallePelicula>();
            foreach (TeatroPelicula item in peliculas)
	        {

                foreach (DetallePelicula item2 in item.EncabezadoPelicula.DetallePelicula)
                {
                    if (item2.TipoFormatoID == iDFormato2D || item2.TipoFormatoID == iDFormato3D) { //Si no tiene 3D, busco por 2D y 2D
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
            
            return Json(query,JsonRequestBehavior.AllowGet);
        }


        [CheckSessionOutAttribute]
        public string CargarFunciones(int RowID_Encabezado)
        {
            string tabla = "";
            List<Funcion> ListaFunciones = db.Funcion.Where(ld => ld.EncabezadoProgramacionID == RowID_Encabezado).ToList();
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("Es-Es");
            foreach (Funcion funcion in ListaFunciones)
            {
                tabla = tabla + "<tr>";
                tabla= tabla + "<td>"+
                        "<div class=\"dropdown\">"+
                               "<a class=\"dropdown-toggle rowlink\" data-toggle=\"dropdown\" href=\"#\" title=\"Opciones función #"+ funcion.RowID+"\">"+
                                    "<i class=\"glyphicon glyphicon-th-list\"></i>"+
                                "</a>"+
                                "<ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"dLabel\">"+
                                    "<li>"+
                                        "<a href=\"javascript:EliminarFuncion("+funcion.RowID+")\" style=\"color:black;\" data-toggle=\"tooltip\" data-placement=\"bottom\" title=\"Eliminar función No."+ funcion.RowID+"\">"+
                                            "<i class=\"ion-trash-a\"></i> Eliminar"+
                                        "</a>"+
                                    "</li>"+
                                    "<li>"+
                                        "<a href=\"javascript:Duplicar_Funcion(" + funcion.RowID + ")\" style=\"color:black;\" data-toggle=\"tooltip\" data-placement=\"bottom\" title=\"Duplicar función No." + funcion.RowID + "\">" +
                                            "<i class=\"ion-edit\"></i> Duplicar"+
                                        "</a>"+
                                    "</li>"+
                                "</ul>"+
                            "</div>"+
                        "</td>";

                tabla += "<td>" + funcion.RowID + "</td>";
                tabla += "<td>"+ funcion.Sala.Teatro.Nombre+" - "+funcion.Sala.Nombre+" "+ funcion.Sala.ServicioSala.First().Opcion.Codigo+"</td>";
                tabla += "<td>" + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal + "</td>";
                tabla += "<td>" + funcion.DetallePelicula.Opcion.Codigo + " " + funcion.DetallePelicula.Opcion1.Codigo2 + "</td>";
                if(funcion.Fecha != null ){
                    tabla += "<td>" + ci.DateTimeFormat.GetDayName(funcion.Fecha.Value.DayOfWeek)+" - "+funcion.Fecha.Value.ToString("dd/MM/yyyy") + "</td>";
                }
                else
                {
                    tabla += "<td>" + funcion.Fecha + "</td>";
                }
                tabla += "<td>";
                if (funcion.HoraInicial != null)
                {
                    tabla += funcion.HoraInicial.Value;
                }else{
                    tabla +=  funcion.HoraInicial;
                }
                tabla += " - ";
                if (funcion.HoraFinal != null)
                {
                    tabla += funcion.HoraFinal.Value;
                }
                else
                {
                    tabla +=  @funcion.HoraFinal ;
                }
                tabla +=  "</td>";
                tabla += "<td>" + (Convert.ToInt32(funcion.TiempoCorto) + funcion.DetallePelicula.EncabezadoPelicula.Duracion + Convert.ToInt32(funcion.TiempoAseo)) + "</td>";

                tabla +=   "<td>";
                tabla +=                         "<table>";
                                        foreach (CinemaPOS.Models.ListaPrecioFuncion precio in funcion.ListaPrecioFuncion)
                                        {
                                            if (precio.ListaDetalle.TipoListaDetalle != null)
                                            {
                tabla +=                                 "<tr>";
                tabla +=                                     "<td>"+precio.ListaDetalle.Precio+" - "+funcion.Sala.ServicioSala.First().Opcion.Codigo+"  "+@precio.ListaDetalle.TipoListaDetalle.Value+"</td>";
                tabla +=                                "</tr>";
                                            }
                                            else
                                            {
                tabla +=                                "<tr>";
                tabla +=                                    "<td>" + precio.ListaDetalle.Precio+" - "+funcion.Sala.ServicioSala.First().Opcion.Codigo+"</td>";
                tabla +=                                "</tr>";
                                            }

                                        }
                tabla +=                   "</table>";
                tabla +=               "</td>";
                                    if (funcion.Estado != null)
                                    {
                tabla +=                    "<td>"+funcion.Estado.Descripcion+" </td>";
                                    }
                                    else
                                    {
                tabla +=                  "<td></td>";
                                    }
                tabla = tabla + "</tr>";
            }
            return tabla;
        }


         [CheckSessionOutAttribute]
        public string CargarFuncionesGraficas(int RowID_Encabezado, int DiasACorrer)
        {
            string tabla = "";
            List<Funcion> ListaFunciones = db.Funcion.Where(ld => ld.EncabezadoProgramacionID == RowID_Encabezado).ToList();
            List<Sala> listaSalas = null;
            EncabezadoProgramacion programacion = ListaFunciones.First().EncabezadoProgramacion;
            if (programacion == null)
            {
                programacion = db.EncabezadoProgramacion.FirstOrDefault(f => f.RowID == ListaFunciones.First().EncabezadoProgramacionID);
                listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID).ToList();
            }
            else
            {
                listaSalas = db.Sala.Where(f => f.TeatroID == programacion.TeatroID).ToList();
            }
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("Es-Es");
            int cantDias = DiasACorrer + 5;

            tabla += "  <thead>";
            tabla += "<tr>";
            tabla += "   <td class=\"day Bordes\" id=\"table-header-days\">";
            tabla += "       <p>Sala/Día</p>";
            tabla += "   </td>";
            for (int i = DiasACorrer; i < cantDias; i++)
            {
                if ((programacion.FechaInicial.Value.AddDays(i)) <= programacion.FechaFinal.Value)
                {
                    tabla += "         <td class=\"day Bordes\" id=\"table-header-days\">";
                    tabla += "              <p>" + ci.DateTimeFormat.GetDayName(programacion.FechaInicial.Value.AddDays(i).DayOfWeek).ToUpper() + "  " + programacion.FechaInicial.Value.AddDays(i).ToString("dd/MM/yyyy") + "</p>";
                    tabla += "         </td>";
                }
            }
            tabla += "      </tr>";
            tabla += "   </thead>";
            tabla += "<tbody>";
         
                 foreach (CinemaPOS.Models.Sala itemSala in listaSalas)
                {
                     tabla +=    "<tr>";
                      tabla +=       "<td class=\"Sala Bordes\" id=\"SalaTitle\">";
                       tabla +=          "<p>"+itemSala.Nombre+"</p>";
                       tabla +=      "</td>";
                       for (int i = DiasACorrer; i < cantDias; i++)
                       {
                        tabla +=         "<td class=\"Bordes\">";
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
                                                tabla += funcion.HoraInicial.Value + "  " + funcion.HoraFinal.Value;// @*@funcion.Fecha.Value.ToString("dd/MM/yyyy")*@
                                                tabla += "</p>";

                                            }
                                        }
                                    }
                                }
                       tabla +=          "</td>";
                        }
                    tabla +=     "</tr>";
                }
            tabla += "</tbody>";
            return tabla;
        }


        [CheckSessionOutAttribute]
        public JsonResult CargarDuracion(Int32 IdPeliculaDetalle)
        {
            DetallePelicula detalle = db.DetallePelicula.Where(f => f.RowID == IdPeliculaDetalle).FirstOrDefault();
            var duracion="";
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
            bool respuesta= false;
            List<String> dias = tarifa.DiasAsignados.Split(',').ToList();


            CultureInfo ci = new CultureInfo("Es-Es");
            var diaFuncion = ci.DateTimeFormat.GetDayName(funcion.Fecha.Value.DayOfWeek);
            foreach(String diaTarifa in dias){
                if (diaTarifa.ToUpper() == diaFuncion.ToUpper().Replace("É", "E").Replace("Á", "A"))
                {
                    respuesta = true;
                    return respuesta;
                }
            }
            return respuesta;
        }

        public JsonResult EliminarFuncion ( int RowID_Funcion)
        {
            if (db.Funcion.FirstOrDefault(f => f.RowID == RowID_Funcion) != null)
            {
                List<ListaPrecioFuncion> PreciosFuncion = db.ListaPrecioFuncion.Where(f=> f.FuncionID == RowID_Funcion).ToList();

                foreach (ListaPrecioFuncion item in PreciosFuncion)
                {
                    db.ListaPrecioFuncion.Remove(item);
                    db.SaveChanges();
                }

                db.Funcion.Remove(db.Funcion.Where(lt => lt.RowID == RowID_Funcion).First());
                db.SaveChanges();
            }
            return Json("");
        }

        public String ValidarFuncion(Funcion funcion , List<Funcion> FuncionesExistentes)
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
                                respuesta += " * " + funcion.DetallePelicula.EncabezadoPelicula.TituloLocal +" " + funcion.DetallePelicula.Opcion.Nombre + " el día " + funcion.Fecha.Value.ToString("dd/MM/yyyy") + " " + funcion.HoraInicial + "\n\n";
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
               ViewBag.listaSalas = db.Sala.Where(f => f.TeatroID == programacion.Sala.TeatroID);
               ViewBag.ListaPeliculas = db.TeatroPelicula.Where(f => f.TeatroID == programacion.Sala.TeatroID);
           }

            return View(funcion);
        }

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
                funcion.Fecha = Util.HoraInsertar(formulario["FechaFuncion"]);
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

    }
}
