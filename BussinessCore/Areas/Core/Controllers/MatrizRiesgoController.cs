using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FINADICore.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class MatrizRiesgoController : FINADICoreController
    {
        public MatrizRiesgoController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Gestión" });
        }
        public ActionResult Index(int id)
        {
            breadcumb.Add(new Message() { DisplayName = "Matriz Riesgo" });
            ViewBag.Breadcrumb = breadcumb;
            ViewBag.MatrizRiesgoId = id;
            return View();
        }

        public IActionResult MatrizDataTable()
        {
            List<MatrizRiesgoCabecera> matrices = _context.MatrizRiesgoCabeceras.ToList();
            var query = from p in matrices
                        select new MatrizDTO
                        {
                            Id = p.Id,
                            Cliente = p.Cliente?.Persona?.Nombres+" "+p.Cliente?.Persona?.Apellido ,
                            Fecha = p.Fecha.ToShortDateString(),
                            Prestamo = p.Prestamo?.Linea?.Nombre+" - "+p.Prestamo?.DestinoFondos?.Nombre
                        };
            return DataTable<MatrizDTO>(query.AsQueryable<MatrizDTO>());
        }
        public ActionResult _Create()
        {
            MatrizViewBag(null);
            return PartialView();
        }
        private void MatrizViewBag(MatrizRiesgoCabecera matriz = null)
        {
            ViewBag.Prestamos = _context.Prestamos.Select(g => new SelectListItem() { Text = g.Linea.Nombre+""+g.DestinoFondos.Nombre, Value = g.Id.ToString() });
            if (matriz != null)
            {
                ViewBag.ClienteId = (matriz.Cliente != null) ? matriz.Cliente.Id.ToString() : "";
                ViewBag.ClienteDescripcion = (matriz.Cliente != null) ? matriz.Cliente.Persona.Nombres?.ToUpper() + " " + matriz.Cliente.Persona.Apellido?.ToUpper() : "";
            }
        }

        public IActionResult ClienteCombo(string term, int? id)
        {
            term = term.ToUpper();
            var items = _context.Clientes
                .Where(x => x.FechaBaja == null)
                .Where(x => (x.Persona.Nombres.ToUpper() + " " + x.Persona.Apellido.ToUpper()).Contains(term.ToUpper()));
            if (id != null)
            {
                items = items.Where(x => x.Id != id);
            }

            var list = items.Select(x => new
            {
                text = $"{x.Persona.Nombres.ToUpper() + " " + x.Persona.Apellido.ToUpper()}",
                id = x.Id
            })
            .Take(30).ToList();

            return Json(list);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Create([Bind("Cliente,Fecha,Prestamo,ResidenteZonaLimistrofe,FrecuenciaAnualCreditos,DeclaraBienesInmuebles,DeclaraBienesMueblesRegistrables,CuentaCorrientePesos,CuentaCorrienteDolares,OtrasInversiones")] MatrizRiesgoCabecera matriz)
        {
            try
            {
                if (matriz.Cliente == null)
                {
                    ModelState.AddModelError("Cliente", "Debe seleccionar un Cliente");
                }
                if (matriz.Prestamo == null)
                {
                    ModelState.AddModelError("Prestamo", "Debe seleccionar un Prestamo");
                }
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    matriz.Cliente = await _context.Clientes.FindAsync(matriz.Cliente.Id);
                    matriz.Prestamo = await _context.Prestamos.FindAsync(matriz.Prestamo.Id);                    
                    await _context.MatrizRiesgoCabeceras.AddAsync(matriz);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente la Matriz.");
                    return RedirectToAction("Index", "MatrizRiesgo");
                }
                else
                {
                    if (matriz.Cliente != null)
                    {
                        matriz.Cliente = await _context.Clientes.FindAsync(matriz.Cliente.Id);
                    }
                    MatrizViewBag(matriz);
                    return PartialView(matriz);
                }

            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar la Matriz. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        public ActionResult _Update(int id)
        {
            try{
                MatrizRiesgoCabecera matriz = _context.MatrizRiesgoCabeceras.Where(s => s.Id == id).First();
                MatrizViewBag(matriz);
                return PartialView(matriz);
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Matriz. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        [HttpPost]
        public async Task<ActionResult> _Update(MatrizRiesgoCabecera matriz)
        {
            try
            {
                if (matriz.Cliente == null)
                {
                    ModelState.AddModelError("Cliente", "Debe seleccionar un Cliente");
                }
                if (matriz.Prestamo == null)
                {
                    ModelState.AddModelError("Prestamo", "Debe seleccionar un Prestamo");
                }
                if (ModelState.IsValid)
                {
                    matriz.Cliente = await _context.Clientes.FindAsync(matriz.Cliente.Id);
                    matriz.Prestamo = await _context.Prestamos.FindAsync(matriz.Prestamo.Id);   
                    _context.MatrizRiesgoCabeceras.Update(matriz);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se modifico corretamente la Matriz.");
                    return RedirectToAction("Index", "MatrizRiesgo");
                }
                else
                {
                    if (matriz.Cliente != null)
                    {
                        matriz.Cliente = await _context.Clientes.FindAsync(matriz.Cliente.Id);
                    }
                    MatrizViewBag(matriz);
                    return PartialView(matriz);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar la Matriz. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        public async Task<IActionResult> _ListarMatrizRenglones(int Id)
        {
            try
            {
                MatrizRiesgoCabecera cabecera = await _context.MatrizRiesgoCabeceras.FindAsync(Id);
                List<MatrizRiesgoRenglones> renglones = await _context.MatrizRiesgoRenglones.Where(x => x.Cabecera.Id == Id).ToListAsync();

                @ViewBag.MatrizId = cabecera.Id;
                @ViewBag.MatrizNombre = cabecera.Cliente?.Persona?.Nombres+" "+cabecera.Cliente?.Persona?.Apellido;
                return PartialView("_ListarMatrizRenglones", renglones);
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al editar los renglones de la Matriz. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        public ActionResult _CreateRenglon(int Id)
        {
            MatrizRenglonViewBag(Id);
            return PartialView();
        }
        private void MatrizRenglonViewBag(int MatrizId)
        {
            ViewBag.MatrizId = MatrizId;
            ViewBag.Probabilidades = _context.MatrizProbabilidades.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            ViewBag.Consecuencias = _context.MatrizConsecuencias.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _CreateRenglon([Bind("Cabecera,Probabilidad,Consecuencia")] MatrizRiesgoRenglones renglon)
        {
            try
            {
                if (renglon.Probabilidad == null)
                {
                    ModelState.AddModelError("Probabilidad", "Debe seleccionar una Probabilidad");
                }
                if (renglon.Consecuencia == null)
                {
                    ModelState.AddModelError("Consecuencia", "Debe seleccionar una Consecuencia");
                }              
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    renglon.Cabecera = await _context.MatrizRiesgoCabeceras.FindAsync(renglon.Cabecera.Id);
                    renglon.Probabilidad = await _context.MatrizProbabilidades.FindAsync(renglon.Probabilidad.Id);
                    renglon.Consecuencia = await _context.MatrizConsecuencias.FindAsync(renglon.Consecuencia.Id);          
                    await _context.MatrizRiesgoRenglones.AddAsync(renglon);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Renglón de la Matriz de Riesgo.");
                    return RedirectToAction("Index", "MatrizRiesgo", new { @id = renglon.Cabecera.Id });
                }
                else
                {
                    MatrizRenglonViewBag(renglon.Cabecera.Id);
                    return PartialView(renglon);
                }

            }
            catch (Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar el Renglón de la Matriz de Riesgo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo", new { @id = renglon.Cabecera.Id });
            }
        }
        public IActionResult DeleteRenglon(int id)
        {
            try
            {
                MatrizRiesgoRenglones renglon = _context.MatrizRiesgoRenglones.Where(s => s.Id == id).First();
                var MatrizId = renglon.Cabecera.Id;
                _context.MatrizRiesgoRenglones.Remove(renglon);
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Renglón de la Matriz de Riesgo.");
                return RedirectToAction("Index", "MatrizRiesgo", new { @id = MatrizId });
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Renglón de la Matriz de Riesgo.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        public ActionResult _UpdateRenglon(int id)
        {
            try
            {
                MatrizRiesgoRenglones renglon = _context.MatrizRiesgoRenglones.Where(s => s.Id == id).First();
                MatrizRenglonViewBag(renglon.Cabecera.Id);
                return PartialView(renglon);
            }
            catch (Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el renglón de la Matriz de Riesgo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo");
            }
        }
        [HttpPost]
        public async Task<ActionResult> _UpdateRenglon(MatrizRiesgoRenglones renglon)
        {
            try
            {
                if (renglon.Probabilidad == null)
                {
                    ModelState.AddModelError("Probabilidad", "Debe seleccionar una Probabilidad");
                }
                if (renglon.Consecuencia == null)
                {
                    ModelState.AddModelError("Consecuencia", "Debe seleccionar una Consecuencia");
                }
                if (ModelState.IsValid)
                {
                    renglon.Cabecera = await _context.MatrizRiesgoCabeceras.FindAsync(renglon.Cabecera.Id);
                    renglon.Probabilidad = await _context.MatrizProbabilidades.FindAsync(renglon.Probabilidad.Id);
                    renglon.Consecuencia = await _context.MatrizConsecuencias.FindAsync(renglon.Consecuencia.Id);
                    _context.MatrizRiesgoRenglones.Update(renglon);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Renglón de la Matriz de Riesgo.");
                    return RedirectToAction("Index", "MatrizRiesgo", new { @id = renglon.Cabecera.Id });
                }
                else
                {
                    MatrizRenglonViewBag(renglon.Cabecera.Id);
                    return PartialView(renglon);
                }
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al modificar el Renglón de la Matriz de Riesgo. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "MatrizRiesgo", new { @id = renglon.Cabecera.Id });
            }
        }
    }
}
