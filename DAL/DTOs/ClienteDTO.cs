namespace DAL.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string NombreCompleto { get; set; }
        public string CUIL { get; set; }
        public string DNI { get; set; }
        public string RazonSocial { get; set; }
        public string Empresa { get; set; }
        public string FechaIngreso { get; set; }
        public bool Estado { get; set; }
        public bool BloquearPrestamos { get; set; }
    }

    public class ClienteSinDisponobleDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string NombreCompleto { get; set; }
        public string CUIL { get; set; }
        public string DNI { get; set; }
        public string RazonSocial { get; set; }
        public string Empresa { get; set; }
        public string FechaIngreso { get; set; }
        public bool Estado { get; set; }
        public bool BloquearPrestamos { get; set; }
        public decimal Disponible { get; set; }
    }

    public class ScoringDTO
    {
        public int Id { get; set; }
        public string TipoPersona { get; set; }
        public int Scoring { get; set; }
        public string Foto { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int DNI { get; set; }
        public bool Embargos { get; set; }
        public string DescripcionEmbargos { get; set; }
        public bool SituacionEspecial { get; set; }
        public string DescripcionSituacionEspecial { get; set; }
        public bool Baja { get; set; }
        public string DescripcionBaja { get; set; }
        public bool Quiebra { get; set; }
        public string DescripcionQuiebra { get; set; }
        public bool OtrosPrestamos { get; set; }
        public string DescripcionOtrosPrestamos { get; set; }
        public string DescripcionDisponible { get; set; }
        public decimal Disponible { get; set; }
        public int CuotasMaximas { get; set; }
        public decimal MontoMaximo { get; set; }
    }
}