using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using FINADICore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class EstadosCivilesController : FINADICoreController
    {
        public EstadosCivilesController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Estados Civiles" });
            ViewBag.Breadcrumb = breadcumb;

            var estadosCiviles = _context.EstadosCiviles.ToList();

            return View(estadosCiviles);
        }
        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Nombre")] EstadosCiviles estadosCivil)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.EstadosCiviles.AddAsync(estadosCivil);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Estado Civil " + estadosCivil.Nombre + ".");
                    return RedirectToAction("Index", "EstadosCiviles");
                }
                else
                {
                    return PartialView(estadosCivil);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Estado Civil. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "EstadosCiviles");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                EstadosCiviles estadosCivil = _context.EstadosCiviles.Where(s => s.Id == id).First();
                _context.EstadosCiviles.Remove(estadosCivil);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Estado Civil " + estadosCivil.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Estado Civil.");
                return RedirectToAction("Index", "EstadosCiviles");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.EstadosCiviles.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(EstadosCiviles estadosCivil)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.EstadosCiviles.Update(estadosCivil);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Estado Civil.");
                    return RedirectToAction("Index", "EstadosCiviles");
                }
                else
                {
                    return PartialView(estadosCivil);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Estado Civil. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "EstadosCiviles");
            }
        }
    }
}
