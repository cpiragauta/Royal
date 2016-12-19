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

            /*TIPO DEESTADOS ACTIVIDADES*/
            public const int ESTADOS_ACTIVIDAD = 14;
            /*REFERENCIAS*/
            public const int ACTIVIDAD_TIPO_RELACION_Cliente = 104;
            public const int ACTIVIDAD_TIPO_RELACION_Oportunidad = 105;
            public const int ACTIVIDAD_TIPO_RELACION_Contacto = 106;
            public const int ACTIVIDAD_TIPO_RELACION_pqrs = 108;

            /*ESTADOS DE OPORTUNIDAD DE VENTA*/
            public const int OPORTUNIDAD_VENTA = 43;
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

        public static DateTime HoraInsertar(string FechaConvertir)
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

    }
}