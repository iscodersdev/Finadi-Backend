using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class LocalidadController : FINADICoreController
    {
        public LocalidadController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Localidad" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoLocalidades(Page<Localidad> page)
        {
            var c = _context.Localidad.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoLocalidades",
                _context.Localidad, c);

            return PartialView("_ListadoLocalidades", page);
        }

        public IActionResult _Create()
        {
            ViewBag.Provincias = _context.Provincia.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Localidad localidad, int provinciaId)
        {
            ModelState.Remove("Id");
            if (provinciaId == 0)
            {
                ModelState.AddModelError("ProvinciaNombre", "Debe Sellecionar una Provincia");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Provincia provincia = _context.Provincia.Find(provinciaId);
                    localidad.IdProvincia = provincia.Id;
                    localidad.ProvinciaNombre = provincia.Descripcion;
                    await _context.Localidad.AddAsync(localidad);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Localidad " + localidad.Descripcion + ".");
                    return RedirectToAction("Index", "Localidad");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Localidad. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Localidad");
                }
            }
            else
            {
                ViewBag.Provincias = _context.Provincia.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
                return PartialView(localidad);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {
            ViewBag.Provincias = _context.Provincia.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            Localidad localidad = await _context.Localidad.FindAsync(Id);
            return PartialView(localidad);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Localidad localidad, int provinciaId)
        {
            if (provinciaId == 0)
            {
                ModelState.AddModelError("ProvinciaNombre", "Debe Sellecionar una Provincia");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Provincia provincia = _context.Provincia.Find(provinciaId);
                    localidad.IdProvincia = provincia.Id;
                    localidad.ProvinciaNombre = provincia.Descripcion;
                    _context.Localidad.Update(localidad);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Localidad " + localidad.Descripcion + ".");
                    return RedirectToAction("Index", "Localidad");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Localidad. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Localidad");
                }

            }
            else
            {
                ViewBag.Provincias = _context.Provincia.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
                return PartialView(localidad);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                Localidad localidad = _context.Localidad.Where(s => s.Id == id).First();
                _context.Localidad.Remove(localidad);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Localidad.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Localidad.");
                return RedirectToAction("Index", "Localidad");
            }
        }

    }
}
