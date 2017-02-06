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

        [CheckSessionOutAttribute]
        public ActionResult VistaPrincipal()
        {
            ViewBag.ListaTeatro = db.Teatro.Where(f => f.Estado.Codigo == "ABIERTO").ToList();
            List<HistoricolLog> Lista = db.HistoricolLog.ToList(); 
            if (Lista.Count != 0) //Si no tiene registros no seteo el viewBag para que no muestre la tabla
            {
                ViewBag.listaHistorioco = db.HistoricolLog.ToList();
            }
            return View();
        }
        public string SincronizarTerceros(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            ws.SincronizarTerceros(RowIdTeatro);
            return "Sincronizado";
        }
        public string SincronizarOpciones(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            if (String.IsNullOrEmpty(ws.ValidarConexion(RowIdTeatro)))
            {
                return "Conexion Fallida";
            }
            else
            {
                ws.SincronizarOpciones(RowIdTeatro);
            }
            return "Sincronizado";
        }
        public string SincronizarEstados(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            ws.SincronizarEstado(RowIdTeatro);
            return "Sincronizado";
        }
        public string SincronizarCiudades(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            ws.SincronizarCiudad(RowIdTeatro);
            return "Sincronizado";
        }
        public string SincronizarUsuarios(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            ws.SincronizarUsuarios(RowIdTeatro);
            return "Sincronizado";
        }
        public string SincronizarTeatros(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            ws.SincronizarTeatros(RowIdTeatro);
            return "Sincronizado";
        }
        public string SincronizarListas(int RowIdTeatro)
        {
            WS_RoyalSinc ws = new WS_RoyalSinc();
            if (String.IsNullOrEmpty(ws.ValidarConexion(RowIdTeatro)))
            {
                return "Conexion Fallida";
            }
            else
            {
                ws.SincronizarListaEncabezado(RowIdTeatro);
            }
            return "Sincronizado";
        }
    }
}
