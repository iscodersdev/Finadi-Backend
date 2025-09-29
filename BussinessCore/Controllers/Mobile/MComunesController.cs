using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Commons.Controllers;
using Commons.Identity.Services;
using FINADICore;
using DAL.DTOs.API;

namespace FINADICore.Controllers.Mobile
{
    [Route("api/[controller]")]
    public class MComunesController : Controller
    {
        private readonly UserService<Usuario> _userManager;
        public FINADIContext _context;
        public MComunesController(FINADIContext context, UserService<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("Paises")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MPaisDTO Paises([FromBody] MPaisDTO modelo)
        {
            try
            {
                if (modelo == null)
                {
                    modelo = new MPaisDTO();
                }
                if (modelo.PaisId != null && modelo.PaisId != 0)
                {
                    var paises = _context.Paises.FirstOrDefault(x => x.Id == modelo.PaisId);
                    if (paises != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";
                        modelo.Paises.Add(new MListPaisDTO
                        {
                            Id = paises.Id,
                            Descripcion = paises.Nombre
                        });
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "El Pais no Existe";
                    }
                    return modelo;
                }
                else
                {
                    var paises = _context.Paises.ToList();
                    if (paises != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in paises)
                        {
                            modelo.Paises.Add(new MListPaisDTO
                            {
                                Id = c.Id,
                                Descripcion = c.Nombre
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "No hay Paises Cargados";
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

        [HttpPost]
        [Route("Provincias")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MProvinciasDTO Provincias([FromBody] MProvinciasDTO modelo)
        {
            try
            {
                if (modelo == null)
                {
                    modelo = new MProvinciasDTO();
                }
                if (modelo.ProvinciaId != null && modelo.ProvinciaId != 0)
                {
                    var paises = _context.Provincia.OrderBy(x => x.Descripcion).FirstOrDefault(x => x.Id == modelo.ProvinciaId);
                    if (paises != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";
                        modelo.Provincias.Add(new MListProvinciasDTO
                        {
                            Id = paises.Id,
                            Descripcion = paises.Descripcion,
                            DescripcionCompleta = paises.DescripcionCompleta,
                            PaisId = paises.Pais.Id
                        });
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "La Provincias no Existe";
                    }
                    return modelo;
                }
                else if (modelo.PaisId != null && modelo.PaisId != 0)
                {
                    var paises = _context.Provincia.OrderBy(x => x.Descripcion).Where(x => x.Pais.Id == modelo.PaisId).ToList();
                    if (paises != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";
                        foreach (var c in paises)
                        {
                            modelo.Provincias.Add(new MListProvinciasDTO
                            {
                                Id = c.Id,
                                Descripcion = c.Descripcion,
                                DescripcionCompleta = c.DescripcionCompleta,
                                PaisId = c.Pais.Id
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "La Provincias no Existe";
                    }
                    return modelo;
                }
                else
                {
                    var paises = _context.Provincia.OrderBy(x => x.Descripcion).ToList();
                    if (paises != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in paises)
                        {
                            modelo.Provincias.Add(new MListProvinciasDTO
                            {
                                Id = c.Id,
                                Descripcion = c.Descripcion,
                                DescripcionCompleta = c.DescripcionCompleta,
                                PaisId = c.Pais.Id
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "No hay Provincias Cargados";
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


        [HttpPost]
        [Route("Localidades")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MLocalidadesDTO Localidades([FromBody] MLocalidadesDTO modelo)
        {
            try
            {
                if (modelo == null)
                {
                    modelo = new MLocalidadesDTO();
                }
                if (modelo.LocalidadId != null && modelo.LocalidadId != 0)
                {
                    var localidades = _context.Localidad.OrderBy(x => x.Descripcion).FirstOrDefault(x => x.Id == modelo.LocalidadId);
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";
                        modelo.Localidades.Add(new MListLocalidadesDTO
                        {
                            Id = localidades.Id,
                            Descripcion = localidades.Descripcion,
                           // ProvinciaId = localidades.Provincia.Id,
                            ProvinciaId = localidades.IdProvincia,
                        });
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "La Localidad no Existe";
                    }
                    return modelo;
                }
                else if (modelo.ProvinciaId != null && modelo.ProvinciaId != 0)
                {
                   // var localidades = _context.Localidad.OrderBy(x => x.Descripcion).Where(x => x.Provincia.Id == modelo.ProvinciaId).ToList();
                    var localidades = _context.Localidad.OrderBy(x => x.Descripcion).Where(x => x.IdProvincia == modelo.ProvinciaId).ToList();
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in localidades)
                        {
                            modelo.Localidades.Add(new MListLocalidadesDTO
                            {
                                Id = c.Id,
                                Descripcion = c.Descripcion,
                                //ProvinciaId = c.Provincia.Id,
                                ProvinciaId = c.IdProvincia,
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "No hay Localidades Cargados";
                    }
                    return modelo;
                }
                else
                {
                    var localidades = _context.Localidad.OrderBy(x => x.Descripcion).ToList();
                    if (localidades != null)
                    {
                        modelo.Status = 200;
                        modelo.Mensaje = "Ok";

                        foreach (var c in localidades)
                        {
                            modelo.Localidades.Add(new MListLocalidadesDTO
                            {
                                Id = c.Id,
                                Descripcion = c.Descripcion,
                                //ProvinciaId = c.Provincia.Id,
                                ProvinciaId = c.IdProvincia,
                            });
                        }
                    }
                    else
                    {
                        modelo.Status = 500;
                        modelo.Mensaje = "No hay Localidades Cargados";
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


    }
}
