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
        //
        // GET: /Sincronizacion/

        CinemaPOSEntities db = new CinemaPOSEntities();

        public ActionResult VistaPrincipal()
        {
            return View(db.SincronizacionMaestros.ToList());
        }

        [CheckSessionOutAttribute]
        public String SincronizarUsuariosDesdeCentral()
        {
            String respuesta = "";
            RoyalSync WS = new RoyalSync();
            WS.SincronizarUsuariosSistema();
            return "SINCRONIZADO OK";
        }

    }
}
