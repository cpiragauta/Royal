using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CinemaPOS.Models;
using System.Security.Cryptography;
using Microsoft.Reporting.WebForms;
using System.Text;
using iTextSharp.text.pdf;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using System.Diagnostics;
using System.Drawing.Printing;

namespace CinemaPOS.Controllers
{
    public class ConveniosCuponesController : Controller
    {
        // GET: /ConveniosCupones/

        CinemaPOSEntities db = new CinemaPOSEntities();

        [CheckSessionOutAttribute]
        public ActionResult VistaConveniosCupones()
        {
            return View(db.EncabezadoConvenio);
        }

        [CheckSessionOutAttribute]
        public ActionResult ConveniosCupones(int? RowId_Covenio)
        {
            ViewBag.TipoFormato = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOFORMATO").ToList();
            ViewBag.TipoConvenio = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCONVENIO").ToList();
            ViewBag.TipoMotivo = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOMOTIVO").ToList();
            ViewBag.TipoClasificacion = db.Opcion.Where(f => f.Tipo.Codigo == "TIPOCLASIFICACIONPELICULA").ToList();
            ViewBag.ListaTeatro = db.Teatro;
            ViewBag.Clientes = db.Tercero.Where(f => f.Activo == true && f.Opcion2.Codigo == "EMPRESA").ToList();
            ViewBag.TipoEstado = db.Estado.Where(f => f.TipoEstado.Codigo == "TIPOCONVENIO").ToList();


            ///***********Reporte de convenio *************//
            List<DetalleConvenio> detalle = new List<DetalleConvenio>();
            detalle = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowId_Covenio).ToList();
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Content/Reportes/Report1.rdlc";

            ////Parametros del reporte
            ReportParameter p1 = new ReportParameter("NombreConvenio", detalle.FirstOrDefault().EncabezadoConvenio.Nombre);
            ReportParameter p2 = new ReportParameter("FechaInicio", detalle.FirstOrDefault().EncabezadoConvenio.FechaInicio.Value.ToShortDateString());
            ReportParameter p3 = new ReportParameter("FechaFinal", detalle.FirstOrDefault().EncabezadoConvenio.FechaFinal.Value.ToShortDateString());
            ReportParameter p4 = new ReportParameter("Formato", detalle.FirstOrDefault().EncabezadoConvenio.Opcion1.Nombre);
            ReportParameter p5 = new ReportParameter("condiciones", detalle.FirstOrDefault().EncabezadoConvenio.Descripcion);


            reportViewer.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5 });
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", detalle));
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);
            ViewBag.ReportViewer = reportViewer;
            ///***********FIN reporte de convenio *************//

            if (RowId_Covenio != null && RowId_Covenio != 0)
            {
                RowId_Covenio = int.Parse(Request.Params["RowId_Covenio"].ToString());
                ViewBag.listaDetalleConvenio = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowId_Covenio).OrderBy(f => f.RowID).ToList();
                return View(db.EncabezadoConvenio.Where(t => t.RowID == RowId_Covenio).FirstOrDefault());
            }
            else
            {
                return View(new EncabezadoConvenio());
            }
        }

        public void PrintFile(int? RowId_Covenio)
        {
            try
            {
                List<DetalleConvenio> detalle = new List<DetalleConvenio>();
                detalle = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowId_Covenio).ToList();
                foreach (var det in detalle)
                {
                    LocalReport localReport = new LocalReport();
                    localReport.ReportPath = @"Content/Reportes/Report1.rdlc";

                    ////Parametros del reporte
                    ReportParameter p1 = new ReportParameter("NombreConvenio", det.EncabezadoConvenio.Nombre);
                    ReportParameter p2 = new ReportParameter("FechaInicio", det.EncabezadoConvenio.FechaInicio.Value.ToShortDateString());
                    ReportParameter p3 = new ReportParameter("FechaFinal", det.EncabezadoConvenio.FechaFinal.Value.ToShortDateString());
                    ReportParameter p4 = new ReportParameter("Formato", det.EncabezadoConvenio.Opcion1.Nombre);
                    ReportParameter p5 = new ReportParameter("condiciones", det.EncabezadoConvenio.Descripcion);
                    localReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5 });
                    localReport.DataSources.Add(new ReportDataSource("DataSet1", det));
                    string deviceInfo =
                     @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>12in</PageWidth>
                <PageHeight>25in</PageHeight>
                <MarginTop>0in</MarginTop>
                <MarginLeft>0in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0in</MarginBottom>
                </DeviceInfo>";
                    byte[] bytes1 = localReport.Render("PDF", deviceInfo);
                    var doc = new Document();
                    var reader = new PdfReader(bytes1);
                    using (FileStream fs = new FileStream(Server.MapPath("~/Content/Reportes/Convenios.pdf"), FileMode.Create))
                    {
                        PdfStamper stamper = new PdfStamper(reader, fs);
                        string Printer = "";
                        if (Printer == null || Printer == "")
                        {
                            stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                            stamper.Close();
                        }
                        else
                        {
                            stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = " + Printer + ";print(pp);\r";
                            stamper.Close();
                        }
                    }
                    reader.Close();
                    SendToPrinter();
                    System.IO.File.Delete(Server.MapPath("~/Content/Reportes/Convenios.pdf"));
                }
            }
            catch (Exception e) { }
        }


        private void SendToPrinter()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "print";
            info.FileName = @"I:/Pangea/Desktop/Proyecto con Reportes Funcionando/CinemaPOS/CinemaPOS/Content/Reportes/Convenios.pdf";
            info.CreateNoWindow = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = new Process();
            p.StartInfo = info;

            p.Start();
            p.WaitForInputIdle();
            System.Threading.Thread.Sleep(3000);
            if (false == p.CloseMainWindow())
                p.Kill();
        }

        /*    public bool PrintPDF(string ghostScriptPath, int numberOfCopies, string printerName, string pdfFileName)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = " -dPrinted -dBATCH -dNOPAUSE -dNOSAFER -q -dNumCopies=" + Convert.ToString(numberOfCopies) + " -sDEVICE=ljet4 -sOutputFile=\"\\\\spool\\%printer%EPSON TM-T88IV Receip";
                startInfo.FileName = ghostScriptPath; 
                startInfo.UseShellExecute = false;

                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;

                Process process = Process.Start(startInfo);

                Console.WriteLine(process.StandardError.ReadToEnd() + process.StandardOutput.ReadToEnd());

                process.WaitForExit(30000);
                if (process.HasExited == false) process.Kill();


                return process.ExitCode == 0;
            }*/


        private System.Drawing.Image CreateBarcode(string data)
        {

            Barcode128 code128 = new Barcode128(); // barcode type
            code128.Code = data;
            System.Drawing.Image barCode = code128.CreateDrawingImage(System.Drawing.Color.Black, System.Drawing.Color.White);
            //barCode.Save(Server.MapPath(“~/barcode.gif”), System.Drawing.Imaging.ImageFormat.Gif); //save file
            return barCode;
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        [CheckSessionOutAttribute]
        public JsonResult GuardarConvenio(FormCollection formulario, int? RowID_EncabezadoConvenio)
        {
            List<DetalleConvenio> listaDetalle = null;
            String respuesta = "";
            int RowID = 0;
            EncabezadoConvenio ObjEncabezado = new EncabezadoConvenio();
            formulario = DeSerialize(formulario);
            if (RowID_EncabezadoConvenio == 0)
            {
                ObjEncabezado = CargarDatosEncabezadoConvenio(ObjEncabezado, formulario);
                try
                {
                    db.EncabezadoConvenio.Add(ObjEncabezado);
                    db.SaveChanges();
                    respuesta = "Guardado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            else if (RowID_EncabezadoConvenio != 0)//Para Actualiar
            {
                ObjEncabezado = db.EncabezadoConvenio.Where(t => t.RowID == RowID_EncabezadoConvenio).FirstOrDefault();
                ObjEncabezado = CargarDatosEncabezadoConvenio(ObjEncabezado, formulario);
                try
                {
                    db.SaveChanges();
                    listaDetalle = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == ObjEncabezado.RowID).ToList();
                    foreach (DetalleConvenio objD in listaDetalle)
                    {
                        objD.EstadoID = ObjEncabezado.EstadoID;
                        db.SaveChanges();
                    }
                    respuesta = "Actualizado Correctamente";
                    RowID = ObjEncabezado.RowID;
                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
            }
            return Json(new { respuesta = respuesta, RowID_EncabezadoConvenio = RowID });

        }

        public JsonResult cargarItemsApe(int RowID_Encabezado)
        {
            string tabla = "";
            int? idE = 0;
            int? cantidad = 0;

            List<DetalleConvenio> listaConvenio = db.DetalleConvenio.Where(f => f.EncabezadoConvenioID == RowID_Encabezado).ToList();
            if (listaConvenio.Count != 0)
            {
                foreach (DetalleConvenio convenio in listaConvenio)
                {
                    tabla = tabla + "<tr>";
                    tabla += "<td>" + convenio.RowID + "</td>";
                    tabla += "<td>" + convenio.Nombre + "</td>";
                    tabla += "<td>" + convenio.Codigo + "</td>";
                    tabla += "<td>" + convenio.Estado.Nombre + "</td>";
                    tabla += "<td>" + convenio.Teatro.Nombre + "</td>";
                    tabla += "<td><center>" + convenio.Porcentaje + "%</center></td>";
                    tabla += "  <td><a href='javascript:EliminarItems(" + convenio.RowID + ");' class=\"ion-trash-a\"></a></td>";
                    tabla = tabla + "</tr>";
                }
                idE = listaConvenio[0].EncabezadoConvenioID;
                cantidad = (db.EncabezadoConvenio.Where(f => f.RowID == idE).FirstOrDefault().Cantidad);
            }

            var data = new { tabla = tabla, cantidad = cantidad };

            return Json(data);
        }



        public EncabezadoConvenio CargarDatosEncabezadoConvenio(EncabezadoConvenio ObjEncabezado, FormCollection formulario)
        {
            ObjEncabezado.Nombre = formulario["Nombre"];
            ObjEncabezado.FechaInicio = ModelosPropios.Util.HoraInsertar(formulario["FechaInicial"]);
            ObjEncabezado.FechaFinal = ModelosPropios.Util.HoraInsertar(formulario["FechaFinal"]);
            ObjEncabezado.EstadoID = db.Estado.FirstOrDefault().RowID; //Cambiar por el que es
            ObjEncabezado.Descripcion = formulario["descripcion"]; //Validar si toca meterla
            ObjEncabezado.TerceroID = Convert.ToInt32(formulario["Cliente"]);
            ObjEncabezado.TipoID = Convert.ToInt32(formulario["TipoConvenio"]);
            ObjEncabezado.FormatoID = Convert.ToInt32(formulario["Formato"]);
            ObjEncabezado.ClasificacionID = Convert.ToInt32(formulario["Clasificacion"]);
            ObjEncabezado.EstadoID = Convert.ToInt32(formulario["EstadoConvenio"]);

            if (ObjEncabezado.RowID == 0)
            {
                ObjEncabezado.CreadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaCreacion = DateTime.Now;
                ObjEncabezado.Cantidad = 0;
            }
            else
            {
                ObjEncabezado.ModificadoPor = Session["usuario_creacion"].ToString();
                ObjEncabezado.FechaModificacion = DateTime.Now;
            }
            return ObjEncabezado;
        }

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


        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        [CheckSessionOutAttribute]
        public JsonResult GuardarDetalleConvenio(FormCollection formulario, int RowID_EncabezadoConvenio, int Cantidad)
        {
            int con = 0;
            String respuesta = "";
            formulario = DeSerialize(formulario);
            DetalleConvenio objDetalle;
            if (RowID_EncabezadoConvenio != 0 && Cantidad != 0)
            {
                //DateTime FechaProgramacion = DateTime.Parse(formulario["FechaInicialFunciones"].ToString());
                for (int i = 0; i < Cantidad; i++)
                {
                    try
                    {
                        DetalleConvenio convenio;
                        objDetalle = new DetalleConvenio();
                        objDetalle.Nombre = formulario["NombreItem"];
                        objDetalle.TeatroAplicaID = db.Teatro.FirstOrDefault().RowID;//formulario["FechaInicialFunciones"];
                        objDetalle.Porcentaje = Convert.ToInt32(formulario["ValorItem"]);
                        objDetalle.EncabezadoConvenioID = RowID_EncabezadoConvenio;
                        objDetalle.EstadoID = db.EncabezadoConvenio.Where(f => f.RowID == RowID_EncabezadoConvenio).FirstOrDefault().EstadoID;
                        objDetalle.CreadoPor = Session["usuario_creacion"].ToString();
                        objDetalle.FechaCreacion = DateTime.Now;
                        convenio = db.DetalleConvenio.Add(objDetalle);
                        db.SaveChanges();
                        convenio.Codigo = Hash(RowID_EncabezadoConvenio + "-" + convenio.RowID + "-" + convenio.FechaCreacion.Value.ToString("dd/MM/yyyy"));
                        con++;


                    }
                    catch (Exception ex)
                    { return Json(new { respuesta = "Error " + ex.Message }); }
                    respuesta = "Guardado Correctamente";
                }
                EncabezadoConvenio enca;
                enca = db.EncabezadoConvenio.Where(f => f.RowID == RowID_EncabezadoConvenio).FirstOrDefault();
                enca.Cantidad = con;
                db.SaveChanges();

            }

            return Json(respuesta);
        }

        [CheckSessionOutAttribute]
        public JsonResult EliminarItems(int RowID_Detalle)
        {
            String respuesta = "";
            if (RowID_Detalle != 0)
            {
                try
                {
                    DetalleConvenio convenio;
                    EncabezadoConvenio encabezado;
                    convenio = db.DetalleConvenio.Where(f => f.RowID == RowID_Detalle).FirstOrDefault();
                    encabezado = db.EncabezadoConvenio.Where(f => f.RowID == convenio.EncabezadoConvenioID).FirstOrDefault();
                    encabezado.Cantidad = (encabezado.Cantidad - 1);
                    db.DetalleConvenio.Remove(convenio);
                    db.SaveChanges();

                }
                catch (Exception ex)
                { return Json(new { respuesta = "Error " + ex.Message }); }
                respuesta = "Guardado Correctamente";
            }
            return Json(respuesta);

        }



    }

}
