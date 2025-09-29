using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class PrestamosController : FINADICoreController
    {
        public PrestamosController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Gestión" });
        }
        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Prestamos por Vendedor" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.Desde = DateTime.Today.AddDays(DateTime.Today.Day * -1);
            ViewBag.Hasta = DateTime.Now;
            return View();
        }

        public IActionResult PrestamosVendedorDataTable(DateTime? Desde, DateTime? Hasta)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            List<Prestamos> prestamos = new List<Prestamos>();
            if (usuario.Vendedores != null)
            {
                var query = _context.Prestamos.Where(x => x.Vendedor.Id == usuario.Vendedores.Id && x.FechaAnulacion == null);
                if (Desde != null)
                    query = query.Where(e => Convert.ToDateTime(e.FechaSolicitado).Date >= Desde);

                if (Hasta != null)
                    query = query.Where(e => Convert.ToDateTime(e.FechaSolicitado).Date <= Hasta);

                query = query.Where(x => x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null);
                prestamos = query.ToList();

            }
            var respuesta = from x in prestamos
                            select new DAL.DTOs.PrestamosDTO
                            {
                                CantidadCuotas = x.CantidadCuotas,
                                Capital = x.Capital,
                                Categoria = x.Cliente.CategoriaLaboral,
                                Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres,
                                Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"),
                                Id = x.Id,
                                MontoCuota = x.MontoCuota,
                                Tipo = x.Linea.Nombre,
                                Estado = x.EstadoActual.Nombre,
                                EstadoId = x.EstadoActual.Id,
                                DNI = x.Cliente.Persona.NroDocumento.ToString()
                            };
            return DataTable(respuesta.AsQueryable<DAL.DTOs.PrestamosDTO>());
        }

        public IActionResult _DescargaExcel(DateTime? Desde, DateTime? Hasta)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/Plantillas/PlantillaPrestamosVendedor.xlsx"));
            using (var package = new ExcelPackage(memoryStream))
            {
                decimal total = 0;
                var workSheet = package.Workbook.Worksheets[1];
                List<DAL.DTOs.PrestamosDTO> renglones = new List<DAL.DTOs.PrestamosDTO>();
                if (usuario.Vendedores != null)
                {
                    renglones = _context.Prestamos.Where(x => x.Vendedor.Id==usuario.Vendedores.Id && Convert.ToDateTime(x.FechaSolicitado).Date >= Desde && Convert.ToDateTime(x.FechaSolicitado).Date <= Hasta && x.FechaAnulacion == null && (x.FirmaOlografica != null || x.FirmaOlograficaConfirmacion != null)).Select(x => new DAL.DTOs.PrestamosDTO { CantidadCuotas = x.CantidadCuotas, Capital = x.Capital, Categoria = x.Cliente.CategoriaLaboral, Cliente = x.Cliente.Persona.Apellido + ", " + x.Cliente.Persona.Nombres, Fecha = Convert.ToDateTime(x.FechaSolicitado).ToString("yyyy-MM-dd HH:mm"), Id = x.Id, MontoCuota = x.MontoCuota, Tipo = x.Linea.Nombre, Estado = x.EstadoActual.Nombre, EstadoId = x.EstadoActual.Id, DNI = x.Cliente.Persona.NroDocumento.ToString(), Celular = x.Cliente.Celular, eMail = x.Cliente.Usuario.Mail }).ToList();
                }
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
                if (renglones.Count() > 0)
                {
                    workSheet.Cells[linea, 4].Value = "Cantidad:";
                    workSheet.Cells[linea, 5].Value = linea - 2;
                    workSheet.Cells[linea, 6].Value = "Total del Periodo:";
                    workSheet.Cells[linea, 7].Value = total;
                }                
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Prestamos del Vendedor del " + Convert.ToDateTime(Desde).ToString("dd_MM_yyyy") + " al " + Convert.ToDateTime(Hasta).ToString("dd_MM_yyyy") + ".xlsx");
            }
        }
        [HttpGet]
        public IActionResult _LegajoOnline(int Id)
        {
            var prestamo = _context.Prestamos.Find(Id);
            string parametros = "&prestamoid=" + prestamo.Id;
            @ViewBag.PDF = "data:application/pdf;base64," + Convert.ToBase64String(common.Reporting("cre_legajo", parametros, "PDF",_context));
            return PartialView();
        }
    }
}
