using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System.IO;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Collections.Generic;
using DAL.DTOs;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Commons.Identity.Extensions;
using FINADICore.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using FINADICore.Controllers;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class CampanasController : FINADICoreController
    {
        private IHostingEnvironment _envirom;
        public CampanasController(FINADIContext context, IHostingEnvironment env) : base(context)
        {
            _envirom = env;
            breadcumb.Add(new Message() { DisplayName = "Gestión" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Campañas" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult CampanasDataTable()
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            List<Campanas> campanas = new List<Campanas>();
            if (usuario == null || usuario.Clientes == null || usuario.Clientes?.Empresa == null)
            {
                campanas = _context.Campanas.Where(x => x.Empresa == null && x.FechaBaja == null).ToList();
            }
            else
            {
                campanas = _context.Campanas.Where(x => x.Empresa.Id == usuario.Clientes.Empresa.Id && x.FechaBaja == null).ToList();
            }
            var query = from c in campanas
                        select new ListCampanasDTO
                        {
                            Id = c.Id,
                            FechaOrden = Convert.ToInt32(c.Fecha.ToString("yyyyMMdd")),
                            Fecha = c.Fecha.ToString("dd/MM/yyyy"),
                            Observaciones = c.Observaciones
                        };
            return DataTable(query.AsQueryable());
        }

        public ActionResult Create()
        {           
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName  == User.Identity.Name);
            if (usuario != null && usuario.Clientes!=null && usuario.Clientes?.Empresa!=null)
            {
                ViewBag.Provincias = FINADICore.FINADI.TraeProvinciasCGE(usuario.Clientes?.Empresa, _context).Provincias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
                ViewBag.Unidades = FINADICore.FINADI.TraeUnidadesCGE(usuario.Clientes?.Empresa, _context, 0).Unidades.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            }
            else
            {
                ViewBag.Provincias = FINADICore.FINADI.TraeProvinciasCGE(_context.Empresas.Find(1), _context).Provincias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
                ViewBag.Unidades = FINADICore.FINADI.TraeUnidadesCGE(_context.Empresas.Find(1), _context, 0).Unidades.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            }            
            return PartialView();         
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(Campanas campana, IFormFile Imagen)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName  == User.Identity.Name);
                if(usuario==null || usuario.Clientes==null || usuario.Clientes.Empresa == null)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al generar la Campaña ya que su usuario no esta vinculado a ninguna empresa.");
                    return RedirectToAction(nameof(Index));
                }
                campana.Empresa = usuario.Clientes.Empresa;
                campana.Fecha = DateTime.Now;
                /*if (Imagen != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        Imagen.CopyTo(memoryStream);
                        campana.Imagen = memoryStream.ToArray();
                    }
                } */               
                await _context.Campanas.AddAsync(campana);
                await _context.SaveChangesAsync();
                if (Imagen != null)
                {
                    ImageService servicio = new ImageService(_envirom);
                    campana.ImagenUrl=servicio.Upload(campana.Id, Imagen);
                    _context.Campanas.Update(campana);
                    _context.SaveChanges();
                }
                AddPageAlerts(PageAlertType.Success, "Se creo correctamente la Campaña.");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al generar la Campaña.");
                return RedirectToAction(nameof(Index));
            }

        }
        public IActionResult Delete(int id)
        {
            try
            {
                Campanas campana = _context.Campanas.Find(id);
                campana.FechaBaja = DateTime.Now;
                _context.Campanas.Update(campana);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                return RedirectToAction("Update", id);
            }
        }
        public ActionResult Update(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            if (usuario != null && usuario.Clientes != null && usuario.Clientes?.Empresa != null)
            {
                ViewBag.Provincias = FINADICore.FINADI.TraeProvinciasCGE(usuario.Clientes?.Empresa, _context).Provincias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
                ViewBag.Unidades = FINADICore.FINADI.TraeUnidadesCGE(usuario.Clientes?.Empresa, _context, 0).Unidades.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            }
            else
            {
                ViewBag.Provincias = FINADICore.FINADI.TraeProvinciasCGE(_context.Empresas.Find(1), _context).Provincias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
                ViewBag.Unidades = FINADICore.FINADI.TraeUnidadesCGE(_context.Empresas.Find(1), _context, 0).Unidades.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            }
            var campana = _context.Campanas.Find(id);
            /*if (campana.Imagen != null)
                ViewBag.Imagen = Convert.ToBase64String(campana.Imagen);*/
            if (!String.IsNullOrWhiteSpace(campana.ImagenUrl)) ViewBag.Imagen = campana.ImagenUrl;
            return PartialView(campana);
        }
        [HttpPost]
        public ActionResult Update(Campanas campana, IFormFile Imagen)
        {
            try
            {
                Campanas d = _context.Campanas.Find(campana.Id);
                d.Observaciones = campana.Observaciones;
                d.TextoMail = campana.TextoMail;
                d.MaximoDisponible = campana.MaximoDisponible;
                d.MinimoDisponible = campana.MinimoDisponible;
                d.UnidadId = campana.UnidadId;
                d.ProvinciaId = campana.ProvinciaId;
                d.Link = campana.Link;
                if (Imagen != null)
                {
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    Imagen.CopyTo(memoryStream);
                    //    d.Imagen = memoryStream.ToArray();
                    //}
                    ImageService servicio = new ImageService(_envirom);
                    d.ImagenUrl = servicio.Upload(campana.Id, Imagen);
                    
                }
                _context.Campanas.Update(d);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se creo correctamente la Campaña.");
                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al generar la Campaña. " + e.Message );
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Genera(int id)
        {
            var arranca = DateTime.Now;
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            var campana = _context.Campanas.Find(id);
            var datos = FINADICore.FINADI.TraeCampana(usuario.Clientes.Empresa, campana);
            _context.CampanasRenglones.RemoveRange(_context.CampanasRenglones.Where(x => x.Cabecera.Id == id));
            if (datos.Renglones != null)
            {
                foreach (var renglon in datos.Renglones)
                {
                    CampanasRenglones nuevorenglon = new CampanasRenglones();
                    nuevorenglon.Apellido = renglon.Apellido;
                    nuevorenglon.Cabecera = campana;
                    nuevorenglon.Celular = renglon.Celular;
                    nuevorenglon.Disponible = renglon.Disponible;
                    nuevorenglon.DNI = renglon.DNI;
                    nuevorenglon.eMail = renglon.eMail;
                    nuevorenglon.ImporteMaximo = renglon.ImporteMaximo;
                    nuevorenglon.Nombres = renglon.Nombres;
                    nuevorenglon.ProvinciaId = renglon.ProvinciaId;
                    nuevorenglon.UnidadId = renglon.UnidadId;
                    nuevorenglon.Provincia = renglon.Provincia;
                    nuevorenglon.Unidad = renglon.Unidad;
                    nuevorenglon.TipoPersona = renglon.TipoPersona;
                    var montomaximo = _context.TiposPersonas.FirstOrDefault(x => x.nombre == renglon.TipoPersona);
                    nuevorenglon.ImporteMaximo = montomaximo.TopePrestamo;
                    _context.CampanasRenglones.Add(nuevorenglon);
                }
            }
            CampanaDTO valores = new CampanaDTO();
            valores.CantidadMinutos = Convert.ToInt32((DateTime.Now - arranca).TotalMinutes);
            valores.CantidadPersonas = datos.Renglones.Count();
            valores.CantidadProvincias = datos.CantidadProvincias;
            valores.CantidadUnidades = datos.CantidadUnidades;
            campana.CantidadPersonas = valores.CantidadPersonas;
            campana.CantidadProvincias = valores.CantidadProvincias;
            campana.CantidadUnidades = valores.CantidadUnidades;
            _context.Campanas.Update(campana);
            _context.SaveChanges();
            return PartialView(valores);
        }

        public IActionResult _TraeUnidades(int ProvinciaId)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName  == User.Identity.Name || ((ClaimsPrincipal)User).IsAdmin()); 
            ViewBag.Unidades = FINADICore.FINADI.TraeUnidadesCGE(usuario.Clientes.Empresa, _context, ProvinciaId).Unidades?.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            return PartialView();
        }
        public IActionResult DescargaExcel(int id)
        {
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaCampana.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                var renglones = _context.CampanasRenglones.Where(x => x.Cabecera.Id == id);
                int linea = 2;
                foreach(var renglon in renglones)
                {
                    workSheet.Cells[linea, 1].Value = renglon.DNI;
                    workSheet.Cells[linea, 2].Value = renglon.Apellido;
                    workSheet.Cells[linea, 3].Value = renglon.Nombres;
                    workSheet.Cells[linea, 4].Value = renglon.Disponible;
                    workSheet.Cells[linea, 5].Value = renglon.Unidad;
                    workSheet.Cells[linea, 6].Value = renglon.Provincia;
                    workSheet.Cells[linea, 7].Value = renglon.TipoPersona;
                    workSheet.Cells[linea, 8].Value = renglon.ImporteMaximo;
                    workSheet.Cells[linea, 9].Value = renglon.eMail;
                    workSheet.Cells[linea, 10].Value = renglon.Celular;
                    linea = linea + 1;
                }
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Campaña.xlsx");
            }
        }
        public IActionResult EnviaMail(int id)
        {
            var arranca = DateTime.Now;
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name || ((ClaimsPrincipal)User).IsAdmin());
            var renglones = _context.CampanasRenglones.Where(x => x.Cabecera.Id == id);
            foreach (var renglon in renglones)
            {
                //string imagen = Convert.ToBase64String(renglon.Cabecera.Imagen);
                string texto = "Hola " + renglon.Apellido + ", " + renglon.Nombres + "!!!<br>";
                texto = texto + renglon.Cabecera.TextoMail + "<br><br>";
                texto = texto + "Tu Monto Preaprobado es de $" + renglon.ImporteMaximo.ToString() + "!!!<br><br>";
                texto = texto + "Para acceder a este Prestamo debes descargar la App FINADI scaneando el QR o haciendo clic en la siguiente imagen<br><br>";
                texto = texto + "<a href='" + renglon.Cabecera.Link + "'>";
                //texto = texto + "<img src='data: image/gif;base64," + imagen + "'><br/></a><br><br>";
                texto = texto + "<img src='" + renglon.Cabecera.ImagenUrl + "'><br/></a><br><br>";
                if (renglon.eMail != null) 
                {
                    FINADICore.FINADI.EnviarMailEmpresa(renglon.Cabecera.Empresa, renglon.eMail, renglon.Cabecera.Observaciones, texto, "");
                    //common.EnviarMail(renglon.eMail,renglon.Cabecera.Observaciones,texto,"",renglon.Cabecera.Imagen);
                }
            }
            AddPageAlerts(PageAlertType.Success, renglones.Count().ToString() + " Mails Enviados");
            CampanaDTO valores = new CampanaDTO();
            valores.CantidadMinutos = Convert.ToInt32((DateTime.Now - arranca).TotalMinutes);
            valores.CantidadPersonas = renglones.Count();
            return PartialView(valores);
        }


        public IActionResult EnviaMailPrueba(int id)
        {
            var arranca = DateTime.Now;
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name || ((ClaimsPrincipal)User).IsAdmin());
            var renglones = _context.CampanasRenglones.Where(x => x.Cabecera.Id == id).FirstOrDefault();

            //string imagen = Convert.ToBase64String(FINADI.ResizeImagen(renglones.Cabecera.Imagen));
            string texto = "Hola " + renglones.Apellido + ", " + renglones.Nombres + "!!!<br>";
            texto = texto + renglones.Cabecera.TextoMail + "<br><br>";
            texto = texto + "Tu Monto Preaprobado es de $" + renglones.ImporteMaximo.ToString() + "!!!<br><br>";
            texto = texto + "Para acceder a este Prestamo debes descargar la App FINADI scaneando el QR o haciendo clic en la siguiente imagen<br><br>";
            texto = texto + "<a href='" + renglones.Cabecera.Link + "'>";
            //texto = texto + "<img src=data:image/jpg;base64," + imagen + "></a><br><br>";
            texto = texto + "<img src='" + renglones.Cabecera.ImagenUrl + "'></a><br><br>";

            //FINADI.EnviarMailEmpresa(renglones.Cabecera.Empresa, "acevedoruben@hotmail.com", renglones.Cabecera.Observaciones, texto, renglones.Cabecera.Imagen);
            //common.EnviarMail("acevedoruben@hotmail.com", renglones.Cabecera.Observaciones, texto, "", renglones.Cabecera.Imagen);
            //FINADI.EnviarMailEmpresa(usuario.UserName, renglones.Cabecera.Observaciones, texto, "", renglones.Cabecera.Imagen);
            FINADICore.FINADI.EnviarMailEmpresa(renglones.Cabecera.Empresa, "acevedoruben@hotmail.com", renglones.Cabecera.Observaciones, texto, "");
            //FINADI.EnviarMailEmpresa(renglones.Cabecera.Empresa, "alangabito89@gmail.com", renglones.Cabecera.Observaciones, "", renglones.Cabecera.Imagen);
            FINADICore.FINADI.EnviarMailEmpresa(renglones.Cabecera.Empresa, usuario.UserName, renglones.Cabecera.Observaciones, texto, "");
            AddPageAlerts(PageAlertType.Success, 1 + " Mails Enviados");
            CampanaDTO valores = new CampanaDTO();
            valores.CantidadMinutos = Convert.ToInt32((DateTime.Now - arranca).TotalMinutes);
            valores.CantidadPersonas = 1;
            return PartialView(valores);
        }

        /*
        public IActionResult EnviaMailPrueba(int id)
        {
            var arranca = DateTime.Now;
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name || ((ClaimsPrincipal)User).IsAdmin());
            var renglones = _context.CampanasRenglones.Where(x => x.Cabecera.Id == id).FirstOrDefault() ;
           
                string texto = "Hola " + renglones.Apellido + ", " + renglones.Nombres + "!!!<br>";
                texto = texto + renglones.Cabecera.TextoMail + "<br><br>";
                texto = texto + "Tu Monto Preaprobado es de $" + renglones.ImporteMaximo.ToString() + "!!!<br><br>";
                texto = texto + "Para acceder a este Prestamo debes descargar la App FINADI scaneando el QR o desdes el siguiente link<br><br>";
                texto = texto + "<a href='https://play.google.com/store/apps/details?id=ar.com.FINADI&hl=es_AR&gl=US'>Descarga Android</a><br><br>";

            //FINADI.EnviarMailEmpresa(renglones.Cabecera.Empresa, "acevedoruben@hotmail.com", renglones.Cabecera.Observaciones, texto, renglones.Cabecera.Imagen);
            common.EnviarMail(usuario.UserName, renglones.Cabecera.Observaciones, texto, "", renglones.Cabecera.Imagen);
           
            AddPageAlerts(PageAlertType.Success, 1 + " Mails Enviados");
            CampanaDTO valores = new CampanaDTO();
            valores.CantidadMinutos = Convert.ToInt32((DateTime.Now - arranca).TotalMinutes);
            valores.CantidadPersonas = 1;
            return PartialView(valores);
        }*/
    }
}