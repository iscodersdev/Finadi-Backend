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
    public class SistemasFinanciacionController : FINADICoreController
    {
        public SistemasFinanciacionController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Sistemas de Financiación" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoSistemasFinanciacion(Page<SistemasFinanciacion> page)
        {
            var c = _context.SistemasFinanciacion.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoSistemasFinanciacion",
                _context.SistemasFinanciacion, c);

            return PartialView("_ListadoSistemasFinanciacion", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(SistemasFinanciacion sistemasFinanciacion)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {                
                try
                {
                    await _context.SistemasFinanciacion.AddAsync(sistemasFinanciacion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Sistemas de Financiación " + sistemasFinanciacion.Nombre + ".");
                    return RedirectToAction("Index", "SistemasFinanciacion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Sistemas de Financiación. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "SistemasFinanciacion");
                }                
            }
            else
            {
                return PartialView(sistemasFinanciacion);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            SistemasFinanciacion sistemasFinanciacion = await _context.SistemasFinanciacion.FindAsync(Id);
            return PartialView(sistemasFinanciacion);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(SistemasFinanciacion sistemasFinanciacion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.SistemasFinanciacion.Update(sistemasFinanciacion);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Sistemas de Financiación " + sistemasFinanciacion.Nombre + ".");
                    return RedirectToAction("Index", "SistemasFinanciacion");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Sistemas de Financiación. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "SistemasFinanciacion");
                }
                
            }
            else
            {
                return PartialView(sistemasFinanciacion);
            }
        }

        public IActionResult Delete(int id)
        {
            try
            {
                SistemasFinanciacion sistemaFinanciacion = _context.SistemasFinanciacion.Where(s => s.Id == id).First();
                _context.SistemasFinanciacion.Remove(sistemaFinanciacion);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Sistema de Financiacion.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Sistema de Financiacion.");
                return RedirectToAction("Index", "SistemasFinanciacion");
            }
        }

    }
}
