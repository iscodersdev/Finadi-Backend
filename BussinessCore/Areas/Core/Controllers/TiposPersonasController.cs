using Commons.Models;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using FINADICore.Controllers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.DTOs;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class TiposPersonasController : FINADICoreController
    {
        public TiposPersonasController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
            breadcumb.Add(new Message() { DisplayName = "Generales" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Tipos de Personas" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }
        public IActionResult TiposPersonasDataTable()
        {
            var tiposPersonas = _context.TiposPersonas.ToList();
            var query = from t in tiposPersonas
                        select new ListTiposPersonasDTO
                        {
                            Id = t.Id,
                            Nombre=t.nombre,
                            LimiteCuotas=t.LimiteCuotas,
                            TopePrestamo=t.TopePrestamo,
                            MontoAmpliacion=t.MontoAmpliacion,
                            Organismo=(t.Organismo!=null)?t.Organismo.Descripcion:"---",
                            Cuota=(t.Cuota!=null)?t.Cuota.ValorCuota.ToString():"---"
                        };
            return DataTable(query.AsQueryable());
        }
        public ActionResult _Create()
        {
            ViewBag.Organismo = _context.Organismos.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Create([Bind("nombre,LimiteCuotas,TopePrestamo,Organismo")] TiposPersonas tipoPersonas)
        {
            try
            {
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    tipoPersonas.Organismo = _context.Organismos.Find(tipoPersonas.Organismo.Id);
                    await _context.TiposPersonas.AddAsync(tipoPersonas);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Tipo de Persona " + tipoPersonas.nombre + ".");
                    return RedirectToAction("Index", "TiposPersonas");
                }
                else
                {
                    ViewBag.Organismo = _context.Organismos.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
                    return PartialView(tipoPersonas);
                }

            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Tipo de Persona. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposPersonas");
            }
        }
        
        public ActionResult _Delete(int id)
        {
            ViewBag.Organismo = _context.Organismos.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            return PartialView(_context.TiposPersonas.Where(s => s.Id == id).First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                TiposPersonas tiposPersonas = _context.TiposPersonas.Where(s => s.Id == id).First();
                _context.TiposPersonas.Remove(tiposPersonas);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Tipo de Persona " + tiposPersonas.nombre + ".");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Tipo de Persona.");
                return RedirectToAction("Index", "TiposPersonas");
            }
        }

        public ActionResult _Update(int id)
        {
            ViewBag.Organismo = _context.Organismos.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            return PartialView(_context.TiposPersonas.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async Task<ActionResult> _Update(TiposPersonas tiposPersonas)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tiposPersonas.Organismo = _context.Organismos.Find(tiposPersonas.Organismo.Id);
                    _context.TiposPersonas.Update(tiposPersonas);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente el Tipo de Persona.");
                    return RedirectToAction("Index", "TiposPersonas");
                }
                else
                {
                    ViewBag.Organismo = _context.Organismos.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
                    return PartialView(tiposPersonas);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Tipo de Persona. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposPersonas");
            }
        }
        public async Task<IActionResult> _Cuota(int id)
        {
            CuotaSocial cuota = new CuotaSocial();
            TiposPersonas tipoPersona = await _context.TiposPersonas.FindAsync(id);
            if (tipoPersona.Cuota != null)
            {
                cuota = tipoPersona.Cuota;
            }
            else
            {   
                cuota.TipoPersona = id;
            }
            return PartialView(cuota);
        }
        [HttpPost]
        public async Task<ActionResult> _Cuota(CuotaSocial newCuota)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    newCuota.Id = 0;
                    TiposPersonas tipoPersonaCuota = await _context.TiposPersonas.FindAsync(newCuota.TipoPersona);
                    newCuota.ValorCuotaEnLetras = common.NumeroALetrasNew(newCuota.ValorCuota) + " Pesos";
                    await _context.CuotasSociales.AddAsync(newCuota);
                    tipoPersonaCuota.Cuota = newCuota;
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente la cuota social del Tipo de Persona " + tipoPersonaCuota.nombre + ".");
                    return RedirectToAction("Index", "TiposPersonas");
                }
                else
                {
                    return PartialView(newCuota);
                }
            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la Cuota Social del Tipo de Persona. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "TiposPersonas");
            }
        }
    }
}
