using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using FINADICore.Controllers;
using System.Threading.Tasks;
using FINADICore.Services;
using DAL.DTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using DAL.Models.Core;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class BandejaDeAprobacionController : FINADICoreController
    {
        public BandejaDeAprobacionController(FINADIContext context) : base(context)
        {
        }
        public ActionResult Index()
        {
            ViewBag.Desde = DateTime.Today.AddDays(DateTime.Today.Day * -1);
            ViewBag.Hasta = DateTime.Now;
            //ViewBag.Estados = _context.EstadosPrestamos.Select(g => new SelectListItem() { Text = "Ver Solo " + g.Nombre, Value = g.Id.ToString() });
            var Estados = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Todos", Value = "Null" }
            };
            Estados.AddRange(_context.EstadosPrestamos.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Nombre }));
            ViewBag.Estados = Estados;
            ViewBag.Ampliacion = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Todos", Value = "0" },
                new SelectListItem() { Text = "Si", Value = "1" },
                new SelectListItem() { Text = "No", Value = "2" }
            };

            return View();
        }

        public IActionResult PrestamosEntidadDataTable(int EstadoId, DateTime? Desde, DateTime? Hasta)
        {
            //var prestamos2 = _context.Prestamos.ToList();
            //var prestamos = _context.Prestamos.Where(x => x.PrestamoCGEId==0)
            //
            if (Desde is null)
                Desde = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-01-01");
            if (Hasta is null)
                Hasta = DateTime.Now;

            var prestamos = _context.Prestamos.Where(x => x.Cliente.Persona.TipoPersona.Organismo.Id!=1 
                                                    && x.Cliente.Persona.TipoPersona.Organismo.Activo == true 
                                                    && Convert.ToDateTime(x.FechaSolicitado).Date >= Desde
                                                    && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta)
                .OrderBy(x => x.Cliente.Persona.Apellido)
                .Select(x => new BandejaDeAprobacionPrestamoDTO
                {
                    MontoCuota = x.MontoCuota,
                    Estado = x.EstadoActual.Nombre, //+ " " + (x.ObservacionesAnulacion ?? ""),
                    Fecha =  (x.FechaSolicitado ?? DateTime.Now),   //Arreglar fecha permir nulll
                    Id = x.Id,
                    MontoPrestado = x.Capital,
                    Saldo = x.Saldo,
                    Apellido=x.Cliente.Persona.Apellido,
                    Nombre=x.Cliente.Persona.Nombres,
                    DNI=x.Cliente.Persona.NroDocumento.ToString(),
                    ClienteId=x.Cliente.Id,
                    EstadoPrestamoId = x.EstadoActual.Id,
                    
                });
            return DataTable(prestamos);
        }
        public IActionResult _DescargaExcel(DateTime? Desde, DateTime? Hasta)
        {
            if (Desde is null)
                Desde = Convert.ToDateTime(DateTime.Now.Year.ToString() + "-01-01");
            if (Hasta is null)
                Hasta = DateTime.Now;

            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaPrestamosApp.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                decimal total = 0;
                var workSheet = package.Workbook.Worksheets[1];
                //var renglones = _context.Prestamos.Where(x => usuario.Administradores == true && Convert.ToDateTime(x.FechaSolicitado).Date >= Desde && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta && x.FechaAnulacion == null && (x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null)).Select(x => new PrestamosReportesDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Celular = x.Cliente.Celular, eMail = x.Cliente.Usuario.UserName });
                var renglones = _context.Prestamos.Where(x=> Convert.ToDateTime(x.FechaSolicitado).Date >= Desde && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta && x.FechaAnulacion == null && (x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null)).Select(x => new PrestamosReportesDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Celular = x.Cliente.Celular, eMail = x.Cliente.Usuario.UserName });
                int linea = 2;
                foreach (var renglon in renglones)
                {
                    workSheet.Cells[linea, 1].Value = renglon.Fecha;
                    workSheet.Cells[linea, 2].Value = renglon.Cliente;
                    workSheet.Cells[linea, 3].Value = renglon.DNI;
                    workSheet.Cells[linea, 4].Value = renglon.eMail;
                    workSheet.Cells[linea, 5].Value = renglon.Celular;
                    workSheet.Cells[linea, 6].Value = renglon.Tipo;
                    workSheet.Cells[linea, 7].Value = renglon.Capital;
                    workSheet.Cells[linea, 8].Value = renglon.CantidadCuotas;
                    workSheet.Cells[linea, 9].Value = renglon.MontoCuota;
                    workSheet.Cells[linea, 10].Value = renglon.Estado;
                    linea = linea + 1;
                    total = total + renglon.Capital;
                }
                workSheet.Cells[linea, 4].Value = "Cantidad:";
                workSheet.Cells[linea, 5].Value = linea - 2;
                workSheet.Cells[linea, 6].Value = "Total del Periodo:";
                workSheet.Cells[linea, 7].Value = total;
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos App Del " + Convert.ToDateTime(Desde).ToString("dd_MM_yyyy") + " al " + Convert.ToDateTime(Hasta).ToString("dd_MM_yyyy") + ".xlsx");
            }
        }

        public IActionResult _LegajoOnlineInae(int id)
        {
            Prestamos prestamo = _context.Prestamos.Where(x => x.Id == id).FirstOrDefault();
            prestamo.MontoCuotaEnLetras = "PESOS " + common.NumeroALetrasNew(prestamo.MontoCuota);
            prestamo.CuotasEnLetras = common.NumeroALetrasNew(Convert.ToDecimal(prestamo.CantidadCuotas));
            prestamo.CapitalEnLetras = "PESOS " + common.NumeroALetrasNew(prestamo.Capital);
            prestamo.Pagare = (prestamo.CantidadCuotas * prestamo.MontoCuota);
            prestamo.PagareEnLetras = common.NumeroALetrasNew(prestamo.CantidadCuotas * prestamo.MontoCuota);
            _context.Prestamos.Update(prestamo);
            _context.SaveChanges();
            string parametros = "&id=" + prestamo.Id.ToString();
            @ViewBag.PDF = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_inaes", parametros, "PDF", _context));

            return PartialView();
        }
        public IActionResult _LegajoMasVenta(int id)
        {
            Prestamos prestamo = _context.Prestamos.Where(x => x.Id == id).FirstOrDefault();
            prestamo.MontoCuotaEnLetras = "PESOS " + common.NumeroALetrasNew(prestamo.MontoCuota);
            prestamo.CuotasEnLetras = common.NumeroALetrasNew(Convert.ToDecimal(prestamo.CantidadCuotas));
            prestamo.CapitalEnLetras = "PESOS " + common.NumeroALetrasNew(prestamo.Capital);
            prestamo.Pagare = (prestamo.CantidadCuotas * prestamo.MontoCuota);
            prestamo.PagareEnLetras = common.NumeroALetrasNew(prestamo.CantidadCuotas * prestamo.MontoCuota);
            _context.Prestamos.Update(prestamo);
            _context.SaveChanges();
            string parametros = "&id=" + prestamo.Id.ToString();
            string nombreReporte;
            switch (prestamo.Inversor.Id)
            {
                case 1:
                    nombreReporte = "reporte_inversor_comercio";
                    break;
                case 2:
                    nombreReporte = "reporte_inversor_masventas";
                    break;
                case 3:
                    nombreReporte = "reporte_inversor_afidanza";
                    break;
                case 4:
                    nombreReporte = "reporte_inversor_onoyen";
                    break;
                case 5:
                    nombreReporte = "reporte_inversor_escobar";
                    break;
                case 6:
                    nombreReporte = "reporte_inversor_fullcredit";
                    break;
                default:
                    nombreReporte = "Sin Inversor";
                    break;

            }

            @ViewBag.PDF = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting(nombreReporte, parametros, "PDF", _context));

            return PartialView();
        }

        [HttpGet]
        public IActionResult _AsignarInversor(int Id)
        {
            InversorViewBag(Id);
            Prestamos inversorPrestamo = _context.Prestamos.Where(x => x.Id == Id).FirstOrDefault();
            return PartialView(inversorPrestamo.Inversor);
        }


        [HttpPost]
        public async Task<IActionResult> _AsignarInversor(int PrestamoId, Inversor InversorId)
        {
            try
            {
                Prestamos prestamo = _context.Prestamos.Where(x => x.Id == PrestamoId).FirstOrDefault();
                Inversor inversorPrestamo = _context.Inversores.Where(x => x.Id == InversorId.Id).FirstOrDefault();

                decimal tasa = inversorPrestamo.TasaActual.Tasa / 100;
                if (inversorPrestamo.TasaActual != null)
                    prestamo.TasaInversor = inversorPrestamo.TasaActual.Id;
                if (inversorPrestamo.TEA != null)
                    prestamo.TEA = inversorPrestamo.TEA.Id;
                if (inversorPrestamo.TNA != null)
                    prestamo.TNA = inversorPrestamo.TNA.Id;
                if (inversorPrestamo.CFTSinImpuesto != null)
                    prestamo.CFTSinImpuesto = inversorPrestamo.CFTSinImpuesto.Id;
                if (inversorPrestamo.CFTConImpuesto != null)
                    prestamo.CFTConImpuesto = inversorPrestamo.CFTConImpuesto.Id;
                if (inversorPrestamo.TEM != null)
                    prestamo.TEM = inversorPrestamo.TEM.Id;

                decimal calculoTEM = (decimal)Math.Pow((double)(1 + tasa), prestamo.CantidadCuotas);
                //decimal capitalInversor = prestamo.MontoCuota * tasa;
                //decimal capitalCuota = prestamo.MontoCuota * (1 - tasa);
                prestamo.TotalCapitalInversor = prestamo.MontoCuota * (calculoTEM - 1) / (tasa * calculoTEM);
                decimal decimalValue = decimal.Round(prestamo.TotalCapitalInversor, 2);
                prestamo.CapitalInversorEnLetras = common.NumeroALetrasNew(decimalValue);
                prestamo.Inversor = inversorPrestamo;

                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
            return RedirectToAction("Index", "PrestamosApp");
        }
        private void InversorViewBag(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Inversores = _context.Inversores.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }


        [HttpGet]
        public IActionResult _ApruebaPrestamoModal(int id)
        {
            ViewBag.Texto = "¿Está Seguro que desea Aprobar el Prestamo?";
            ViewBag.PrestamoId = id;
            ViewBag.Action = "_ApruebaPrestamo";
            return PartialView("_Alert");
        }


        [HttpPost]
        public IActionResult _ApruebaPrestamo(int id)
        {
            try
            {
                var prestamo = _context.Prestamos.Find(id);
                Usuario user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).FirstOrDefault();
                EstadosPrestamos estadoPrestamo = _context.EstadosPrestamos.Find(6); //Confirma persona
                if (user != null)
                {
                    prestamo.AprobadoPor = user;
                    prestamo.EstadoActual = estadoPrestamo;
                    prestamo.FechaAprobacion = DateTime.Now;
                    _context.Prestamos.Update(prestamo);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, $"Se Aprobó el Prestamo Correctamente de {prestamo.Cliente.Persona.GetNombreCompleto()}.");
                    
                    //Envio mail de Aprobación al cliente
                    string html = "";

                    html = "<br/>Estimado: " + prestamo.Cliente.Persona.Apellido + ", " + prestamo.Cliente.Persona.Nombres + "<br/><br/>";
                    html += "Nos Agrada Comunicarle que Su Solicitud de Descuento: " + prestamo.Id + ", ha sido <b>APROBADA</b> por la Entidad <br/><br/>";
                    html += "<b>Importe Solicitado:</b>" + prestamo.Capital + "<br/>";
                    html += "<b>Cantidad de Cuotas:</b>" + prestamo.CantidadCuotas + "<br/>";
                    html += "<b>Couta Mensual:</b>" + prestamo.MontoCuota + "<br/><br/>";
                    html += "Sin Otro Particular Saludamos a Ud. Muy Atentamente<br/>";

                    string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Aprobado.png><br/>";

                    common.EnviarMail(prestamo.Cliente.Usuario.Email.Trim(), "Solicitud de Descuento FINADI", html, "", imagen);

                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Error al Aprobar el Prestamo. Intentelo más tarde.");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult _AnulaPrestamoModal(int id)
        {
            ViewBag.Texto = "¿Está Seguro que desea Anular el Prestamo?";
            ViewBag.PrestamoId = id;
            ViewBag.Action = "_AnulaPrestamo";
            return PartialView("_Alert");
        }

        [HttpPost]
        public IActionResult _AnulaPrestamo(int id)
        {
            try
            {
                var prestamo = _context.Prestamos.Find(id);
                Usuario user = _context.Users.Where(x => x.UserName == this.User.Identity.Name).FirstOrDefault();
                EstadosPrestamos estadoPrestamo = _context.EstadosPrestamos.Find(9);//Aprobado Por Administración
                if (user != null)
                {
                    prestamo.AprobadoPor = user;
                    prestamo.EstadoActual = estadoPrestamo;
                    prestamo.FechaAnulacion = DateTime.Now;
                    prestamo.EstadoActual = estadoPrestamo;
                    prestamo.ObservacionesAnulacion = "Rechazado Por Administración";
                    _context.Prestamos.Update(prestamo);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, $"Se Anuló el Prestamo Correctamente de {prestamo.Cliente.Persona.GetNombreCompleto()}.");
                    //Envio mail de Anulación al cliente
                    string html = "";

                    html = "<br/>Estimado: " + prestamo.Cliente.Persona.Apellido + ", " + prestamo.Cliente.Persona.Nombres + "<br/><br/>";
                    html += "Su Solicitud de Descuento: " + prestamo.Id + ", ha sido <b>RECHAZADA</b> por la Entidad <br/><br/>";
                    html += "<b>Importe Solicitado:</b>" + prestamo.Capital + "<br/>";
                    html += "<b>Cantidad de Cuotas:</b>" + prestamo.CantidadCuotas + "<br/>";
                    html += "<b>Couta Mensual:</b>" + prestamo.MontoCuota + "<br/><br/>";
                    html += "Sin Otro Particular Saludamos a Ud. Muy Atentamente<br/>";

                    string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Rechazo.png><br/>";

                    common.EnviarMail(prestamo.Cliente.Usuario.Email.Trim(), "Solicitud de Descuento FINADI", html, "", imagen);


                }
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Error al Anular el Prestamo. Intentelo más tarde.");
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult _VerDatosPersona(int id)
        {
            Clientes persona = _context.Clientes.FirstOrDefault(x => x.Id == id);
            common common = new common();
            if (persona.Persona.Foto != null)
                ViewBag.Foto = common.ByteaImage(persona.Persona.Foto);
            else
                ViewBag.Foto = false;
            return PartialView(persona);
        }

        [HttpGet]
        public ActionResult _EditarPrestamo(int id)
        {
            Prestamos prestamo = _context.Prestamos.FirstOrDefault(x => x.Id == id);
            ViewBag.Total = prestamo.CantidadCuotas * prestamo.MontoCuota;
            ViewBag.TipoPersonaId = prestamo.Cliente.Persona.TipoPersona.Id;
            ViewBag.NombreCompleto = $"{prestamo.Cliente.Persona.GetNombreCompleto()} ({prestamo.Cliente.Persona.NroDocumento})";
            ViewBag.reload = false;
            ViewBag.LineaPrestamo = _context.LineasPrestamos.Where(x=>x.FechaBaja == null && _context.LineasPrestamosTiposPersonas.Count(y=>y.TipoPersona.Id == prestamo.Cliente.Persona.TipoPersona.Id && x.Id == y.LineaPrestamo.Id)>0 ).Select(g => new SelectListItem() { Text = g.Nombre+" - (Monto: "+g.CapitalMinimo+" - "+g.CapitalMaximo+")", Value = g.Id.ToString() });
            return PartialView(prestamo);
        }

        [HttpPost]
        public ActionResult EditarPrestamo(int LineaPlanPrestamoId, Prestamos prestamo, string montoPrestamo, int cuotaPrestamo, string valorCuotaPrestamo)
        {
            try
            {
                //decimal valorCuotaPrestamoTran = Convert.ToDecimal(valorCuotaPrestamo.Replace(".", ","));
                decimal valorCuotaPrestamoTran = Convert.ToDecimal(valorCuotaPrestamo);
                Prestamos prestamoUpdate = _context.Prestamos.Find(prestamo.Id);
                if (valorCuotaPrestamo.ToString() != "0" && montoPrestamo != "0" && cuotaPrestamo != 0)
                {
                    LineasPrestamosPlanes lineaPlanPrestamo = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Id == LineaPlanPrestamoId);
                    prestamoUpdate.Capital = Convert.ToDecimal(montoPrestamo);
                    prestamoUpdate.CantidadCuotas = cuotaPrestamo;
                    prestamoUpdate.MontoCuota = Convert.ToDecimal(valorCuotaPrestamoTran);
                    prestamoUpdate.CapitalEnLetras = common.NumeroALetras(Convert.ToDouble(montoPrestamo));
                    prestamoUpdate.MontoCuotaEnLetras = common.NumeroALetrasNew(valorCuotaPrestamoTran);
                    prestamoUpdate.CuotasEnLetras = common.NumeroALetras(Convert.ToDouble(cuotaPrestamo));
                    _context.Prestamos.Update(prestamoUpdate);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se Modificó correctamente la solicitud de Prestamo.");
                    ViewBag.reload = true;

                    //Envio mail de Modificación al cliente
                    string html = "";

                    html = "<br/>Estimado: " + prestamoUpdate.Cliente.Persona.Apellido + ", " + prestamoUpdate.Cliente.Persona.Nombres + "<br/><br/>";
                    html += "Su Solicitud de Descuento: " + prestamoUpdate.Id + ", ha sido <b>MODIFICADA</b> por la Entidad <br/><br/>";
                    html += "<b>Importe Solicitado:</b>" + prestamoUpdate.Capital + "<br/>";
                    html += "<b>Cantidad de Cuotas:</b>" + prestamoUpdate.CantidadCuotas + "<br/>";
                    html += "<b>Couta Mensual:</b>" + prestamoUpdate.MontoCuota + "<br/><br/>";
                    html += "Sin Otro Particular Saludamos a Ud. Muy Atentamente<br/>";

                    string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Modificado.png><br/>";

                    common.EnviarMail(prestamoUpdate.Cliente.Usuario.Email.Trim(), "Solicitud de Descuento FINADI", html, "",imagen);

                    return PartialView("_EditarPrestamo", prestamo);
                }
                else
                {
                    ViewBag.reload = false;
                    ViewBag.NombreCompleto = $"{prestamoUpdate.Cliente.Persona.GetNombreCompleto()} ({prestamoUpdate.Cliente.Persona.NroDocumento})";
                    ModelState.AddModelError("MontoCuota", "Debe Elegir los Nuevos Valores.");
                    return PartialView("_EditarPrestamo", prestamo);
                }

            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la solicitud de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public ActionResult _SueldoNetoPrimerVencimiento(int id)
        {
            Prestamos prestamo = _context.Prestamos.FirstOrDefault(x => x.Id == id);
            return PartialView(prestamo);
        }

        [HttpPost]
        public ActionResult _SueldoNetoPrimerVencimiento(Prestamos prestamo)
        {
            try
            {
                Prestamos prestamoUpdate = _context.Prestamos.Find(prestamo.Id);

                prestamoUpdate.SueldoNetoMensual = prestamo.SueldoNetoMensual;
                prestamoUpdate.FechaPrimerVencimiento = prestamo.FechaPrimerVencimiento;
                _context.Prestamos.Update(prestamoUpdate);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se Modificó correctamente el Prestamo.");
                return RedirectToAction("Index");


            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la solicitud de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index");
            }

        }

        public string PlanesLineasPrestamoCombo(int LinePrestamoId, string q)
        {

            var lineaPlanPrestamo = _context.LineasPrestamosPlanes
                .Where(x => x.Linea.Id == Convert.ToInt32(LinePrestamoId)).Where(x => x.MontoPrestado <= Convert.ToDecimal(q)).OrderByDescending(x => x.MontoPrestado).Select(x => x.MontoPrestado).ToList().Distinct().Take(20);


            string option = "<option value='0' disabled='disabled' selected>Seleccione Cuota</option>";
            foreach (var item in lineaPlanPrestamo)
            {
                option += $"<option value='{Convert.ToInt32(item)}'><span class=\"glyphicon fa fa-money\"></span> <i class=\"bi bi-arrow-down-circle-fill\"></i>{Convert.ToInt32(item)}</option>";
            }
            return option;
        }


        public string ComboCuotaPrestamo(int LinePrestamoId, decimal monto)
        {

            var cantidadCuotas = _context.LineasPrestamosPlanes.Where(x => LinePrestamoId == x.Linea.Id && x.MontoPrestado == monto && x.Activo == true).OrderBy(x => x.CantidadCuotas).Select(x => x.CantidadCuotas).ToList().Distinct();
            string option = "<option value='0' disabled='disabled' selected>Seleccione Cuota</option>";
            foreach (var item in cantidadCuotas)
            {
                option += $"<option value='{item}'>{item}</option>";
            }
            return option;
        }

        public decimal ComboValorCuotaPrestamo(int cuotas, decimal monto, int LinePrestamoId)
        {

            var cantidadCuotas = _context.LineasPrestamosPlanes.Where(x => LinePrestamoId == x.Linea.Id && x.MontoPrestado == monto && x.CantidadCuotas == cuotas && x.Activo == true).OrderBy(x => x.MontoCuota).Select(x => x.MontoCuota).FirstOrDefault();
            return cantidadCuotas;
        }

        public ActionResult _Adjunto(int Id)
        {
            ViewBag.PrestamoId = Id;
            ViewBag.CargarAdjunto = false;
            Prestamos prestamo = _context.Prestamos.Find(Id);
            // Estados de Prestamos
            // 2 -> Esperando Transferencia /-/ 8-> Cerrar Prestamo
            if (prestamo.EstadoActual.Id == 2 || prestamo.EstadoActual.Id == 8)
            {
                ViewBag.CargarAdjunto = true;
            }

            if (prestamo.AdjuntoTransferencia != null)
            {
                ViewBag.AdjuntoTransferencia = Convert.ToBase64String(prestamo.AdjuntoTransferencia);
                ViewBag.ExtensionAdjuntoTransferencia = prestamo.ExtensionAdjuntoTransferencia;
            }
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Adjunto(int Id, IFormFile AdjuntoPrestamo)
        {
            try
            {
                Prestamos prestamo = _context.Prestamos.Find(Id);
                if (AdjuntoPrestamo != null)
                {
                    int index = AdjuntoPrestamo.FileName.LastIndexOf('.');
                    string fileExtension = AdjuntoPrestamo.FileName.Substring(index + 1);
                    using (var memoryStream = new MemoryStream())
                    {
                        await AdjuntoPrestamo.CopyToAsync(memoryStream);
                        prestamo.AdjuntoTransferencia = memoryStream.ToArray();
                        prestamo.ExtensionAdjuntoTransferencia = fileExtension;
                        prestamo.EstadoActual = _context.EstadosPrestamos.Find(7); //Estado Prestamo Transferido.
                    }
                }

                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");
            }
        }



        [HttpGet]
        public ActionResult _NuevoPrestamoOtroOrg()
        {
            return PartialView();
        }

        public JsonResult ClientePrestamoCombo(string q)
        {
            var ListClientes = _context.Clientes.Where(x => x.Persona.NroDocumento==q).ToList();

            var Clientes = ListClientes.Select(x => new
            {
                Text = $"{x.Persona.Apellido} {x.Persona.Nombres} {x.Persona.NroDocumento} ({x.Persona.TipoPersona.nombre} - {x.Persona.TipoPersona.Organismo.Descripcion})",
                Value = x.Id,
                Icon = "fa fa-user"
            }).Take(20).ToArray();
            return Json(Clientes);
        }

        [HttpGet]
        public int TipoOrganismoAjax(int clienteId)
        {
            Clientes cliente = _context.Clientes.FirstOrDefault(x => x.Id == clienteId);
            return cliente.Persona.TipoPersona.Id;
        }

        [HttpGet]
        public ActionResult _SolicitarPrestamo(int TipoPersonaId)
        {
            ViewBag.TipoPersonaId=TipoPersonaId;
            return PartialView("_SelectPrestamo");
        }

        [HttpPost]
        public ActionResult NuevoPrestamoOtroOrganismo(int LineaPlanPrestamoId, string montoPrestamo, int cuotaPrestamo, decimal valorCuotaPrestamo, int client)
        {
            try
            {
                Prestamos prestamo = new Prestamos();
                if (valorCuotaPrestamo.ToString() != "0" && montoPrestamo != "0" && cuotaPrestamo != 0)
                {
                    prestamo.Cliente = _context.Clientes.Find(client);
                    LineasPrestamosPlanes lineaPlanPrestamo = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Id == LineaPlanPrestamoId);
                    prestamo.Capital = Convert.ToDecimal(montoPrestamo);
                    prestamo.CantidadCuotas = cuotaPrestamo;
                    prestamo.MontoCuota = Convert.ToDecimal(valorCuotaPrestamo);
                    prestamo.CapitalEnLetras = common.NumeroALetras(Convert.ToDouble(montoPrestamo));
                    prestamo.MontoCuotaEnLetras = common.NumeroALetrasNew(valorCuotaPrestamo);
                    prestamo.CuotasEnLetras = common.NumeroALetras(Convert.ToDouble(cuotaPrestamo));
                    prestamo.EstadoActual = _context.EstadosPrestamos.Find(6); //Confirmar por Persona
                    //_context.Prestamos.Add(prestamo);
                    //_context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se Generó correctamente la solicitud de Prestamo.");
                    ViewBag.reload = true;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.reload = false;
                    ModelState.AddModelError("MontoCuota", "Debe Elegir los Nuevos Valores.");
                    return PartialView("_SolicitarPrestamo", prestamo);
                }

            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al Generar la solicitud de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public ActionResult _EnviarNotificacionPrestamoDTO(string id)
        {
            try
            {
                var index = id.IndexOf(",");
                var dni = id.Substring(0, index);
                var idPrestamo = id.Substring(index + 1, (id.Length - index)-1);


                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == dni).FirstOrDefault();
                ViewBag.PrestamoId = idPrestamo;
                return PartialView("_EnviarNotificacionPrestamo", cliente);
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo del DNI " + id);
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        public ActionResult _EnviarNotificacionPrestamo(Clientes client, string PrestamoId)
        {
            try
            {
                //usuario Login
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);

                //Cliente base de datos Local
                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == client.Persona.NroDocumento).FirstOrDefault();

                string destinatario = cliente.Usuario.UserName;
                //string destinatario = "acevedoruben@hotmail.com";                
                if (destinatario != null && cliente.Usuario.RecordarPassword)
                {
                    string pass = common.Encrypt(cliente.Persona.NroDocumento.ToString() + DateTime.Now.ToString(), "FINADI");
                    cliente.Password = pass;
                    cliente.Usuario.RecordarPassword = true;
                    _context.Clientes.Update(cliente);
                    _context.SaveChanges();
                    string userName = this.User.Identity.Name;
                    string titulo = "FINADI - Terminá tu Préstamo Aprobado descargando nuestra App";
                    string texto = $"Estimado: {cliente.Persona.Apellido},{cliente.Persona.Nombres} , para poder Iniciar en la App debes registrar tu contraseña <a href = 'http://emutual.com.ar:91/Identity/Account/ResetPassword?code=" + pass + "'> Haga Click Aqui</a>. Y luego ingresa a nuestra App tu email y contraseña  </br>";
                    texto += "</br>";
                    texto += "https://play.google.com/store/apps/details?id=ar.com.finadi.finadiapp&pcampaignid=web_share";
                    string q = "";
                    common.EnviarMail(destinatario, titulo, texto, q);
                    common.EnviarMail("app@FINADI.org.ar", titulo, texto, q);
                }
                else
                {
                    string userName = this.User.Identity.Name;
                    string titulo = "FINADI - Terminá tu Préstamo Aprobado ingresando a la App";
                    string texto = $"Estimado: {cliente.Persona.Apellido},{cliente.Persona.Nombres} , para poder finalizar la solicitud de tu prestamo debes ingresar a la App con tu email y contraseña  </br>";
                    texto += "</br>";
                    texto += "https://play.google.com/store/apps/details?id=ar.com.finadi.finadiapp&pcampaignid=web_share";
                    string q = "";
                    common.EnviarMail(destinatario, titulo, texto, q);
                    common.EnviarMail("app@FINADI.org.ar", titulo, texto, q);

                }
                AddPageAlerts(PageAlertType.Success, "Se envio Correctamente la Notificaciones a " + cliente.Persona.Apellido + " " + cliente.Persona.Nombres);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo.");
                return RedirectToAction("Index");
            }
        }


        public ActionResult _AdjuntoFotoCertificadoDescuento(int Id)
        {
            ViewBag.PrestamoId = Id;
            ViewBag.CargarAdjunto = false;
            Prestamos prestamo = _context.Prestamos.Find(Id);

            if (prestamo.EstadoActual.Id == 1 || prestamo.EstadoActual.Id == 6 || prestamo.EstadoActual.Id == 2)
            {
                ViewBag.CargarAdjunto = true;
            }

            if (prestamo.FotoCertificadoDescuento != null)
            {
                List<AdjuntoDTO> certificados = new List<AdjuntoDTO>();
                foreach (var item in prestamo.FotoCertificadoDescuento)
                {
                    certificados.Add(new AdjuntoDTO
                    {
                        Adjunto = Convert.ToBase64String(item.Adjunto),
                        Id = item.Id
                    });
                }
                ViewBag.AdjuntoTransferencia = certificados;
            }
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _AdjuntoFotoCertificadoDescuento(int Id, List<IFormFile> AdjuntoPrestamo)
        {
            try
            {
                Prestamos prestamo = _context.Prestamos.Find(Id);
                List<Adjuntos> adjuntos = new List<Adjuntos>();
                if (AdjuntoPrestamo != null)
                {
                    foreach (var item in AdjuntoPrestamo)
                    {
                        int index = item.FileName.LastIndexOf('.');
                        string fileExtension = item.FileName.Substring(index + 1);
                        using (var memoryStream = new MemoryStream())
                        {
                            await item.CopyToAsync(memoryStream);
                            adjuntos.Add(new Adjuntos
                            {
                                Adjunto = memoryStream.ToArray(),
                                Extension = fileExtension,
                                Fecha = DateTime.Now
                            });
                            //prestamo.EstadoActual = _context.EstadosPrestamos.Find(7);
                        }
                    }
                    prestamo.FotoCertificadoDescuento.AddRange(adjuntos);
                    //prestamo.FotoCertificadoDescuento = adjuntos;
                }
                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");
            }
        }


        public ActionResult _AdjuntoTransferencia(int Id)
        {
            ViewBag.PrestamoId = Id;
            ViewBag.CargarAdjunto = false;
            Prestamos prestamo = _context.Prestamos.Find(Id);

            if (prestamo.EstadoActual.Id == 1 || prestamo.EstadoActual.Id == 6 || prestamo.EstadoActual.Id == 2)
            {
                ViewBag.CargarAdjunto = true;
            }

            if (prestamo.FotoCertificadoDescuento != null)
            {
                List<AdjuntoDTO> certificados = new List<AdjuntoDTO>();
                foreach (var item in prestamo.FotoCertificadoDescuento)
                {
                    certificados.Add(new AdjuntoDTO
                    {
                        Adjunto = Convert.ToBase64String(item.Adjunto),
                        Id = item.Id
                    });
                }
                ViewBag.AdjuntoTransferencia = certificados;
            }
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _AdjuntoTransferencia(int Id, List<IFormFile> AdjuntoPrestamo)
        {
            try
            {
                Prestamos prestamo = _context.Prestamos.Find(Id);
                List<Adjuntos> adjuntos = new List<Adjuntos>();
                if (AdjuntoPrestamo != null)
                {
                    foreach (var item in AdjuntoPrestamo)
                    {
                        int index = item.FileName.LastIndexOf('.');
                        string fileExtension = item.FileName.Substring(index + 1);
                        using (var memoryStream = new MemoryStream())
                        {
                            await item.CopyToAsync(memoryStream);
                            adjuntos.Add(new Adjuntos
                            {
                                Adjunto = memoryStream.ToArray(),
                                Extension = fileExtension,
                                Fecha = DateTime.Now
                            });
                            //prestamo.EstadoActual = _context.EstadosPrestamos.Find(7);
                        }
                    }
                    prestamo.FotoCertificadoDescuento.AddRange(adjuntos);
                    //prestamo.FotoCertificadoDescuento = adjuntos;
                }
                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Adjunto al Prestamo.");
                return RedirectToAction("Index", "BandejaDeAprobacion");
            }
        }



        [HttpGet]
        public async Task<ActionResult> BorrarAdjuntoFotoCertificado(int Id)
        {
            try
            {
                Adjuntos adjuntos = _context.Adjuntos.Find(Id);
                _context.Adjuntos.Remove(adjuntos);
                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se Borro correctamente el Adjunto.");
                return RedirectToAction("Index", "BandejaDeAprobacion");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al borrar el Adjunto.");
                return RedirectToAction("Index", "BandejaDeAprobacion");
            }
        }

        [HttpGet]
        public async Task<bool> CerrarPrestamo(int Id)
        {
            try
            {
                Prestamos prestamo = _context.Prestamos.Find(Id);
                prestamo.EstadoActual=_context.EstadosPrestamos.Find(8);
                _context.Prestamos.Update(prestamo);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (System.Exception e)
            {
                return false;
            }
        }


    }
}