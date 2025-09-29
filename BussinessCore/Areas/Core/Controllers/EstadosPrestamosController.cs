using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class EstadosPrestamosController : FINADICoreController
    {
        public EstadosPrestamosController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Estados de Prestamos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoEstadosPrestamos(Page<EstadosPrestamos> page)
        {
            var c = _context.EstadosPrestamos.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoEstadosPrestamos",
                _context.EstadosPrestamos, c);

            return PartialView("_ListadoEstadosPrestamos", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(EstadosPrestamos estadoPrestamo)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {                
                try
                {
                    await _context.EstadosPrestamos.AddAsync(estadoPrestamo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Estado de Prestamo " + estadoPrestamo.Nombre + ".");
                    return RedirectToAction("Index", "EstadosPrestamos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Estado de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "EstadosPrestamos");
                }                
            }
            else
            {
                return PartialView(estadoPrestamo);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            EstadosPrestamos estadosPrestamo = await _context.EstadosPrestamos.FindAsync(Id);
            return PartialView(estadosPrestamo);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(EstadosPrestamos estadosPrestamo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.EstadosPrestamos.Update(estadosPrestamo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Estado de Prestamo " + estadosPrestamo.Nombre + ".");
                    return RedirectToAction("Index", "EstadosPrestamos");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Estado de Prestamo. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "EstadosPrestamos");
                }
                
            }
            else
            {
                return PartialView(estadosPrestamo);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                EstadosPrestamos estadosPrestamos = _context.EstadosPrestamos.Where(s => s.Id == id).First();
                _context.EstadosPrestamos.Remove(estadosPrestamos);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Estados de Prestamo.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Estado de Prestamo.");
                return RedirectToAction("Index", "EstadosPrestamos");
            }
        }


    }
}
