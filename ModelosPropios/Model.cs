using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CinemaPOS.Models;
using CinemaPOS.Controllers;

namespace CinemaPOS.ModelosPropios
{
    public class Model
    {
        public partial class UsuarioRolSistema
        {

            public UsuarioSistema Usuario { get; set; }
            public Rol Rol { get; set; }

            public List<UsuarioSistema> ListaUsuarios { get; set; }


            public List<Rol> ListaRoles { get; set; }

        }
        public partial class Modelos_Pqrs{
            public Tercero Terceros { get; set; }
            public List<Pqrs> Pqrs_Terceros { get; set; }
            public List<Seguimiento> Pqrs_Seguimiento { get; set; }
            public List<Teatro> Teatros { get; set; }
        }
        public partial class Funcion_Vista
        {
            //public Funcion Funciones { get; set; }
            //public List<DetallePelicula> detalle_pelicula { get; set; }
            public DetallePelicula Detalle_Pelicula { get; set; }
            public List<Funciones> Funciones { get; set; }
        }

        //public partial class Tarifas_funcion
        //{
        //    public Funcion Funcion { get; set; }

        //    public List<ListaPrecioFuncion> Tarifas_de_funcion{get;set;}
        //}
    }
}