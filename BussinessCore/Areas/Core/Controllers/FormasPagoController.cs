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
    public class FormasPagoController : FINADICoreController
    {
        public FormasPagoController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Fromas de Pago" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> _ListadoFormasPago(Page<FormasPago> page)
        {
            var c = _context.FormasPago.Count();
            if (c < 1) { c = 1; }
            page.SelectPage("/Clientes/_ListadoFormasPago",
                _context.FormasPago, c);

            return PartialView("_ListadoFormasPago", page);
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(FormasPago formaPago)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.FormasPago.AddAsync(formaPago);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente la Forma de Pago " + formaPago.Nombre + ".");
                    return RedirectToAction("Index", "FormasPago");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear la Forma de Pago. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "FormasPago");
                }
            }
            else
            {
                return PartialView(formaPago);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            FormasPago formaPago = await _context.FormasPago.FindAsync(Id);
            return PartialView(formaPago);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(FormasPago formaPago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.FormasPago.Update(formaPago);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Forma de Pago " + formaPago.Nombre + ".");
                    return RedirectToAction("Index", "FormasPago");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Forma de Pago. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "FormasPago");
                }

            }
            else
            {
                return PartialView(formaPago);
            }
        }


        public IActionResult Delete(int id)
        {
            try
            {
                FormasPago formaPago = _context.FormasPago.Where(s => s.Id == id).First();
                _context.FormasPago.Remove(formaPago);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente la Forma de Pagos.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar la Forma de Pagos.");
                return RedirectToAction("Index", "FormasPago");
            }
        }

    }
}
