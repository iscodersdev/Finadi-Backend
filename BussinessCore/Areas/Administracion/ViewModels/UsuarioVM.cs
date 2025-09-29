using DAL.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FINADICore.Areas.Administracion.ViewModels
{
    public class UsuarioVM
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Mail")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Mail { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Password")]
        public string Password { get; set; }
        [DisplayName("¿Es Administrador?")]
        public bool Administrador { get; set; }
        public virtual Persona Persona { get; set; }
        public IEnumerable<SelectListItem> TipoDocumento { get; set; }
        public IEnumerable<SelectListItem> EstadoCivil { get; set; }
        public IEnumerable<SelectListItem> Genero { get; set; }
        public IEnumerable<SelectListItem> Pais { get; set; }
    }

    public class UsuarioPasswordVM
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Nueva Contraseña")]
        public string Password { get; set; }
    }
}
