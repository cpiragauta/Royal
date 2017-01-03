using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using CinemaPOS.Models;
using System.IO;


namespace CinemaPOS.ModelosPropios
{
    public class MailSender
    {
        private static String Server { get; set; }
        private static String User { get; set; }
        private static String Password { get; set; }
        private static String Domain { get; set; }
        private static String HtmlEnvelope { get; set; }
        private static String Port { get; set; }
        private static bool EnableSSL { get; set; }
        private static String MailFrom { get; set; }
        private static bool SendMailEnabled { get; set; }

        private static CinemaPOSEntities db;

        private static void SetMailOptions()
        {
            List<Parametros> listParametros = db.Parametros.ToList();

                Server = listParametros.Where(f => f.cod_parametro == "MAILSERVER").First().valor_parametros;

                User = listParametros.Where(f => f.cod_parametro == "MAILUSER").First().valor_parametros;

                Password = listParametros.Where(f => f.cod_parametro == "MAILPASSWD").First().valor_parametros;
                try
                {
                    MailFrom = listParametros.Where(f => f.cod_parametro == "MAILFROM").First().valor_parametros;
                }
                catch { }
                try
                {
                    SendMailEnabled = true;
                    SendMailEnabled = bool.Parse(listParametros.Where(f => f.cod_parametro == "SEND.MAIL").First().valor_parametros);
                }
                catch { }
            


        }
        public static void SendEmail(EnvioMail message)
        {
            IList<string> toAddress = new List<string>();

            try
            {

                if (string.IsNullOrEmpty(message.MTo))
                    return;

                SetMailOptions();

                if (!SendMailEnabled)
                    return;

                if (!string.IsNullOrEmpty(HtmlEnvelope))
                    message.Mensaje = HtmlEnvelope.Replace("__CONTENT", message.Mensaje);

                MailMessage objMessage = new MailMessage();

                objMessage.From = new MailAddress(MailFrom); //new MailAddress(message.mfrom);


                foreach (String address in message.MTo.Replace(" ", ";").Split(';'))
                {
                    if (string.IsNullOrEmpty(address.Trim()))
                        continue;

                    if (toAddress.Contains(address.Trim()))
                        continue;

                    try
                    {
                        objMessage.To.Add(address.Trim());
                        toAddress.Add(address.Trim());
                    }
                    catch { }
                }

                objMessage.Subject = message.Asunto;
                objMessage.Body = message.Mensaje;


                objMessage.IsBodyHtml = true;

                SmtpClient smtpClient = new SmtpClient(Server);

                if (string.IsNullOrEmpty(User))
                    smtpClient.UseDefaultCredentials = true;

                else
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(User, Password, Domain);
                }

                if (!string.IsNullOrEmpty(Port))
                    smtpClient.Port = int.Parse(Port);

                if (EnableSSL)
                    smtpClient.EnableSsl = true;

                smtpClient.Send(objMessage);
            }
            catch
            {
                throw;
            }

        }

        public static void Enviar_Actividad(object Actividad,string CodPlantilla,string usuario,string adjunto,string Copiacorreo)
        {
            if (db == null)
                db = new CinemaPOSEntities();

            Plantillas plantilla = db.Plantillas.FirstOrDefault(f => f.NombrePlantilla == CodPlantilla);
            string EmailSend = "";
            UsuarioSistema m_to = new UsuarioSistema();
            m_to = db.UsuarioSistema.Where(f => f.NombreUsuario == usuario).FirstOrDefault();
            EmailSend = m_to.Correo;
            string mensaje = "", subject = "", usuariocreacion = "";
            mensaje = ReemplazarPlantillaActividades(plantilla.CuerpoMsj,(Actividades)Actividad);
            subject = ReemplazarPlantillaActividades(plantilla.Titulo, (Actividades)Actividad);

            usuariocreacion = ((Actividades)Actividad).creadoPor;
            string fecha = Convert.ToString(DateTime.Now.ToString("yyyy/MM/dd"));
            DateTime Fe = Convert.ToDateTime(fecha);
            EnvioMail SendMail = new EnvioMail
            {
                MTo = EmailSend,
                Mensaje = mensaje,
                CreadoPor = usuariocreacion,
                Asunto = subject,
                FechaCreacion = Fe,
                FechaEnviado = Fe,
                sent = 0
            };

            if (!string.IsNullOrEmpty(adjunto))
            {
                try
                {
                    //Crear el attachment con el file creado
                    //Carpeta Documentos - Adjuntos
                    StreamWriter file = new StreamWriter(adjunto);
                    file.WriteLine(mensaje);
                    file.Close();

                    SendMail.Adjunto = adjunto;
                }
                catch { }
            }
            db.EnvioMail.Add(SendMail);
            db.SaveChanges();

            SendPendingMessages();
        }
        public static string ReemplazarPlantillaActividades(string mensaje, Actividades actividad)
        {
            string Logo = "";
            try
            {
                Parametros log = db.Parametros.Where(f => f.cod_parametro == "LOGOMAIL").First();
                Logo = log.valor_parametros;
                Logo = "<img src='" + Logo + "'/><br/>";
            }
            catch{}

            string detalles = actividad.referenciado_a + " " + actividad.descripcion;
            var tipo = db.Opcion.Where(f => f.RowID == actividad.tipoActividadID).FirstOrDefault();
            var prioridad = db.Opcion.Where(f => f.RowID == actividad.prioridadID).FirstOrDefault();
            var estado = db.Estado.Where(f => f.RowID == actividad.estadoID).FirstOrDefault();
            DateTime fechaFin = Convert.ToDateTime(actividad.fechaFin);
            mensaje = mensaje.Replace("__LOGO", Logo);
            mensaje = mensaje.Replace("__ACTIVIDAD", tipo.Codigo);
            mensaje = mensaje.Replace("__ASUNTO", actividad.asunto);
            mensaje = mensaje.Replace("__PRIORIDAD",prioridad.Codigo);
            mensaje = mensaje.Replace("__REFERENCIADO", actividad.referenciado_a);
            mensaje = mensaje.Replace("__DESCRIPCION", actividad.descripcion);
            mensaje = mensaje.Replace("__FECHAINICIO", actividad.fechaInicio.ToString("dd/MM/yyyy"));
            mensaje = mensaje.Replace("__FECHAFIN", fechaFin.ToString("dd/MM/yyyy"));
            mensaje = mensaje.Replace("__ESTADO", estado.Nombre);
            mensaje = mensaje.Replace("__COD", actividad.rowID.ToString());
            mensaje = mensaje.Replace("__TITULO", actividad.asunto);
            mensaje = mensaje.Replace("__EMPRESA", "ROYAL FILMS");
            
            try
            {
                mensaje = mensaje.Replace("__LOGO", "");
                mensaje = mensaje.Replace("__ACTIVIDAD", "");
                mensaje = mensaje.Replace("__ASUNTO", "");
                mensaje = mensaje.Replace("__PRIORIDAD", "");
                mensaje = mensaje.Replace("__REFERENCIADO", "");
                mensaje = mensaje.Replace("__DESCRIPCION", "");
                mensaje = mensaje.Replace("__FECHAINICIO", "");
                mensaje = mensaje.Replace("__FECHAFIN", "");
                mensaje = mensaje.Replace("__ESTADO", "");
                mensaje = mensaje.Replace("__COD", "");
                mensaje = mensaje.Replace("__TITULO","");
                mensaje = mensaje.Replace("__EMPRESA", "");
            }
            catch{}
            return mensaje;
        }
        public static void SendPendingMessages()
        {
            if (db == null)
                db = new CinemaPOSEntities();

            IList<EnvioMail> msgList = db.EnvioMail.Where(f=>f.sent==0).ToList();

            if (msgList == null || msgList.Count == 0)
                return;

            foreach (EnvioMail msg in msgList)
            {
                try
                {
                    MailSender.SendEmail(msg);
                    msg.sent = 1;
                    msg.FechaEnviado = DateTime.Now;
                }
                catch (Exception ex)
                {
                }
            }

            db.SaveChanges();

        }
    }
}