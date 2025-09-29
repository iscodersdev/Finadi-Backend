using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Commons.Identity.Services;
using DAL.Data;
using DAL.Models;
using System.Linq;
using System;
using System.Globalization;
using System.Security.Claims;
using Commons.Identity.Extensions;

namespace FINADICore.Controllers
{
    public class HomeController : FINADICoreController
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserService<Usuario> _userManager;
        private readonly PlenarioService _plenarioService;
        public HomeController(FINADIContext context, UserService<Usuario> userManager, SignInManager<Usuario> signInManager, PlenarioService plenarioService) : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _plenarioService = plenarioService;
        }
        public IActionResult Index()
        {

            //ViewBag.Breadcrumb = breadcumb;
            //ActualizaPersonaUser();
            ////var establecimiento = GetEstablecimiento();
            AddPageAlerts(PageAlertType.Success, $"Bienvenido {User.Identity.Name}!");
            //int PrestamosCGE = _context.Prestamos.Count(x => x.PrestamoCGEId != 0 );
            //int OtrosOrganismos = _context.Prestamos.Count(x => x.PrestamoCGEId == 0 );
            //int Clientes = _context.Clientes.Count(x => x.FechaBaja == null);
            ////int Compras  = _context.Compras.Count();


            //ViewBag.title1 = "Prestamos CGE";
            //ViewBag.title2 = "Prestamos Otros Organismos";
            //ViewBag.title3 = "Cantidad Clientes";
            //ViewBag.title4 = "Cantidad Compras";
            //@ViewBag.Uno = PrestamosCGE.ToString();
            //@ViewBag.Dos = OtrosOrganismos.ToString();
            //@ViewBag.Tres = Clientes.ToString();
            //@ViewBag.Cuatro = "0";
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            ViewBag.title1 = "Socios Con App";
            ViewBag.title2 = "Cantidad Prestamos Del Mes";
            ViewBag.title3 = "Total Prestado del Mes";
            ViewBag.title4 = "Cantidad Socios Nuevos del Mes";
            if (usuario.Administradores || ((ClaimsPrincipal)User).IsAdmin())
            {
                @ViewBag.Uno = _context.Clientes.Count().ToString();
                @ViewBag.Dos = _context.Prestamos.Where(x => x.FechaSolicitado >= DateTime.Today.AddDays(-30) && x.FechaAnulacion == null && x.FirmaOlografica != null  ).Count().ToString();
                @ViewBag.Tres = _context.Prestamos.Where(x => x.FechaSolicitado >= DateTime.Today.AddDays(-30) && x.FechaAnulacion == null && x.FirmaOlografica != null && x.EstadoActual.Id != 4 && x.EstadoActual.Id != 5 && x.EstadoActual.Id != 9 && x.EstadoActual.Id != 11 && x.EstadoActual.Id != 13 && x.EstadoActual.Id != 15 && x.EstadoActual.Id != 16).Sum(x => x.Capital).ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                @ViewBag.Cuatro = _context.Clientes.Where(x => x.FechaIngreso.Date >= DateTime.Today.AddDays(-30).Date).Count();
            }
            else 
            {
                @ViewBag.Uno = "0";
                @ViewBag.Dos = "0";
                @ViewBag.Tres = "0";
                @ViewBag.Cuatro = "0";
            } 
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new DAL.Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

    }
}