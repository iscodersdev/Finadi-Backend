using System;
using FINADICore.Controllers;
using Commons.Identity.Services;
using Commons.Models;
using System.IO;
using System.Linq;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Numeric;
using System.Net.Http;
using DAL.Models.Core;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class CreditoSinDisponibleController : FINADICoreController
    {
        private readonly UserService<Usuario> _userService;
        public CreditoSinDisponibleController(FINADIContext context, UserService<Usuario> userService) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Gestión" });
            _userService = userService;
        }
        public ActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Clientes" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public IActionResult ClienteDataTable()
        {
            var lineas = _context.LineasPrestamosTiposPersonas.Where(x => x.TipoPersona.Organismo.Id==1).Select(x => x.LineaPrestamo).ToList();
            var capitalMinimo = lineas.Min(x => x.CapitalMinimo);
            var empresa = _context.Empresas.FirstOrDefault();
            var clientes = _context.ClientesSinDisponible.Where(c=>c.Cliente.Persona.TipoPersona.Organismo.Id==1).ToList().Select(p=> new ClienteSinDisponobleDTO
            {
                Id = p.Id,
                NombreCompleto =(p.Cliente.Persona!=null) ? p.Cliente.Persona.GetNombreCompleto() : "---",
                DNI = p.Cliente.Persona.NroDocumento,
                FechaIngreso = (p.Cliente.FechaIngreso.ToShortDateString()!=null) ? p.Cliente.FechaIngreso.ToShortDateString() : "-----",
                Estado = p.Cliente.ClienteValidado,
                BloquearPrestamos = p.Cliente.BloquearPrestamos,
                Disponible = p.Disponible
            });      
            return DataTable(clientes.Where(x=>x.Disponible<capitalMinimo).AsQueryable<ClienteSinDisponobleDTO>());
        }



        


        public decimal DisponibleCGE(Empresas empresa, Clientes cliente)
        {
            MTraeDsiponibleCGEDTO disponible = new MTraeDsiponibleCGEDTO();
            disponible.UAT = LoginCGE(empresa);
            disponible.DNI = Convert.ToInt32(cliente.Persona.NroDocumento);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("TraeDisponible", disponible).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MTraeDsiponibleCGEDTO>();
                    readTask.Wait();
                    disponible = readTask.Result;
                    return disponible.Disponible;
                }
                else
                {
                    return 0;
                }
            }               
        }

        public string LoginCGE(Empresas empresa)
        {
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        return login.UAT;
                    }
                }
                return response.ToString();
            }
        }
    }
}




//var clientes = _context.Clientes.Where(x => x.FechaBaja==null && x.Persona!=null).Where(e => e.Usuario.Email!="admin@admin.com" || e.Usuario.Email!="pruebasFINADI@FINADI.com").Take(20).ToList();

//var query = from p in clientes
//            select new ClienteSinDisponobleDTO
//            {
//                Id = p.Id,
//                NombreCompleto =(p.Persona!=null) ? p.Persona.GetNombreCompleto() : "---",
//                DNI = p.Persona.NroDocumento,
//                //NombreCompleto =(p.Persona!=null)?p.Persona.GetNombreCompleto():"---",
//                //DNI = (p.Persona != null)? p.Persona.NroDocumento:"---",
//                FechaIngreso = (p.FechaIngreso.ToShortDateString()!=null) ? p.FechaIngreso.ToShortDateString() : "-----",
//                Estado = p.ClienteValidado,
//                BloquearPrestamos = p.BloquearPrestamos
//            };





//return DataTable(query.AsQueryable<ClienteSinDisponobleDTO>());