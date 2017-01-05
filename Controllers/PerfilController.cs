using CinemaPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace CinemaPOS.Controllers
{
    public class PerfilController : Controller
    {

        CinemaPOSEntities db = new CinemaPOSEntities();

        // GET: Perfil
        public ActionResult Index()
        {
            return View();
        }

        #region general
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
         
        #region Menu

        [CheckSessionOutAttribute]
        public ActionResult Menu(int? RowID_Lista)
        {
            ViewBag.Menu = db.Menu.ToList();
            ViewBag.Tipo = db.TipoMenu.Where(f=>f.Activo == true).ToList();

            if (RowID_Lista != null)
            {
                return View(db.Menu.Where(le => le.RowID == RowID_Lista).FirstOrDefault());
            }
            else
            {
                return View(new Menu());
            }
        }
        public ActionResult VistaMenu()
        {
            ViewBag.Menu = db.Menu.ToList();
            return View();
        }
        [CheckSessionOutAttribute]
        public Boolean Guardar_Menu(FormCollection formulario, int? RowID_Encabezado)
        {
            Boolean tipoAlert = true;
            try
            {
                Menu objMenu = new Menu();
                if (RowID_Encabezado == 0)
                {
                    
                    formulario = DeSerialize(formulario);
                    objMenu.Nombre = formulario["nombre"];
                    objMenu.Descripcion = formulario["descripcion"];
                    objMenu.URL = formulario["url"];
                    objMenu.TipoMenuID = Convert.ToInt32(formulario["menu"]);
                    objMenu.CreadoPor = Session["usuario_creacion"].ToString();
                    objMenu.FechaCreacion = DateTime.Now;
                    objMenu.Orden =Convert.ToInt32(formulario["orden"]);
                    objMenu.Imagen = formulario["icono"];
                    if (formulario["activo"] == null)
                    {
                        objMenu.Activo = false;
                    }
                    else
                    {
                        objMenu.Activo = true;
                    }

                    if (formulario["sincronizado"] == null)
                    {
                        objMenu.Sincronizado = false;

                    }
                    else
                    {
                        objMenu.Sincronizado = true;

                    }
                    db.Menu.Add(objMenu);
                    db.SaveChanges();
                }
                else
                {
                    objMenu = db.Menu.Where(le => le.RowID == RowID_Encabezado).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objMenu.Nombre = formulario["nombre"];
                    objMenu.Descripcion = formulario["descripcion"];
                    objMenu.URL = formulario["url"];
                    objMenu.TipoMenuID = Convert.ToInt32(formulario["menu"]);
                    objMenu.ModificadoPor = Session["usuario_creacion"].ToString();
                    objMenu.FechaModificacion = DateTime.Now;
                    objMenu.Orden = Convert.ToInt32(formulario["orden"]);
                    objMenu.Imagen = formulario["icono"];
                    if (formulario["activo"] == null)
                    {
                        objMenu.Activo = false;
                    }
                    else
                    {
                        objMenu.Activo = true;
                    }

                    if (formulario["sincronizado"] == null)
                    {
                        objMenu.Sincronizado = false;

                    }
                    else
                    {
                        objMenu.Sincronizado = true;

                    }
                    db.SaveChanges();
                }
                int codigo_encabezado = objMenu.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;
        }
        #endregion

        #region TipoMenu

        [CheckSessionOutAttribute]
        public ActionResult VistaTipoMenu()
        {
            ViewBag.TipoMenu = db.TipoMenu.ToList();
            return View();
        }
        public ActionResult TipoMenu(int? RowID_TipoMenu)
        {
            if (RowID_TipoMenu != null && RowID_TipoMenu != 0)//existe
            {
                return View(db.TipoMenu.Where(t => t.RowID == RowID_TipoMenu).FirstOrDefault());
            }
            else
            {
                return View(new TipoMenu());
            }
        }
        public Boolean Guardar_TipoMenu(FormCollection formulario, int? RowID_TipoMenu)
        {
            Boolean tipoAlert = true;
            try
            {
                TipoMenu objTipoMenu = new TipoMenu();
                if (RowID_TipoMenu == 0)
                {

                    formulario = DeSerialize(formulario);
                    objTipoMenu.Nombre = formulario["nombre"];
                    objTipoMenu.CreadoPor = Session["usuario_creacion"].ToString();
                    objTipoMenu.FechaCreacion = DateTime.Now;
                    objTipoMenu.Orden = Convert.ToInt32(formulario["orden"]);
                    if (formulario["activo"] == null)
                    {
                        objTipoMenu.Activo = false;
                    }
                    else
                    {
                        objTipoMenu.Activo = true;
                    }

                    if (formulario["sincronizado"] == null)
                    {
                        objTipoMenu.Sincronizado = false;

                    }
                    else
                    {
                        objTipoMenu.Sincronizado = true;

                    }
                    db.TipoMenu.Add(objTipoMenu);
                    db.SaveChanges();
                }
                else
                {
                    objTipoMenu = db.TipoMenu.Where(le => le.RowID == RowID_TipoMenu).FirstOrDefault();
                    formulario = DeSerialize(formulario);
                    objTipoMenu.Nombre = formulario["nombre"];
                    objTipoMenu.ModificadoPor = Session["usuario_creacion"].ToString();
                    objTipoMenu.FechaModificacion = DateTime.Now;
                    objTipoMenu.Orden = Convert.ToInt32(formulario["orden"]);
                    if (formulario["activo"] == null)
                    {
                        objTipoMenu.Activo = false;
                    }
                    else
                    {
                        objTipoMenu.Activo = true;
                    }

                    if (formulario["sincronizado"] == null)
                    {
                        objTipoMenu.Sincronizado = false;

                    }
                    else
                    {
                        objTipoMenu.Sincronizado = true;

                    }
                    db.SaveChanges();
                }
                int codigo_encabezado = objTipoMenu.RowID;
            }
            catch (Exception e)
            {
                tipoAlert = false;
            }
            return tipoAlert;
        }
        #endregion

        
    }
}