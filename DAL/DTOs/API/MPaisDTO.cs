using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOs.API
{
    public class MPaisDTO
    {
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int? PaisId { get; set; }
        public List<MListPaisDTO> Paises { get; set; } = new List<MListPaisDTO>();
    }

    public class MListPaisDTO
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}


