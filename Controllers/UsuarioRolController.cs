using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using CinemaPOS.Controllers;
using System.IO;

namespace CinemaPOS.Controllers
{
    public class UsuarioRolController : Controller
    {
        CinemaPOSEntities db = new CinemaPOSEntities();

        #region General
        private FormCollection DeSerialize(FormCollection formulario)
        {
            FormCollection collection = new FormCollection();
            //un-encode, and add spaces back in
            string querystring = Uri.UnescapeDataString(formulario["formulario"]).Replace("+", " ");
            var split = querystring.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (string s in split)
            {
                string text = s.Substring(0, s.IndexOf("="));
                string value = s.Substring(s.IndexOf("=") + 1);

                if (items.Keys.Contains(text))
                    items[text] = items[text] + "," + value;
                else
                    items.Add(text, value);
            }
            foreach (var i in items)
            {
                collection.Add(i.Key, i.Value);
            }
            return collection;
        }
        #endregion

        #region Usuario

       

        [CheckSessionOutAttribute]
        public ActionResult VistaUsuarioSistema()
        {
            ViewBag.Usuarios = db.UsuarioSistema.ToList();
            return View();
        }

        [CheckSessionOutAttribute]
        public ActionResult UsuarioSistema(int? RowID_UsuarioSistema)
        {
            ViewBag.Teatros = db.Teatro.ToList();
            ViewBag.Roles = db.Rol.ToList();
            ViewBag.Pantallas = db.Menu.Where(f => f.Activo == true).ToList();
            if (RowID_UsuarioSistema != null && RowID_UsuarioSistema != 0)
            {
                RowID_UsuarioSistema = int.Parse(Request.Params["RowID_UsuarioSistema"].ToString());
                return View(db.UsuarioSistema.Where(t => t.RowID == RowID_UsuarioSistema).FirstOrDefault());

            }
            else
            {
                return View(new UsuarioSistema());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarUsuarioSistema(FormCollection formulario, int? RowID_UsuarioSistema, HttpPostedFileBase foto_empleado)
        {
            Boolean respuesta = false;

            UsuarioSistema ObjUsuarioSistema = new UsuarioSistema();
            string ruta_foto = ObjUsuarioSistema.Foto_Empleado;
            String nombreU = formulario["nombreUsuario"];


            // inserta solamente el afiche en miniatura



            if (RowID_UsuarioSistema == 0)
            {
                if (db.UsuarioSistema.Where(f => f.NombreUsuario == nombreU).Count() == 0)
                {
                    ObjUsuarioSistema = CargarUsuarioSistema(ObjUsuarioSistema, formulario);
                    if (foto_empleado != null)
                    {
                        ruta_foto = formulario["nombre"] + formulario["apellido"] + Path.GetExtension(foto_empleado.FileName);
                        foto_empleado.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Fotos_Empleados/" + ruta_foto));
                        ruta_foto = "Repositorio_Imagenes/Fotos_Empleados/" + ruta_foto;
                        ObjUsuarioSistema.Foto_Empleado = ruta_foto;
                    }
                    try
                    {
                        db.UsuarioSistema.Add(ObjUsuarioSistema);
                        db.SaveChanges();
                        respuesta = true;

                    }
                    catch (Exception ex)
                    { return Json("Error " + ex.Message); }
                }
            }
            if (RowID_UsuarioSistema != 0)//Para Actualiar
            {
                ObjUsuarioSistema = db.UsuarioSistema.Where(t => t.RowID == RowID_UsuarioSistema).FirstOrDefault();
                List<UsuarioSistema> listaUsuario = db.UsuarioSistema.Where(f => f.NombreUsuario == nombreU).ToList();
                if (listaUsuario.Count() == 1 && listaUsuario.FirstOrDefault().RowID == ObjUsuarioSistema.RowID)
                {
                    ObjUsuarioSistema = CargarUsuarioSistema(ObjUsuarioSistema, formulario);
                    if (foto_empleado != null)
                    {
                        String path = "~/" + ObjUsuarioSistema.Foto_Empleado;
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(Server.MapPath(path));
                        }
                        
                        //documento.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Imagenes_Generales/" + adjunto));
                        //adjunto = "Repositorio_Imagenes/Imagenes_Generales/" + adjunto;

                        ruta_foto = formulario["nombre"] + formulario["apellido"] + Path.GetExtension(foto_empleado.FileName);
                        foto_empleado.SaveAs(Server.MapPath("~/Repositorio_Imagenes/Fotos_Empleados/" + ruta_foto));
                        ruta_foto = "Repositorio_Imagenes/Fotos_Empleados/" + ruta_foto;
                        ObjUsuarioSistema.Foto_Empleado = ruta_foto;
                    }

                    //if (foto_empleado != null)
                    //{
                    //    ObjUsuarioSistema.Foto_Empleado = ruta_foto;
                    //}
                    //else
                    //{
                    //    ObjUsuarioSistema.Foto_Empleado = ruta_foto;
                    //}

                    try
                    {
                        db.SaveChanges();
                        respuesta = true;
                    }
                    catch (Exception ex)
                    { return Json("Error " + ex.Message); }

                }

            }
            return Json(respuesta);
        }
        public UsuarioSistema CargarUsuarioSistema(UsuarioSistema ObjUsuarioSistema, FormCollection formulario)
        {
            ObjUsuarioSistema.NombreUsuario = formulario["nombreUsuario"];
            ObjUsuarioSistema.Contrasena = formulario["contrasena"];
            ObjUsuarioSistema.Nombre = formulario["nombre"];
            ObjUsuarioSistema.Apellido = formulario["apellido"];
            ObjUsuarioSistema.Correo = formulario["correo"];
            ObjUsuarioSistema.Telefono = formulario["telefono"];
            ObjUsuarioSistema.Sincronizado = false;
            ObjUsuarioSistema.RolID = int.Parse(formulario["rol"].ToString());
            ObjUsuarioSistema.TeatroID = int.Parse(formulario["teatro"].ToString());
            if (formulario["activo"] == null)
            {
                ObjUsuarioSistema.Activo = false;
            }
            else
            {
                ObjUsuarioSistema.Activo = true;
            }
            if (ObjUsuarioSistema.RowID == 0)
            {
                ObjUsuarioSistema.CreadoPor = Session["usuario_creacion"].ToString();
                ObjUsuarioSistema.FechaCreacion = DateTime.Now;
            }
            else
            {
                ObjUsuarioSistema.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjUsuarioSistema.FechaModificacion = DateTime.Now;
            }
            return ObjUsuarioSistema;
        }

        #endregion

        #region Roles

        [CheckSessionOutAttribute]
        public ActionResult VistaRolesSistema()
        {
            ViewBag.Roles = db.Rol.ToList();
            return View();
        }

        public String listaMenus(int rolID)
        {
            try
            {
                String menusA = "";
                List<RolMenu> lista = db.RolMenu.Where(p => p.RolID == rolID).ToList();
                for (int i = 0; i < lista.Count; i++)
                {
                    menusA = menusA + " - " + lista[i].Menu.Nombre;
                }

                return menusA;
            }
            catch (Exception e)
            {

            }
            return "";

        }

        [CheckSessionOutAttribute]
        public ActionResult RolesSistema(int? RowID_Roles)
        {
            if (RowID_Roles != null && RowID_Roles != 0)
            {
                RowID_Roles = int.Parse(Request.Params["RowID_Roles"].ToString());
                return View(db.Rol.Where(t => t.RowID == RowID_Roles).FirstOrDefault());
            }
            else
            {
                return View(new Rol());
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarRolesSistema(FormCollection formulario, int RowID_RolesSistema)
        {
            String respuesta = "";
            Rol ObjRolSistema = new Rol();
            formulario = DeSerialize(formulario);
            if (RowID_RolesSistema == 0)
            {

                ObjRolSistema = CargarRolSistema(ObjRolSistema, formulario);
                try
                {
                    db.Rol.Add(ObjRolSistema);
                    db.SaveChanges();
                    RowID_RolesSistema = ObjRolSistema.RowID;
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Guardado Correctamente";

            }
            else if (RowID_RolesSistema != 0)//Para Actualiar
            {
                ObjRolSistema = db.Rol.Where(t => t.RowID == RowID_RolesSistema).FirstOrDefault();
                ObjRolSistema = CargarRolSistema(ObjRolSistema, formulario);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                { return Json("Error " + ex.Message); }
                respuesta = "Actualizado Correctamente";

            }
            return Json(new { respuesta = respuesta, RolID = RowID_RolesSistema });
        }

        public Rol CargarRolSistema(Rol ObjRolSistema, FormCollection formulario)
        {
            ObjRolSistema.Nombre = formulario["RolNombre"];
            ObjRolSistema.Sincronizado = false;
            if (ObjRolSistema.RowID == 0)
            {
                ObjRolSistema.CreadoPor = Session["usuario_creacion"].ToString();
                ObjRolSistema.FechaCreacion = DateTime.Now;
            }
            else
            {
                ObjRolSistema. ModificadoPor = Session["usuario_creacion"].ToString();
                ObjRolSistema.FechaModificacion = DateTime.Now;
            }
            return ObjRolSistema;
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarPermisosRol(int RolID, String pantallasAsignadas, String pantallasNOAsignadas)
        {
            pantallasAsignadas = pantallasAsignadas.TrimEnd(',').TrimStart(',');
            pantallasNOAsignadas = pantallasNOAsignadas.TrimEnd(',').TrimStart(',');
            String[] patallasAsignadas2 = pantallasAsignadas.Split(',');
            String[] pantallasNOAsignadas2 = pantallasNOAsignadas.Split(',');
            //Para quitar los que selecciono
            for (int i = 0; i < pantallasNOAsignadas2.Length; i++)
            {
                for (int j = 0; j < patallasAsignadas2.Length; j++)
                {
                    if (pantallasNOAsignadas2[i] == patallasAsignadas2[j])
                    {
                        pantallasNOAsignadas2[i] = "0";
                        break;
                    }
                }
            }

            foreach (String PantallaID in patallasAsignadas2)
            {
                int PantallaID2 = Convert.ToInt32(PantallaID);
                RolMenu pantalla = db.RolMenu.FirstOrDefault(f => f.RolID == RolID && f.MenuID == PantallaID2);
                //Si no existe la creo
                if (pantalla == null)
                {
                    pantalla = new RolMenu();
                    pantalla.MenuID = Convert.ToInt32(PantallaID);
                    if (RolID == 0)
                    {
                        pantalla.RolID = 0;
                    }
                    else
                    {
                        pantalla.RolID = RolID;
                    }
                    pantalla.Estado = true;
                    pantalla.CreadoPor = Session["usuario_creacion"].ToString();
                    pantalla.FechaCreacion = DateTime.Now;
                    db.RolMenu.Add(pantalla);
                    db.SaveChanges();
                }
                else
                { //Si existe la actualizo
                    pantalla.RolID = RolID;
                    pantalla.Estado = true;
                    pantalla.ModificadoPor = Session["usuario_creacion"].ToString();
                    pantalla.FechaModificacion = DateTime.Now;
                    db.SaveChanges();
                }
            }
            foreach (String PantallaID in pantallasNOAsignadas2)
            {
                int PantallaID2 = Convert.ToInt32(PantallaID);
                if (PantallaID2 != 0)
                {
                    RolMenu pantalla = db.RolMenu.FirstOrDefault(f => f.RolID == RolID && f.MenuID == PantallaID2);
                    //Si existe, le cambio el estado
                    if (pantalla != null)
                    {
                        pantalla.Estado = false;
                        pantalla.ModificadoPor = Session["usuario_creacion"].ToString();
                        pantalla.FechaModificacion = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
            return Json("OK");
        }

        [CheckSessionOutAttribute]
        public String CargarPantallasAsignadas(Int32 IdRol)
        {
            String datos = "<option disabled value='' >Seleccione una Opción</option>";
            List<RolMenu> pantallasAsignadas = db.RolMenu.Where(f => f.RolID == IdRol && f.Estado == true).ToList();
            List<Menu> pantallasDisponibles = db.Menu.Where(f => f.Activo == true).ToList();
            foreach (Menu pantalla in pantallasDisponibles)
            {
                bool encontrada = false;
                foreach (RolMenu pantallaAsig in pantallasAsignadas)
                {
                    if (pantalla.RowID == pantallaAsig.MenuID)
                    { encontrada = true; }
                }
                if (encontrada)
                {
                    datos = datos + "<option value='" + pantalla.RowID + "' selected >" + pantalla.Nombre + "</option>";
                }
                else
                {
                    datos = datos + "<option value='" + pantalla.RowID + "'>" + pantalla.Nombre + "</option>";
                }
            }
            return datos;
        }

        #endregion

    }
}
