using FINADICore.Controllers;
using Castle.Core.Internal;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class LineasPrestamosController : FINADICoreController
    {
        private int LineaLineaPrestamoId;
        private IHostingEnvironment _env;
        public LineasPrestamosController(FINADIContext context, IHostingEnvironment env) : base(context, env)
        {
            _env = env;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index(string LineaPrestamoId, string TipoPersonaLineaPrestamo = null)
        {
            breadcumb.Add(new Message() { DisplayName = "Lineas de Prestamos" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.LineaPrestamoId = LineaPrestamoId;
            ViewBag.TipoPersonaLineaPrestamo = TipoPersonaLineaPrestamo;
            return View();
        }

        public async Task<IActionResult> _ListadoLineasPrestamos(Page<LineasPrestamos> page)
        {
            var c =  _context.LineasPrestamos.Where(x=>x.FechaBaja == null).Count();
            
            var lineasPrestamos = _context.LineasPrestamos.Where(x => x.FechaBaja == null);

            if (c < 1) { c = 1; }
            page.SelectPage("/LineasPrestamos/_ListadoLineasPrestamos",
                lineasPrestamos, c);

            return PartialView("_ListadoLineasPrestamos", page);
        }

        public IActionResult _ListadoLineasPrestamosPlanes(int id)
        {
            ViewBag.LineaPrestamoId = id;
            return PartialView("_ListadoLineasPrestamosPlanes");
        }
        public async Task<IActionResult> LineasPrestamosPlanesDataTable(int id)
        {
            List<LineasPrestamosPlanes> LineasPrestamosPlanes = new List<LineasPrestamosPlanes>();
            LineasPrestamosPlanes = await _context.LineasPrestamosPlanes.Where(s => s.Linea.Id == id && s.Activo == true).ToListAsync();
            var query = from c in LineasPrestamosPlanes
                        select new LineasPrestamosPlanesDTO
                        {
                            Id = c.Id,
                            MontoPrestado = c.MontoPrestado,
                            CantidadCuotas = c.CantidadCuotas,
                            MontoCuota = c.MontoCuota,
                            MargenDisponible = c.MargenDisponible
                        };
            return DataTable(query.AsQueryable());
        }


        public async Task<IActionResult> _ListadoLineasPrestamosTipoPersona(int id)
        {
            ViewBag.LineaPrestamoId = id;
            return PartialView("_ListadoLineasPrestamosTipoPersona");
        }


        public async Task<IActionResult> LineasPrestamosTipoPersonasDataTable(int id)
        {

            List<LineasPrestamosTiposPersonas> tipoPersona = new List<LineasPrestamosTiposPersonas>();
            tipoPersona = await _context.LineasPrestamosTiposPersonas.Where(x=>x.LineaPrestamo.Id==id).ToListAsync();
            var query = from c in tipoPersona
                        select new LineasPrestamosTipoPersonaDTO
                        {
                            Id = c.Id,
                            NombreTipoPersona = c.TipoPersona.nombre,
                            Organismo = c.TipoPersona.Organismo.Descripcion,
                        };
            return DataTable(query.AsQueryable());
        }
        [HttpGet]
        public IActionResult _AddTipoPersonaLineasPrestamos(int id)
        {
            ViewBag.LineaPrestamoId = id;
            List<TiposPersonas> listaTiposPersonas = _context.LineasPrestamosTiposPersonas.Where(x => x.LineaPrestamo.Id == id).Select(x=>x.TipoPersona).ToList();
            ViewBag.TipoPersonas = _context.TiposPersonas.Where(x=>!listaTiposPersonas.Contains(x)).Select(g => new SelectListItem() { Text = g.nombre +" - "+g.Organismo.Descripcion, Value = g.Id.ToString() });
            return PartialView();
        }

        [HttpPost]
        public IActionResult _AddTipoPersonaLineasPrestamos(LineasPrestamosTiposPersonas lineasPrestamosTiposPersonas, int[] tiposPersonas)
        {
            try
            {
                LineasPrestamos linea = _context.LineasPrestamos.Find(lineasPrestamosTiposPersonas.LineaPrestamo.Id);  
                foreach (var itemTipoPersonaId in tiposPersonas)
                {
                    LineasPrestamosTiposPersonas prestamoTipoPersona = new LineasPrestamosTiposPersonas();
                    prestamoTipoPersona.Id = 0;
                    prestamoTipoPersona.LineaPrestamo = linea;
                    prestamoTipoPersona.TipoPersona = _context.TiposPersonas.Find(itemTipoPersonaId);
                    _context.Add(prestamoTipoPersona);
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al Agregar el Tipo Persona a la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "LineasPrestamos", new { @TipoPersonaLineaPrestamo = lineasPrestamosTiposPersonas.LineaPrestamo.Id });
            }
            AddPageAlerts(PageAlertType.Success, "Se Agrego correctamente correctamente el Tipo Persona a la Linea de Prestamo.");
            return RedirectToAction("Index", "LineasPrestamos", new { @TipoPersonaLineaPrestamo = lineasPrestamosTiposPersonas.LineaPrestamo.Id });
        }

        [HttpGet]
        public IActionResult DeleteTipoPersonaLineasPrestamos(int id)
        {
            LineasPrestamosTiposPersonas linea = _context.LineasPrestamosTiposPersonas.Find(id);
            int idPrestamo = linea.LineaPrestamo.Id;
            try
            {                
                _context.Remove(linea);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se borro correctamente el Tipo de Persona de la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "LineasPrestamos", new { @TipoPersonaLineaPrestamo = idPrestamo });
            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al Borrar el Tipo Persona de la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "LineasPrestamos", new { @TipoPersonaLineaPrestamo = idPrestamo });
            }
        }

        public IActionResult _Create()
        {
            LineasPrestamosViewBag();
            return PartialView();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(LineasPrestamos lineaPrestamo)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    lineaPrestamo.SistemaFinanciacion = await _context.SistemasFinanciacion.FindAsync(lineaPrestamo.SistemaFinanciacion.Id);
                    lineaPrestamo.Moneda = await _context.Monedas.FindAsync(lineaPrestamo.Moneda.Id);
                    await _context.LineasPrestamos.AddAsync(lineaPrestamo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Linea de Prestamo " + lineaPrestamo.Nombre + ".");
                    return RedirectToAction("Index", "LineasPrestamos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LineasPrestamos");
                }
            }
            else
            {
                LineasPrestamosViewBag();
                return PartialView(lineaPrestamo);
            }
        }
        [HttpGet]
        public async Task<IActionResult> _CreatePlanes(string Id)
        {
            ViewBag.LineaPrestamoId = Id;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _CreatePlanes(LineasPrestamosPlanes lineaPrestamoplanes, int LineaPrestamoId)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    LineasPrestamos lineaprestamos = await _context.LineasPrestamos.FindAsync(LineaPrestamoId);
                    lineaPrestamoplanes.Linea = lineaprestamos;
                    lineaPrestamoplanes.Id = 0;
                    lineaPrestamoplanes.Activo = true;

                    await _context.LineasPrestamosPlanes.AddAsync(lineaPrestamoplanes);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Plan para la Linea de Prestamo .");
                    return RedirectToAction("Index", new { @LineaPrestamoId = LineaPrestamoId });
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", new { @LineaPrestamoId = LineaPrestamoId });
                }
            }
            else
            {
                LineasPrestamosViewBag();
                return PartialView(lineaPrestamoplanes);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {
            LineasPrestamosViewBag();
            LineasPrestamos lineaPrestamo = await _context.LineasPrestamos.FindAsync(Id);
            return PartialView(lineaPrestamo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(LineasPrestamos lineaPrestamo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    lineaPrestamo.SistemaFinanciacion = await _context.SistemasFinanciacion.FindAsync(lineaPrestamo.SistemaFinanciacion.Id);
                    lineaPrestamo.Moneda = await _context.Monedas.FindAsync(lineaPrestamo.Moneda.Id);
                    lineaPrestamo.TipoDescuentoCGEId = 1;
                    _context.LineasPrestamos.Update(lineaPrestamo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Linea de Prestamo " + lineaPrestamo.Nombre + ".");
                    return RedirectToAction("Index", "LineasPrestamos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LineasPrestamos");
                }

            }
            else
            {
                LineasPrestamosViewBag();
                return PartialView(lineaPrestamo);
            }
        }
        public async Task<IActionResult> _UpdatePlanes(int Id)
        {
            LineasPrestamosPlanes lineaPrestamoPlanes = await _context.LineasPrestamosPlanes.FindAsync(Id);
            ViewBag.LineaPrestamoId = lineaPrestamoPlanes.Linea.Id ;
            return PartialView(lineaPrestamoPlanes);
           
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _UpdatePlanes(LineasPrestamosPlanes lineaPrestamoplanes,int LineaPrestamoId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    lineaPrestamoplanes.Activo = true;
                     _context.LineasPrestamosPlanes.Update(lineaPrestamoplanes);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se Actualizo correctamente el Plan para la Linea de Prestamo .");
                    return RedirectToAction("Index", new { @LineaPrestamoId = LineaPrestamoId }); ;
                    
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Linea de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "LineasPrestamos");
                }

            }
            else
            {
                LineasPrestamosViewBag();
                return PartialView(lineaPrestamoplanes);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                LineasPrestamos lineaPrestamo = _context.LineasPrestamos.Where(s => s.Id == id).First();
                //_context.LineasPrestamos.Remove (lineaPrestamo);
                lineaPrestamo.FechaBaja = DateTime.Now;
                _context.LineasPrestamos.Update(lineaPrestamo);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Linea de Prestamo.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Linea de Prestamo.");
                return RedirectToAction("Index", "LineasPrestamos");
            }
        }
        public IActionResult DeletePlanes(int id)
        {
            try
            {
                LineasPrestamosPlanes lineaPrestamoPlanes = _context.LineasPrestamosPlanes.Where(s => s.Id == id).First();
                LineaLineaPrestamoId = lineaPrestamoPlanes.Linea.Id;
                lineaPrestamoPlanes.Activo = false;
                _context.LineasPrestamosPlanes.Update (lineaPrestamoPlanes);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Plan de la Linea de Prestamo.");
                return RedirectToAction("Index", new { @LineaPrestamoId = LineaLineaPrestamoId });
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Plan de la Linea de Prestamo.");
                return RedirectToAction("Index", "LineasPrestamos");
            }
        }
        private void LineasPrestamosViewBag()
        {
            ViewBag.SistemaFinanciacion = _context.SistemasFinanciacion.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            ViewBag.Moneda = _context.Monedas.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> _EditaValores(int id)
        {
            ViewBag.LineaPrestamoId = id;
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> _LineasPrestamosPlanesExell(IFormFile file, int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var lineaprestamos = await _context.LineasPrestamos.FindAsync(id);
            
            if (lineaprestamos == null || file == null) return NotFound();
            List<LineasPrestamosPlanes> lineasPrestamosplanes = new List<LineasPrestamosPlanes>();
            lineasPrestamosplanes = await _context.LineasPrestamosPlanes.Where(x => x.Linea.Id == id && x.Activo == true).ToListAsync();
            
            if (lineasPrestamosplanes != null )
            {
                foreach (var valor in lineasPrestamosplanes)
                {
                    valor.Activo = false;
                }
                _context.LineasPrestamosPlanes.UpdateRange(lineasPrestamosplanes);
                _context.SaveChanges();
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                using (var package = new ExcelPackage(memoryStream))
                {
                    decimal flagCFT = 0;
                    decimal flagTEM = 0;
                    decimal flagTNA = 0;
                    var workSheet = package.Workbook.Worksheets.First();
                    var noOfRow = workSheet.Dimension.End.Row;
                    List<LineasPrestamosPlanes> lineasPrestamo = new List<LineasPrestamosPlanes>();
                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        var renglon = new LineasPrestamosPlanes();
                        renglon.Linea = lineaprestamos;
                        renglon.MontoPrestado = 0;
                        renglon.CantidadCuotas = 0;
                        renglon.MontoCuota = 0;
                        renglon.CFT = 0;
                        renglon.MargenDisponible = 0;
                        renglon.TEM = 0;
                        renglon.TNA = 0;
                        renglon.CapitalFINADI = 0;

                        if (workSheet.Cells[rowIterator, 1].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            renglon.MontoPrestado = Convert.ToDecimal(workSheet.Cells[rowIterator, 1].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 2].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            renglon.CantidadCuotas = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 3].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            renglon.MontoCuota = Convert.ToDecimal(workSheet.Cells[rowIterator, 3].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 4].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            if (flagCFT == 0 && Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value.ToString())!=0)
                            {
                                flagCFT = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value.ToString());
                            }
                            renglon.CFT = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 5].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            renglon.MargenDisponible = Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 6].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            if (flagTEM == 0 && Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value.ToString()) != 0)
                            {
                                flagTEM = Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value.ToString());
                            }
                            renglon.TEM = Convert.ToDecimal(workSheet.Cells[rowIterator, 6].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 7].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            if (flagTNA == 0 && Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value.ToString()) != 0)
                            {
                                flagTNA = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value.ToString());
                            }
                            renglon.TNA = Convert.ToDecimal(workSheet.Cells[rowIterator, 7].Value.ToString());
                        }
                        if (workSheet.Cells[rowIterator, 8].Value != null && !workSheet.Cells[rowIterator, 1].IsNullOrEmpty())
                        {
                            renglon.CapitalFINADI = Convert.ToDecimal(workSheet.Cells[rowIterator, 8].Value.ToString());
                        }
                        renglon.Activo = true;
                        renglon.UsuarioCarga = usuario;
                        lineasPrestamo.Add(renglon);
                    }
                    var lineasSinCFT = lineasPrestamo.Where(x => x.CFT == 0).ToList();
                    foreach(var s in lineasSinCFT)
                    {
                        s.CFT = flagCFT;
                    }
                    if (lineasPrestamo.Count() > 0)
                    {
                        _context.LineasPrestamosPlanes.AddRange(lineasPrestamo);
                        _context.SaveChanges();
                    }
                }
            }
            lineaprestamos.TipoDescuentoCGEId = 1;
            _context.Update(lineaprestamos);
            _context.SaveChanges();
            return RedirectToAction("Index", "LineasPrestamos");
        }
        public ActionResult _BajaPlantilla(int id)
        {
            var valores = _context.LineasPrestamosPlanes.Where(x => x.Linea.Id == id && x.Activo ==true);

            string plantillaPath = _env.WebRootPath + "/Plantillas/PlantillaPlanesPrestamos.xlsx";

            using (var package = new ExcelPackage(new FileStream(
                plantillaPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite)))
            {
                var currentSheet = package.Workbook.Worksheets;
                var workSheet = currentSheet.First();

                if (!valores.IsNullOrEmpty())
                {

                    int i = 2;

                    foreach (var valor in valores)
                    {
                        workSheet.Cells[i, 1].Value = valor?.MontoPrestado;
                        workSheet.Cells[i, 2].Value = valor?.CantidadCuotas;
                        workSheet.Cells[i, 3].Value = valor?.MontoCuota ;
                        workSheet.Cells[i, 4].Value = valor?.CFT;
                        workSheet.Cells[i, 5].Value = valor?.MargenDisponible ;
                        workSheet.Cells[i, 6].Value = valor?.TEM;
                        workSheet.Cells[i, 7].Value = valor?.TNA;
                        workSheet.Cells[i, 8].Value = valor?.CapitalFINADI;
                        i++;

                    }
                }
                var content = new System.IO.MemoryStream(package.GetAsByteArray());

                var contentType = "APPLICATION/octet-stream";
                var fileName = "PlantillaLineasPlanes.xlsx";
                return File(content, contentType, fileName);
            }
        }

    }
}
