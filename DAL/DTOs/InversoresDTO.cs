
namespace DAL.DTOs
{
    public class ListInversoresDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Domicilio { get; set; }
        public string CUIT { get; set; }
        public decimal Tasa { get; set; }
        public decimal TasaNominalAnual { get; set; }
        public decimal TasaEfectivaAnual { get; set; }
        public decimal TasaEfectivaMensual { get; set; }
        public decimal CFTSinImpuesto { get; set; }
        public decimal CFTConImpuesto { get; set; }
    }
}
