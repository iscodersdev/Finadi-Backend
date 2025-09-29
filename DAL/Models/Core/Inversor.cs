using System.ComponentModel.DataAnnotations;

namespace DAL.Models.Core
{
    public class Inversor
    {
        public int Id { get; set; }
        [Display(Name = "Nombre del Inversor:")]
        public string Nombre { get; set; }
        [Display(Name = "Domicilio Legal:")]
        public string Domicilio { get; set; }
        [Display(Name = "CUIT:")]
        public string CUIT { get; set; }
        public virtual TasaInversor TasaActual { get; set; }
        public virtual TNA TNA { get; set; }
        public virtual TEA TEA { get; set; }
        public virtual CFTSinImpuesto CFTSinImpuesto { get; set; }
        public virtual CFTConImpuesto CFTConImpuesto { get; set; }
        public virtual TEM TEM { get; set; }
        public bool FirmaReporte { get; set; }
        public byte[] Logo { get; set; }

        public bool Activo { get; set; }
    }
    public class TasaInversor
    {
        public int Id { get; set; }
        [Display(Name = "Tasa:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }

    public class TNA
    {
        public int Id { get; set; }
        [Display(Name = "Tasa Nominal Anual:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }

    public class TEA
    {
        public int Id { get; set; }
        [Display(Name = "Tasa Efectiva Anual:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }
    public class CFTSinImpuesto
    {
        public int Id { get; set; }
        [Display(Name = "Costo Financiero Total:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }

    public class TEM
    {
        public int Id { get; set; }
        [Display(Name = "Tasa Efectiva Mensual:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }
    public class CFTConImpuesto
    {
        public int Id { get; set; }
        [Display(Name = "Costo Financiero Total con Impuesto:")]
        public decimal Tasa { get; set; }
        public int Inversor { get; set; }
    }
}
