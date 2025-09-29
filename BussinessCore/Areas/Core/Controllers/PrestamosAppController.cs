using FINADICore.Controllers;
using FINADICore.Services;
using Commons.Identity.Extensions;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class PrestamosAppController : FINADICoreController
    {
        private readonly CGEService _cgeService;
        public PrestamosAppController(FINADIContext context, CGEService cgeService) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Prestamos App" });
            _cgeService = cgeService;
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Prestamos Solicitados Por App" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.Desde = DateTime.Today.AddDays(DateTime.Today.Day * -1);
            ViewBag.Hasta = DateTime.Now;
            ViewBag.PendienteCGE = _context.Prestamos.Where(x => x.EstadoActual.Id == 11).Count();
            var Estados =  new List<SelectListItem>()
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
            //var prestamos = _context.Prestamos.Where(x => x.FechaAnulacion == null && x.PrestamoCGEId>0).Take(50).ToList();            
            //ActualizaPrestamoCGE(prestamos);               
            return View();
        }

        public async Task<IActionResult> PrestamosAppDataTable(DateTime? Desde,DateTime? Hasta)
         {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var roles = _context.Roles.FirstOrDefault(x => x.ShowName == "gestionadministrativa");
            var rolesUsu = _context.UserRoles.Where(x => x.UserId == usuario.Id).ToList();

            if (usuario.Administradores || ((ClaimsPrincipal)User).IsAdmin() || rolesUsu.FirstOrDefault(x => x.RoleId == roles.Id) != null)
            {   
                var prestamos = _context.Prestamos.Where(x => x.FechaAnulacion == null && x.EstadoActual.Id != 11);
               
                if (Desde != null)
                    prestamos = prestamos.Where(e => Convert.ToDateTime(e.FechaSolicitado).Date >= Desde);


                if (Hasta != null)
                    prestamos = prestamos.Where(e => Convert.ToDateTime(e.FechaSolicitado).Date <= Hasta);

              // SE COMENTA HASTA ENTENDER POR QUE VA A CGE
              // var actualizar = _context.Prestamos.Where(x => (x.EstadoActual.Id == 1 || x.EstadoActual.Id == 9 )  && x.FechaAnulacion == null).ToList();
              // ActualizaPrestamoCGE(actualizar);
                
                //prestamos = prestamos.Where(x => x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null);
                return DataTable(prestamos.Select(x => new PrestamosReportesDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Inversor = (x.Inversor!=null) ? x.Inversor.Nombre : "Sin Inversor", MontoCuotaAmpliada = x.MontoCuotaAmpliacion, Disponible = x.Ampliado==true?x.MontoCuota-x.MontoCuotaAmpliacion : x.MontoCuota, AmpliacionFiltroId = (x.Ampliado == true) ? "1" : "2", EstadoFiltroId = x.EstadoActual.Nombre }));
            }
            else
            {
              return null;
            }

        }
        public bool ActualizaPrestamoCGE(List<Prestamos> prestamos)
        {
            foreach (var prestamo in prestamos)
            {
                MEstadoPrestamoDTO consulta = new MEstadoPrestamoDTO();
                consulta.UAT = LoginCGE(prestamo.Cliente.Empresa);
                using (var client = new HttpClient())
                {
                    consulta.PrestamoCGEId = prestamo.PrestamoCGEId;
                    client.BaseAddress = new Uri("https://www.cge.mil.ar:83/api/mentidades/");
                    HttpResponseMessage response = client.PostAsJsonAsync("ConsultaEstadoPrestamo", consulta).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<MEstadoPrestamoDTO>();
                        readTask.Wait();
                        consulta = readTask.Result;
                        if (consulta.Status == 200)
                        {
                            var estado = _context.EstadosPrestamos.FirstOrDefault(x => x.EstadoCGEId == consulta.EstadoId);
                            if (estado != null)
                            {
                                prestamo.EstadoActual = estado;
                                prestamo.Capital = consulta.Capital;
                                prestamo.CantidadCuotas = consulta.CantidadCuotas;
                                prestamo.MontoCuota = consulta.MontoCuota;
                            }
                            if (prestamo.Pagare == null || prestamo.Pagare == 0)
                            {
                                prestamo.Pagare = (prestamo.CantidadCuotas * prestamo.MontoCuota);
                                prestamo.PagareEnLetras = common.NumeroALetrasNew(prestamo.CantidadCuotas * prestamo.MontoCuota);
                            }
                            if (prestamo.TotalCapitalInversor > 0 && prestamo.CapitalInversorEnLetras == null)
                            {
                                prestamo.CapitalInversorEnLetras  = common.NumeroALetrasNew(prestamo.TotalCapitalInversor );
                            }
                            prestamo.FechaAnulacion = consulta.FechaAnulado;
                        }
                    }
                }
                _context.Prestamos.Update(prestamo);
            }
            _context.SaveChanges();
            return true;
        }

        public void _SolicitaPrestamoCGE(List<Prestamos> prestamoBase)
        {
            foreach (var itemPrestamo in prestamoBase)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        MSolicitaPrestamoCGEDTO solicitud = new MSolicitaPrestamoCGEDTO();
                        solicitud.DNI = Convert.ToInt64(itemPrestamo.Cliente.Persona.NroDocumento);
                        solicitud.EntidadId = itemPrestamo.Cliente.Empresa.EntidadIdCGE;
                        solicitud.FotoDNIAnverso = itemPrestamo.Cliente.FotoDNIAnverso;
                        solicitud.FotoDNIReverso = itemPrestamo.Cliente.FotoDNIReverso;
                        solicitud.FotoSosteniendoDNI = itemPrestamo.Cliente.FotoSosteniendoDNI;
                        solicitud.TipoPrestamoId = itemPrestamo.Linea.TipoDescuentoCGEId;
                        solicitud.Ampliado = itemPrestamo.Ampliado;
                        solicitud.MontoCuota = itemPrestamo.MontoCuota;
                        solicitud.ImporteSolicitado = itemPrestamo.Capital;
                        solicitud.CantidadCuotas = itemPrestamo.CantidadCuotas;
                        solicitud.FirmaOlografica = itemPrestamo.FirmaOlografica;
                        solicitud.UAT = LoginCGE(itemPrestamo.Cliente.Empresa);
                        solicitud.EntidadId = itemPrestamo.Cliente.Empresa.EntidadIdCGE;
                        //solicitud.Precancelaciones = itemPrestamo.;
                        solicitud.MontoCuotaAmpliado = itemPrestamo.Cliente.Persona.TipoPersona.MontoAmpliacion;
                        client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                        HttpResponseMessage response = client.PostAsJsonAsync("SolicitaPrestamo", solicitud).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var readTask = response.Content.ReadAsAsync<MSolicitaPrestamoCGEDTO>();
                            readTask.Wait();
                            solicitud = readTask.Result;
                            if (solicitud.Status == 200)
                            {
                                itemPrestamo.PrestamoCGEId = solicitud.PrestamoCGEId;
                                _context.Prestamos.Update(itemPrestamo);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
                
            }            
        }


        public string LoginCGE(Empresas empresa)
        {
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        return login.UAT;
                    }
                }
                return response.ToString();
            }
        }
        public IActionResult _ActualizarEstados()
        {
            var prestamos = _context.Prestamos.Where(x => x.FechaAnulacion == null && x.PrestamoCGEId > 0).ToList();
            ActualizaPrestamoCGE(prestamos);
            return RedirectToAction("Index");
        }

        public IActionResult _EnviarPrestamosCGE()
        {
            var prestamos = _context.Prestamos.Where(x => x.FechaAnulacion == null && x.EstadoActual.Id==11).ToList();
            _SolicitaPrestamoCGE(prestamos);
            return RedirectToAction("Index");
        }

        public IActionResult _DescargaExcel(DateTime? Desde,DateTime? Hasta)
        {
            if (Desde is null)
                Desde = Convert.ToDateTime (DateTime.Now.Year.ToString ()+"-01-01");
            if (Hasta  is null)
                Hasta = DateTime.Now;

            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName  == User.Identity.Name);
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaPrestamosApp.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                decimal total = 0;
                var workSheet = package.Workbook.Worksheets[1];
                var renglones = _context.Prestamos.Where(x => usuario.Administradores == true  && Convert.ToDateTime(x.FechaSolicitado).Date >= Desde && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta && x.FechaAnulacion == null && (x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null)).Select(x => new PrestamosReportesDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Celular = x.Cliente.Celular, eMail = x.Cliente.Usuario.UserName });
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
        public IActionResult _LegajoOnline(int id)
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
            string pdf;
            //if (prestamo.Cliente.Persona.TipoPersona.Organismo.Id  ==  1) 
            //{
            //     pdf = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_mutual", parametros, "PDF", _context));
            //}
            //else
            //{
            //     pdf = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_mutual_gendarmeria", parametros, "PDF", _context));
            //}
           switch(prestamo.Cliente.Persona.TipoPersona.Organismo.Id)
           {
                case 4:
                    pdf = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_mutual_gendarmeria", parametros, "PDF", _context));
                    break;
                case 13:
                    pdf = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_mutual_afip", parametros, "PDF", _context));
                    break;
                default :
                    pdf = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("reporte_mutual", parametros, "PDF", _context)); 
                    break;
            }
            @ViewBag.PDF=pdf;   


            return PartialView();
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
                    nombreReporte= "reporte_inversor_comercio";
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

        [HttpGet]
        public ActionResult _SueldoNetoPrimerVencimiento(int id)
        {
            ViewBag.Gendermeria = false;
            Prestamos prestamo = _context.Prestamos.FirstOrDefault(x => x.Id == id);
            if (prestamo.Cliente.Persona.TipoPersona.Organismo.Id==4){
                ViewBag.Gendermeria = true;
            }
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
                prestamoUpdate.FechaPresentacionDeInversor = prestamo.FechaPresentacionDeInversor;
                prestamoUpdate.TotalCapitalInversor  = prestamo.TotalCapitalInversor;
                if (prestamo.TotalCapitalInversor > 0 && prestamo.CapitalInversorEnLetras == null)
                {
                    prestamoUpdate.CapitalInversorEnLetras = common.NumeroALetrasNew(prestamo.TotalCapitalInversor);
                }

                if(prestamoUpdate.Cliente.Persona.TipoPersona.Organismo.Id==4)
                {
                    prestamoUpdate.CodigoEstadistico= prestamo.CodigoEstadistico;
                    prestamoUpdate.FechaEmisionAnexo = prestamo.FechaEmisionAnexo;
                }

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


        private void InversorViewBag(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Inversores = _context.Inversores.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }


        
        public ActionResult _EnviarNotificacionPrestamosAprobados()
        {
            try
            {
                string userName = this.User.Identity.Name;

                var usuario = _context.Users.Where(x => x.UserName == userName).FirstOrDefault();
                MTraePrestamosAprobadosDTO prestamosAprobados = new MTraePrestamosAprobadosDTO();
                prestamosAprobados.UAT = LoginCGE(usuario.Clientes.Empresa);
                var ListaPrestamos = _cgeService.MTraePrestamosAprobadosDTO(prestamosAprobados);

                string titulo = "FINADI - Terminá tu Préstamo Preaprobado desde nuestra App";
                string texto = "https://play.google.com/store/apps/details?id=ar.com.FINADI&hl=es";
                string cliente = "";
                
                foreach (var item in ListaPrestamos.Prestamos)
                {
                    var prestamos = _context.Prestamos.Find(item.Id);
                    string destinatario = prestamos.Cliente.Usuario.Mail;  
                    common.EnviarMail(destinatario, titulo, texto, cliente);
                }

                AddPageAlerts(PageAlertType.Success, "Se envio las Notificaciones Correctamente.");
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo.");
                return RedirectToAction("Index");
            }

        }
    }
}
