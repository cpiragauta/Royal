using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;

namespace CinemaPOS.Controllers.Cuenta
{
    public class CuentaController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();

        //[AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["usuario"] != null)
            {
                return RedirectToAction("Index", "Inicio");
            }
            else
            {
                Session.Clear();
                Session.RemoveAll();
            }
            //ViewBag.Message = "Your contact page.";
            return View();
        }

        [HttpPost]
        //[NoCache]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (ValidateLogin(model.NombreUsuario, model.Contraseña))
                {
                    return RedirectToAction("Index", "Inicio");
                }

                ModelState.AddModelError("", "El usuario o la contraseña ingresados son incorrectos.");
            }

            //ViewBag.Message = "Your contact page.";
            return View();
        }

        private bool ValidateLogin(string username, string passwd)
        {
            UsuarioSistema usuario = db.UsuarioSistema.FirstOrDefault(f => f.NombreUsuario == username && f.Contrasena == passwd);
            UsuarioSistema user = db.UsuarioSistema.FirstOrDefault();
            List<TipoMenu> tipoMenu;
            List<Menu> menu;

            if (usuario != null)
            {
                Session["usuario"] = usuario;
                Session["usuario_creacion"] = usuario.NombreUsuario;
                if (usuario.Rol.Nombre != "administrador")
                {
                    List<RolMenu> menuxRol = db.RolMenu.Where(f => f.Rol.RowID == usuario.Rol.RowID && f.Menu.Activo).ToList();//agregar condicion "activo" para rolmenu

                    tipoMenu = new List<TipoMenu>();
                    menu = new List<Menu>();

                    foreach (var item in menuxRol)
                    {
                        menu.Add(item.Menu);

                        if (tipoMenu.Where(f => f.RowID == item.Menu.TipoMenu.RowID).Count() == 0)
                        {
                            tipoMenu.Add(item.Menu.TipoMenu);
                        }
                    }
                }
                else
                {
                    tipoMenu = db.TipoMenu.ToList();
                    menu = db.Menu.Where(f => f.Activo == true).ToList();
                }

                Session["menu"] = menu;
                Session["tipoMenu"] = tipoMenu;
            }
            else
            {
                return false;
            }

            return true;
        }

        public ActionResult limpiarSession()
        {
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Login", "Cuenta");
        }

    }
}
