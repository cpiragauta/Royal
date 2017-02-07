using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CinemaPOS.Models;
using CinemaPOS.Controllers;
using System.Globalization;
namespace CinemaPOS.ModelosPropios
{
    public class Util
    {
        /*Conexion a base de datos para call Center*/
        public static string CADENA_BD_CC { get; set; }


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
            public string fechanacimiento { get; set; }
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
            public string teatro { get; set; }
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

            public const string ESTADO_ASIGNADA = "ASIGNADO";
            public const string ESTADO_CANCELADA = "CANCELADA";
            public const string ESTADO_ACTIVO = "ACTIVO";
            public const string ESTADO_INACTIVO = "INACTIVO";

            /*VALOR POR DEFFECTO NO. REDENCIONES*/
            public const int REDENCIONES_No4 = 4;

            /*ESTADOS COTIZACIONES*/
            public const string TIPO_ESTADO_COTIZACION = "TIPOCOTIZACION";
            public const string ESTADO_NUEVA = "NUEVA";
            public const string ESTADO_EN_INVESTIGACION = "ENINVESTIGACIÓN";
            public const string ESTADO_COTIZADO = "COTIZADO";
            public const string ESTADO_EN_CIERRE = "ENCIERRE";
            public const string ESTADO_GANADA = "GANADA";
            public const string ESTADO_PEDIDA = "PEDIDA";

            /* TIPO DE ACTIVIDADES*/
            public const string TIPO_ACTIVIDAD = "TIPOACTIVIDAD";
            /* TIPO REFERENCIA*/
            public const string TIPO_REFERENCIA = "TIPOREFERENCIA";
            /*PRIORIDAD*/
            public const string PRIORIDAD = "PRIORIDAD";

            /*TIPO DE ESTADOS ACTIVIDADES*/
            public const string ESTADOS_ACTIVIDAD = "TIPOACTIVIDAD";
            /*ESTADOS DE ACTIVIDADES*/
            public const string ACTIVIDAD_CERRADA = "CERRADA";
            public const string ACTIVIDAD_EN_CURSO = "ENCURSO";
            public const string ACTIVIDAD_EN_ESPERA = "ENESPERA";
            public const string ACTIVIDAD_PROGRAMADA = "PROGRAMADA";
            /*REFERENCIAS*/
            public const string ACTIVIDAD_TIPO_RELACION_Cliente = "CLIENTE";
            public const string ACTIVIDAD_TIPO_RELACION_Oportunidad = "PROSPECTO";
            public const string ACTIVIDAD_TIPO_RELACION_Contacto = "CONTACTO";
            public const string ACTIVIDAD_TIPO_RELACION_pqrs = "PQRS";

            /*ESTADOS DE OPORTUNIDAD DE VENTA*/
            public const string OPORTUNIDAD_VENTA = "PEDIDA";
            /*TIPO SELLO*/
            public const string TIPO_SELLO = "SELLO";
            /*TIPO DE USUARIOS*/
            public const string ROL_ADMINISTRADOR = "ADMINISTRADOR";

            /*TIPO DE AREA*/
            public const string TIPO_AREA = "TIPOAREAPQRS";

            /*TIPO ESTADO SEGUIMIENTO PQRS*/
            public const string TIPO_ESTADO_PQRS = "TIPOACTIVIDAD";

            public const string ESTADO_CONVENIO_REDIMIDO = "REDIMIDO";
            public const string TIPO_ESTADO_CONVENIO = "TIPOCONVENIO";

            /*Para validar los permisos de los usuarios*/
            public const string NOMBRE_TEATRO_CENTRAL = "CENTRAL";
            /*Estado apertura caja usuario*/
            public const string ESTADO_CONTROL_CAJA_USUARIO = "PORRECIBIR";


            public const string TIPO_VERSION_2D = "2D";
            public const string TIPO_VERSION_3D = "3D";
            public const string SERVICIO_ULTRA = "Ultra";
            public const string SERVICIO_GENERAL = "General";
            public const string SERVICIO_VIP = "VIP";
            public const string SERVICIO_PLUS = "Plus";
            public const string SERVICIO_4DX = "4DX";
            public const string SERVICIO_4TD = "4TD";
            public const string SERVICIO_Covan = "Covan";
            public const string SERVICIO_Atmos = "Atmos";

            /*FORMATOS PELICULA*/
            public const string FORMATO_2D = "2D";
            public const string FORMATO_3D = "3D";
            public const string FORMATO_4D = "4D";

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


        public class funcionesUtil
        {
            public Funciones Funcion { get; set; }

        }
        public class FuncionEncabezado
        {
            public string titulo { get; set; }
            public string version { get; set; }
            public string Formato { get; set; }

            public int pelicula { get; set; }
            public int peliculaDetalleID { get; set; }
            public string Poster { get; set; }
            public List<FuncionesTvShow> Lista { get; set; }
            public string Servicio { get; set; }
        }

        public class PreciosEncabezado
        {
            public int RowID { get; set; }
            public string Dias { get; set; }
            public string Precio { get; set; }
            public string Formato { get; set; }
            public string TipoTarifa { get; set; }
            public string Teatro { get; set; }
            public string Servicio { get; set; }
            public DateTime FechaInicial { get; set; }
            public DateTime FechaFinal { get; set; }
            public string GrupoDias { get; set; }
        }
    }
}