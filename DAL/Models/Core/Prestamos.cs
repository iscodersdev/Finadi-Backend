using Commons.Identity;
using Commons.Models;
using DAL.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Vendedores
    {
        public int Id { get; set; }
        public virtual Persona Persona { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Mail { get; set; }
    }
    public class SistemasFinanciacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class FormasPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class Monedas
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class LineasPrestamos
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal CapitalMinimo { get; set; }
        public decimal CapitalMaximo { get; set; }
        public decimal CuotaMinima { get; set; }
        public decimal CuotaMaxima { get; set; }
        public decimal Intervalo { get; set; }
        public int CantidadCuotasMinima { get; set; }
        public int CantidadCuotasMaxima { get; set; }
        public virtual SistemasFinanciacion SistemaFinanciacion { get; set; }
        public virtual Monedas Moneda { get; set; }
        public int TipoDescuentoCGEId { get; set; }
        public DateTime? FechaBaja { get; set; }

    }
    public class LineasPrestamosPlanes
    {
        public int Id { get; set; }
        public virtual LineasPrestamos Linea {get;set;}
        public decimal MontoPrestado { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal MontoCuota { get; set; }
        public decimal CFT { get; set; }
        public decimal TEM { get; set; }
        public decimal TNA { get; set; }
        public decimal CapitalFINADI { get; set; }

        public decimal MargenDisponible { get; set; }
        public bool Activo { get; set; }
        public virtual Usuario UsuarioCarga { get; set; }
    }
    public class EstadosPrestamos 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool AnulablePersona { get; set; }
        public bool ConfirmablePersona { get; set; }
        public bool Transferido { get; set; }
        public string Color { get; set; }
        public int EstadoCGEId { get; set; }
    }
    public class DestinosFondos 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public virtual List<DestinosFondosRubros> DestinosFondosRubros { get; set; }
    }
    public class DestinosFondosRubros
    {
        public int Id { get; set; }
        public virtual DestinosFondos DestinosFondos { get; set; }
        public virtual Rubro Rubro { get; set; }
    }


    public class Prestamos 
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { set; get; }
        public string Domicilio { set; get; }
        public virtual LineasPrestamos Linea { get; set; }
        public virtual DestinosFondos DestinoFondos { get; set; }
        public decimal Capital { get; set; }
        public virtual EstadosPrestamos EstadoActual { get; set; }
        public DateTime? FechaSolicitado { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public DateTime? FechaLiquidacion { get; set; }
        public DateTime? FechaPresentacionDeInversor { get; set; }
        public int CantidadCuotas { get; set; }
        public DateTime? FechaPrimerVencimiento { get; set; }
        public virtual Usuario IngresadoPor { get; set; }
        public virtual Usuario AprobadoPor { get; set; }
        public virtual Usuario LiquidadoPor { get; set; }
        public string Observaciones { get; set; }
        public virtual Usuario OficialCuenta { get; set; }
        public string CBU { get; set; }
        public DateTime? FechaPago { get; set; }
        public virtual FormasPago FormaPago { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public string ObservacionesAnulacion { get; set; }
        public int PrestamoCGEId { get; set; }
        public byte[] LegajoElectronico { get; set; }
        public byte[] FirmaOlografica { get; set; }
        public byte[] ConstanciaCBU { get; set; }
        public decimal MontoCuota { get; set; }
        public byte[] FirmaOlograficaConfirmacion { get; set; }
        public int PrestamoNumero { get; set; }
        public decimal Saldo { get; set; }
        public int CuotasRestantes { get; set; }
        public string CapitalEnLetras { get; set; }
        public string CuotasEnLetras { get; set; }
        public string MontoCuotaEnLetras { get; set; }
        public decimal CFT { get; set; }
        public byte[] FotoReciboHaber { get; set; }
        public virtual List<Adjuntos> FotoCertificadoDescuento { get; set; }
        public byte[] CertificadoDescuento { get; set; }
        public byte[] AdjuntoTransferencia { get; set; }
        public string ExtensionAdjuntoTransferencia { get; set; }
        public virtual Vendedores Vendedor { get; set; }
        public TipoPrestamo Tipos { get; set; }
        public decimal Pagare { get; set; }
        public string PagareEnLetras { get; set; }
        public decimal TotalCapitalInversor { get; set; }
        public string CapitalInversorEnLetras { get; set; }
        public virtual Inversor Inversor { get; set; }
        public decimal SueldoNetoMensual { get; set; }        
        public int TasaInversor { get; set; }
        public int TNA { get; set; }
        public int TEA { get; set; }
        public int TEM { get; set; }
        public int CFTSinImpuesto { get; set; }
        public int CFTConImpuesto { get; set; }
        public bool Ampliado { get; set; }
        public decimal MontoMensualDisponible { get; set; }
        public decimal MontoCuotaAmpliacion { get; set; }
        public decimal TEMAmprom { get; set; }
        public decimal TNAAmprom { get; set; }
        public decimal CapitalAmprom { get; set; }
        public int CodigoEstadistico { get; set; }
        public DateTime FechaEmisionAnexo { get; set; }
        public bool Integracion { get; set; }
        public virtual Paises Pais { get; set; }
        public virtual Provincia Provincia { get; set; }
        public virtual Localidad Localidad { get; set; }
        public string Calle { get; set; }
        public string CalleNro { get; set; }
        public string Piso { get; set; }
        public string Dpto { get; set; }
        public string CodPostal { get; set; }

    }

    public enum TipoPrestamo
    {
        PrestamoPersonal,
        CompraBienes
    }

    public class TiposMovimientos 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Credito { get; set; }
        public bool Debito { get; set; }
    }
    public class CuentasCorrientes
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal Credito { get; set; }
        public decimal Debito { get; set; }
        public decimal Saldo { get; set; }
        public virtual TiposMovimientos TipoMovimiento { get; set; }
        public virtual Prestamos Prestamo { get; set; }
    }


    public class MatrizRiesgoCabecera
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public virtual Prestamos Prestamo { get; set; }
        [Display(Name ="¿Es residente de una Zona Limitrofe?")]
        public bool ResidenteZonaLimistrofe { get; set; }
        [Display(Name = "Frecuencia Anual de Creditos")]
        public int FrecuenciaAnualCreditos { get; set; }
        [Display(Name = "¿Declara Bienes Inmuebles?")]
        public bool DeclaraBienesInmuebles { get; set; }
        [Display(Name = "¿Declara Bienes Muebles Registrables?")]
        public bool DeclaraBienesMueblesRegistrables { get; set; }
        [Display(Name = "¿Posee Cuenta Corriente en Pesos?")]
        public bool CuentaCorrientePesos { get; set; }
        [Display(Name = "¿Posee Cuenta Corriente en Dolares?")]
        public bool CuentaCorrienteDolares { get; set; }
        [Display(Name = "¿Posee otras Inversiones?")]
        public bool OtrasInversiones { get; set; }
    }

    public class MatrizRiesgoRenglones
    {
        public int Id { get; set; }
        public virtual MatrizRiesgoCabecera Cabecera { get; set; }
        public virtual MatrizProbabilidades Probabilidad { get; set; }
        public virtual MatrizConsecuencias Consecuencia { get; set; }
    }

    public class Campanas
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaBaja { get; set; }
        public virtual Empresas Empresa { get; set; }
        public string Observaciones { get; set; }
        public string TextoMail { get; set; }
        public int ProvinciaId { get; set; }
        public decimal MinimoDisponible { get; set; }
        public decimal MaximoDisponible { get; set; }
        public int UnidadId { get; set; }
        public int CantidadPersonas { get; set; }
        public int CantidadUnidades { get; set; }
        public int CantidadProvincias { get; set; }
        public byte[] Imagen { get; set; }
        public string ImagenUrl { get; set; }
        public string Link { get; set; }
    }
    public class CampanasRenglones
    {
        public int Id { get; set; }
        public virtual Campanas Cabecera { get; set; }
        public Int64 DNI { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public string eMail { get; set; }
        public string Celular { get; set; }
        public decimal Disponible { get; set; }
        public decimal ImporteMaximo { get; set; }
        public int UnidadId { get; set; }
        public int ProvinciaId { get; set; }
        public string Unidad { get; set; }
        public string Provincia { get; set; }
        public string TipoPersona { get; set; }
    }
    public class MTraeListaProvinciasDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public List<MListaProvinciasDTO> Provincias { get; set; }
    }
    public class MListaProvinciasDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class MTraeListaUnidadesDTO
    {
        public string UAT { get; set; }
        public int Status { get; set; }
        public string Mensaje { get; set; }
        public int ProvinciaId { get; set; }
        public List<MListaUnidadesDTO> Unidades { get; set; }
    }
    public class MListaUnidadesDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ClientesSinDisponible
    {
        public int Id { get; set; }
        public virtual Clientes Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Disponible { get; set; }
    }

    public class Adjuntos
    {
        public int Id { get; set; }
        public byte[] Adjunto { get; set; }
        public string Extension { get; set; }
        public DateTime Fecha { get; set; }
    }

    
}