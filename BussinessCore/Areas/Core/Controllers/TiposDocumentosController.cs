using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BussinessCore.Areas.Core.Controllers
{
    [Area("Core")]
    public class TiposDocumentosController : FINADICoreController
    {
        public TiposDocumentosController(FINADIContext context) : base(context)
        {
            _context = context;
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Generales" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Tipos de Documento" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult TiposDocumentosDataTable()
        {
            var tiposDocumento = _context.TipoDocumento.ToList();
            var query = from v in tiposDocumento
                        select new ListTiposDocumentoDTO
                        {
                            Id = v.Id,
                            NroDocumento = v.Descripcion
                        };
            return DataTable(query.AsQueryable());
        }

        public ActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("Descripcion")] TipoDocumento tipoDocumento)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    await _context.TipoDocumento.AddAsync(tipoDocumento);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Tipo de Documento "+tipoDocumento.Descripcion+".");
                    return RedirectToAction("Index", "TiposDocumentos");
                }
                else
                {
                    return PartialView(tipoDocumento);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Tipo de Documento. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposDocumentos");
            }
        }

        public ActionResult _Delete(int id)
        {
            return PartialView(_context.TipoDocumento.Where(s => s.Id == id).First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                TipoDocumento tipoDocumento = _context.TipoDocumento.Where(s => s.Id == id).First();
                _context.TipoDocumento.Remove(tipoDocumento);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Tipo de Documento "+ tipoDocumento.Descripcion+ ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Tipo de Documento.");
                return RedirectToAction("Index", "TiposDocumentos");
            }
        }

        public ActionResult _Update(int id)
        {
            return PartialView(_context.TipoDocumento.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(TipoDocumento tipoDocumento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.TipoDocumento.Update(tipoDocumento);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Tipo de Documento.");
                    return RedirectToAction("Index", "TiposDocumentos");
                }
                else
                {
                    return PartialView(tipoDocumento);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Tipo de Documento. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposDocumentos");
            }
        }
    }
}
