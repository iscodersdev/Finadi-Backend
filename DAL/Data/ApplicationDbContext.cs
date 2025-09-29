using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commons.Identity;
using DAL.Models;
using DAL.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Data
{
    public class FINADIContext : CommonsDbContext<Usuario>
    {
        public FINADIContext(DbContextOptions<FINADIContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        //BASICAS
        public DbSet<DatosEstructura> DatosEstructura { get; set; }
        public DbSet<Provincia> Provincia { get; set; }
        public DbSet<Localidad> Localidad { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<Paises> Paises { get; set; }
        public DbSet<EstadosCiviles> EstadosCiviles { get; set; }
        public DbSet<TiposPersonas> TiposPersonas { get; set; }
        public DbSet<Genero> Genero { get; set; }
        public DbSet<Organismo> Organismos { get; set; }
        public DbSet<CuotaSocial> CuotasSociales { get; set; }
        public DbSet<Inversor> Inversores { get; set; }
        public DbSet<TasaInversor> TasasInversores { get; set; }
        public DbSet<TNA> TasasNominalAnual { get; set; }
        public DbSet<TEA> TasasEfectivaAnual { get; set; }
        public DbSet<CFTSinImpuesto> CFTSinImpuesto { get; set; }
        public DbSet<CFTConImpuesto> CFTConImpuesto { get; set; }
        public DbSet<TEM> TEM { get; set; }
      
        //CLIENTES
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<TiposClientes> TiposClientes { get; set; }
        public DbSet<Referencia> Referencias { get; set; }

        //EMPRESAS
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Grupos> Grupos { get; set; }

        //PRESTAMOS
        public DbSet<Prestamos> Prestamos { get; set; }
        public DbSet<Vendedores> Vendedores { get; set; }
        public DbSet<SistemasFinanciacion> SistemasFinanciacion { get; set; }
        public DbSet<FormasPago> FormasPago { get; set; }
        public DbSet<Monedas> Monedas { get; set; }
        public DbSet<LineasPrestamos> LineasPrestamos { get; set; }
        public DbSet<LineasPrestamosTiposPersonas> LineasPrestamosTiposPersonas { get; set; }
        public DbSet<LineasPrestamosPlanes> LineasPrestamosPlanes { get; set; }
        public DbSet<EstadosPrestamos> EstadosPrestamos { get; set; }
        public DbSet<DestinosFondos> DestinoFondos { get; set; }
        public DbSet<DestinosFondosRubros> DestinoFondosRubros { get; set; }
        public DbSet<TiposMovimientos> TiposMovimientos { get; set; }
        public DbSet<CuentasCorrientes> CuentasCorrientes { get; set; }
    

        //MATRIZ
        public DbSet<MatrizProbabilidades> MatrizProbabilidades { get; set; }
        public DbSet<MatrizConsecuencias> MatrizConsecuencias { get; set; }
        public DbSet<MatrizRiesgoCabecera> MatrizRiesgoCabeceras { get; set; }
        public DbSet<MatrizRiesgoRenglones> MatrizRiesgoRenglones { get; set; }

        //CORE
        public DbSet<UAT> UAT { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<ClientesSinDisponible> ClientesSinDisponible { get; set; }

        //MOBILE
        public DbSet<Invitaciones> Invitaciones { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<ClientesServicios> ClientesServicios { get; set; }
        public DbSet<Novedades> Novedades { get; set; }
        public DbSet<NotificacionesPersonas> NotificacionesPersonas { get; set; }
        public DbSet<Reservas> Reservas { get; set; }
        public DbSet<Horarios> Horarios { get; set; }
        public DbSet<CuentaCorriente> CuentaCorriente { get; set; }
        public DbSet<Conceptos> Conceptos { get; set; }
        public DbSet<Puestos> Puestos { get; set; }
        public DbSet<PuestosCodigos> PuestosCodigos { get; set; }
        public DbSet<Accesos> Accesos { get; set; }
        public DbSet<EstadosDeudas> EstadosDeudas { get; set; }

        //CAMPANAS PUBLICITARIAS
        public DbSet<Campanas> Campanas { get; set; }
        public DbSet<CampanasRenglones> CampanasRenglones { get; set; }

        //Billetera
        public DbSet<Billetera> Billeteras { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<CuentaBancaria> CuentasBancarias { get; set; }
        public DbSet<ServicioBilletera> ServiciosBilletera { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<InstitucionFinanciera> InstitucionesFinancieras { get; set; }
        public DbSet<MovimientoBilletera> MovimientosBilletera { get; set; }
        public DbSet<TipoMovimientoBilletera> TipoMovimientoBilletera { get; set; }
        public DbSet<Compras> ComprasProductos { get; set; }
        public DbSet<Adjuntos> Adjuntos { get; set; }

        //MOCK API EXTERNA DE PAGOS Y COBROS
        public DbSet<MockServicio> MockServicios { get; set; }

        #region Proveedores
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Rubro> Rubros { get; set; }
        public DbSet<ProveedorRubro> ProveedorRubros { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Financiacion> FinanciacionProductos { get; set; }

        #endregion
        public override List<IWorkSpace> GetIWorkSpaces()
        {
            return new List<IWorkSpace>();
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //base.Database.SetCommandTimeout(0);
            base.OnModelCreating(builder);
            builder.Entity<Usuario>()
                .HasOne(c => c.Clientes)
                .WithOne(u => u.Usuario)
                .HasForeignKey<Clientes>(x => x.UsuarioId);
        }


    }

    //Tuve que agregar esto para que funcione la migracion, hay que resolverlo luego
    public class FINADIContextFactory : IDesignTimeDbContextFactory<FINADIContext>
    {
        public FINADIContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FINADIContext>();
            // optionsBuilder.UseSqlServer("Server=localhost;Database=FINADICore;Trusted_Connection=True;MultipleActiveResultSets=true");
            //optionsBuilder.UseSqlServer("Server=localhost;Database=FINADI;Trusted_Connection=True;MultipleActiveResultSets=true");
            optionsBuilder.UseSqlServer("Server=186.189.235.119;Database=FINADI_prod;user=sa;password=Crisis2040;MultipleActiveResultSets=true");

            return new FINADIContext(optionsBuilder.Options);
        }
    }



}
