using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;


namespace CinemaPOS.Controllers
{
    public class CheckSessionOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

            HttpSessionStateBase session = filterContext.HttpContext.Session;
            UsuarioSistema user = (UsuarioSistema)session["Usuario"];

            //VALIDACIONES DE LOGIN
            
                if ((user == null) || (session.IsNewSession))
                {
                    //send them off to the login page
                    var url = new UrlHelper(filterContext.RequestContext);
                    var loginUrl = url.Content("~/Cuenta/Login");
                    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                }
                else
                {
                    //List<RolMenu> PantallasAutorizadas = user.Rol.RolMenu.ToList();
                    //String PantallaSolicitada = "/" + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "/" + filterContext.ActionDescriptor.ActionName;
                    //bool acceso = false;
                    //if (PantallaSolicitada.Contains("Index")) //Si apunta al index,
                    //{ return; }
                    //if (session["ValidarPantalla"] == null)
                    //{
                    //    foreach (RolMenu item in PantallasAutorizadas)
                    //    {
                    //        if (item.Menu.URL == PantallaSolicitada && item.Menu.Activo == true)
                    //        {
                    //            session["ValidarPantalla"] = "NO"; //Para que no valide la proxima pantalla a la que va a ingresar
                    //            acceso = true;
                    //            return;
                    //        }
                    //    }
                    //    if (acceso == false)
                    //    {
                    //        var url = new UrlHelper(filterContext.RequestContext);
                    //        var loginUrl = url.Content("~/Inicio/Index");
                    //        filterContext.HttpContext.Response.Redirect(loginUrl, true);
                    //    }
                    //}
                    //else
                    //{
                    //    session["ValidarPantalla"] = null;
                    //    return;
                    //}
                    
                //else if (user.ind_cambiarclave == 1)
                //{
                //    var url = new UrlHelper(filterContext.RequestContext);
                //    var loginUrl = url.Content("~/Account/CambiarClave");
                //    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                //}
                //else if (user.acepto_condiciones == null)
                //{
                //    var url = new UrlHelper(filterContext.RequestContext);
                //    var loginUrl = url.Content("~/Account/AceptarAcuerdo");
                //    filterContext.HttpContext.Response.Redirect(loginUrl, true);
                //}
            }


        }
    }

}