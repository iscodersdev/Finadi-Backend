namespace DAL.DTOs
{
    public class PrestamosDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public string Cliente { get; set; }
        public string DNI { get; set; }
        public string Categoria { get; set; }
        public decimal Capital { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public string Estado { get; set; }
        public int EstadoId { get; set; }
        public string Fecha { get; set; }
        public string Celular { get; set; }
        public string eMail { get; set; }
    }
}
