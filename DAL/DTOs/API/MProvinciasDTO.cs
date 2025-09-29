using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.API
{
    public class MProvinciasDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? PaisId { get; set; }
        public int? ProvinciaId { get; set; }
        public List<MListProvinciasDTO> Provincias { get; set; } = new List<MListProvinciasDTO>();
    }

    public class MListProvinciasDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
        public int PaisId { get; set; }
    }
}
