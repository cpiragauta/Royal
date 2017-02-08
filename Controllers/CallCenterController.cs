using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.ModelosPropios;

namespace CinemaPOS.Controllers
{
    public class CallCenterController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();



        [CheckSessionOutAttribute]
        public ActionResult VistaSeleccionarTeatro()
        {
            ViewBag.ListaTeatros = db.Teatro.Where(f => f.Estado.Codigo == "ABIERTO");            
            return View();
        }

        [CheckSessionOutAttribute]
        public bool ValidarConexion(int RowIDTeatro)
        {
            bool conexionExitosa = false;
            try
            {
                Teatro teatro = db.Teatro.FirstOrDefault(f=>f.RowID == RowIDTeatro);
                if (String.IsNullOrEmpty(teatro.CadenaBD))
	            {
		            return false;
	            }
                String ip = Session["IP"].ToString();
                Taquilla taquilla = db.Taquilla.FirstOrDefault(f => f.IP == ip);
                Session["Taquilla"] = taquilla;
                Session["RowID_Taquilla"] = taquilla.RowID;
                taquilla.TeatroID = teatro.RowID;
                db.SaveChanges();

                db.Database.Connection.ConnectionString = teatro.CadenaBD;
                Session["RowID_Teatro"] = teatro.RowID;
                Util.CADENA_BD_CC = teatro.CadenaBD;
                //Prueba para ver si puede consultar
                List<Taquilla> asd = db.Taquilla.ToList();
                conexionExitosa = true;
            }
            catch (Exception)
            {
                conexionExitosa = false;
            }
            return conexionExitosa;
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarReserva(FormCollection formulario)
        {
            String codigoReserva = "", respuesta="";
            formulario = DeSerialize(formulario);
            Reserva reserva = new Reserva();
            string cadena = db.Database.Connection.ConnectionString;

            try //Guardo en el Central
            {
                int idTeatro = Convert.ToInt32(Session["RowID_Teatro"].ToString());
                Teatro teatro = db.Teatro.FirstOrDefault(f => f.RowID == idTeatro);
                //Le asigno la cadena del teatro
                db.Database.Connection.ConnectionString = teatro.CadenaBD;
                codigoReserva += teatro.Nombre.Substring(0, 3).ToUpper(); //Primeras 3 letras del teatro de la reserva
                codigoReserva += formulario["Identificacion"].ToString().Substring(0, 3).ToUpper(); //Primeros 3 digitos de la identificacion
                reserva.ClienteID = formulario["Identificacion"].ToString();
                reserva.NombresCliente = formulario["NombresCliente"].ToString();
                reserva.TelefonoCliente = formulario["TelefonoCliente"].ToString();
                reserva.Estado = db.Estado.FirstOrDefault(f => f.Codigo == "RESERVADA" && f.TipoEstado.Codigo == "TIPORESERVA");
                reserva.FechaReserva = DateTime.Now;
                reserva.FechaCreacion = DateTime.Now;
                reserva.CreadoPor = Session["usuario_creacion"].ToString();
                db.Reserva.Add(reserva);
                db.SaveChanges();
                codigoReserva += reserva.RowID;      //Id con el que se creo en el central
                reserva.CodigoReserva = codigoReserva;
                db.SaveChanges();
                respuesta = "Guardado Correctamente en el Central";
            }
            catch (Exception ex)
            {
                respuesta = "Error Guardado en el Central";
            }
            String[] DatosSillas = (formulario["DatosSillasSeleccionadas"].ToString()).Split('_');
            int RowIDFuncion = Convert.ToInt32(DatosSillas[0]);
            int tarifaID = Convert.ToInt32(DatosSillas[1]);
            string IDSillas = DatosSillas[2];
            AsignarBoletasAReserva(IDSillas, tarifaID, RowIDFuncion, reserva);

            return Json(new { codigoReserva = codigoReserva });
        }


        [CheckSessionOutAttribute]
        public void AsignarBoletasAReserva(string IDSillas, int IDTarifas, int RowIDFuncion, Reserva reserva)
        {
            UsuarioSistema usuariotaquilla = (UsuarioSistema)(Session["usuario"]);
            Taquilla taquillas = db.Taquilla.FirstOrDefault(f => f.Nombre == "GENERICA");
            string Boletas = "";
            BoletaReservada objBoleta = new BoletaReservada();
            var RowIDSillas = IDSillas.TrimEnd(',').Split(',');
           // int RowIDTarifa = int.Parse(IDTarifas.Split(',')[0].ToString());
            string cadena = db.Database.Connection.ConnectionString;
            int RowIDTarifa = IDTarifas;
            int RowIDSilla = 0;
            string tipo_respuesta = "";
            string Respuesta = "";
            int[] rowid_sillasvendidas = new int[RowIDSillas.Length];
            List<BoletaReservada> ListaBoletas = new List<BoletaReservada>();
            for (int i = 0; i < RowIDSillas.Length; i++)
            {

                RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                objBoleta = db.BoletaReservada.Where(BV => BV.SillaID == RowIDSilla && BV.FuncionID == RowIDFuncion).FirstOrDefault();
                if (objBoleta != null)
                {
                    ListaBoletas.Add(objBoleta);
                }
            }
            if (ListaBoletas.Count() == 0)
            {
                List<BoletaReservada> listaboletasimprime = new List<BoletaReservada> { };
                BoletaReservada objBoletaReserva = new BoletaReservada();
                for (int i = 0; i < RowIDSillas.Length; i++)
                {
                    RowIDSilla = int.Parse(RowIDSillas[i].ToString());
                    objBoletaReserva.FuncionID = RowIDFuncion;
                    objBoletaReserva.TarifaID = RowIDTarifa;
                    objBoletaReserva.SillaID = RowIDSilla;
                    objBoletaReserva.FechaReserva = DateTime.Now;
                    objBoletaReserva.FechaCreacion = DateTime.Now;
                    objBoletaReserva.TaquillaID = taquillas.RowID;
                    //objBoletaReserva.TaquillaID = 53;
                    objBoletaReserva.Estado = db.Estado.FirstOrDefault(f => f.Codigo == "RESERVADA" && f.TipoEstado.Codigo == "TIPORESERVA");
                    objBoletaReserva.UsuarioID = usuariotaquilla.RowID;
                    objBoletaReserva.ReservaID = reserva.RowID;
                    objBoletaReserva.CreadoPor = Session["usuario_creacion"].ToString();
                    db.BoletaReservada.Add(objBoletaReserva);
                    db.SaveChanges();
                    rowid_sillasvendidas[i] = int.Parse(objBoletaReserva.RowID.ToString());
                    Boletas += "";
                }
            }
            else if (ListaBoletas.Count() != 0)
            {
                Respuesta = "Las siguientes boletas ya se encuentran Reservadas";
            }
           // return Json(new { tipo_respuesta = tipo_respuesta, Respuesta = Respuesta, data = ListaBoletas, html = Boletas }, JsonRequestBehavior.AllowGet);
        }




        [CheckSessionOutAttribute]
        public JsonResult LiberarManualmente(FormCollection formulario)
        {
            String respuesta = "", tipoRespuesta = "error";
            formulario = DeSerialize(formulario);
            String cadenaActual = db.Database.Connection.ConnectionString;
            try 
            {
                int idTeatro = Convert.ToInt32(formulario["TeatroID"].ToString());
                string codigoReserva = formulario["CodigoReserva"].ToString();
                //Establezco conexion con el teatro
                db.Database.Connection.ConnectionString = db.Teatro.FirstOrDefault(f => f.RowID == idTeatro).CadenaBD;
                //Busco Reserva y cambio estado
                Estado LiberadaManual = db.Estado.FirstOrDefault(f => f.Codigo == "LIBERADAMANUAL" && f.TipoEstado.Codigo == "TIPORESERVA");
                Reserva reserva = db.Reserva.FirstOrDefault(f => f.CodigoReserva == codigoReserva);
                reserva.EstadoID = LiberadaManual.RowID;
                reserva.FechaModificacion = DateTime.Now;
                reserva.ModificadoPor = Session["usuario_creacion"].ToString();
                db.SaveChanges();
                //Busco boletas de la reserva y cambio el estado 
                List<BoletaReservada> Boletas = db.BoletaReservada.Where(f => f.ReservaID == reserva.RowID).ToList();

                foreach (BoletaReservada item in Boletas)
                {
                    item.EstadoID = LiberadaManual.RowID;
                    item.FechaModificacion = DateTime.Now;
                    item.ModificadoPor = Session["usuario_creacion"].ToString();
                    db.SaveChanges();
                }
                respuesta = "Reserva Liberada correctamente";
                tipoRespuesta= "success";
                //Devuelvo la cadena de conexion que tenia
                db.Database.Connection.ConnectionString = cadenaActual;
            }
            catch (Exception ex)
            {
                db.Database.Connection.ConnectionString = cadenaActual;
                tipoRespuesta = "Error Liberando Reserva";
            }
            return Json(new { respuesta = respuesta, tipoRespuesta = tipoRespuesta });
        }


        

            [CheckSessionOutAttribute]
        public JsonResult LiberarBoletaManualmente(int RowID_Boleta, int TeatroID)
        {
            String respuesta = "", tipoRespuesta = "error";
            String cadenaActual = db.Database.Connection.ConnectionString;
            try 
            {
                //Establezco conexion con el teatro
                db.Database.Connection.ConnectionString = db.Teatro.FirstOrDefault(f => f.RowID == TeatroID).CadenaBD;
                Estado LiberadaManual = db.Estado.FirstOrDefault(f => f.Codigo == "LIBERADAMANUAL" && f.TipoEstado.Codigo == "TIPORESERVA");
                //Busco la boleta y cambio el estado 
                BoletaReservada Boleta = db.BoletaReservada.FirstOrDefault(f => f.RowID == RowID_Boleta);
                Boleta.EstadoID = LiberadaManual.RowID;
                Boleta.FechaModificacion = DateTime.Now;
                Boleta.ModificadoPor = Session["usuario_creacion"].ToString();
                db.SaveChanges();
                respuesta = "Boleta Liberada correctamente";
                tipoRespuesta= "success";
                //Devuelvo la cadena de conexion que tenia
                db.Database.Connection.ConnectionString = cadenaActual;
            }
            catch (Exception ex)
            {
                db.Database.Connection.ConnectionString = cadenaActual;
                tipoRespuesta = "Error Liberando Reserva";
            }
            return Json(new { respuesta = respuesta, tipoRespuesta = tipoRespuesta });
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


        

        [CheckSessionOutAttribute]
        public JsonResult RedimirReserva(string CodigoReserva)
        {
            /**/
            string tipoRespuesta = "warning", respuesta = "";
            if (String.IsNullOrEmpty(CodigoReserva))
            {
                return Json(new { tipoRespuesta = tipoRespuesta, respuesta = "Ingrese un código valido" });
            }
            else
            {
                Reserva reserva;
                //Busco por Codigo Reserva 
                reserva = db.Reserva.FirstOrDefault(f => f.CodigoReserva.Equals(CodigoReserva));
                if(reserva != null){
                    respuesta = "Codigo valido";
                    tipoRespuesta = "success";
                }

                //Busco Por Telefono
                if (reserva == null)
                {
                    reserva = db.Reserva.FirstOrDefault(f => f.TelefonoCliente.Equals(CodigoReserva));
                    if (reserva != null)
                    {
                        respuesta = "Codigo valido";
                        tipoRespuesta = "success";
                    }
                }
                if (reserva == null)
                {
                    respuesta = "No se encontro reserva con el código " + CodigoReserva;
                }
            }
            return Json(new { tipoRespuesta = tipoRespuesta, respuesta = respuesta },JsonRequestBehavior.AllowGet);
        }


    }
}
