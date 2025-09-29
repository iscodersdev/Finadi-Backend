using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Core
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Int64 CUIT { set; get; }
        public string RazonSocial { get; set; }
        public string Domicilio { get; set; }
        public virtual List<ProveedorRubro> Rubros { get; set; }
        public virtual Empresas Empresa { get; set; }
        public byte[] Foto { get; set; }

        public bool Activo { get; set; }
    }
    public class ProveedorRubro
    {
        public int Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Rubro Rubro { get; set; }
    }
    public class Rubro
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public virtual List<ProveedorRubro> Proveedores { get; set; }
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Rubro Rubro { get; set; }
        public bool Activo { get; set; }
        public string DescripcionAmpliada { get; set; }
        public decimal Precio { get; set; }
        public decimal? Oferta { get; set; }
        public bool Financiable { get; set; }
        public virtual List<Financiacion> FinanciacionProducto { get; set; }
    }

    public class Financiacion
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        public int CantidadCuotas { get; set; }
        public decimal InteresesPorCuota { get; set; }
    }
    public class Venta
    {
        public int Id { get; set; }
        public virtual Producto Producto { get; set; }
        public virtual Prestamos Prestamo { get; set; }
        
    }
}
