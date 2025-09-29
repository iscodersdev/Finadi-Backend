using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers

{
    [Area("Core")]
    public class ScoringController : FINADICoreController
    {
        public ScoringController(FINADIContext context) : base(context)
        {
        }
        public ActionResult Index()
        {
            ScoringDTO scoring = new ScoringDTO();
            return View(scoring);
        }

        [HttpPost]
        public JsonResult BuscarPersonas(string Persona)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);
            string apellido = "";
            string nombres = "";
            int DNI = 0;
            Int32.TryParse(Persona.Trim(), out DNI);
            if (DNI == 0)
            {
                if (Persona.Contains(","))
                {
                    apellido = Persona.Split(',')[0].Trim();
                    nombres = Persona.Split(',')[1].Trim();
                }
                else
                {
                    apellido = Persona;
                    nombres = "";
                }
            }
            MTraeListarPersonasDTO datos = new MTraeListarPersonasDTO();
            if(usuario!=null && usuario.Clientes!=null && usuario.Clientes.Empresa != null)
            {
                datos = FINADICore.FINADI.TraeClientes(usuario.Clientes.Empresa, apellido, nombres, DNI, this.HttpContext.Session.GetString("UAT"));
            }
            else
            {
                datos = FINADICore.FINADI.TraeClientes(_context.Empresas.Find(1), apellido, nombres, DNI, this.HttpContext.Session.GetString("UAT"));
            }
            var personas = datos.Renglones.Select(x => new
            {
                Text = $"{x.Apellido.Trim()}, {x.Nombres.Trim()} - DNI: {x.DNI.ToString()} - ",
                Value = Convert.ToInt32(x.DNI),
                Subtext = $"{x.TipoPersona}",
                Icon = "fa fa-user"
            }).ToArray();
            HttpContext.Session.SetString("UAT", datos.UAT);
            return Json(personas);
        }

        public IActionResult ScoringPersona(int DNI)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName  == User.Identity.Name);
            ScoringDTO scoring = new ScoringDTO();
            MDatosScoringDTO cliente = new MDatosScoringDTO();
            if (usuario!=null && usuario.Clientes!=null && usuario.Clientes.Empresa!=null)
            {
                cliente = FINADI.TraeDatosScoring(usuario.Clientes.Empresa, DNI, _context);
            }
            else
            {
                cliente = FINADI.TraeDatosScoring(_context.Empresas.Find(1), DNI, _context);
            }
            scoring.Scoring = 10;
            scoring.Apellido = cliente.Apellido;
            scoring.Nombres = cliente.Nombres;
            if (cliente.Foto==null)
            {
                scoring.Foto= "data:image/gif;base64,"+Convert.ToBase64String(new MemoryStream(System.IO.File.ReadAllBytes("wwwroot/images/Avatar.jpeg")).ToArray());
            }
            else
            {
                scoring.Foto = "data:image/gif;base64," + Convert.ToBase64String(cliente.Foto);
            }
            scoring.DescripcionOtrosPrestamos = "NO Presenta Otros Prestamos";
            if (cliente.OtrosPrestamos > 0)
            {
                scoring.OtrosPrestamos = true;
                scoring.DescripcionOtrosPrestamos = "Presenta Otros Prestamos Por " + cliente.OtrosPrestamos.ToString("C2");
                scoring.Scoring = scoring.Scoring - 1;
            }
            if (cliente.Disponible > 0)
            {
                scoring.Disponible = cliente.Disponible;
                scoring.DescripcionDisponible = "Disponible " + cliente.Disponible.ToString("C2");
                scoring.Scoring = scoring.Scoring - 1;
            }
            else
            {
                scoring.Disponible = cliente.Disponible;
                scoring.DescripcionDisponible = "Si Disponible " + cliente.Disponible.ToString("C2");
                scoring.Scoring = scoring.Scoring - 6;
            }
            
            scoring.DescripcionEmbargos = "NO Presenta Embargos";
            if (cliente.Embargos > 0)
            {
                scoring.Embargos = true;
                scoring.DescripcionEmbargos = "Presenta Embargos Por " + cliente.Embargos.ToString("C2");
                scoring.Scoring = scoring.Scoring - 2;
            }
            scoring.DNI = cliente.DNI;
            scoring.TipoPersona = cliente.TipoPersona;
            scoring.SituacionEspecial = cliente.SituacionEspecial;
            scoring.DescripcionSituacionEspecial = "NO Presenta Situacion Especial";
            if (cliente.SituacionEspecial == true)
            {
                scoring.DescripcionSituacionEspecial = "Presenta Situacion Especial";
                scoring.Scoring = 0;
            }
            scoring.Quiebra = cliente.Quiebra;
            scoring.DescripcionQuiebra = "NO Presenta Quiebra";
            if (cliente.Quiebra == true)
            {
                scoring.DescripcionSituacionEspecial = "Presenta Quiebra";
                scoring.Scoring = 0;
            }
            scoring.DescripcionBaja = "Persona Activa";
            if (cliente.Baja != null)
            {
                scoring.Baja = true;
                scoring.DescripcionBaja = "Presenta Baja el Dia " + Convert.ToDateTime(cliente.Baja).ToString("dd/MM/yyyy");
                scoring.Scoring = 0;
            }
            return PartialView(scoring);
        }
    }
}