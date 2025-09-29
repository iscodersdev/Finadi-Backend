using System;
using System.Linq;
using System.Threading.Tasks;
using FINADICore.Areas.Administracion.ViewModels;
using FINADICore.Controllers;
using FINADICore.Services;
using BussinessCore.Areas.Administracion.ViewModels;
using Commons.Identity.Services;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FINADICore.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    public class UsuariosController : FINADICoreController
    {


        private readonly FINADIContext _context;
        private readonly UserService<Usuario> _userService;
        private readonly UserManager<Usuario> _userManager;
        private readonly CGEService _CGEService;

        public UsuariosController(FINADIContext context, UserService<Usuario> userService, UserManager<Usuario> userManager, CGEService CGEService) : base(context)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
            _CGEService = CGEService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult UsuariosDataTable()
        {
            var usuarios = _context.Users.ToList();
            var query = from usu in usuarios
                        where usu.Personas != null ? usu.Personas.CreadoPorUsuarioId != null : false
                        select new UserDTViewModel
                        {
                            Id = usu.Id,
                            Usuario = usu.UserName,
                            Nombre = usu.Personas?.GetNombreCompleto(),
                            Empresa = usu.Clientes?.Empresa?.RazonSocial

                        };
            return DataTable(query.AsQueryable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            UsuarioVM newUser = CargarViewBag(new UsuarioVM());
            return PartialView(newUser);
        }


        [HttpPost]
        public async Task<IActionResult> Create(UsuarioVM usuario)
        {
            try
            {
                ModelState.Remove("Persona.TipoDocumento.Descripcion");
                ModelState.Remove("Persona.Genero.Descripcion");
                if (ModelState.IsValid)
                {
                    Usuario nuevoUsuario = new Usuario()
                    {
                        UserName = usuario.Mail,
                        Email = usuario.Mail,
                        Mail = usuario.Mail,
                        Password = usuario.Password,
                        Administradores = usuario.Administrador,
                        PhoneNumberConfirmed = true
                    };
                    var result = await _userService.CreateAsync(nuevoUsuario, usuario.Password);
                    nuevoUsuario = _context.Usuarios.FirstOrDefault(x => x.UserName == usuario.Mail);
                    
                    Persona nuevaPersona = new Persona();

                    nuevaPersona.TipoDocumento = _context.TipoDocumento.Find(usuario.Persona.TipoDocumento.Id);
                    nuevaPersona.EstadoCivil = _context.EstadosCiviles.Find(usuario.Persona.EstadoCivil.Id);
                    nuevaPersona.Pais = _context.Paises.Find(usuario.Persona.Pais.Id);
                    nuevaPersona.Genero = _context.Genero.Find(usuario.Persona.Genero.Id);
                    nuevaPersona.Apellido = usuario.Persona.Apellido;
                    nuevaPersona.Nombres = usuario.Persona.Nombres;
                    nuevaPersona.CantidadHijos = usuario.Persona.CantidadHijos;
                    nuevaPersona.Cuil = usuario.Persona.Cuil;
                    nuevaPersona.FechaNacimiento = usuario.Persona.FechaNacimiento;
                    nuevaPersona.NroDocumento = usuario.Persona.NroDocumento;

                    var usuarioLogueado = _context.Users.Where(x => x.UserName == User.Identity.Name).First();
                    nuevaPersona.CreadoPorUsuarioId = usuarioLogueado.Id;
                    nuevaPersona.FechaCreacionModificacion = DateTime.Now;
                    await _context.Personas.AddAsync(nuevaPersona);
                    nuevoUsuario.Personas = nuevaPersona;

                    Clientes nuevoCliente = new Clientes();

                    nuevoCliente.EsUsuarioInterno = true;
                    nuevoCliente.Empresa = _context.Empresas.FirstOrDefault();
                    nuevoCliente.Persona = nuevaPersona;
                    nuevoCliente.CreadoPorUsuarioId = usuarioLogueado.Id;
                    nuevoCliente.FechaCreacionModificacion = DateTime.Now;
                    nuevoCliente.Usuario = nuevoUsuario;

                    await _context.Clientes.AddAsync(nuevoCliente);

                    nuevoUsuario.Clientes = nuevoCliente;
                    _context.Usuarios.Update(nuevoUsuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    usuario = CargarViewBag(usuario);
                    return PartialView(usuario);
                }

            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            Usuario updateUser = await _context.Usuarios.FindAsync(Id);
            UsuarioVM updateModel = new UsuarioVM()
            {
                UserId = updateUser.Id,
                Mail = updateUser.Email,
                Password = updateUser.Password,
                Administrador = updateUser.Administradores,
                Persona = updateUser.Personas
            };
            updateModel = CargarViewBag(updateModel);
            return PartialView(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UsuarioVM usuario)
        {
            try
            {
                ModelState.Remove("Persona.TipoDocumento.Descripcion");
                ModelState.Remove("Persona.Genero.Descripcion");
                ModelState.Remove("Persona.Id");
                if (ModelState.IsValid)
                {
                    Persona updatePersona = new Persona();
                    if (usuario.Persona.Id != 0)
                    {
                        updatePersona = await _context.Personas.FindAsync(usuario.Persona.Id);
                    }

                    updatePersona.NroDocumento = usuario.Persona.NroDocumento;
                    updatePersona.Nombres = usuario.Persona.Nombres;
                    updatePersona.Apellido = usuario.Persona.Apellido;
                    updatePersona.Cuil = usuario.Persona.Cuil;
                    updatePersona.FechaNacimiento = usuario.Persona.FechaNacimiento;
                    updatePersona.TipoDocumento = await _context.TipoDocumento.FindAsync(usuario.Persona.TipoDocumento.Id);
                    updatePersona.EstadoCivil = await _context.EstadosCiviles.FindAsync(usuario.Persona.EstadoCivil.Id);
                    updatePersona.Pais = await _context.Paises.FindAsync(usuario.Persona.Pais.Id);
                    updatePersona.Genero = await _context.Genero.FindAsync(usuario.Persona.Genero.Id);
                    var usuarioLogueado = _context.Users.Where(x => x.UserName == User.Identity.Name).First();
                    updatePersona.CreadoPorUsuarioId = usuarioLogueado.Id;
                    updatePersona.FechaCreacionModificacion = DateTime.Now;

                    if (usuario.Persona.Id != 0)
                    {
                        _context.Personas.Update(updatePersona);
                    }
                    else
                    {
                        _context.Personas.Add(updatePersona);
                    }
                    await _context.SaveChangesAsync();

                    var user = await _context.Users.FindAsync(usuario.UserId);
                    user.Personas = updatePersona;
                    user.UserName = usuario.Mail;
                    user.NormalizedUserName = usuario.Mail.ToUpper();
                    user.Email = usuario.Mail;
                    user.NormalizedEmail = usuario.Mail.ToUpper();
                    user.Mail = usuario.Mail;
                    user.Password = usuario.Password;
                    user.Administradores = usuario.Administrador;
                    if (usuario.Administrador)
                    {
                        user.Clientes = null;
                    }
                    else
                    {
                        if (user.Clientes == null)
                        {
                            user.Clientes = new Clientes();
                            user.Clientes.Empresa = _context.Empresas.Find(1);
                            user.Clientes.Persona = updatePersona;
                            updatePersona.CreadoPorUsuarioId = usuarioLogueado.Id;
                            updatePersona.FechaCreacionModificacion = DateTime.Now;
                            await _context.Clientes.AddAsync(user.Clientes);
                        }

                    }
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, usuario.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    usuario = CargarViewBag(usuario);
                    return PartialView(usuario);
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }

        }



        [HttpGet]
        public async Task<IActionResult> UpdatePasswordWeb(string Id)
        {
            Usuario updateUser = await _context.Usuarios.FindAsync(Id);
            UsuarioPasswordVM updateModel = new UsuarioPasswordVM()
            {
                UserId = updateUser.Id,
                Password = updateUser.Password,
            };
            return PartialView("UpdatePassword", updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePasswordWeb(UsuarioPasswordVM User)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Usuario updatePersona = await _context.Users.FindAsync(User.UserId);

                    var token = await _userManager.GeneratePasswordResetTokenAsync(updatePersona);

                    var result = await _userManager.ResetPasswordAsync(updatePersona, token, User.Password);

                    if (result.Succeeded)
                    {
                        AddPageAlerts(PageAlertType.Success, "Se Modificó Correctamente la Constraseña.");
                    }
                    else
                    {
                        AddPageAlerts(PageAlertType.Error, "Hubo un error al Modificar la Contraseña. Intentelo nuevamente mas tarde.");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return PartialView("UpdatePassword", User);
                }
            }
            catch (Exception e)
            {

                return RedirectToAction("Index");
            }

        }



        public UsuarioVM CargarViewBag(UsuarioVM user)
        {
            user.TipoDocumento = _context.TipoDocumento.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            user.EstadoCivil = _context.EstadosCiviles.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            user.Pais = _context.Paises.Select(g => new SelectListItem() { Text = g.Nombre, Value = g.Id.ToString() });
            user.Genero = _context.Genero.Select(g => new SelectListItem() { Text = g.Descripcion, Value = g.Id.ToString() });
            return user;
        }

        [HttpPost]
        public string validarUsuariosCGE()
        {
            int contUsuarios = 0;
            if (User.Identity.Name == "pruebasFINADI@FINADI.com")
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(@"wwwroot/Plantillas/UsuariosCGE.txt");
                try
                {
                    var listDNI = _context.Personas.GroupBy(x => x.NroDocumento).Where(x => x.Count() > 1).Select(x => x.Key).ToList();



                    foreach (var item in listDNI.Where(x => x != null))
                    {
                        var uat = _CGEService.LoginCGE(_context.Empresas.First());
                        var Usuarios = _context.Usuarios.Where(x => x.Personas.NroDocumento == item).ToList();

                        foreach (var itemUsuarios in Usuarios)
                        {
                            if (itemUsuarios.Personas != null)
                            {
                                var clienteCGE = _CGEService.TraeDatosPersonaCGE(uat, Convert.ToInt32(itemUsuarios.Personas.NroDocumento));
                                if (clienteCGE.Usuario != null && itemUsuarios.Email != null && clienteCGE.Usuario.Mail != null)
                                {

                                    if (itemUsuarios.Email.ToLower() == clienteCGE.Usuario.Mail.ToLower())
                                    {
                                        contUsuarios++;
                                        itemUsuarios.PhoneNumberConfirmed = true;
                                        _context.Users.Update(itemUsuarios);
                                        _context.SaveChanges();

                                        string linea = $"Nro: {contUsuarios}, DNI: {itemUsuarios.Personas.NroDocumento} , UsuarioId: {itemUsuarios.Id} , Username: {itemUsuarios.UserName}, Email: {itemUsuarios.Email} , Apellido: {itemUsuarios.Personas.Apellido} , Nombre: {itemUsuarios.Personas.Nombres}, Fecha: {DateTime.Now}";
                                        file.WriteLine(linea);
                                    }
                                }
                            }
                        }
                    }
                    file.Close();
                    return $"Se Actualizaron {contUsuarios} Usuarios";
                }
                catch (Exception e)
                {
                    file.Close();
                    return $"Error: {e.Message} , se puedieron actualizar {contUsuarios} Usuarios";
                }
            }
            else
            {
                return "Usuario No Autorizado";
            }

        }


















        //public async Task<IActionResult> LoginAs([FromServices] CommonsCps.Services.SignInService<Usuario> signInService, string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var usuario = _context.Users.Where(u => u.Id == id).Include(u => u.Persona).FirstOrDefault();

        //    if (usuario == null)
        //    {
        //        return NotFound();
        //    }

        //    await signInService.SignInAsync(usuario, false);

        //    AddPageAlerts(PageAlertType.Info, $"Has iniciado sesión como {usuario.Persona?.GradoAbreviatura} {usuario.Persona?.Apellido.ToUpper()}, {usuario.Persona?.Nombre}");

        //    return RedirectToAction("Index", "Home");
        //}

        //public IActionResult _DetalleUsuario(string id)
        //{
        //    Usuario usuario = _context.Users.Where(u => u.Id == id).Include(u => u.Persona).Include(u => u.DatosSimulacion).FirstOrDefault();
        //    UsuarioViewModel usuarioVM = new UsuarioViewModel
        //    {
        //        Usuario = usuario,
        //        Roles = _userService.GetRoles(usuario.Id).Result.ToList<IdentityRole>()
        //    };
        //    return PartialView(usuarioVM);
        //}

        //[HttpGet]
        //public IActionResult _ReportesUsuario()
        //{
        //    List<SelectListItem> reportes = new List<SelectListItem>();
        //    reportes.Add(new SelectListItem("Usuarios y Roles", ReportesUsuario.UsuariosyRoles.ToString()));
        //    reportes.Add(new SelectListItem("Roles y Funciones", ReportesUsuario.RolesyFunciones.ToString()));
        //    reportes.Add(new SelectListItem("Funciones y Rutas", ReportesUsuario.FuncionesyRutas.ToString()));
        //    ViewBag.Reportes = reportes;
        //    ViewBag.Roles = _context.AspNetRoles.Select(r => new SelectListItem() { Text = r.ShowName, Value = r.Id });
        //    ViewBag.Funciones = _context.AspNetFunctions.Select(f => new SelectListItem() { Text = f.Name, Value = f.Id });
        //    return PartialView();
        //}

        //[HttpPost]
        //[AutoValidateAntiforgeryToken]
        //public IActionResult _ReportesUsuario(ReportesUsuario reporte, string rol = "0", string funcion = "0")
        //{
        //    string rep = "";
        //    string parametros = "";
        //    switch (reporte)
        //    {
        //        case ReportesUsuario.UsuariosyRoles:
        //            rep = "adm_usuarios_roles";
        //            parametros = "&user_id=0&rol_id=" + rol;
        //            break;
        //        case ReportesUsuario.RolesyFunciones:
        //            rep = "adm_roles_funciones";
        //            parametros = "&rol_id=" + rol + "&funcion_id=" + funcion;
        //            break;
        //        case ReportesUsuario.FuncionesyRutas:
        //            rep = "adm_funciones_rutas";
        //            parametros = "&funcion_id=" + funcion;
        //            break;
        //        default:
        //            break;
        //    }
        //    @ViewBag.PDF = "data:application/pdf;base64," + Convert.ToBase64String(Reportes.Reporting(rep, parametros, "PDF"));
        //    return PartialView("_Reporte");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _BuscarPersonaPorDocumento(int NumeroDocumento, string txtError = "")
        //{
        //    var persona = _integracionAPIService.BuscarPersonaPorDocumento(NumeroDocumento);
        //    if (persona == null) return RedirectToAction("_BuscarPersonaPorDocumento", new { txtError = "No se encontro la persona" });
        //    ViewBag.txtError = txtError;

        //    //Aca se puede extender para atender otros casos de uso
        //    return PartialView("_CrearUsuarioPersona", persona);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _CrearUsuarioPorDocumento(int NumeroDocumento)
        //{
        //    switch (_usuarioGEDEAService.CrearUsuarioGEDEA(NumeroDocumento))
        //    {
        //        case ResultadosUsuario.UsuarioExistente:
        //            return RedirectToAction("_BuscarPersonaPorDocumento", new { txtError = "El usuario ya existe" });
        //        case ResultadosUsuario.PersonaInexistente:
        //            return RedirectToAction("_BuscarPersonaPorDocumento", new { txtError = "No se encontro el nombre de usuario asociado a la persona" });
        //        case ResultadosUsuario.UsuarioCreado:
        //            AddPageAlerts(PageAlertType.Success, "Se ha creado el usuario correctamente.");
        //            return View("Index");
        //        default:
        //            AddPageAlerts(PageAlertType.Success, "Se ha creado el usuario correctamente.");
        //            return View("Index");
        //    }
        //}


        //public async Task<IActionResult> CrearUsuariosPrueba()
        //{

        //var user = new Usuario { UserName = "basico", Email = "basico@prueba.com" };
        //await _userService.CreateAsync(user, "Probando.33");
        //user = new Usuario { UserName = "auxPersonal", Email = "auxPersonal@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");
        //user = new Usuario { UserName = "jefePersonal", Email = "jefePersonal@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");
        //user = new Usuario { UserName = "auxElemento", Email = "auxElemento@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");
        //user = new Usuario { UserName = "jefeElemento", Email = "jefeElemento@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");
        //user = new Usuario { UserName = "usuarioDGOD", Email = "usuarioDGOD@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");
        //user = new Usuario { UserName = "usuarioDGPB", Email = "usuarioDGPB@prueba.com" };
        //await _userService.CreateAsync(user, "Pepe.33");

        //AddPageAlerts(PageAlertType.Success, "Se crearon los usuarios de prueba satisfactoriamente.");
        //    return View("Index");
        //}

    }
}
