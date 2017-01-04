using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;

namespace CinemaPOS.Controllers
{
    public class SincronizacionController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();

        public ActionResult VistaPrincipal()
        {
            //return View(db.SincronizacionMaestros.ToList());
            return View();
        }

        [CheckSessionOutAttribute]
        public String SincronizarUsuariosDesdeCentral()
        {
            String respuesta = "";
            RoyalSync WS = new RoyalSync();
            WS.SincronizarUsuariosSistema();
            return "SINCRONIZADO OK";
        }

        public String SincronizarSalasDesdeCentral()
        {
            String respuesta = "";
            RoyalSync WS = new RoyalSync();
            WS.SincronizarSalasSistema();
            return "SINCRONIZADO OK";
        }

        public String Sincronizarteatroscentral()
        {
            String respuesta = "SINCRONIZADO OK";
            RoyalSync WS = new RoyalSync();
            while ((WS.SincronizarTeatros() == false))
            {
                WS = new RoyalSync();
                WS.SincronizarTeatros();
            }
            return respuesta;
        }

        public String SincronizacionTerceros()
        {
            String respuesta = "SINCRONIZADO OK";
            RoyalSync WS = new RoyalSync();
            while ((WS.SincronizarTerceros() == false))
            {
                WS = new RoyalSync();
                WS.SincronizarTerceros();
            }
            return respuesta;
        }

        public String SincronizacionTaquillas()
        {
            String respuesta = "SINCRONIZADO OK";
            RoyalSync WS = new RoyalSync();
            while ((WS.SincronizarTaquillas() == false))
            {
                WS = new RoyalSync();
                WS.SincronizarTaquillas();
            }
            return respuesta;
        }
    }
}
