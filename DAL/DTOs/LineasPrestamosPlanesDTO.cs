namespace DAL.DTOs
{
    public class LineasPrestamosPlanesDTO
    {
        public int Id { get; set; }
        public decimal MontoPrestado { get; set; }
        public decimal CFT { get; set; }
        public decimal MontoCuota { get; set; }        
        public int CantidadCuotas { get; set; }
        public decimal MargenDisponible { get; set; }
    }
}
