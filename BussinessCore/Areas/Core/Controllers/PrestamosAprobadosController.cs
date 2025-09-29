using FINADICore.Areas.Administracion.ViewModels;
using FINADICore.Controllers;
using FINADICore.Services;
using Castle.Core.Internal;
using Commons.Identity.Extensions;
using Commons.Identity.Services;
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
    public class PrestamosAprobadosController : FINADICoreController
    {
        private readonly CGEService _cgeService;
        private readonly UserService<Usuario> _userService;
        public PrestamosAprobadosController(FINADIContext context, CGEService cgeService, UserService<Usuario> userService) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Prestamos Aprobados" });
            _cgeService = cgeService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Prestamos Aprobados" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.Desde = DateTime.Today.AddDays(DateTime.Today.Day * -1);
            ViewBag.Hasta = DateTime.Now;
            return View();
        
        }

        public async Task<IActionResult> PrestamosAprobadosDataTable(DateTime? Desde, DateTime? Hasta)
        {

            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var roles = _context.Roles.FirstOrDefault(x => x.ShowName == "gestionadministrativa");
            var rolesUsu = _context.UserRoles.Where(x => x.UserId == usuario.Id).ToList();

            if (usuario.Administradores || ((ClaimsPrincipal)User).IsAdmin() || rolesUsu.FirstOrDefault(x => x.RoleId == roles.Id) != null)
            {
                MTraePrestamosAprobadosDTO prestamosAprobados = new MTraePrestamosAprobadosDTO();
                prestamosAprobados.UAT = LoginCGE(usuario.Clientes.Empresa);
                var prestamos = _cgeService.MTraePrestamosAprobadosDTO(prestamosAprobados);
                List<PrestamosReportesDTO> datosDataTable = new List<PrestamosReportesDTO>();

                foreach (var item in prestamos.Prestamos)
                {
                    //Verifica si existe el Cliente sino existe lo crea.
                    DAL.Models.Usuario Usuario = await VerificarCargarCliente(prestamosAprobados.UAT, item.DNI.ToString(), usuario.Clientes.Empresa);
                    if (Usuario != null)
                    {
                        Prestamos prestamoIntegracion = _context.Prestamos.Where(x => x.PrestamoCGEId == item.Id).FirstOrDefault();
                        if (prestamoIntegracion == null )
                        {
                            datosDataTable.Add(new PrestamosReportesDTO
                            {
                                CantidadCuotas = item.CantidadCuotas,
                                Capital = item.ImportePrestamo,
                                Categoria = Usuario.Clientes.CategoriaLaboral,
                                Cliente = Usuario.Clientes.Persona.Apellido + ", " + Usuario.Clientes.Persona.Nombres,
                                Fecha = Convert.ToDateTime(item.FechaHoraAprobado).ToString("yyyy-MM-dd HH:mm"),
                                Id = item.Id,
                                MontoCuota = item.ImporteCuota,
                                DNI = Usuario.Clientes.Persona.NroDocumento.ToString(),
                                ConfirmoEmail = Usuario.Clientes.Usuario.EmailConfirmed
                            });                            
                        }
                        else if (prestamoIntegracion.Integracion && prestamoIntegracion.EstadoActual.Id == 6)
                        {
                            datosDataTable.Add(new PrestamosReportesDTO
                            {
                                CantidadCuotas = item.CantidadCuotas,
                                Capital = item.ImportePrestamo,
                                Categoria = Usuario.Clientes.CategoriaLaboral,
                                Cliente = Usuario.Clientes.Persona.Apellido + ", " + Usuario.Clientes.Persona.Nombres,
                                Fecha = Convert.ToDateTime(item.FechaHoraAprobado).ToString("yyyy-MM-dd HH:mm"),
                                Id = item.Id,
                                MontoCuota = item.ImporteCuota,
                                DNI = Usuario.Clientes.Persona.NroDocumento.ToString(),
                                ConfirmoEmail = Usuario.Clientes.Usuario.EmailConfirmed
                            });
                        }
                    }
                    else 
                    {
                        AddPageAlerts(PageAlertType.Error , "Verificar el dni:"+item.DNI +" faltan datos");
                        return DataTable(datosDataTable.AsQueryable());
                    }
                    
                }
                return DataTable(datosDataTable.AsQueryable());
            }
            else
            {
                return null;
            }

        }


        public async Task<IActionResult> PrestamosProblemasEntidadDataTable(int EstadoId)
        {

            //var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            MTraePrestamosAprobadosDTO prestamosAprobados = new MTraePrestamosAprobadosDTO();
            var empresa = _context.Empresas.Find(1);
            prestamosAprobados.UAT = LoginCGE(empresa);
            var prestamos = _cgeService.MTraePrestamosAprobadosDTO(prestamosAprobados);
            List<PrestamosReportesDTO> datosDataTable = new List<PrestamosReportesDTO>();
            List<ProblemasBandejaDeAprobacionPrestamoDTO> PrestamosConProblemas = new List<ProblemasBandejaDeAprobacionPrestamoDTO>();

            foreach (var item in prestamos.Prestamos)
            {
                ProblemasBandejaDeAprobacionPrestamoDTO Problema = new ProblemasBandejaDeAprobacionPrestamoDTO();

                //Verifica si existe el Cliente sino existe lo crea.
                DAL.Models.Usuario Usuario = await VerificarCargarCliente(prestamosAprobados.UAT, item.DNI.ToString(), empresa);


                if (Usuario != null)
                {
                    Prestamos prestamoIntegracion = _context.Prestamos.Where(x => x.PrestamoCGEId == item.Id).FirstOrDefault();
                    if(prestamoIntegracion == null || (prestamoIntegracion.Integracion && prestamoIntegracion.EstadoActual.Id == 6))
                    {

                        Problema.IdPrestamo = item.Id;
                        Problema.Fecha = item.FechaHoraAprobado;
                        if (Usuario.Clientes==null)
                        {
                            Problema.Observacion = "El Prestamo no tiene un Cliente Asignado.";
                            PrestamosConProblemas.Add(Problema);
                        }
                        else if (Usuario.Clientes.Persona==null)
                        {
                            Problema.Observacion = "El Prestamo no tiene una Persona Asignado.";
                            PrestamosConProblemas.Add(Problema);
                        }
                        else if (Usuario.Clientes.Persona.NroDocumento==null)
                        {
                            Problema.Observacion = "La Persona que Generó el Prestamo no tiene un DNI Asignado.";
                            PrestamosConProblemas.Add(Problema);
                        }
                        else if (VerificarCorreos(empresa, Usuario.Clientes.Persona.NroDocumento, Usuario.Clientes.Usuario.UserName)==1)
                        {
                            Problema.Observacion = "El Usuario no tiene resgistrado un correo en la CGE";
                            Problema.DNI = Usuario.Clientes.Persona.NroDocumento;
                            PrestamosConProblemas.Add(Problema);
                        }
                        else if (VerificarCorreos(empresa, Usuario.Clientes.Persona.NroDocumento, Usuario.Clientes.Usuario.UserName)==2)
                        {
                            Problema.Observacion = "El Usuario no tiene resgistrado un correo";
                            Problema.DNI = Usuario.Clientes.Persona.NroDocumento;
                            PrestamosConProblemas.Add(Problema);
                        }
                        else if (VerificarCorreos(empresa, Usuario.Clientes.Persona.NroDocumento, Usuario.Clientes.Usuario.UserName)==3)
                        {
                            Problema.DNI = Usuario.Clientes.Persona.NroDocumento;
                            Problema.Observacion = "El correo del Usuario no coincidecon el correo registrado en la CGE";
                            PrestamosConProblemas.Add(Problema);
                        }
                    }
                }
                else
                {
                    Problema.IdPrestamo = item.Id;
                    Problema.Fecha = item.FechaHoraAprobado;
                    Problema.DNI = item.DNI.ToString();
                    Problema.Observacion = "Faltan Datos o correo no coincide.";
                    PrestamosConProblemas.Add(Problema);
                }
            }        
            return DataTable(PrestamosConProblemas.AsQueryable());
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
                var renglones = _context.Prestamos.Where(x => usuario.Administradores == true && Convert.ToDateTime(x.FechaSolicitado).Date >= Desde && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta && x.FechaAnulacion == null && (x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null)).Select(x => new PrestamosReportesDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Celular = x.Cliente.Celular, eMail = x.Cliente.Usuario.UserName });
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

        private void InversorViewBag(int Id)
        {
            ViewBag.Id = Id;
            ViewBag.Inversores = _context.Inversores.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }



        public ActionResult _EnviarNotificacionesPrestamosAprobados()
        {
            try
            {
                var ListClientes = _context.Clientes.Where(x => x.Usuario.EmailConfirmed == false).ToList();

                string titulo = "FINADI - Terminá tu Préstamo Aprobado desde nuestra App";
                string texto = "https://play.google.com/store/apps/details?id=ar.com.FINADI&hl=es";
                string client = "";


                foreach (var item in ListClientes)
                {
                    Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == item.Persona.NroDocumento).FirstOrDefault();
                    if (cliente == null)
                    {
                        cliente = FINADI.TraePersonaCGE(cliente.Empresa,Convert.ToInt32(item.Persona.NroDocumento), "", _context);
                    }
                    string destinatario = cliente.Usuario.Mail;

                    if (destinatario != null)
                    {
                        common.EnviarMail(destinatario, titulo, texto, client);
                    }
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

        [HttpGet]
        public ActionResult _AgregarTelefono(string id)
        {
            try
            {
                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == id).FirstOrDefault();

                return PartialView("_AgregarTelefono", cliente);
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al Agregar el Telefono " + id);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult _AgregarTelefono(Clientes cliente)
        {
            try
            {
                var clienteUpdate = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == cliente.Persona.NroDocumento);
                
                clienteUpdate.Telefono = cliente.Telefono;
                _context.Clientes.Update(clienteUpdate);
                _context.SaveChanges();

                AddPageAlerts(PageAlertType.Success, "Se Modificó Correctamente el Telefono de " + cliente.Persona.Apellido + " " + cliente.Persona.Nombres);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al Modificar el número de telefono.");
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

                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
                string UAT = LoginCGE(usuario.Clientes.Empresa);

                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == dni).FirstOrDefault();
                if (cliente == null)
                {
                    cliente = FINADI.TraePersonaCGE(usuario.Clientes.Empresa, Convert.ToInt32(dni), "", _context);
                }
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
                string UAT = LoginCGE(usuario.Clientes.Empresa);

                MTraePrestamosAprobadosDTO prestamosAprobados = new MTraePrestamosAprobadosDTO()
                {
                    DNI = Convert.ToInt64(client.Persona.NroDocumento),
                    UAT = UAT
                };
                //Cliente base de datos Local
                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == client.Persona.NroDocumento).FirstOrDefault();

                //Trae el Prestamo del Cliente
                var prestamos = _cgeService.MTraePrestamosAprobadosDTO(prestamosAprobados).Prestamos.First();

                if (_context.Prestamos.Where(x => x.PrestamoCGEId == prestamos.Id).FirstOrDefault() == null)
                {
                    MRegistraPersonaDTO clienteCGE = FINADI.TraeDatosPersona20CGE(cliente.Empresa, cliente.Usuario.UserName, _context, 0);                    

                    if (prestamos.Id == Convert.ToInt64(PrestamoId))
                    {
                        if (clienteCGE.TipoPersona == "Soldados")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(3);
                        }
                        if (clienteCGE.TipoPersona == "Militares")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(1);
                        }
                        if (clienteCGE.TipoPersona == "Civiles" || clienteCGE.TipoPersona == "Docentes")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(2);
                        }

                        
                        var lineaPrestamo = _context.LineasPrestamosTiposPersonas.Where(x => x.TipoPersona.Id == cliente.Persona.TipoPersona.Id).FirstOrDefault();

                        Prestamos prestamo = new Prestamos();
                        prestamo.FechaSolicitado = DateTime.Now;
                        prestamo.Capital = prestamos.ImportePrestamo;
                        prestamo.Cliente = cliente;
                        prestamo.Linea = lineaPrestamo.LineaPrestamo;
                        prestamo.EstadoActual = _context.EstadosPrestamos.Find(6);
                        //prestamo.PrestamoCGEId = 0;
                        prestamo.CantidadCuotas = prestamos.CantidadCuotas;
                        prestamo.MontoCuota = prestamos.ImporteCuota;
                        prestamo.PrestamoCGEId = prestamos.Id;
                        prestamo.CFT = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.Id && x.Activo == true).CFT;
                        prestamo.TEMAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.Id && x.Activo == true).TEM;
                        prestamo.TNAAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.Id && x.Activo == true).TNA;
                        prestamo.CapitalAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.Id && x.Activo == true && x.MontoPrestado == prestamos.ImportePrestamo).CapitalFINADI;
                        prestamo.Integracion = true;
                        _context.Prestamos.Add(prestamo);
                        _context.SaveChanges();
                    }
                    else
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo.");
                        return RedirectToAction("Index");
                    }
                }
                string destinatario = cliente.Usuario.UserName ;
                //string destinatario = "acevedoruben@hotmail.com";
                if (destinatario != null && cliente.Usuario.RecordarPassword)
                {
                    string pass = common.Encrypt(cliente.Persona.NroDocumento.ToString() + DateTime.Now.ToString(), "FINADI");
                    cliente.Password = pass;
                    cliente.Usuario.RecordarPassword = false;
                    _context.Clientes.Update(cliente);
                    _context.SaveChanges();
                    string userName = this.User.Identity.Name;
                    string titulo = "FINADI - Terminá tu Préstamo Aprobado descargando nuestra App";
                    string texto = $"Estimado: {cliente.Persona.Apellido},{cliente.Persona.Nombres} , para poder Iniciar en la App debes registrar tu contraseña <a href = 'http://emutual.com.ar:91/Identity/Account/ResetPassword?code=" + pass + "'> Haga Click Aqui</a>. Y luego ingresa a nuestra App tu email y contraseña  </br>";
                    texto += "</br>";
                    texto += "https://play.google.com/store/apps/details?id=ar.com.FINADI&hl=es";
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
                    texto += "https://play.google.com/store/apps/details?id=ar.com.FINADI&hl=es";
                    string q = "";
                    common.EnviarMail(destinatario, titulo, texto, q);
                    common.EnviarMail("acevedoruben@hotmail.com", titulo, texto, q);
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



        [HttpGet]
        public ActionResult _EnviarNotificacionWapp(string id)
        {
            try
            {
                var index = id.IndexOf(",");
                var dni = id.Substring(0, index);
                var idPrestamo = id.Substring(index + 1, (id.Length - index) - 1);

                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
                string UAT = LoginCGE(usuario.Clientes.Empresa);

                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == dni).FirstOrDefault();
                if (cliente == null)
                {
                    cliente = FINADI.TraePersonaCGE(usuario.Clientes.Empresa, Convert.ToInt32(dni), "", _context);
                }
                ViewBag.PrestamoId = idPrestamo;
                return PartialView("_EnviarNotificacionWapp", cliente);
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo del DNI " + id);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult _EnviarNotificacionWapp(Clientes client, string PrestamoId)
        {
            try
            {
                //usuario Login
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
                string UAT = LoginCGE(usuario.Clientes.Empresa);

                MTraePrestamosAprobadosDTO prestamosAprobados = new MTraePrestamosAprobadosDTO()
                {
                    DNI = Convert.ToInt64(client.Persona.NroDocumento),
                    UAT = UAT
                };
                //Cliente base de datos Local
                Clientes cliente = _context.Clientes.Where(x => x.Persona.NroDocumento == client.Persona.NroDocumento).FirstOrDefault();
                if (String.IsNullOrWhiteSpace(cliente.Telefono))
                {
                    AddPageAlerts(PageAlertType.Error, "El cliente no posee un telefono registrado para enviar la Notificación.");
                    return RedirectToAction("Index");
                }

                //Trae el Prestamo del Cliente
                var prestamos = _cgeService.MTraePrestamosAprobadosDTO(prestamosAprobados).Prestamos.First();

                if (_context.Prestamos.Where(x => x.PrestamoCGEId == prestamos.Id).FirstOrDefault() == null)
                {
                    MRegistraPersonaDTO clienteCGE = FINADI.TraeDatosPersona20CGE(cliente.Empresa, cliente.Usuario.UserName, _context, 0);

                    if (prestamos.Id == Convert.ToInt64(PrestamoId))
                    {
                        if (clienteCGE.TipoPersona == "Soldados")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(3);
                        }
                        if (clienteCGE.TipoPersona == "Militares")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(1);
                        }
                        if (clienteCGE.TipoPersona == "Civiles" || clienteCGE.TipoPersona == "Docentes")
                        {
                            cliente.Persona.TipoPersona = _context.TiposPersonas.Find(2);
                        }


                        var lineaPrestamo = _context.LineasPrestamosTiposPersonas.Where(x => x.TipoPersona.Id == cliente.Persona.TipoPersona.Id).FirstOrDefault();

                        Prestamos prestamo = new Prestamos();
                        prestamo.FechaSolicitado = DateTime.Now;
                        prestamo.Capital = prestamos.ImportePrestamo;
                        prestamo.Cliente = cliente;
                        prestamo.Linea = lineaPrestamo.LineaPrestamo;
                        prestamo.EstadoActual = _context.EstadosPrestamos.Find(6);
                        //prestamo.PrestamoCGEId = 0;
                        prestamo.CantidadCuotas = prestamos.CantidadCuotas;
                        prestamo.MontoCuota = prestamos.ImporteCuota;
                        prestamo.PrestamoCGEId = prestamos.Id;
                        prestamo.CFT = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.LineaPrestamo.Id && x.Activo == true).CFT;
                        prestamo.TEMAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.LineaPrestamo.Id && x.Activo == true).TEM;
                        prestamo.TNAAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.LineaPrestamo.Id && x.Activo == true).TNA;
                        prestamo.CapitalAmprom = _context.LineasPrestamosPlanes.FirstOrDefault(x => x.Linea.Id == lineaPrestamo.LineaPrestamo.Id && x.Activo == true && x.MontoPrestado == prestamos.ImportePrestamo).CapitalFINADI;
                        prestamo.Integracion = true;
                        _context.Prestamos.Add(prestamo);
                        _context.SaveChanges();
                    }
                    else
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo.");
                        return RedirectToAction("Index");
                    }
                }
                string destinatario = cliente.Telefono;
                //string destinatario = "acevedoruben@hotmail.com";
                if (destinatario != null && cliente.Usuario.RecordarPassword)
                {
                    string pass = common.Encrypt(cliente.Persona.NroDocumento.ToString() + DateTime.Now.ToString(), "FINADI");
                    cliente.Password = pass;
                    cliente.Usuario.RecordarPassword = false;
                    _context.Clientes.Update(cliente);
                    _context.SaveChanges();
                    string userName = this.User.Identity.Name;
                    string texto = "FINADI - Terminá tu Préstamo Aprobado descargando nuestra App. ";
                    texto += $"Estimado: {cliente.Persona.Apellido.Trim()},{cliente.Persona.Nombres.Trim()} , para poder Iniciar en la App debes registrar tu contraseña http://emutual.com.ar:91/Identity/Account/ResetPassword?code=" + pass + " . Y luego ingresa a nuestra App tu email y contraseña. ";
                    texto += "https://play.google.com/store/apps/details?id=ar.com.FINADI";
                    WhatsAppService.EnviaWhatsAppImagen(new DAL.DTOs.WhatsApp.WhatsAppDTO { Telefono = destinatario, Texto = texto, ImagenUrl = "https://play-lh.googleusercontent.com/P_jNv_a45jMyYe4Fw_JZa7VgcpWmuURc3AlBtm9-wlhW96BOSwfnu7GQ4edC0QBr4A=w480-h960-rw" });
                }
                else
                {
                    string userName = this.User.Identity.Name;
                    string texto = "FINADI - Terminá tu Préstamo Aprobado ingresando a la App. ";
                    texto += $"Estimado: {cliente.Persona.Apellido.Trim()},{cliente.Persona.Nombres.Trim()} , para poder finalizar la solicitud de tu prestamo debes ingresar a la App con tu email y contraseña. ";
                    texto += "https://play.google.com/store/apps/details?id=ar.com.FINADI";
                    WhatsAppService.EnviaWhatsAppImagen(new DAL.DTOs.WhatsApp.WhatsAppDTO { Telefono = destinatario, Texto = texto, ImagenUrl = "https://play-lh.googleusercontent.com/P_jNv_a45jMyYe4Fw_JZa7VgcpWmuURc3AlBtm9-wlhW96BOSwfnu7GQ4edC0QBr4A=w480-h960-rw" });
                }
                AddPageAlerts(PageAlertType.Success, "Se envio Correctamente la Notificacion por WhatsApp a " + cliente.Persona.Apellido + " " + cliente.Persona.Nombres);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al enviar la notificación del Préstamo.");
                return RedirectToAction("Index");
            }
        }




        private async Task<Usuario> VerificarCargarCliente(string UAT, string Dni, Empresas empresa)
        {            
            Clientes datosPersonales = _cgeService.TraeDatosPersonaCGE(UAT, Convert.ToInt32(Dni), "");
            Persona persona = _context.Personas.Where(x => x.NroDocumento == Dni).FirstOrDefault();
            
            if (datosPersonales.Usuario.UserName.Trim()==null || datosPersonales.Usuario.UserName.Trim() == "") 
            {
                return null;
            }
            Usuario usuario = _context.Users.Where(x => x.UserName == datosPersonales.Usuario.UserName.Trim()).FirstOrDefault();


            if (persona == null)
            {
                if (usuario == null)
                {
                    var user = new Usuario()
                    {
                        UserName = datosPersonales.Usuario.UserName.Trim(),
                        Email = datosPersonales.Usuario.UserName.Trim(),
                        Mail = datosPersonales.Usuario.UserName.Trim(),
                        RecordarPassword = true
                    };

                    var result = await _userService.CreateAsync(user, "Ejercito2021");

                    Clientes nuevocliente = new Clientes()
                    {
                        Empresa = empresa,
                        TipoCliente = _context.TiposClientes.Find(1),
                        Celular = (datosPersonales.Celular != null) ? datosPersonales.Celular : "",
                        RazonSocial = (datosPersonales.Persona.Apellido + ", " + datosPersonales.Persona.Apellido).ToUpper()
                    };


                    nuevocliente.Persona = new Persona()
                    {
                        NroDocumento = datosPersonales.Persona.NroDocumento.ToString(),
                        Apellido = datosPersonales.Persona.Apellido,
                        Nombres = datosPersonales.Persona.Nombres,
                        FechaNacimiento = (datosPersonales.Persona.FechaNacimiento != null) ? Convert.ToDateTime(datosPersonales.Persona.FechaNacimiento) : new DateTime(),
                        TipoDocumento = _context.TipoDocumento.Find(1),
                        Pais = _context.Paises.Find(1),
                        TipoPersona = _context.TiposPersonas.Find(1)
                    };


                    Usuario userLocal = _context.Usuarios.Where(x => x.UserName == user.UserName).FirstOrDefault();
                    await _context.Personas.AddAsync(nuevocliente.Persona);
                    await _context.Clientes.AddAsync(nuevocliente);
                    userLocal.Clientes = nuevocliente;
                    userLocal.Personas = nuevocliente.Persona;
                    _context.Usuarios.Update(userLocal);
                    await _context.SaveChangesAsync();
                    return user;

                }
                else
                {
                    if (usuario.Personas != null)
                        return usuario;
                    else
                    {
                        _context.Remove(usuario);
                        _context.SaveChanges();
                        return null;
                    }
                }

            }
            else if (usuario != null)
                return usuario;
            else
                return null;            
        }




        private int VerificarCorreos(Empresas Empresa, string DNI, string correo)
        {
            var datosPresona = FINADI.TraeDatosPersonaCGE(Empresa, Convert.ToInt32(DNI), _context, 0);
            if (datosPresona.Mail==null)
            {
                return 1;
            }
            else if (correo==null)
            {
                return 2;
            }
            else if (datosPresona.Mail.Trim()!=correo.Trim())
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

    }
}