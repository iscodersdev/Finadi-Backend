using Commons.Models;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using FINADICore.Controllers;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class NovedadesController : FINADICoreController
    {
        public NovedadesController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Novedades" });
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Novedades" });
            return View();
        }
        public ActionResult Create()
        {
            return PartialView();
        }
        public IActionResult ObtenerNovedades(Page<Novedades> page)
        {

            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            if(usuario== null || usuario.Clientes?.Empresa == null)
            {
                page.SelectPage("/Novedades/ObtenerNovedades", _context.Novedades, x => x.Empresa == null && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText)));
            }
            else
            {
                page.SelectPage("/Novedades/ObtenerNovedades",
                    _context.Novedades,
                    x => x.Empresa.Id == usuario.Clientes.Empresa.Id && (x.Titulo.Contains(page.SearchText) || x.Texto.Contains(page.SearchText))
                    );
            }
            return PartialView("_ListadoNovedades", page);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Create(Novedades novedad)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
          //  novedad.Empresa = _context.Empresas.FirstOrDefault();
            _context.Novedades.Add(novedad);
            _context.SaveChanges();
            return RedirectToAction("Index", "Novedades");
        }
        public IActionResult Delete(int id)
        {
            try
            {
                Novedades novedad = _context.Novedades.Where(s => s.Id == id).First();
                _context.Novedades.Remove(novedad);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                return RedirectToAction("Update", id);
            }
        }
        public ActionResult Update(int id)
        {
            return PartialView(_context.Novedades.Where(s => s.Id == id).First());
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Update(Novedades novedad, int colorId)
        {
            Novedades d = _context.Novedades.Where(s => s.Id == novedad.Id).First();
            d.Titulo = novedad.Titulo;
            d.Texto = novedad.Texto==null?" ":novedad.Texto;
            d.Fecha = novedad.Fecha;
            d.Publica = novedad.Publica;
            _context.SaveChanges();
            return RedirectToAction("Index", "Novedades");
        }
        [HttpGet]
        public async Task<IActionResult> _CambiarImagen(int id)
        {
            var Novedad = await _context.Novedades.FindAsync(id);

            if (Novedad == null) return NotFound();

            return PartialView(Novedad);
        }
        [HttpPost]
        public async Task<IActionResult> _CambiarImagen(IFormFile file, int id)
        {
            var Novedad = await _context.Novedades.FindAsync(id);

            if (Novedad == null) return NotFound();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                Novedad.Foto = Convert.ToBase64String(memoryStream.ToArray());
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Notificacion(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.Email == User.Identity.Name);
            var ListaPush = _context.Clientes.Where(x=>x.Empresa.Id==usuario.Clientes.Empresa.Id);
            var novedad = _context.Novedades.Find(id);
            //var comon = new common();
            //foreach (var push in ListaPush)
            //{
            //    comon.Envia_Push(push.DeviceId, "Mensaje Clubbers: Se Agrego una Novedad", "Titulo: " + novedad.Titulo + " - Puede Consultarla en el Menu Novedades!!!", push.Id, _context);
            //}
            //_context.SaveChanges();
            AddPageAlerts(PageAlertType.Success, ListaPush.Count().ToString() + " Notificaciones Enviadas!");
            return RedirectToAction("Index", "Novedades");
        }
    }
    public static class Extensions
    {
        public static StringContent AsJson(this object o)
         => new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
    }
}