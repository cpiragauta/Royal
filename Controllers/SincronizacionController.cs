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
            ViewBag.ListaTeatros = db.Teatro.Where(f=> f.Estado.Nombre == "Abierto").ToList();
            return View(db.SincronizacionMaestros.ToList());
        }



        public ActionResult SincronizacionMaestros(int RowID_Teatro)
        {
            ViewBag.Teatro = db.Teatro.FirstOrDefault(f => f.RowID == RowID_Teatro);
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

          [CheckSessionOutAttribute]
        public String SincronizarRolesDesdeCentral()
        {
            RoyalSync WS = new RoyalSync();
           // WS.SincronizarUsuariosSistema();
            return "SINCRONIZADO OK";
        }
        

        public String SincronizarSalasDesdeCentral()
        {
            String respuesta = "";
            RoyalSync WS = new RoyalSync();
            //WS.SincronizarSalasSistema();
            return "SINCRONIZADO OK";
        }

        public String Sincronizarteatroscentral()
        {
            RoyalSync WS = new RoyalSync();
            WS.SincronizarTeatros();
            return "SINCRONIZADO OK";
        }

        public String SincronizarTerceros()
        {
            String respuesta = "";
            RoyalSync WS = new RoyalSync();
            WS.SincronizarTerceros();
            return "SINCRONIZADO OK";
        }
    }
}
