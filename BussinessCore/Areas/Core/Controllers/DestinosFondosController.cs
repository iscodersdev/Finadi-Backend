using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class DestinosFondosController : FINADICoreController
    {
        public DestinosFondosController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Destinos de Fondos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoDestinosFondos(Page<DestinosFondos> page)
        {
            var c = _context.DestinoFondos.Where(x=>x.Activo).Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoDestinosFondos",
                _context.DestinoFondos.Where(x => x.Activo), c);

            return PartialView("_ListadoDestinosFondos", page);
        }

        public IActionResult _Create()
        {
            DestinoFondosDTO modelo = new DestinoFondosDTO();
            modelo = CreateDestinoFondosDTO(modelo);
            return PartialView(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create([Bind("Nombre, RubrosSeleccionados")] DestinoFondosDTO nuevoDestino)
        {
            try
            {
                if (nuevoDestino.RubrosSeleccionados == null || nuevoDestino.RubrosSeleccionados.Count() < 1)
                {
                    ModelState.AddModelError("RubrosSeleccionados", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    DestinosFondos destino = new DestinosFondos
                    {
                        Nombre = nuevoDestino.Nombre,                        
                        Activo = true
                    };
                    await _context.DestinoFondos.AddAsync(destino);
                    foreach (int r in nuevoDestino.RubrosSeleccionados)
                    {
                        DestinosFondosRubros rubro = new DestinosFondosRubros
                        {
                            DestinosFondos = destino,
                            Rubro = await _context.Rubros.FindAsync(r)
                        };
                        await _context.DestinoFondosRubros.AddAsync(rubro);
                    }
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Destino de Fondos " + destino.Nombre + ".");
                    return RedirectToAction("Index", "DestinosFondos");
                }
                else
                {
                    return PartialView(CreateDestinoFondosDTO(nuevoDestino));
                }

            }
            catch(Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Destino de Fondos. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "DestinosFondos");
            }           
        }


        public ActionResult _Update(int id)
        {
            DestinosFondos destino = _context.DestinoFondos.Find(id);
            if (destino != null)
            {
                DestinoFondosDTO editDestinoFondos = new DestinoFondosDTO();
                editDestinoFondos = CreateDestinoFondosDTO(editDestinoFondos, destino);
                return PartialView(editDestinoFondos);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Destino de Fondos. Intentelo nuevamente mas tarde.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Update([Bind("DestinoFondosId, Nombre, RubrosSeleccionados")] DestinoFondosDTO editDestinoFondos)
        {
            try
            {
                if (editDestinoFondos.RubrosSeleccionados == null || editDestinoFondos.RubrosSeleccionados.Count() < 1)
                {
                    ModelState.AddModelError("RubrosSeleccionados", "Debe seleccionar un Rubro");
                }
                if (ModelState.IsValid)
                {
                    DestinosFondos destinoFondos = await _context.DestinoFondos.FindAsync(editDestinoFondos.DestinoFondosId);
                    _context.DestinoFondosRubros.RemoveRange(destinoFondos.DestinosFondosRubros.Where(r => !editDestinoFondos.RubrosSeleccionados.Contains(r.Rubro.Id)).ToList());
                    foreach (int r in editDestinoFondos.RubrosSeleccionados.Where(x => !destinoFondos.DestinosFondosRubros.Select(y => y.Rubro.Id).ToList().Contains(x)))
                    {
                        DestinosFondosRubros rubro = new DestinosFondosRubros
                        {
                            DestinosFondos = destinoFondos,
                            Rubro = await _context.Rubros.FindAsync(r)
                        };
                        await _context.DestinoFondosRubros.AddAsync(rubro);
                    }

                    destinoFondos.Nombre = editDestinoFondos.Nombre;
                    destinoFondos.Activo = true;
                    _context.DestinoFondos.Update(destinoFondos);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Destino de Fondos.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return PartialView(CreateDestinoFondosDTO(editDestinoFondos));
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Destino de Fondos. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult _Delete(int id)
        {
            DestinosFondos destino = _context.DestinoFondos.Find(id);
            if (destino != null)
            {
                DestinoFondosDTO deleteDestinoFOndos = new DestinoFondosDTO();
                deleteDestinoFOndos = CreateDestinoFondosDTO(deleteDestinoFOndos, destino);
                return PartialView(deleteDestinoFOndos);
            }
            AddPageAlerts(PageAlertType.Error, "Hubo un error al borrar el Destino de Fondos. Intentelo nuevamente mas tarde.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int DestinoFondosId)
        {
            try
            {
                DestinosFondos destino = _context.DestinoFondos.Find(DestinoFondosId);
                if (destino != null)
                {
                    destino.Activo = false;
                    _context.DestinoFondos.Update(destino);
                    _context.SaveChanges();
                    AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Destino de Fondos " + destino.Nombre + ".");
                    return RedirectToAction(nameof(Index));
                }
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Destino de Fondos. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al eliminar el Destino de Fondos. Intentelo nuevamente mas tarde.");
                return RedirectToAction(nameof(Index));
            }
        }
        private DestinoFondosDTO CreateDestinoFondosDTO(DestinoFondosDTO destinoDto, DestinosFondos fondos = null)
        {
            destinoDto.Rubros = _context.Rubros.Where(x=>x.Activo).Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            if (fondos != null)
            {
                destinoDto.DestinoFondosId = fondos.Id;
                destinoDto.Nombre = fondos.Nombre;
                destinoDto.RubrosSeleccionados = fondos.DestinosFondosRubros.Select(x => x.Rubro.Id).ToList();
            }
            return destinoDto;
        }

    }
}
