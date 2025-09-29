using FINADICore.Controllers;
using Commons.Models;
using DAL.Data;
using DAL.DTOs;
using DAL.Models.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINADICore.Areas.Core.Controllers
{
    [Area("Core")]
    public class InversoresController : FINADICoreController
    {
        public InversoresController(FINADIContext context) : base(context)
        {
            breadcumb.Add(new Message() { DisplayName = "Datos" });
        }

        public IActionResult Index()
        {
            breadcumb.Add(new Message() { DisplayName = "Inversores" });
            ViewBag.Breadcrumb = breadcumb;
            return View();
        }

        public async Task<IActionResult> InversoresDataTable()
        {
            var inversores = _context.Inversores.Where(x=>x.Activo).ToList();
            var query = from v in inversores
                        select new ListInversoresDTO
                        {
                            Id = v.Id,
                            Nombre = v.Nombre,
                            Domicilio = v.Domicilio,
                            CUIT = v.CUIT,
                            Tasa=v.TasaActual.Tasa,
                            TasaNominalAnual = v.TNA==null ? 0 : v.TNA.Tasa,
                            TasaEfectivaAnual = v.TEA == null ? 0 : v.TEA.Tasa,
                            TasaEfectivaMensual = v.TEM == null ? 0 : v.TEM.Tasa,
                            CFTSinImpuesto = v.CFTSinImpuesto == null ? 0 : v.CFTSinImpuesto.Tasa,
                            CFTConImpuesto = v.CFTConImpuesto == null ? 0 : v.CFTConImpuesto.Tasa,
                        };
            return DataTable(query.AsQueryable());
        }

        public IActionResult _Create()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(Inversor newInversor)
        {
            ModelState.Remove("Id");
            ModelState.Remove("TasaInversor.Tasa");
            ModelState.Remove("TNA.Tasa");
            ModelState.Remove("TEA.Tasa");
            ModelState.Remove("CFTSinImpuesto.Tasa");
            ModelState.Remove("CFTConImpuesto.Tasa");
            ModelState.Remove("TEM.Tasa");
            if (ModelState.IsValid)
            {
                try
                {
                    Inversor inver = new Inversor
                    {
                        Nombre = newInversor.Nombre,
                        Domicilio = newInversor.Domicilio,
                        CUIT = newInversor.CUIT,
                        Activo=true
                    };
                    await _context.Inversores.AddAsync(inver);
                    await _context.SaveChangesAsync();
                    TasaInversor nuevaTasa = new TasaInversor
                    {
                        Tasa = newInversor.TasaActual.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.TasasInversores.AddAsync(nuevaTasa);

                    TNA nuevaTasaNominalAnual = new TNA
                    {
                        Tasa = newInversor.TNA.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.TasasNominalAnual.AddAsync(nuevaTasaNominalAnual);

                    TEA nuevaTasaEfectivalAnual = new TEA
                    {
                        Tasa = newInversor.TEA.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.TasasEfectivaAnual.AddAsync(nuevaTasaEfectivalAnual);

                    TEM nuevaTasaEfectivalMensual = new TEM
                    {
                        Tasa = newInversor.TEM.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.TEM.AddAsync(nuevaTasaEfectivalMensual);

                    CFTSinImpuesto nuevaCFTSinImpuesto = new CFTSinImpuesto
                    {
                        Tasa = newInversor.CFTSinImpuesto.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.CFTSinImpuesto.AddAsync(nuevaCFTSinImpuesto);

                    CFTConImpuesto nuevaCFTConImpuesto = new CFTConImpuesto
                    {
                        Tasa = newInversor.CFTConImpuesto.Tasa,
                        Inversor = inver.Id
                    };
                    await _context.CFTConImpuesto.AddAsync(nuevaCFTConImpuesto);

                    inver.TasaActual = nuevaTasa;
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se creó correctamente el Inversor " + newInversor.Nombre + ".");
                    return RedirectToAction("Index", "Inversores");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al crear el Inversor. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "newInversor");
                }
            }
            else
            {
                return PartialView(newInversor);
            }
        }


        public async Task<IActionResult> _Update(int Id)
        {

            Inversor model = await _context.Inversores.FindAsync(Id);
            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Update(Inversor editInversor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
                    invers.Nombre = editInversor.Nombre;
                    invers.Domicilio = editInversor.Domicilio;
                    invers.CUIT = editInversor.CUIT;
                    invers.FirmaReporte = editInversor.FirmaReporte;
                    _context.Inversores.Update(invers);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente el Inversor " + editInversor.Nombre + ".");
                    return RedirectToAction("Index", "Inversores");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el Inversor. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Inversores");
                }

            }
            else
            {
                return PartialView(editInversor);
            }
        }

        public async Task<IActionResult> _Tasas(int Id)
        {
            Inversor model = await _context.Inversores.FindAsync(Id);
            return PartialView(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Tasas(Inversor editInversor)
        {
            ModelState.Remove("Id");
            ModelState.Remove("TasaInversor.Tasa");
            ModelState.Remove("TNA.Tasa");
            ModelState.Remove("TEA.Tasa");
            ModelState.Remove("CFTSinImpuesto.Tasa");
            ModelState.Remove("CFTConImpuesto.Tasa");
            ModelState.Remove("TEM.Tasa");
            if (ModelState.IsValid)
            {
                try
                {
                    Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);

                    if(invers.TNA == null)
                    {
                        TNA newTNA = new TNA
                        {
                            Tasa = editInversor.TNA.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasNominalAnual.AddAsync(newTNA);
                        invers.TNA = newTNA;
                    }
                    else if (invers.TNA.Tasa != editInversor.TNA.Tasa)
                    {
                        TNA newTNA = new TNA
                        {
                            Tasa = editInversor.TNA.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasNominalAnual.AddAsync(newTNA);
                        invers.TNA = newTNA;
                    }


                    if (invers.TEM == null)
                    {
                        TEM newTEM = new TEM
                        {
                            Tasa = editInversor.TEM.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TEM.AddAsync(newTEM);
                        invers.TEM = newTEM;
                    }
                    else if(invers.TEM.Tasa != editInversor.TEM.Tasa)
                    {
                        TEM newTEM = new TEM
                        {
                            Tasa = editInversor.TEM.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TEM.AddAsync(newTEM);
                        invers.TEM = newTEM;
                    }

                    if (invers.TasaActual == null)
                    {
                        TasaInversor newTasa = new TasaInversor
                        {
                            Tasa = editInversor.TasaActual.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasInversores.AddAsync(newTasa);
                        invers.TasaActual = newTasa;
                    }
                    else if(invers.TasaActual.Tasa != editInversor.TasaActual.Tasa)
                    {
                        TasaInversor newTasa = new TasaInversor
                        {
                            Tasa = editInversor.TasaActual.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasInversores.AddAsync(newTasa);
                        invers.TasaActual = newTasa;
                    }

                    if (invers.TEA == null)
                    {
                        TEA newTEA = new TEA
                        {
                            Tasa = editInversor.TEA.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasEfectivaAnual.AddAsync(newTEA);
                        invers.TEA = newTEA;
                    }
                    else if(invers.TEA.Tasa != editInversor.TEA.Tasa)
                    {
                        TEA newTEA = new TEA
                        {
                            Tasa = editInversor.TEA.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.TasasEfectivaAnual.AddAsync(newTEA);
                        invers.TEA = newTEA;
                    }


                   

                    if (invers.CFTSinImpuesto == null)
                    {
                        CFTSinImpuesto newCFTSinImpuesto = new CFTSinImpuesto
                        {
                            Tasa = editInversor.CFTSinImpuesto.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.CFTSinImpuesto.AddAsync(newCFTSinImpuesto);
                        invers.CFTSinImpuesto = newCFTSinImpuesto;
                    }
                    else if (invers.CFTSinImpuesto.Tasa != editInversor.CFTSinImpuesto.Tasa)
                    {
                        CFTSinImpuesto newCFTSinImpuesto = new CFTSinImpuesto
                        {
                            Tasa = editInversor.CFTSinImpuesto.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.CFTSinImpuesto.AddAsync(newCFTSinImpuesto);
                        invers.CFTSinImpuesto = newCFTSinImpuesto;
                    }

                    if (invers.CFTConImpuesto == null)
                    {
                        CFTConImpuesto newCFTConImpuesto = new CFTConImpuesto
                        {
                            Tasa = editInversor.CFTConImpuesto.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.CFTConImpuesto.AddAsync(newCFTConImpuesto);
                        invers.CFTConImpuesto = newCFTConImpuesto;
                    }
                    else if (invers.CFTConImpuesto.Tasa != editInversor.CFTConImpuesto.Tasa)
                    {
                        CFTConImpuesto newCFTConImpuesto = new CFTConImpuesto
                        {
                            Tasa = editInversor.CFTConImpuesto.Tasa,
                            Inversor = editInversor.Id
                        };
                        await _context.CFTConImpuesto.AddAsync(newCFTConImpuesto);
                        invers.CFTConImpuesto = newCFTConImpuesto;
                    }


                    _context.Inversores.Update(invers);
                    await _context.SaveChangesAsync();
                    AddPageAlerts(PageAlertType.Success, "Se editó correctamente las Tasas del Inversor " + invers.Nombre + ".");
                    return RedirectToAction("Index", "Inversores");
                }
                catch (Exception e)
                {
                    AddPageAlerts(PageAlertType.Error, "Hubo un error al editar las Tasas del Inversor. Intentelo nuevamente mas tarde.");
                    return RedirectToAction("Index", "Inversores");
                }

            }
            else
            {
                return PartialView(editInversor);
            }
        }


        #region Tasas

        //public async Task<IActionResult> _Tasa(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _Tasa(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            TasaInversor newTasa = new TasaInversor
        //            {
        //                Tasa = editInversor.TasaActual.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.TasasInversores.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.TasaActual = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Tasa del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Tasa del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}

        //public async Task<IActionResult> _TasaNominalAnual(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _TasaNominalAnual(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            TNA newTasa = new TNA
        //            {
        //                Tasa = editInversor.TNA.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.TasasNominalAnual.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.TNA = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Tasa Nominal Anual del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Tasa Nominal Anual del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}


        //public async Task<IActionResult> _TasaEfectivaAnual(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _TasaEfectivaAnual(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            TEA newTasa = new TEA
        //            {
        //                Tasa = editInversor.TEA.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.TasasEfectivaAnual.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.TEA = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Tasa Efectiva Anual del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Tasa Efectiva Anual del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}

        //public async Task<IActionResult> _CFTSinImpuesto(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _CFTSinImpuesto(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            CFTSinImpuesto newTasa = new CFTSinImpuesto
        //            {
        //                Tasa = editInversor.CFTSinImpuesto.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.CFTSinImpuesto.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.CFTSinImpuesto = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente el CFT sin Impuesto del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el CFT sin Impuesto del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}




        //public async Task<IActionResult> _CFTconImpuesto(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _CFTconImpuesto(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            CFTConImpuesto newTasa = new CFTConImpuesto
        //            {
        //                Tasa = editInversor.CFTConImpuesto.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.CFTConImpuesto.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.CFTConImpuesto = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente el CFT con Impuesto del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar el CFT con Impuesto del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}


        //public async Task<IActionResult> _TasaEfectivaMensual(int Id)
        //{

        //    Inversor model = await _context.Inversores.FindAsync(Id);
        //    return PartialView(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> _TasaEfectivaMensual(Inversor editInversor)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            TEM newTasa = new TEM
        //            {
        //                Tasa = editInversor.TEM.Tasa,
        //                Inversor = editInversor.Id
        //            };
        //            await _context.TEM.AddAsync(newTasa);

        //            Inversor invers = await _context.Inversores.FindAsync(editInversor.Id);
        //            invers.TEM = newTasa;
        //            _context.Inversores.Update(invers);
        //            await _context.SaveChangesAsync();
        //            AddPageAlerts(PageAlertType.Success, "Se editó correctamente la Tasa Efectiva Mensual del Inversor " + invers.Nombre + ".");
        //            return RedirectToAction("Index", "Inversores");
        //        }
        //        catch (Exception e)
        //        {
        //            AddPageAlerts(PageAlertType.Error, "Hubo un error al editar la Tasa Efectiva Mensual del Inversor. Intentelo nuevamente mas tarde.");
        //            return RedirectToAction("Index", "Inversores");
        //        }

        //    }
        //    else
        //    {
        //        return PartialView(editInversor);
        //    }
        //}

        #endregion

        public async Task<ActionResult> _Delete(int id)
        {
            return PartialView(await _context.Inversores.FindAsync(id));
        }

        public IActionResult Delete(int id)
        {
            try
            {
                Inversor inversor = _context.Inversores.Where(s => s.Id == id).First();
                inversor.Activo = false;
                _context.SaveChanges();
                AddPageAlerts(PageAlertType.Success, "Se eliminó correctamente el Inversor.");
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                AddPageAlerts(PageAlertType.Success, "Hubo un error al eliminar el Inversor.");
                return RedirectToAction("Index", "Inversores");
            }
        }


        public ActionResult _Image(int Id)
        {
            ViewBag.InversorId = Id;
            Inversor inversor = _context.Inversores.Where(s => s.Id == Id).First();

            
            if (inversor.Logo != null)
                ViewBag.LogoInversor = Convert.ToBase64String(inversor.Logo);     
            
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> _Image(int Id, IFormFile LogoInversor)
        {
            try
            {
            Inversor inversor = _context.Inversores.Where(s => s.Id == Id).First();
               
                if (LogoInversor != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await LogoInversor.CopyToAsync(memoryStream);
                        inversor.Logo = memoryStream.ToArray();
                    }
                }                

                _context.Inversores.Update(inversor);

                await _context.SaveChangesAsync();
                AddPageAlerts(PageAlertType.Success, "Se cargo correctamente el Logo del Inversor.");
                return RedirectToAction("Index", "Inversores");

            }
            catch (System.Exception e)
            {
                AddPageAlerts(PageAlertType.Error, "Hubo un error al cargar Logo del Inversor. Intentelo nuevamente mas tarde.");
                return RedirectToAction("Index", "Inversores");
            }
        }

    }
}
