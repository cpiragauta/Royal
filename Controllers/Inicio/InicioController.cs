using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;

namespace CinemaPOS.Controllers.Inicio
{
    public class InicioController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult prueba()
        {
            return View();
        }
    }
}
