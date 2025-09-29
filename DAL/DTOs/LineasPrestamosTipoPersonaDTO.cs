using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs
{
    public class LineasPrestamosTipoPersonaDTO
    {
        public int Id { get; set; }
        public string NombreTipoPersona { get; set; }
        public string Organismo { get; set; }
    }
}
