using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DAL.Data;
using Serilog;
using BussinessCore.API.Filters;
using DAL.Mobile;
using DAL.Models;
using System.Linq;

namespace BussinessCore.API.Controllers.Billetera
{
    [TypeFilter(typeof(ChequeaUatApiAttribute))]
    [ApiController]
    [Route("api/[controller]")]
    public class MClientesController : BaseApiController
    {

        public MClientesController(FINADIContext context) : base(context)
        {
        
        }

        [HttpPost]
        [Route("TraeLocalidades")]
        public MTraeLocalidadesDTO TraeLocalidades([FromBody] MTraeLocalidadesDTO modelo)
        {
            try
            {
                if (modelo == null)
                {
                    modelo = new MTraeLocalidadesDTO();
                }
                if (modelo.LocalidadId != null && modelo.LocalidadId != 0)
                {
                    var localidad = _context.Localidad.FirstOrDefault(x => x.Id == modelo.LocalidadId);
                    if (localidad != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";
                        modelo.Localidades.Add(new MLocalidadDTO
                        {
                            Id = localidad.Id,
                            DescripcionLocalidad = localidad.Descripcion,
                            ProvinciaId = localidad.Provincia.Id,
                            DescripcionProvincia = localidad.Provincia.Descripcion,
                            PaisId = localidad.Provincia.Pais.Id,
                            DescripcionPais = localidad.Provincia.Pais.Nombre
                        });
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "Localidad Inexistente";
                    }
                    return modelo;
                }
                else if (modelo.ProvinciaId != null && modelo.ProvinciaId != 0)
                {
                    var localidades = _context.Localidad.Where(x => x.Activo && x.Provincia.Id == modelo.ProvinciaId).OrderBy(x => x.Descripcion);
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in localidades)
                        {
                            modelo.Localidades.Add(new MLocalidadDTO
                            {
                                Id = c.Id,
                                DescripcionLocalidad = c.Descripcion,
                                ProvinciaId = c.Provincia.Id,
                                DescripcionProvincia = c.Provincia.Descripcion,
                                PaisId = c.Provincia.Pais.Id,
                                DescripcionPais = c.Provincia.Pais.Nombre
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "Localidades Inexistentes";
                    }
                    return modelo;
                }
                else if (modelo.PaisId != null && modelo.PaisId != 0)
                {
                    var localidades = _context.Localidad.Where(x => x.Activo && x.Provincia.Pais.Id == modelo.PaisId).OrderBy(x => x.Descripcion);
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in localidades)
                        {
                            modelo.Localidades.Add(new MLocalidadDTO
                            {
                                Id = c.Id,
                                DescripcionLocalidad = c.Descripcion,
                                ProvinciaId = c.Provincia.Id,
                                DescripcionProvincia = c.Provincia.Descripcion,
                                PaisId = c.Provincia.Pais.Id,
                                DescripcionPais = c.Provincia.Pais.Nombre
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "Localidades Inexistentes";
                    }
                    return modelo;
                }
                else
                {
                    var localidades = _context.Localidad.Where(x => x.Activo).OrderBy(x => x.Descripcion);
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in localidades)
                        {
                            modelo.Localidades.Add(new MLocalidadDTO
                            {
                                Id = c.Id,
                                DescripcionLocalidad = c.Descripcion,
                                ProvinciaId = c.Provincia.Id,
                                DescripcionProvincia = c.Provincia.Descripcion,
                                PaisId = c.Provincia.Pais.Id,
                                DescripcionPais = c.Provincia.Pais.Nombre
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "Localidades Inexistentes";
                    }
                    return modelo;
                }
            }
            catch (Exception e)
            {
                modelo.Status = 500;
                modelo.Mensaje = "Error del Servidor";
                return modelo;
            }
        }
        [HttpPost("ClienteBilletera")]
        public async Task<IActionResult> ClienteBilletera([FromBody] ClienteBilleteraDTO clienteBilleteraDTO)
        {
            try
            {
                //var usuario = TraeUsuarioUAT(clienteBilleteraDTO.UAT);
                var billetera = TraeBilleteraCVU(clienteBilleteraDTO.CVU);
                var cliente = billetera.Cliente;

                return new JsonResult(new ClienteBilleteraDTO { Status = 200, UAT = clienteBilleteraDTO.UAT, Mensaje = "Datos de contacto enviados", Nombre = cliente.RazonSocial  });
            }
            catch (Exception e)
            {
                Log.Error($"Error en consulta de medios de pago - {e.Message}");
                return new JsonResult(new RespuestaAPI { Status = 500, UAT = clienteBilleteraDTO.UAT, Mensaje = "Error en consulta de contacto" });
            }

        }

    }


}
