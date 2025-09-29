using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using DAL.Data;
using DAL.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Commons.Identity.Services;
using Commons.Controllers;
using FINADICore;
using Microsoft.AspNetCore.Identity;

namespace FINADI.Controllers
{
    [Route("api/[controller]")]
    public class MUsuarioController : BaseController
    {
        private readonly UserService<Usuario> _userService;
        private readonly SignInManager<Usuario> _signInManager;
        public FINADIContext _context;
        public MUsuarioController(FINADIContext context, UserService<Usuario> userService, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userService = userService;
            _signInManager = signInManager;
        }
        [HttpPost]
        [Route("Login")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public async Task<MLoginDTO> Login([FromBody] MLoginDTO Login)
        {
            var result = _signInManager.PasswordSignInAsync(Login.Mail, Login.Password, Login.Recordarme, lockoutOnFailure: true);
            if (result.Result.Succeeded)
            {
                //var cliente = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == Login.NumeroDocumento.ToString() && x.FechaBaja == null && (x.Empresa.Id == Login.EmpresaId || Login.EmpresaId == 0));
                var cliente = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == Login.NumeroDocumento.ToString() && x.FechaBaja == null );
                if (cliente == null)
                {
                    Login.Status = 500;
                    Login.Mensaje = "Numero Documento";
                    return Login;
                }
                Login.Apellido = cliente.Persona.Apellido;
                Login.Nombres = cliente.Persona.Nombres;
                Login.TipoPersonaId = cliente.Persona.TipoPersona.Id;
                Login.Status = 200;
                Login.UAT = FINADICore.common.Encrypt(DateTime.Now.ToString("ffffssmmHHddMMyyyy") + cliente.Id.ToString(), "FINADI");
                if (cliente.TipoCliente == null)
                {
                    Login.Categoria = "No Asociado Aun";
                }
                else
                {
                    Login.Categoria = cliente.TipoCliente.Nombre;
                }
                Login.Foto = cliente.Persona.Foto;
                Login.Mail = cliente.Usuario.Mail;
                Login.FondoMutual = cliente.Empresa.FondoMobile;
                Login.Celular = cliente.Celular;
                Login.ColorFontCarnet = cliente.Empresa.ColorFontCarnet;
                Login.ColorCarnet = cliente.Empresa.ColorCarnet;
                Login.Twitter = cliente.Empresa.Twitter;
                Login.Facebook = cliente.Empresa.Facebook;
                Login.Instagram = cliente.Empresa.Instagram;
                Login.BloquearPrestamos = cliente.BloquearPrestamos;
                if (cliente.Empresa != null)
                {
                    Login.LogoMutual = cliente.Empresa.LogoMutual;
                }
                if (cliente.NumeroCliente == null)
                {
                    Login.NumeroCliente = "No cliente";
                }
                else
                {
                    Login.NumeroCliente = cliente.NumeroCliente;
                }
                Login.PrimerIngreso = false;
                if (cliente.Usuario.DeviceId != Login.DeviceId)
                {
                    Login.PrimerIngreso = true;
                }
                cliente.Usuario.DeviceId = Login.DeviceId;
                //cliente.RecordarPassword = Login.Recordarme;
                _context.Clientes.Update(cliente);

                var persona = _context.Personas.FirstOrDefault(x => x.Id == cliente.Persona.Id);

                UAT uat = new UAT();
                uat.Cliente = cliente;
                uat.Token = Login.UAT;
                uat.FechaHora = DateTime.Now;
                if (persona != null)
                {
                    uat.Persona = persona;
                }
                _context.UAT.Add(uat);
                _context.SaveChanges();
                return Login;
            }
            else
            {
                Login.Status = 500;
                Login.Mensaje = "Password Incorrectos";
                return Login;
            }

        }
        [HttpPost]
        [Route("Login20")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MLoginDTO Login20([FromBody] MLoginDTO Login)
        {
            var result = _signInManager.PasswordSignInAsync(Login.Mail, Login.Password, Login.Recordarme, lockoutOnFailure: true);
            if (result.Result.Succeeded)
            {

                //var cliente = _context.Clientes.FirstOrDefault(x => x.Usuario.UserName == Login.Mail && x.FechaBaja == null && (x.Empresa.Id == Login.EmpresaId || Login.EmpresaId == 0));
                var cliente = _context.Clientes.FirstOrDefault(x => x.Usuario.UserName == Login.Mail && x.FechaBaja == null );
                if (cliente == null)
                {
                    Login.Status = 500;
                    Login.Mensaje = "eMail o Password Incorrectos";
                    return Login;
                }
                Login.Apellido = cliente.Persona.Apellido;
                Login.Nombres = cliente.Persona.Nombres;
                Login.TipoPersonaId = cliente.Persona.TipoPersona.Id;
                Login.Status = 200;
                Login.UAT = FINADICore.common.Encrypt(DateTime.Now.ToString("ffffssmmHHddMMyyyy") + cliente.Id.ToString(), "FINADI");
                if (cliente.TipoCliente == null)
                {
                    Login.Categoria = "No Asociado Aun";
                }
                else
                {
                    Login.Categoria = cliente.TipoCliente.Nombre;
                }
                Login.Foto = cliente.Persona.Foto;
                Login.Mail = cliente.Usuario.Email;
                if (Login.Mail != null)
                {
                    string asteriscos = "***********************************************************************";
                    string[] correoinicial = Login.Mail.Split("@");
                    Login.MailOculto = Login.Mail.Substring(0, 2) + asteriscos.Substring(0, correoinicial[0].Length - 2) + "@" + correoinicial[1];
                }
                Login.NumeroDocumento = Int64.Parse(cliente.Persona.NroDocumento);
                Login.FondoMutual = cliente.Empresa.FondoMobile;
                Login.Celular = cliente.Celular;
                Login.ColorFontCarnet = cliente.Empresa.ColorFontCarnet;
                Login.ColorCarnet = cliente.Empresa.ColorCarnet;
                Login.Twitter = cliente.Empresa.Twitter;
                Login.Facebook = cliente.Empresa.Facebook;
                Login.Instagram = cliente.Empresa.Instagram;
                Login.BloquearPrestamos = cliente.BloquearPrestamos;
                if (cliente.Empresa != null)
                {
                    Login.LogoMutual = cliente.Empresa.LogoMutual;
                }
                if (cliente.NumeroCliente == null)
                {
                    Login.NumeroCliente = "No cliente";
                }
                else
                {
                    Login.NumeroCliente = cliente.NumeroCliente;
                }
                Login.PrimerIngreso = false;
                if (cliente.Usuario.DeviceId != Login.DeviceId)
                {
                    Login.PrimerIngreso = true;
                }
                cliente.Usuario.DeviceId = Login.DeviceId;
                _context.Clientes.Update(cliente);

                var persona = _context.Personas.FirstOrDefault(x => x.Id == cliente.Persona.Id);

                UAT uat = new UAT();
                uat.Cliente = cliente;
                uat.Token = Login.UAT;
                uat.FechaHora = DateTime.Now;
                if (persona != null)
                {
                    uat.Persona = persona;
                }
                

                _context.UAT.Add(uat);
                _context.SaveChanges();
            }
            else
            {
                Login.Status = 500;
                Login.Mensaje = "eMail o Password Incorrectos";
            }
            return Login;

        }
        [HttpPost]
        [Route("RegistraPersona")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MRegistraPersonaDTO RegistraPersona([FromBody] MRegistraPersonaDTO Registro)
        {
            var cliente = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == Registro.NumeroDocumento.ToString());
            var tipopersona = _context.TiposPersonas.FirstOrDefault(x => x.Id.ToString() == Registro.TipoPersona);
            if (cliente != null && Registro.NumeroDocumento != 0)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Numero Documento Ya Existente!!!";
                return Registro;
            }
            if (Registro.Password1 != Registro.Password2 || Registro.Password1 == null)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Password No Coincidentes o Requeridas!!!";
                return Registro;
            }
           
            // var empresa = _context.Empresas.FirstOrDefault(x => x.Id == Registro.EmpresaId);
            var empresa = _context.Empresas.FirstOrDefault();
            if (empresa == null)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Mutual Inexistente!!!";
                return Registro;
            }

            if (Registro.Token != 0)
            {
                if (FINADICore.FINADI.ValidaToken(empresa, Registro.NumeroDocumento, Registro.Token) == false)
                {
                    Registro.Status = 500;
                    Registro.Mensaje = "Token Invalido!!!";
                    return Registro;
                }
            }
            var user = new Usuario()
            {
                UserName = Registro.Mail,
                Email = Registro.Mail,

            };

            var result = _userService.CreateAsync(user, Registro.Password1.ToString());

            Clientes nuevocliente = new Clientes()
            {
                Empresa = empresa,
                TipoCliente = _context.TiposClientes.Find(1),
                Celular = (Registro.Celular != null) ? Registro.Celular : "",
                Persona = new Persona()
                {
                    NroDocumento = Registro.NumeroDocumento.ToString(),
                    Apellido = Registro.Apellido,
                    Nombres = Registro.Nombres,
                    TipoPersona = tipopersona,
                    FechaNacimiento = (Registro.FechaNacimiento != null) ? Convert.ToDateTime(Registro.FechaNacimiento) : new DateTime()
                }
            };
            user.Clientes = nuevocliente;
            _context.Clientes.Add(nuevocliente);
            _context.Usuarios.Update(user);
            _context.SaveChanges();

            Registro.Status = 200;
            if (Registro.NumeroDocumento == 0)
            {
                Registro.Mensaje = "Cliente Registrado Debe Proporcionar el NumeroDocumento presencialmente";
            }
            else
            {
                Registro.Mensaje = "Cliente Registrado Correctamente!!!";
            }
            return Registro;
        }
        [HttpPost]
        [Route("TraeEmpresas")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeEmpresasDTO TraeEmpresas([FromBody] MTraeEmpresasDTO Empresas)
        {
            var empresas = _context.Empresas.Where(x => x.FechaBaja == null).OrderBy(x => x.RazonSocial);
            if (empresas != null)
            {
                Empresas.Status = 200;
                Empresas.Mensaje = "Ok";
                List<MListaEmpresas> lista = new List<MListaEmpresas>();
                foreach (var empresa in empresas)
                {
                    MListaEmpresas Empresa = new MListaEmpresas();
                    Empresa.Id = empresa.Id;
                    Empresa.Nombre = empresa.RazonSocial;
                    Empresa.Logo = empresa.LogoMutual;
                    lista.Add(Empresa);
                }
                Empresas.Empresas = lista;
            }
            else
            {
                Empresas.Status = 500;
                Empresas.Mensaje = "Lista Inexistente";
            }
            return Empresas;
        }

        [HttpPost]
        [Route("TraeDatosUsuario")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public async Task<MTraeDatosUsuarioDTO> TraeDatosUsuario([FromBody] MTraeDatosUsuarioDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            var cliente = Uat.Cliente;
            uat.Apellido = cliente.Persona.Apellido;
            uat.Nombres = cliente.Persona.Nombres;
            uat.Status = 200;
            if (cliente.TipoCliente == null)
            {
                uat.Categoria = "No Asociado Aun";
            }
            else
            {
                uat.Categoria = cliente.TipoCliente.Nombre;
            }
            uat.Foto = cliente.Persona.Foto;
            uat.Mail = cliente.Usuario.UserName;
            uat.Celular = cliente.Celular;
            uat.DeviceId = cliente.Usuario.DeviceId;
            uat.FechaNacimiento = cliente.Persona.FechaNacimiento;
            uat.FondoMobile = cliente.Empresa.FondoMobile;
            uat.ColorCarnet = cliente.Empresa.ColorCarnet;
            uat.ColorFontCarnet = cliente.Empresa.ColorFontCarnet;
            uat.Facebook = cliente.Empresa.Facebook;
            uat.Instagram = cliente.Empresa.Instagram;
            uat.Domicilio = cliente.Domicilio;
            uat.Twitter = cliente.Empresa.Twitter;
            uat.WhatsApp = cliente.Empresa.WhatsApp;
            if (cliente.Empresa != null)
            {
                uat.LogoMutual = cliente.Empresa.LogoMutual;
            }
            if (cliente.NumeroCliente == null)
            {
                uat.NumeroCliente = "No cliente";
            }
            else
            {
                uat.NumeroCliente = cliente.NumeroCliente;
            }
            return uat;
        }

        [HttpPost]
        [Route("ActualizaDatosPersona")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MActualizaDatosPersonaDTO ActualizaDatosPersona([FromBody] MActualizaDatosPersonaDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            if (uat.Password1 != null & uat.Password1 != uat.Password2)
            {
                uat.Status = 500;
                uat.Mensaje = "Passwords deben Coincidir";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Datos Actualizados Correctamente!!!";
            var cliente = Uat.Cliente;
            cliente.Domicilio = uat.Domicilio;
            cliente.Celular = uat.Celular;
            cliente.Persona.FechaNacimiento = Convert.ToDateTime(uat.FechaNacimiento);
            //cliente.Persona.Mail = uat.Mail; *** Cambiar en la tabla User
            //if (uat.Password1 != null)
            //{
            //    cliente.Password = uat.Password1;
            //}
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
            return uat;
        }
        [HttpPost]
        [Route("ActualizaFoto")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MActualizaFotoDTO ActualizaFoto([FromBody] MActualizaFotoDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            if (uat.Foto == null)
            {
                uat.Status = 500;
                uat.Mensaje = "Debe Enviar Foto";
                return uat;
            }
            uat.Status = 200;
            uat.Mensaje = "Foto Actualizada Correctamente!!!";
            var cliente = Uat.Cliente;
            cliente.Persona.Foto = uat.Foto;
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
            return uat;
        }
        [HttpPost]
        [Route("RecuperaPassword")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public async Task<MRecuperaPasswordDTO> RecuperaPassword([FromBody] MRecuperaPasswordDTO uat)
        {
            Clientes cliente = new Clientes();
            try
            {
                cliente = _context.Clientes.FirstOrDefault(x => x.Usuario.UserName == uat.email.ToString() && x.Usuario.UserName != null);
            }
            catch
            {
                uat.Status = 500;
                uat.Mensaje = "Persona Sin Correo Declarado";
                return uat;
            }
            if (cliente == null)
            {
                uat.Status = 500;
                uat.Mensaje = "Persona Inexistente";
                return uat;
            }
            var user = await _userService.FindByEmailAsync(cliente.Usuario.UserName.ToString());
            string pass = common.Encrypt(cliente.Persona.NroDocumento.ToString() + DateTime.Now.ToString(), "FINADI");
            if (user == null)
            {
                var newuser = new Usuario() { UserName = cliente.Usuario.UserName.ToString(), Email = cliente.Usuario.Mail };
                var result1 = await _userService.CreateAsync(newuser, pass);
                user = await _userService.FindByEmailAsync(cliente.Usuario.UserName.ToString());
            }
            cliente.Password = pass;
            _context.Clientes.Update(cliente);
            _context.SaveChanges();
            string sHTML = "";
            string asteriscos = "***********************************************************************";
            string[] correoinicial = cliente.Usuario.UserName.Split("@");

            if (correoinicial.Length < 2)
            {
                uat.Status = 500;
                uat.Mensaje = "Persona Sin Correo Declarado";
                return uat;
            }
            sHTML += $"Estimado: {cliente.Persona.Apellido},{cliente.Persona.Nombres}, para Poder Recuperar Su Contraseña <a href = 'http://web.finadi.com.ar/Identity/Account/ResetPassword?code=" + pass + "'> Haga Click Aqui</a>.";
            string imagen = "<img src =http://web.finadi.com.ar/images/Placa_Recupera_Contrasena.png><br/>";
            //common.EnviarMail("acevedoruben@hotmail.com", "FINADI - Recuperacion de Contraseña", sHTML, "");
            common.EnviarMail(cliente.Usuario.UserName, "FINADI - Recuperacion de Contraseña", sHTML, "", imagen);
            uat.Status = 200;
            uat.Mensaje = "Para Recuperar su Contrasena Se Ha Enviado un Correo a la Casilla: " + cliente.Usuario.UserName.Substring(0, 2) + asteriscos.Substring(0, correoinicial[0].Length - 2) + "@" + correoinicial[1] + " En el Caso de No Verlo en Bandeja De Entrada, revise su Correo No Deseado o SPAM";
            return uat;
        }
        [HttpPost]
        [Route("PreLogin")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MPreLoginDTO PreLogin([FromBody] MPreLoginDTO Login)
        {
            string NumeroDocumento = Login.NumeroDocumento;
            Login.Password = "";
            Login.Recordarme = false;
            Login.Status = 200;
            Login.Mensaje = "No Recuerda";
            if (NumeroDocumento != null)
            {
                var persona = FINADICore.FINADI.CompruebaUsuarioFINADI(Convert.ToInt32(Login.NumeroDocumento), _context);
                var clienteNumeroDocumento = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == NumeroDocumento);
                if (clienteNumeroDocumento != null)
                {
                    Login.GIFLogoMutual = clienteNumeroDocumento.Empresa.GIFLogoMutual;
                    Login.LogoMutual = clienteNumeroDocumento.Empresa.LogoMutual;
                    Login.NombrFINADI = clienteNumeroDocumento.Empresa.Abreviatura;
                    Login.ColorFondo = clienteNumeroDocumento.Empresa.ColorFondo;
                    Login.ColorBotones = clienteNumeroDocumento.Empresa.ColorBotones;
                    Login.ImagenLogin = clienteNumeroDocumento.Empresa.ImagenLogin;
                    Login.ColorLogin = clienteNumeroDocumento.Empresa.ColorLogin;
                }
            }
            Login.NumeroDocumento = "";
            if (Login.DeviceId == null)
            {
                Login.Status = 200;
                Login.Mensaje = "Device Invalido";
                Login.NecesitaRegistro = true;
                return Login;
            }
            var cliente = _context.Clientes.FirstOrDefault(x => x.Usuario.DeviceId == Login.DeviceId);
            if (cliente == null)
            {
                Login.Status = 200;
                Login.Mensaje = "Primer Login";
                return Login;
            }
            if (cliente.Usuario.RecordarPassword == true)
            {
                Login.Status = 200;
                Login.Mensaje = "Login Ok";
                Login.NumeroDocumento = cliente.Persona.NroDocumento.ToString();
                //Login.Password = cliente.Password;
                Login.Recordarme = true;
                Login.GIFLogoMutual = cliente.Empresa.GIFLogoMutual;
                Login.LogoMutual = cliente.Empresa.LogoMutual;
                Login.NombrFINADI = cliente.Empresa.Abreviatura;
                Login.ColorFondo = cliente.Empresa.ColorFondo;
                Login.ColorBotones = cliente.Empresa.ColorBotones;
                Login.ImagenLogin = cliente.Empresa.ImagenLogin;
                Login.ColorLogin = cliente.Empresa.ColorLogin;
                return Login;
            }
            return Login;
        }
        [Route("PreLogin20")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MPreLogin20DTO PreLogin20([FromBody] MPreLogin20DTO Login)
        {
            string eMail = Login.eMail;
            Login.Password = "";
            Login.Recordarme = false;
            Login.Status = 200;
            Login.Mensaje = "Ok";
            if (eMail != null)
            {
                var persona = FINADICore.FINADI.CompruebaUsuarioFINADI20(Login.eMail, _context);
                var clienteeMail = _context.Clientes.Where(x => x.Usuario.Mail == eMail);
                List<MMutualesDTO> lista = new List<MMutualesDTO>();
                foreach (var clientemail in clienteeMail)
                {
                    var mutual = new MMutualesDTO();
                    mutual.GIFLogoMutual = clientemail.Empresa.GIFLogoMutual;
                    mutual.LogoMutual = clientemail.Empresa.LogoMutual;
                    mutual.NombrFINADI = clientemail.Empresa.Abreviatura;
                    mutual.ColorFondo = clientemail.Empresa.ColorFondo;
                    mutual.ColorBotones = clientemail.Empresa.ColorBotones;
                    mutual.ImagenLogin = clientemail.Empresa.ImagenLogin;
                    mutual.ColorLogin = clientemail.Empresa.ColorLogin;
                    mutual.ClienteId = clientemail.Empresa.Id;
                    lista.Add(mutual);
                }
                Login.Mutuales = lista;
                return Login;
            }
            Login.eMail = "";
            if (Login.DeviceId == null)
            {
                Login.Status = 200;
                Login.Mensaje = "Device Invalido";
                Login.NecesitaRegistro = true;
                return Login;
            }
            var cliente = _context.Clientes.Where(x => x.Usuario.DeviceId == Login.DeviceId);
            if (cliente.Count() == 0)
            {
                Login.Status = 200;
                Login.Mensaje = "Primer Login";
                return Login;
            }
            if (cliente.FirstOrDefault().Usuario.RecordarPassword == true)
            {
                var clientes = _context.Clientes.Where(x => x.Persona.NroDocumento == cliente.FirstOrDefault().Persona.NroDocumento);
                List<MMutualesDTO> lista = new List<MMutualesDTO>();
                foreach (var clientedoc in clientes)
                {
                    var mutual = new MMutualesDTO();
                    mutual.GIFLogoMutual = clientedoc.Empresa.GIFLogoMutual;
                    mutual.LogoMutual = clientedoc.Empresa.LogoMutual;
                    mutual.NombrFINADI = clientedoc.Empresa.Abreviatura;
                    mutual.ColorFondo = clientedoc.Empresa.ColorFondo;
                    mutual.ColorBotones = clientedoc.Empresa.ColorBotones;
                    mutual.ImagenLogin = clientedoc.Empresa.ImagenLogin;
                    mutual.ColorLogin = clientedoc.Empresa.ColorLogin;
                    mutual.ClienteId = clientedoc.Empresa.Id;
                    lista.Add(mutual);
                    Login.Mutuales = lista;
                }
                Login.Status = 200;
                Login.Mensaje = "Login Ok";
                Login.eMail = cliente.FirstOrDefault().Usuario.Mail;
                if (Login.eMail != null)
                {
                    string asteriscos = "***********************************************************************";
                    string[] correoinicial = cliente.FirstOrDefault().Usuario.Mail.Split("@");
                    Login.MailOculto = cliente.FirstOrDefault().Usuario.Mail.Substring(0, 2) + asteriscos.Substring(0, correoinicial[0].Length - 2) + "@" + correoinicial[1];
                }
                Login.Password = cliente.FirstOrDefault().Usuario.Password;
                Login.Recordarme = true;
                return Login;
            }
            return Login;
        }
        [HttpPost]
        [Route("TraeCredenciales")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MTraeCredencialesDTO TraeCredenciales([FromBody] MTraeCredencialesDTO uat)
        {
            var Uat = _context.UAT.FirstOrDefault(x => x.Token == uat.UAT);
            if (uat == null)
            {
                uat.Status = 500;
                uat.Mensaje = "UAT Invalida";
                return uat;
            }
            uat.ColorCarnet = Uat.Cliente.Empresa.ColorCarnet;
            uat.ColorFontCarnet = Uat.Cliente.Empresa.ColorFontCarnet;
            uat.LogoClub = Uat.Cliente.Empresa.LogoMutual;
            uat.Status = 200;
            uat.Mensaje = "Credenciales Ok";
            var Clientes = _context.Clientes.Where(x => x.Id == Uat.Cliente.Id || x.DependeDe.Id == Uat.Cliente.Id).OrderBy(x => x.NumeroCliente);
            List<CredencialDTO> lista = new List<CredencialDTO>();
            foreach (var cliente in Clientes)
            {
                var credencial = new CredencialDTO();
                credencial.Apellido = cliente.Persona.Apellido;
                credencial.NumeroDocumento = cliente.Persona.NroDocumento.ToString();
                if (cliente.Persona.Foto != null)
                {
                    credencial.Foto = cliente.Persona.Foto;
                }
                credencial.Nombres = cliente.Persona.Nombres;
                if (cliente.NumeroCliente == null)
                {
                    credencial.NumeroCliente = "No Asociado Aun";
                }
                else
                {
                    credencial.NumeroCliente = cliente.NumeroCliente;
                }
                if (cliente.TipoCliente != null)
                {
                    credencial.TipoCliente = cliente.TipoCliente.Nombre;
                }
                lista.Add(credencial);
            }
            uat.Credenciales = lista;
            return uat;
        }
        [HttpPost]
        [Route("TraeOrganismo")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public async Task<MTraeOrganismosDTO> TraeOrganismos([FromBody] MTraeOrganismosDTO organismo)
        {
            //var tipopersona = _context.TiposPersonas.Where(x => x.Organismo.Descripcion != "Ejercito").OrderBy(x => x.Organismo.Orden);
            var tipopersona = _context.Organismos.Where(x => x.Activo).OrderBy(x => x.Orden);
            if (tipopersona != null)
            {
                organismo.Status = 200;
                organismo.Mensaje = "Ok";
                List<MListaOrganismos> lista = new List<MListaOrganismos>();
                foreach (var organi in tipopersona)
                {
                    MListaOrganismos Organismo = new MListaOrganismos();
                    Organismo.Id = organi.Id;
                    Organismo.Descripcion = organi.Descripcion;
                    lista.Add(Organismo);
                }
                organismo.Organismos = lista;

            }
            else
            {
                organismo.Status = 500;
                organismo.Mensaje = "Lista Inexistente";
            }
            return organismo;
        }
        [HttpPost]
        [Route("PreRegistro")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]
        public MPreRegistroDTO PreRegistro([FromBody] MPreRegistroDTO Registro)
        {
            var preregistro = new MPreRegistroDTO();
            //var cliente = _context.Clientes.FirstOrDefault(x => x.Usuario.UserName  == Registro.eMail.Trim());
            var usuario = _context.Usuarios.FirstOrDefault(x => x.UserName == Registro.eMail.Trim());
            if (usuario != null && usuario.EmailConfirmed)
            {
                preregistro.Status = 500;
                preregistro.Mensaje = "Socio Ya Ingresado, debera recuperar contraseña";
                return preregistro;
            }
            int token = common.NiumeroRandom(100000, 999999);
            string html = "";


            var organismo = _context.Organismos.Find(Registro.OrganismoId);
            if (organismo.APIEjercito == true)
            {
                var datoscge = FINADICore.FINADI.TraeDatosPersona20CGE(_context.Empresas.FirstOrDefault(), Registro.eMail, _context, token);

                if (datoscge.Status == 300)
                {
                    preregistro.Status = 300;
                    preregistro.Mensaje = "No es personal de Ejercito";

                    return preregistro;
                }
                token = datoscge.Token;
                preregistro.Apellido = datoscge.Apellido;
                preregistro.Celular = datoscge.Celular;
                preregistro.eMail = datoscge.Mail;
                if (preregistro.eMail != null)
                {
                    string asteriscos = "***********************************************************************";
                    string[] correoinicial = preregistro.eMail.Split("@");
                    preregistro.MailOculto = preregistro.eMail.Substring(0, 2) + asteriscos.Substring(0, correoinicial[0].Length - 2) + "@" + correoinicial[1];
                }
                preregistro.Mensaje = datoscge.Mensaje;
                preregistro.Status = datoscge.Status;
                preregistro.Nombres = datoscge.Nombres;
                preregistro.NumeroDocumento = datoscge.NumeroDocumento;

                html = "<br/>Sr: " + datoscge.Nombres + " " + datoscge.Apellido + "<br/><br/>";
                html += "Su Token de Registro es: " + token.ToString() + "<br/><br/>";
                string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Token.png><br/>";
                common.EnviarMail(datoscge.Mail, "Token Registro FINADI", html, "", imagen);
                return preregistro;

            }

            // SI ES OTRO ORGANISMO
            var tipopersona = _context.TiposPersonas.Where(x => x.Organismo.Activo && x.Organismo.Id == Registro.OrganismoId).OrderBy(x => x.Organismo.Orden);
            if (tipopersona != null)
            {
                List<MListaTipoPersonas> lista = new List<MListaTipoPersonas>();
                foreach (var tipopers in tipopersona)
                {
                    MListaTipoPersonas TipoPersona = new MListaTipoPersonas();
                    TipoPersona.Id = tipopers.Id;
                    TipoPersona.Descripcion = tipopers.nombre;
                    lista.Add(TipoPersona);
                }
                preregistro.TipoPersonas = lista;
            }

            preregistro.Status = 300;
            preregistro.Mensaje = "No es personal de Ejercito";

            if (usuario == null)
            {
                var user = new Usuario()
                {
                    UserName = Registro.eMail,
                    Email = Registro.eMail,
                    Token = token
                };
                var result = _userService.CreateAsync(user, "12345678");
                if (result.Result.Succeeded)
                {
                    html = "<br/>Sr: " + Registro.eMail + "<br/><br/>";
                    html += "Su Token de Registro es: " + token.ToString() + "<br/><br/>";
                    string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Token.png><br/>";
                    common.EnviarMail(Registro.eMail.Trim(), "Token Registro FINADI", html, "", imagen);
                    preregistro.Mensaje = "Usuario creado con Exito";
                }
            }
            else
            {
                if (!usuario.EmailConfirmed)
                {
                    usuario.Token = token;
                    _context.Usuarios.Update(usuario);
                    _context.SaveChanges();
                    html = "<br/>Sr: " + Registro.eMail + "<br/><br/>";
                    html += "Su Token de Registro es: " + token.ToString() + "<br/><br/>";
                    string imagen = "<img src=http://web.finadi.com.ar/images/Placa_Token.png><br/>";
                    common.EnviarMail(Registro.eMail.Trim(), "Token Registro FINADI", html, "", imagen);
                    preregistro.Mensaje = "Envio email con Token !!";
                }
                else
                {
                    preregistro.Status = 500;
                    preregistro.Mensaje = "Socio Ya Ingresado, debera recuperar contraseña";

                }
            }

            return preregistro;

        }
        [HttpPost]
        [Route("RegistraPersona20")]
        [EnableCors("CorsPolicy")]
        [AllowAnonymous]

        public async Task<MRegistraPersonaDTO> RegistraPersona20([FromBody] MRegistraPersonaDTO Registro)
        {
            if (Registro.Mail == null)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Debe Ingresar un eMail!!!";
                return Registro;
            }
            var cli = _context.Clientes.Where(x => x.Usuario.Personas.NroDocumento.Trim() == Registro.NumeroDocumento.ToString().Replace(".", "").Trim());
            if (cli.Count() > 0)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Socio Ya Existente con el Nro Dni !";
                return Registro;
            }
            var user = await _userService.FindByEmailAsync(Registro.Mail.ToString());
            if (user != null && Registro.Mail == user.UserName && user.EmailConfirmed)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Socio Ya Existente!!!";
                return Registro;
            }
            if (Registro.Password1 != Registro.Password2 || Registro.Password1 == null)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Password No Coincidentes o Requeridas!!!";
                return Registro;
            }

            //var empresa = _context.Empresas.FirstOrDefault(x => x.Id == Registro.EmpresaId);
            var empresa = _context.Empresas.FirstOrDefault();

            if (empresa == null)
            {
                Registro.Status = 500;
                Registro.Mensaje = "Mutual Inexistente!!!";
                return Registro;
            }

            if (Registro.TipoPersona == null)
            {
                Registro.TipoPersona = _context.TiposPersonas.FirstOrDefault().Id.ToString();
            }

            if (user is null)
            {
                user = new Usuario()
                {

                    UserName = Registro.Mail,
                    Email = Registro.Mail
                };
                var result = await _userService.CreateAsync(user, Registro.Password1.ToString());
                user = await _userService.FindByEmailAsync(Registro.Mail.ToString());

            }
            else
            {
                var result = await _userService.ChangePasswordAsync(user, "12345678", Registro.Password1.ToString());
                if (result.Succeeded)
                {
                    Registro.Mensaje = "Usuario actualizado correctamente!";

                }
            }
            DateTime fechanacimiento = DateTime.Now;
            try
            {
                fechanacimiento = Convert.ToDateTime(Registro.FechaNacimiento);
            }
            catch
            {

            }
            Clientes nuevocliente = new Clientes();
            if (user.Clientes != null)
            {
                nuevocliente = user.Clientes;
            }
            nuevocliente.Empresa = empresa;
            nuevocliente.TipoCliente = _context.TiposClientes.Find(1);
            nuevocliente.Celular = (Registro.Celular != null) ? Registro.Celular : "";
            nuevocliente.NumeroCelularDetectado = (Registro.NumeroCelularDetectado != null) ? Registro.NumeroCelularDetectado : "";
            if (nuevocliente.Persona != null)
            {
                nuevocliente.Persona.NroDocumento = Registro.NumeroDocumento.ToString();
                nuevocliente.Persona.Apellido = Registro.Apellido;
                nuevocliente.Persona.Nombres = Registro.Nombres;
                nuevocliente.Persona.FechaNacimiento = fechanacimiento;
            }
            else
            {
                nuevocliente.Persona = new Persona()
                {
                    NroDocumento = Registro.NumeroDocumento.ToString(),
                    Apellido = Registro.Apellido,
                    Nombres = Registro.Nombres,
                    FechaNacimiento = fechanacimiento,
                };
            }
            Registro.Status = 200;
            if (Registro.NumeroDocumento == 0)
            {
                Registro.Mensaje = "Cliente Registrado Debe Proporcionar el NumeroDocumento presencialmente";
            }
            else
            {
                Registro.Mensaje = "Cliente Registrado Correctamente!!!";
            }
            //nuevocliente.Persona.TipoPersona = _context.TiposPersonas.Find(Convert.ToInt32(Registro.TipoPersona));

            var tipopersona = _context.TiposPersonas.Find(Convert.ToInt32(Registro.TipoPersona));
             if (tipopersona.Organismo.APIEjercito == true)
            {   //ES DEL EJERCITO
                var persona = FINADICore.FINADI.TraeDatosPersona20CGE(empresa, Registro.Mail, _context, Registro.Token);
                if (persona.Status == 200)
                {
                    if (FINADICore.FINADI.ValidaToken20(empresa, Registro.Mail, Registro.Token) == false)
                    {
                        Registro.Status = 500;
                        Registro.Mensaje = "Token Invalido!!!";
                        return Registro;
                    }
                    if (persona.TipoPersona == "Soldados")
                    {
                        nuevocliente.Persona.TipoPersona = _context.TiposPersonas.Find(3);
                    }
                    if (persona.TipoPersona == "Militares")
                    {
                        nuevocliente.Persona.TipoPersona = _context.TiposPersonas.Find(1);
                    }
                    if (persona.TipoPersona == "Civiles" || persona.TipoPersona == "Docentes")
                    {
                        nuevocliente.Persona.TipoPersona = _context.TiposPersonas.Find(2);
                    }
                }

            }
            else 
            { //OTROS ORGANISMOS
                nuevocliente.Persona.TipoPersona = _context.TiposPersonas.Find(Convert.ToInt32(Registro.TipoPersona));
                var validartoken = _context.Usuarios.FirstOrDefault(x => x.UserName == Registro.Mail);
                if (validartoken.Token != Registro.Token)
                {
                    Registro.Status = 500;
                    Registro.Mensaje = "Token Invalido!!!";
                    return Registro;
                }
            }
                 
            nuevocliente.NroDocReferido = Registro.Referido;
            nuevocliente.RazonSocial = nuevocliente.Persona.Apellido + ", " + nuevocliente.Persona.Nombres;
            nuevocliente.FechaIngreso = DateTime.Now;
            if (nuevocliente.Id != 0)
            {
                _context.Clientes.Update(nuevocliente);
            }
            else
            {
                _context.Clientes.Add(nuevocliente);
            }
            _context.SaveChanges();
            user.Clientes = nuevocliente;
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = true;
            user.Password = Registro.Password1;
            user.Personas = nuevocliente.Persona;
            _context.Usuarios.Update(user);
            _context.SaveChanges();

            Registro.Status = 200;
            if (Registro.NumeroDocumento == 0)
            {
                Registro.Mensaje = "Cliente Registrado Debe Proporcionar el NumeroDocumento presencialmente";
            }
            else
            {
                Registro.Mensaje = "Cliente Registrado Correctamente!!!";
            }

            return Registro;
        }
    }
}