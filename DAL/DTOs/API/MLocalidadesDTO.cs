using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.API
{
    public class MLocalidadesDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }
        public List<MListLocalidadesDTO> Localidades { get; set; } = new List<MListLocalidadesDTO>();
    }

    public class MListLocalidadesDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int ProvinciaId { get; set; }
    }
}
