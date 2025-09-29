using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BussinessCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class TiposMovimientosController : FINADICoreController
    {
        public TiposMovimientosController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Generales" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Tipos de Movimientos" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult TiposMovimientosDataTable()
        {
            var tiposMovimientos = _context.TiposMovimientos.ToList();
            var query = from t in tiposMovimientos
                        select new ListTiposMovimientosDTO
                        {
                            Id = t.Id,
                            Nombre = t.Nombre,
                            Credito = (t.Credito) ? "SI" : "NO",
                            Debito = (t.Debito) ? "SI" : "NO"
                        };
            return DataTable(query.AsQueryable());
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Nombre,Credito,Debito")] TiposMovimientos tiposMovimientos)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.TiposMovimientos.AddAsync(tiposMovimientos);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Tipo de Movimiento " + tiposMovimientos.Nombre + ".");
                    return RedirectToAction("Index", "TiposMovimientos");
                }
                else
                {
                    return PartialView(tiposMovimientos);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Tipo de Movimiento. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposMovimientos");
            }
        }

        public ActionResult _Delete(int id)
        {
            return PartialView(_context.TiposMovimientos.Where(s => s.Id == id).First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                TiposMovimientos tiposMovimientos = _context.TiposMovimientos.Where(s => s.Id == id).First();
                _context.TiposMovimientos.Remove(tiposMovimientos);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Tipo de Movimiento " + tiposMovimientos.Nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Tipo de Movimiento.");
                return RedirectToAction("Index", "TiposMovimientos");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.TiposMovimientos.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(TiposMovimientos tiposMovimientos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.TiposMovimientos.Update(tiposMovimientos);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Tipo de Movimiento.");
                    return RedirectToAction("Index", "TiposMovimientos");
                }
                else
                {
                    return PartialView(tiposMovimientos);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Tipo de Movimiento. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposMovimientos");
            }
        }
    }
}
