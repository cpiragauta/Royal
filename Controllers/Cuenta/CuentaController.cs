using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Net;

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

        public string get_ip_local(string ip)
        {
            string ip_return = "";
            string strHostName = string.Empty;
            // Getting Ip address of local machine…
            // First get the host name of local machine.
            // Then using host name, get the IP address list..
            string[] segmentos = ip.ToString().Split('.');
            for (int i = 0; i < segmentos.Length; i++)
            {
                ip_return += segmentos[i].PadLeft(3, '0') + ".";
            }
            ip_return = ip_return.TrimEnd('.');
            //Session["IP"] = ip_return;
            //ip_return += "Nombre de la computadora: " +strHostName;
            return ip_return;
        }

        [HttpPost]
        //[NoCache]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            
            string IP =get_ip_local(model.ip);
            
            if (ModelState.IsValid)
            {
                if (ValidateLogin(model.NombreUsuario, model.Contraseña))
                {
                    if (Session["POS"].ToString() == "ACTIVO")
                    {
                        Taquilla objtaquilla = db.Taquilla.Where(taq => taq.IP == IP).FirstOrDefault();
                        if (objtaquilla == null)/*la taquilla no esta registrada en el sistema*/
                        {
                            ModelState.AddModelError("", "La estación no esta registrada en el sistema.");
                        }
                        else
                        {
                            Session["RowID_Taquilla"] = objtaquilla.RowID.ToString();
                            return RedirectToAction("VistaPrincipal", "POS");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Inicio");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "El usuario o la contraseña ingresados son incorrectos.");
                }


            }

            //ViewBag.Message = "Your contact page.";
            return View();
        }

        private bool ValidateLogin(string username, string passwd)
        {
            Session["POS"] = "INACTIVO";
            UsuarioSistema usuario = db.UsuarioSistema.FirstOrDefault(f => f.NombreUsuario == username && f.Contrasena == passwd);
            UsuarioSistema user = db.UsuarioSistema.FirstOrDefault();
            List<TipoMenu> tipoMenu;
            List<Menu> menu;
            if (usuario != null)
            {
                Session["usuario"] = usuario;
                Session["usuario_creacion"] = usuario.NombreUsuario;
                if (usuario.Rol.Nombre.ToUpper() != "ADMINISTRADOR")
                {
                    List<RolMenu> menuxRol = db.RolMenu.Where(f => f.Rol.RowID == usuario.Rol.RowID && f.Menu.Activo == true && f.Estado == true).ToList();//agregar condicion "activo" para rolmenu

                    tipoMenu = new List<TipoMenu>();
                    menu = new List<Menu>();

                    foreach (var item in menuxRol)
                    {
                        if (item.Menu.Nombre == "POS")
                        {
                            Session["POS"] = "ACTIVO";
                        }
                        menu.Add(item.Menu);
                        if (tipoMenu.Where(f => f.RowID == item.Menu.TipoMenu.RowID).Count() == 0)
                        {
                            tipoMenu.Add(item.Menu.TipoMenu);
                        }
                    }
                }
                else
                {
                    validaPOS(username, passwd);
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
        private bool validaPOS(string User, string Pass)
        {
            bool retorna = false;
            UsuarioSistema objusuario = db.UsuarioSistema.Where(us => us.NombreUsuario == User && us.Contrasena == Pass).FirstOrDefault();
            foreach (var item in objusuario.Rol.RolMenu.Where(m => m.Rol.RowID == objusuario.RolID).ToList())
            {
                if (item.Menu.Nombre == "POS")
                {
                    retorna = true;
                }

            }
            return retorna;
        }

    }
}
