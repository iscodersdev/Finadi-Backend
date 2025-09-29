using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Data;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Serilog;
using BussinessCore.API.Filters;
using System.Data;
using Microsoft.AspNetCore.Http;
using BusinessCore.Services;
using DAL.Mobile;
using DAL.Models.Core;

namespace BussinessCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MVentasController : BaseApiController
    {
        private readonly NotificacionAPIService _notificacionAPIService;
        private readonly FINADIContext _context;

        public MVentasController(FINADIContext context, NotificacionAPIService notificacionAPIService) : base(context)
        {
            _notificacionAPIService = notificacionAPIService;
            _context = context;
        }

        

        [HttpPost("ConfirmaVentas")]
        public async Task<IActionResult> ConfirmaVentas([FromBody] MVentaDTO confirmaVentaDTO)
        {
            try
            {
                //var billeteraPagadora = TraeBilletera(TraeUsuarioUAT(confirmaVentaDTO.UAT));

                //Producto producto = _context.Productos.FirstOrDefault(x => x.Id == confirmaVentaDTO.ProductoId);

                //var movimiento = new MovimientoBilletera
                //{
                //    Fecha = DateTime.Now,
                //    Monto = detallePago.Monto,
                //    TipoMovimiento = _context.TipoMovimientoBilletera.Find((int)TipoMovimientoBilleteraEnum.PagoServicio)
                //};

                //var exito = true;
                //switch (confirmaPagoDTO.TipoMedioPago)
                //{
                //    case TipoMedioPago.TipoBilletera:
                //        billeteraPagadora.Movimientos.Add(movimiento);
                //        break;
                //    case TipoMedioPago.TipoTarjeta:
                //        var tarjetaPagadora = billeteraPagadora.Tarjetas.Where(t => t.Id == confirmaPagoDTO.MetodoPagoId).FirstOrDefault();
                //        tarjetaPagadora.Movimientos.Add(movimiento);
                //        break;
                //    case TipoMedioPago.TipoCuenta:
                //        var cuentaPagadora = billeteraPagadora.Cuentas.Where(c => c.Id == confirmaPagoDTO.MetodoPagoId).FirstOrDefault();
                //        cuentaPagadora.Movimientos.Add(movimiento);
                //        break;
                //    default:
                //        exito = false;
                //        break;
                //}
                //if (exito)
                //{
                //    billeteraPagadora.Saldo -= detallePago.Monto;
                //    _context.Update(billeteraPagadora);
                //    _context.SaveChanges();
                //    _notificacionAPIService.Envia_Push(billeteraPagadora.Cliente.Usuario.DeviceId, "Pago de servicios", $"Ha abonado un servicio por ${detallePago.Monto} desde su billetera");
                return new JsonResult(new RespuestaAPI { Status = 200, UAT = confirmaVentaDTO.UAT , Mensaje = "Venta ejecutada satifactoriamente" });
                //}
                //else
                //{
                //    return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaPagoDTO.UAT, Mensaje = "Error en el registro del pago" });
                //}
            }
            catch (Exception e)
            {
                Log.Error($"Error en confirmacion de pagos para servicios - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 400, UAT = confirmaVentaDTO.UAT, Mensaje = "Error en la confirmacion de pago" });
            }

        }


    }
}
