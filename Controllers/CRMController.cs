using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.ModelosPropios;
namespace CinemaPOS.Controllers
{
    public class CRMController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();
        //
        // GET: /CRM/
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
        public ActionResult RegistroClienteTarjeta()
        {
            ViewBag.Ciudades = db.Ciudad.ToList();
            ViewBag.Generos = db.Opcion.Where(g => g.Tipo.Codigo == "TIPOSEXO").ToList();
            ViewBag.TipoCliente = db.Opcion.Where(g => g.Tipo.Codigo == "TIPOCLIENTE").ToList();
            return View();
        }
        public JsonResult GuardarCliente(FormCollection formulario)
        {
            formulario = DeSerialize(formulario);
            ClienteRoyal obj_cliente =new  ClienteRoyal();
            obj_cliente.Apellido = formulario["apellidos"];
            obj_cliente.Nombre = formulario["nombres"];
            obj_cliente.Documento = formulario["identificacion"];
            obj_cliente.CorreoElectronico = formulario["ciudad"];
            obj_cliente.Telefono = formulario["telefono"];
            obj_cliente.CiudadResidenciaID = int.Parse(formulario["ciudad"].ToString());
            obj_cliente.GeneroID = int.Parse(formulario["genero"]);
            obj_cliente.FechaNacimiento = ModelosPropios.Util.HoraInsertar(formulario["fecha_nacimiento"]);
            obj_cliente.ClasificacionID = int.Parse(formulario["clasificacion"]);
            obj_cliente.CiudadResidenciaID = int.Parse(formulario["nombre"]);
            db.ClienteRoyal.Add(obj_cliente);
            db.SaveChanges();
            return  Json("asd");
        }

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
                            genero = (clientes.GeneroID == null) ? "" : db.Opcion.Where(f=>f.RowID==clientes.GeneroID).FirstOrDefault().Nombre,
                            fechaNac = clientes.FechaNacimiento,
                            tarjetaID = (membresia.Codigo==null)? "": membresia.Codigo,
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
            ViewBag.TipoIdentificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOIDENTIFICACION").ToList();
            ViewBag.Ciudades = db.Ciudad.ToList();
            ViewBag.SexoID = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOSEXO").ToList();
            ViewBag.Clasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLIENTE").ToList();
            ViewBag.preferencias = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOGENEROPELICULA").ToList();
            TarjetaMembresiaClienteRoyal tmembresia = new TarjetaMembresiaClienteRoyal();
            try
            {tmembresia = db.TarjetaMembresiaClienteRoyal.Where(f => f.ClienteRoyalID == rowID).FirstOrDefault();}
            catch{}
            TarjetaMembresia membresia = new TarjetaMembresia();
            try
            {membresia = db.TarjetaMembresia.Where(f => f.RowID == tmembresia.TarjetaMembresiaID).FirstOrDefault();}
            catch{}
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
            if (cliente == null || rowID == 0)
            {cliente = new Models.ClienteRoyal();}

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
                cliente.FechaNacimiento = ModelosPropios.Util.HoraInsertar(formulario["FechaNacimiento"]);
                cliente.CiudadResidenciaID = Convert.ToInt32(formulario["CiudadResidenciaID"]);
                cliente.GeneroID = Convert.ToInt32(formulario["GeneroID"]);
                cliente.ClasificacionID = Convert.ToInt32(formulario["ClasificacionID"]);
                cliente.preferenciasID = Convert.ToInt32(formulario["preferenciasID"]);
                cliente.CreadoPor = Session["usuario_creacion"].ToString();
                cliente.FechaNacimiento = DateTime.Now;
                string cofd = formulario["TarjetaMembresiaClienteRoyal"].ToString();
                TarjetaMembresia membresia = db.TarjetaMembresia.Where(f => f.Codigo == cofd).FirstOrDefault();
                if (membresia == null)
                {membresia = new TarjetaMembresia();}
                string codigo = formulario["TarjetaMembresiaClienteRoyal"];
                membresia.Codigo = codigo;
                membresia.CreadoPor = Session["usuario_creacion"].ToString();
                membresia.FechaCreacion = DateTime.Now;
                membresia.EstadoID = Util.Constantes.ESTADO_ACTIVO;
                membresia.NoRedenciones = Util.Constantes.REDENCIONES_No4;
                if (membresia == null || rowID == 0)
                {db.TarjetaMembresia.Add(membresia);}
                db.SaveChanges();
                if (rowID == null || rowID == 0)
                {db.ClienteRoyal.Add(cliente);}
                db.SaveChanges();

                TarjetaMembresiaClienteRoyal tarjetaID = db.TarjetaMembresiaClienteRoyal.Where(f => f.ClienteRoyalID == rowID).FirstOrDefault();
                if (tarjetaID == null || rowID == 0)
                { tarjetaID = new TarjetaMembresiaClienteRoyal(); }
                tarjetaID.TarjetaMembresiaID = membresia.RowID;
                tarjetaID.ClienteRoyalID = cliente.RowID;
                tarjetaID.FechaActivacion = DateTime.Now;
                tarjetaID.EstadoID = Util.Constantes.ESTADO_ACTIVO;
                tarjetaID.NoRedencionesAprob = Util.Constantes.REDENCIONES_No4;
                if (rowID == null || rowID == 0)
                {db.TarjetaMembresiaClienteRoyal.Add(tarjetaID);}
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
            ViewBag.estado = db.Estado.Where(f => f.TipoEstadoID == Util.Constantes.TIPO_ESTADO_COTIZACION).ToList();
            ViewBag.ListaPrecios = db.ListaEncabezado.ToList();
            OportunidadVenta cotizacion = db.OportunidadVenta.Where(f => f.RowID == rowid).FirstOrDefault();
            if (cotizacion==null || rowid==null)
            {cotizacion = new OportunidadVenta();}
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
                if ((rowID!=null))
                { venta= db.OportunidadVenta.Where(f => f.RowID == rowID).FirstOrDefault(); }

                venta.ProspectoID = Convert.ToInt32(formulario["ProspectoIDx"]);
                venta.TeatroID = Convert.ToInt32(formulario["TeatroID"]);
                venta.ContactoID = Convert.ToInt32(formulario["ContactoHiden"]);
                venta.Titulo = formulario["Titulo"];
                venta.Valor = formulario["Valor"];
                venta.FechaApertura = ModelosPropios.Util.HoraInsertar(formulario["FechaApertura"]);
                venta.FechaCierra = ModelosPropios.Util.HoraInsertar(formulario["FechaCierra"]);
                venta.FechaEntrega = ModelosPropios.Util.HoraInsertar(formulario["FechaEntrega"]);
                venta.ListaPrecioID = Convert.ToInt32(formulario["ListaPrecioID"]);
                venta.Descripcion = formulario["Descripcion"];
                venta.EstadoID = Convert.ToInt32(formulario["EstadoID"]);
                if (rowID==0 || rowID==null)
                    {db.OportunidadVenta.Add(venta);}
                db.SaveChanges();
                return Json(new { respuesta = "ok", cotizaciones = venta.RowID });
            }
            catch (Exception ex)
            {return Json("error"+ex);}

        }
        #endregion

        #region ACTIVIDADES
        [CheckSessionOutAttribute]
        public ActionResult ListadoActividades()
        {
            List<Util.Actividades> actividades = new List<Util.Actividades>();

            actividades = (from actividad in db.Actividades
                           join tipo in db.Opcion on actividad.tipoActividadID equals tipo.RowID
                           join referencia in db.Opcion on actividad.tipoReferenciaID equals referencia.RowID
                           join prioridad in db.Opcion on actividad.prioridadID equals prioridad.RowID
                           join estado in db.Opcion on actividad.estadoID equals estado.RowID
                           select new Util.Actividades
                           {
                               rowid = actividad.rowID,
                               asunto = actividad.asunto,
                               tipoAct = (actividad.tipoActividadID == 0)? "" : db.Opcion.Where(f=>f.RowID==actividad.tipoActividadID).FirstOrDefault().Nombre,
                               fechaInicio = actividad.fechaInicio,
                               FechaFin = actividad.fechaFin,
                               tipoRef = (actividad.tipoReferenciaID == 0)? "" : db.Opcion.Where(f=>f.RowID==actividad.tipoReferenciaID).FirstOrDefault().Nombre,
                               referenciado_a = actividad.referenciado_a,
                               prioridad = (actividad.prioridadID == 0)? "" : db.Opcion.Where(f=>f.RowID==actividad.prioridadID).FirstOrDefault().Nombre,
                               estado = (actividad.estadoID == 0)? "" : db.Estado.Where(f=>f.RowID == actividad.estadoID).FirstOrDefault().Nombre,
                               descripcion = actividad.descripcion

                           }).ToList();
            ViewBag.Listado = actividades;
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult Actividades(int? rowid)
        {
            Actividades actividad = db.Actividades.Where(f => f.rowID == rowid).FirstOrDefault();
            if (actividad==null)
            { actividad = new Models.Actividades(); }
            ViewBag.TipoAct = db.Opcion.Where(f => f.TipoID == Util.Constantes.TIPO_ACTIVIDAD).ToList();
            ViewBag.tipoRef = db.Opcion.Where(f => f.TipoID == Util.Constantes.TIPO_REFERENCIA).ToList();
            ViewBag.Prioridad = db.Opcion.Where(f => f.TipoID == Util.Constantes.PRIORIDAD).OrderBy(f=>f.NumOrden).ToList();
            ViewBag.Estados = db.Estado.Where(f => f.TipoEstadoID == Util.Constantes.ESTADOS_ACTIVIDAD).ToList();
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

            List<Util.ModalReferencias> lista = new List<Util.ModalReferencias>();
            switch (t_referencia)
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
                                 columna2= item.Nombre + " " + item.Apellidos,
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
                    lista = (from item in db.OportunidadVenta.Where(f => f.EstadoID != Util.Constantes.OPORTUNIDAD_VENTA).OrderByDescending(f => f.RowID).ToList()
                             select new Util.ModalReferencias()
                             {
                                 rowid = item.RowID,
                                 valor = (item.ProspectoID == null) ? "" : item.Titulo + " - " + db.Teatro.Where(f => f.RowID == item.ProspectoID).FirstOrDefault().Nombre,
                                 columna1 = (item.ProspectoID == null) ? "" : db.Teatro.Where(f => f.RowID == item.ProspectoID).FirstOrDefault().Nombre,
                                 columna2 = item.CreadoPor,
                                 columna3 = item.fechaCreacion.ToString(),
                                 columna4 = (item.EstadoID==0)?"":item.Estado.Nombre
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
                                 columna3 = (item.EmpresaID == null)?"": db.Tercero.Where(f=>f.RowID==item.EmpresaID).FirstOrDefault().Nombre + " "+ db.Tercero.Where(f => f.RowID == item.EmpresaID).FirstOrDefault().Apellidos,
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
                                 columna2 = (item.TerceroID == null)?"": db.Tercero.Where(f=>f.RowID==item.TerceroID).FirstOrDefault().Nombre + " " + db.Tercero.Where(f => f.RowID == item.TerceroID).FirstOrDefault().Apellidos,
                                 columna3 = (item.TipoSolicitudID==null)?"": db.Opcion.Where(f=>f.RowID==item.TipoSolicitudID).FirstOrDefault().Nombre,
                                 columna4 = (item.EstadoID==null)?"": db.Estado.Where(f=>f.RowID==item.EstadoID).FirstOrDefault().Nombre
                             }).ToList();
                    break;
            }
            return View(lista.ToList());
        }
        #endregion
    }
}
