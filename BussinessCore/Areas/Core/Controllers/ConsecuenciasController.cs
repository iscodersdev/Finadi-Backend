using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using FINADICore.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class ConsecuenciasController : FINADICoreController
    {
        public ConsecuenciasController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Específicos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Matriz - Consecuencias" });
            ViewBag.Breadcrumb = breadcumb;
            return View(_context.MatrizConsecuencias.ToList());
        }
        public IActionResult _Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(MatrizConsecuencias consecuencia)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.MatrizConsecuencias.AddAsync(consecuencia);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Consecuencia " + consecuencia.Nombre + ".");
                    return RedirectToAction("Index", "Consecuencias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Consecuencia. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Consecuencias");
                }
            }
            else
            {
                return PartialView(consecuencia);
            }
        }
        public async Task<IActionResult> _Update(int Id)
        {
            return PartialView(await _context.MatrizConsecuencias.FindAsync(Id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(MatrizConsecuencias consecuencia)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.MatrizConsecuencias.Update(consecuencia);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Consecuencia " + consecuencia.Nombre + ".");
                    return RedirectToAction("Index", "Consecuencias");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Consecuencia. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Consecuencias");
                }

            }
            else
            {
                return PartialView(consecuencia);
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                MatrizConsecuencias consecuencia = _context.MatrizConsecuencias.Where(s => s.Id == id).First();
                _context.MatrizConsecuencias.Remove(consecuencia);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Consecuencia.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Consecuencia.");
                return RedirectToAction("Index", "Consecuencias");
            }
        }
    }
}
