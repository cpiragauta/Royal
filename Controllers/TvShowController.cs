using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Security.Cryptography;
using System.Text;

namespace CinemaPOS.Controllers
{
    public class TvShowController : Controller
    {
        //
        // GET: /ConveniosCupones/

        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult VistaTVShow()
        {
            ViewBag.Funcion = db.Funcion.Where(f=> f.Fecha==DateTime.Now && f.EncabezadoProgramacion.TeatroID==23);
            ViewBag.Teatros = db.Teatro.ToList();

            return View(db.Funciones);
        }

        [CheckSessionOutAttribute]
        public ActionResult VistaProgramacion()
        {
            ViewBag.Funcion = db.Funcion.Where(f=> f.Fecha==DateTime.Now && f.EncabezadoProgramacion.TeatroID==23);
            ViewBag.Teatros = db.Teatro.ToList();

            return View(db.Funciones);
        }
        

        
             
     }

}
