using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class OrganismoController : FINADICoreController
    {
        public OrganismoController(FINADIContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Organismos" });
            ViewBag.Breadcrumb = breadcumb;
            var organismos = _context.Organismos.ToList();
            return View(organismos);
        }

        public ActionResult _Create()
        {
            OrganismoViewBag();
            return PartialView();
        }       

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Descripcion, CUIT, Localidad, Provincia, Tlelefono, CodigoPostal, Domicilio, Orden, CodigoDescuento, Activo")] Organismo organismo)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                   // organismo.Activo = true;
                    organismo.Provincia = _context.Provincia.Find(organismo.Provincia.Id);
                    organismo.Localidad = _context.Localidad.Find(organismo.Localidad.Id);
                    await _context.Organismos.AddAsync(organismo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Organismo" + organismo.Descripcion + ".");
                    return RedirectToAction("Index", "Organismo");
                }
                else
                {
                    return PartialView(organismo);
                }

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Organismo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Organismo");
            }
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Organismo organismo = _context.Organismos.Where(s => s.Id == id).First();
                organismo.Activo = false;
                _context.Organismos.Update(organismo);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se desactivo correctamente el Organismo " + organismo.Descripcion + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Organismo.");
                return RedirectToAction("Index", "Organismo");
            }
        }

        public ActionResult _Update(int id)
        {
            OrganismoViewBag();
            return PartialView(_context.Organismos.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(Organismo organismo)
        {
            try
            {                
                if (ModelState.IsValid)
                {
                    organismo.Provincia = _context.Provincia.Find(organismo.Provincia.Id);
                    organismo.Localidad = _context.Localidad.Find(organismo.Localidad.Id);
                    _context.Organismos.Update(organismo);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Organismo. " + organismo.Descripcion + ".");
                    return RedirectToAction("Index", "Organismo");
                }
                else
                {
                    OrganismoViewBag();
                    return PartialView(organismo);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Organismo.. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Organismo");
            }
        }

        private void OrganismoViewBag()
        {
            var provincias = _context.Provincia.ToList();
            ViewBag.Provincias = provincias.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            ViewBag.Localidades = (provincias != null && provincias.Count() > 0) ? _context.Localidad.Where(x => x.IdProvincia == provincias.First().Id).Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() }) : _context.Localidad.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
        }

        [HttpPost]
        public string SelectLocalidades(int id)
        {
            string array = "";
            var localidad = _context.Localidad.Where(x => x.IdProvincia == id).ToList();
            foreach (var loc in localidad)
            {
                array += "<option value='" + loc.Id + "'>" + loc.Descripcion + "</option>";
            }

            return array;
        }
    }
}
