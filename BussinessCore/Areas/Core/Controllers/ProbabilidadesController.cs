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
    public class ProbabilidadesController : FINADICoreController
    {
        public ProbabilidadesController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Específicos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Matriz - Probabilidades" });
            ViewBag.Breadcrumb = breadcumb;
            return View(_context.MatrizProbabilidades.ToList());
        }
        public IActionResult _Create()
        {
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(MatrizProbabilidades probabilidad)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.MatrizProbabilidades.AddAsync(probabilidad);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Probabilidad " + probabilidad.Nombre + ".");
                    return RedirectToAction("Index", "Probabilidades");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Probabilidad. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Probabilidades");
                }
            }
            else
            {
                return PartialView(probabilidad);
            }
        }
        public async Task<IActionResult> _Update(int Id)
        {            
            return PartialView(await _context.MatrizProbabilidades.FindAsync(Id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(MatrizProbabilidades probabilidad)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.MatrizProbabilidades.Update(probabilidad);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Probabilidad " + probabilidad.Nombre + ".");
                    return RedirectToAction("Index", "Probabilidades");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Probabilidad. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Probabilidades");
                }

            }
            else
            {
                return PartialView(probabilidad);
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                MatrizProbabilidades probabilidad = _context.MatrizProbabilidades.Where(s => s.Id == id).First();
                _context.MatrizProbabilidades.Remove(probabilidad);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Probabilidad.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Probabilidad.");
                return RedirectToAction("Index", "Probabilidades");
            }
        }
    }
}
