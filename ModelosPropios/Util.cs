using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CinemaPOS.Models;
using CinemaPOS.Controllers;
using System.Globalization;
using System.Drawing;
using System.IO;

namespace CinemaPOS.ModelosPropios
{
    public class Util
    {
        public class ClientesRoyal
        {
            public ClienteRoyal Clienteroyal { get; set; }
            public string ciudad { get; set; }
            public Opcion opciones { get; set; }
            public int rowid { get; set; }
            public string identificacion { get; set; }
            public string correo { get; set; }
            public string nombreCompleto { get; set; }
            public string apellido { get; set; }
            public string telefono { get; set; }
            public string ciudades { get; set; }
            public string genero { get; set; }
            public string fechanacimiento { get; set;}
            public string tarjetaid { get; set; }
            public string informacion { get; set; }
            public string clasificacion { get; set; }
            public string preferencias { get; set; }
            public string documento { get; set; }
            public DateTime? fechaNac { get; set; }
            public string tarjetaID { get; set; }
            public bool? info { get; set; }
            public string pref { get; set; }
        }
        public class Cotizaciones
        {
            
            public int? rowid { get; set; }
            public string prospecto { get; set; }
            public string teatro { get; set;}
            public string contacto { get; set; }
            public Contacto contactos { get; set; }
            public string titulo { get; set; }
            public string valor { get; set; }
            public DateTime? fechaApertura { get; set; }
            public DateTime? fechaCierre { get; set; }
            public DateTime? fechaEntrega { get; set; }
            public string listaPrecios { get; set; }
            public string descripcion { get; set; }
            public int? prospectoID { get; set; }
            public int? teatroID { get; set; }


        }
        public static class Constantes
        {
            /* ESTADOS TARJETA MEMBRESIA*/

            public const int ESTADO_ASIGNADA = 36;
            public const int ESTADO_CANCELADA = 37;
            public const int ESTADO_ACTIVO = 9;
            public const int ESTADO_INACTIVO = 10;

            /*VALOR POR DEFFECTO NO. REDENCIONES*/
            public const int REDENCIONES_No4 = 4;

            /*ESTADOS COTIZACIONES*/
            public const int TIPO_ESTADO_COTIZACION = 13;
            public const int ESTADO_NUEVA = 38;
            public const int ESTADO_EN_INVESTIGACION = 39;
            public const int ESTADO_COTIZADO = 40;
            public const int ESTADO_EN_CIERRE = 41;
            public const int ESTADO_GANADA = 42;
            public const int ESTADO_PEDIDA = 43;

            /* TIPO DE ACTIVIDADES*/
            public const int TIPO_ACTIVIDAD = 20;
            /* TIPO REFERENCIA*/
            public const int TIPO_REFERENCIA = 21;
            /*PRIORIDAD*/
            public const int PRIORIDAD = 22;

            /*TIPO DE ESTADOS ACTIVIDADES*/
            public const int ESTADOS_ACTIVIDAD = 14;
            /*ESTADOS DE ACTIVIDADES*/
            public const int ACTIVIDAD_CERRADA = 49;
            public const int ACTIVIDAD_EN_CURSO = 45;
            public const int ACTIVIDAD_EN_ESPERA = 46;
            public const int ACTIVIDAD_PROGRAMADA = 44;
            /*REFERENCIAS*/
            public const int ACTIVIDAD_TIPO_RELACION_Cliente = 104;
            public const int ACTIVIDAD_TIPO_RELACION_Oportunidad = 105;
            public const int ACTIVIDAD_TIPO_RELACION_Contacto = 106;
            public const int ACTIVIDAD_TIPO_RELACION_pqrs = 108;

            /*ESTADOS DE OPORTUNIDAD DE VENTA*/
            public const int OPORTUNIDAD_VENTA = 43;
            /*TIPO SELLO*/
            public const int TIPO_SELLO = 99;
            /*TIPO DE USUARIOS*/
            public const int ROL_ADMINISTRADOR = 1;

            /*TIPO DE AREA*/
            public const int TIPO_AREA = 14;

            /*TIPO ESTADO SEGUIMIENTO PQRS*/
            public const int TIPO_ESTADO_PQRS = 14;
            public const string ESTADO_CONVENIO_REDIMIDO = "REDIMIDO";
            public const string TIPO_ESTADO_CONVENIO = "TIPOCONVENIO";

        }
        public class Actividades
        {
            public int rowid { get; set; }
            public string tipoAct { get; set; }
            public string asunto { get; set; }
            public string prioridad { get; set; }
            public string referenciado_a { get; set; }
            public string descripcion { get; set; }
            public DateTime fechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string estado { get; set; }
            public string tipoRef { get; set; }

        }
        public class ModalReferencias
        {
            public int rowid { get; set; }
            public string valor { get; set; }
            public string columna1 { get; set; }
            public string columna2 { get; set; }
            public string columna3 { get; set; }
            public string columna4 { get; set; }
        }
        public class fullCalendar
        {
            public int id { get; set; }
            public string title { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string backgroundColor { get; set; }
            public string borderColor { get; set; }
            public bool allDay { get; set; }
            public bool url { get; set; }
        }
        public static DateTime FechaInsertar(string FechaConvertir)
        {
            try
            {
                IFormatProvider culture = new CultureInfo("es-ES", true);
                return DateTime.ParseExact(FechaConvertir, "dd/MM/yyyy", culture);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }

        }

        public String Redimensionar(Image Imagen_Original, string nombre)
        {
            //RUTA DEL DIRECTORIO TEMPORAL 
            String DirTemp = Path.GetTempPath() + @"\" + nombre + ".jpg"; ;
            //IMAGEN ORIGINAL A REDIMENSIONAR 
            Bitmap imagen = new Bitmap(Imagen_Original);
            //CREAMOS UN MAPA DE BIT CON LAS DIMENSIONES QUE QUEREMOS PARA LA NUEVA IMAGEN 
            Bitmap nuevaImagen = new Bitmap(Imagen_Original.Width, Imagen_Original.Height);
            //CREAMOS UN NUEVO GRAFICO 
            Graphics gr = Graphics.FromImage(nuevaImagen);
            //DIBUJAMOS LA NUEVA IMAGEN 
            gr.DrawImage(imagen, 0, 0, nuevaImagen.Width, nuevaImagen.Height);
            //LIBERAMOS RECURSOS 
            gr.Dispose();
            //GUARDAMOS LA NUEVA IMAGEN ESPECIFICAMOS LA RUTA Y EL FORMATO 
            nuevaImagen.Save(DirTemp, System.Drawing.Imaging.ImageFormat.Jpeg);
            //LIBERAMOS RECURSOS 
            nuevaImagen.Dispose();
            imagen.Dispose();
            return DirTemp;
        }
        public Byte[] Imagen_A_Bytes(String ruta)
        {
            FileStream foto = new FileStream(ruta, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Byte[] arreglo = new Byte[foto.Length];
            BinaryReader reader = new BinaryReader(foto);
            arreglo = reader.ReadBytes(Convert.ToInt32(foto.Length));
            return arreglo;
        }
        public Image Bytes_A_Imagen(Byte[] ImgBytes)
        {
            Bitmap imagen = null;
            Byte[] bytes = (Byte[])(ImgBytes);
            MemoryStream ms = new MemoryStream(bytes);
            imagen = new Bitmap(ms);
            return imagen;
        }
    }
}