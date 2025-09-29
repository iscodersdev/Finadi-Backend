namespace DAL.DTOs
{
    public class ListTiposPersonasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int LimiteCuotas { get; set; }
        public decimal TopePrestamo { get; set; }
        public decimal MontoAmpliacion { get; set; }
        public string Organismo { get; set; }
        public string Cuota { get; set; }
    }
}
