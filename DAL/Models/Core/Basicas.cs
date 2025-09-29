using Commons.Identity;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Provincia
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string Descripcion { get; set; }
        public string DescripcionCompleta { get; set; }
        public virtual Paises Pais { get; set; }
    }

    public class Localidad
    {
        public int Id { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int IdDepartamento { get; set; }
        public int IdProvincia { get; set; }
        public string Descripcion { get; set; }
        public string ProvinciaNombre { get; set; }
        public virtual Provincia Provincia { get; set; }
        public bool Activo { get; set; }
    }

    public class DatosEstructura
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Sigla { get; set; }
        public string Numero { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string CUIT { get; set; }
        public string Telefono { get; set; }
        public string FAX { get; set; }
        public string Sucursal { get; set; }
        public string CBU { get; set; }
        public string Convenio { get; set; }
        public string Entidad { get; set; }
        public string NombreOrganismo { get; set; }
        public string NombreDependencia { get; set; }
        public string URLReportes { get; set; }
        public string UsuarioReportes { get; set; }
        public string CredencialReportes { get; set; }
    }


    public class Paises
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoDocumento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Tipo de Documento")]
        public string Descripcion { get; set; }
    }

    public class Genero
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo Requerido"), Display(Name = "Genero")]
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }

    }
    public class EstadosCiviles
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class MatrizProbabilidades
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class MatrizConsecuencias
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    public class TiposPersonas
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        [Display(Name = "Limite de Cuotas")]
        public int LimiteCuotas { get; set; }
        [Display(Name = "Tope de Prestamo")]
        public decimal TopePrestamo { get; set; }
        [Display(Name = "Monto de Ampliación")]
        public decimal MontoAmpliacion { get; set; }
        [Display(Name = "Tope Cantidad Cuota Ampliacion")]
        public int TopeCantCuotasAmpliacion { get; set; }
        public virtual Organismo Organismo { get; set; }
        public virtual CuotaSocial Cuota { get; set; }
    }

    public class LineasPrestamosTiposPersonas
    {
        public int Id { get; set; }
        public virtual LineasPrestamos LineaPrestamo { get; set; }
        public virtual TiposPersonas TipoPersona { get; set; }
    }
    public class Organismo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public Int64 CUIT { get; set; }
        public virtual Localidad Localidad { get; set; }
        public virtual Provincia Provincia { get; set; }
        public string Telefono { get; set; }
        public string CodigoPostal { get; set; }
        public string Domicilio { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
        public bool APIEjercito { get; set; }
        public string CodigoDescuento { get; set; }
    }
    public class CuotaSocial
    {
        public int Id { get; set; }
        [Display(Name = "Valor de la Cuota Social:")]
        public decimal ValorCuota { get; set; }
        public string ValorCuotaEnLetras { get; set; }
        [Display(Name = "Impuesta por:")]
        public string ImpusoCuota { get; set; }
        public int TipoPersona { get; set; }
    }
}