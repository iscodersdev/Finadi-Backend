using System;  
using DAL.Data;
using System.Net.Http;
using System.Linq;
using DAL.Models;
using MimeKit;
using MimeKit.Text;
//using MailKit.Net.Smtp;
using MailKit.Security;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Net.Mail;
using System.Net;


namespace FINADICore
{
    public class FINADI
    {
        public static string UrlFINADI = "http://138.99.6.117/api/socios/";
        public static Clientes CompruebaUsuarioFINADI(int DNI, FINADIContext _context)
        {
            Clientes persona = new Clientes();
            string uat = LoginFINADI();
            using (var client = new HttpClient())
            {
                ClientesDTO cliente = new ClientesDTO();
                cliente.UAT = uat;
                cliente.DNI = DNI;
                client.BaseAddress = new Uri(UrlFINADI);
                HttpResponseMessage response = client.PostAsJsonAsync("TraeSocio", cliente).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<ClientesDTO>();
                    readTask.Wait();
                    cliente = readTask.Result;
                    if (cliente.Status == 500)
                    {
                        return persona;
                    }
                    if (cliente.Activo == false)
                    {
                        return persona;
                    }
                    var empresa = _context.Empresas.Find(2);
                    persona = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == DNI.ToString());
                    if (persona == null)
                    {
                        persona = TraePersonaCGE(empresa, DNI, cliente.Codigo, _context);
                    }
                }
            }
            return persona;
        }
        public static Clientes CompruebaUsuarioFINADI20(string eMail, FINADIContext _context)
        {
            Clientes persona = new Clientes();
            string uat = LoginFINADI();
            using (var client = new HttpClient())
            {
                ClientesDTO cliente = new ClientesDTO();
                cliente.UAT = uat;
                cliente.eMail = eMail;
                client.BaseAddress = new Uri(UrlFINADI);
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("TraeSocio20", cliente).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<ClientesDTO>();
                        readTask.Wait();
                        cliente = readTask.Result;
                        if (cliente.Status == 500)
                        {
                            return persona;
                        }
                        if (cliente.Activo == false)
                        {
                            return persona;
                        }
                        var empresa = _context.Empresas.Find(2);
                        persona = _context.Clientes.FirstOrDefault(x => x.Usuario.Mail == eMail);
                        if (persona == null)
                        {
                            persona = TraePersonaCGE20(empresa, eMail, cliente.Codigo, _context);
                        }
                    }
                }
                catch
                {
                    return persona;
                }
            }
            return persona;
        }
        public static Clientes TraePersonaCGE(Empresas empresa, int DNI, string Codigo, FINADIContext _context)
        {
            Clientes cliente = new Clientes();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.DNI = DNI;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosPersona", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {

                                    cliente.Persona = new Persona
                                    {
                                        Apellido = datospersona.Apellido,
                                        Cuil = datospersona.CUIL.ToString(),
                                        FechaNacimiento = datospersona.FechaNacimiento,
                                        Nombres = datospersona.Nombres,
                                        NroDocumento = DNI.ToString(),
                                        TipoDocumento = _context.TipoDocumento.Find(1),
                                        Pais = _context.Paises.Find(1),
                                        TipoPersona = _context.TiposPersonas.Find(1)
                                    };

                                    cliente.CategoriaLaboral = datospersona.Categoria;
                                    cliente.Celular = datospersona.Celular;
                                    cliente.Empresa = empresa;
                                    cliente.EsMilitar = true;
                                    cliente.Usuario = new Usuario();
                                    cliente.Usuario.Mail = datospersona.eMail;
                                    cliente.Usuario.Password = DNI.ToString();

                                    cliente.NumeroLegajoLaboral = datospersona.NOU.ToString();
                                    cliente.NumeroCliente = Codigo;
                                    cliente.RazonSocial = datospersona.Apellido + ", " + datospersona.Nombres;
                                    cliente.TipoCliente = _context.TiposClientes.Find(1);
                                    cliente.FechaIngreso = DateTime.Now;
                                    cliente.FechaIngresoLaboral = datospersona.FechaIngreso;
                                    _context.Personas.Add(cliente.Persona);
                                    _context.Clientes.Add(cliente);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }

        public static Clientes TraePersonaCGE20(Empresas empresa, string eMail, string Codigo, FINADIContext _context)
        {
            Clientes cliente = new Clientes();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.eMail = eMail;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosPersona20", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    cliente.Persona = new Persona
                                    {
                                        Apellido = datospersona.Apellido,
                                        Cuil = datospersona.CUIL.ToString(),
                                        FechaNacimiento = datospersona.FechaNacimiento,
                                        Nombres = datospersona.Nombres,
                                        NroDocumento = datospersona.DNI.ToString(),
                                        TipoDocumento = _context.TipoDocumento.Find(1),
                                        Pais = _context.Paises.Find(1),
                                        TipoPersona = _context.TiposPersonas.Find(1)
                                    };

                                    cliente.CategoriaLaboral = datospersona.Categoria;
                                    cliente.Celular = datospersona.Celular;
                                    cliente.Empresa = empresa;
                                    cliente.EsMilitar = true;

                                    //cliente.Usuario.Mail = datospersona.eMail;
                                    //cliente.Usuario.Password = datospersona.DNI.ToString();

                                    cliente.NumeroLegajoLaboral = datospersona.NOU.ToString();
                                    cliente.NumeroCliente = Codigo;
                                    cliente.RazonSocial = datospersona.Apellido + ", " + datospersona.Nombres;
                                    cliente.TipoCliente = _context.TiposClientes.Find(1);
                                    //cliente.FechaIngreso = DateTime.Now;
                                    cliente.FechaIngresoLaboral = datospersona.FechaIngreso;
                                    _context.Personas.Add(cliente.Persona);
                                    _context.Clientes.Add(cliente);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }
        public static Clientes ActualizaPersonaCGE20(Empresas empresa, Clientes cliente, string Codigo, FINADIContext _context)
        {
            //Clientes cliente = new Clientes();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.eMail = cliente.Usuario.UserName;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosPersona20", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200 )
                                {
                                    //cliente.Persona 
                                    //{
                                    //cliente.Persona.Apellido = datospersona.Apellido;
                                    cliente.Persona.Cuil = datospersona.CUIL.ToString();
                                    //cliente.Persona.FechaNacimiento = datospersona.FechaNacimiento;
                                    //cliente.Persona.Nombres = datospersona.Nombres;
                                    //cliente.Persona.NroDocumento = datospersona.DNI.ToString();
                                    cliente.Persona.TipoDocumento = _context.TipoDocumento.Find(1);
                                    cliente.Persona.Pais = _context.Paises.Find(1);
                                    //cliente.Persona.TipoPersona = _context.TiposPersonas.Find(1);
                                    //};
                                    cliente.Usuario.Mail = cliente.Usuario.UserName;
                                    cliente.CategoriaLaboral = datospersona.Categoria;
                                    cliente.Celular = datospersona.Celular;
                                    cliente.Empresa = empresa;
                                    cliente.EsMilitar = true;

                                    cliente.Usuario.Mail = datospersona.eMail;
                                    //cliente.Usuario.Password = datospersona.DNI.ToString();
                                    cliente.DestinoLaboral = datospersona.Unidad; 
                                    cliente.NumeroLegajoLaboral = datospersona.NOU.ToString();
                                    //cliente.NumeroCliente = Codigo;
                                    cliente.RazonSocial = datospersona.Apellido + ", " + datospersona.Nombres;
                                    cliente.TipoCliente = _context.TiposClientes.Find(1);
                                    //cliente.FechaIngreso = DateTime.Now;
                                    cliente.FechaIngresoLaboral = datospersona.FechaIngreso;
                                    
                                    _context.Clientes.Update(cliente);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }

        public static string LoginFINADI()
        {
            using (var client = new HttpClient())
            {
                LoginDTO login = new LoginDTO();
                login.UsuarioId = "f27c48c3-5e2a-4f44-994e-d5a9b357499a";
                login.Password = "AQAAAAEAACcQAAAAEGzU7b87CI1dA0xq9z5Uxs+H4WpmcX/VrDVVy5nJEwKOz2IIfzU5AjexKSgNU/Kv7g==";
                client.BaseAddress = new Uri(UrlFINADI);
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<LoginDTO>();
                        readTask.Wait();
                        login = readTask.Result;
                        if (login.Status == 200)
                        {
                            return login.UAT;
                        }
                    }
                    return response.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
        public static bool ActualizaPrestamos(int DNI, FINADIContext _context)
        {
            Clientes persona = new Clientes();
            string uat = LoginFINADI();
            using (var client = new HttpClient())
            {
                PrestamosClienteDTO cliente = new PrestamosClienteDTO();
                cliente.UAT = uat;
                cliente.DNI = DNI;
                client.BaseAddress = new Uri(UrlFINADI);
                HttpResponseMessage response = client.PostAsJsonAsync("TraePrestamos", cliente).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<PrestamosClienteDTO>();
                    readTask.Wait();
                    cliente = readTask.Result;
                    if (cliente.Status == 500)
                    {
                        return false;
                    }
                    var prestamos = _context.Prestamos.Where(x => x.Cliente.Persona.NroDocumento == DNI.ToString());
                    foreach (var prestamo in prestamos)
                    {
                        foreach (var prestamoamma in cliente.Prestamos)
                        {
                            if (prestamo.PrestamoNumero == prestamoamma.Id)
                            {
                                prestamo.CantidadCuotas = prestamoamma.CantidadCuotas;
                                prestamo.Capital = prestamoamma.Capital;
                                prestamo.FechaSolicitado = prestamoamma.Fecha;
                                prestamo.Saldo = prestamoamma.Saldo;
                                prestamo.CuotasRestantes = prestamoamma.CuotasRestantes;
                                prestamo.PrestamoNumero = prestamoamma.Id;
                                if (prestamoamma.Saldo == 0)
                                {
                                    prestamo.EstadoActual = _context.EstadosPrestamos.Find(3);
                                }
                                else
                                {
                                    prestamo.EstadoActual = _context.EstadosPrestamos.Find(4);
                                }
                                prestamo.MontoCuota = prestamoamma.MontoCuota;
                                prestamo.CFT = common.CalculaCFT(Convert.ToDouble(prestamoamma.Capital), prestamoamma.CantidadCuotas, Convert.ToDouble(prestamo.MontoCuota));
                                _context.Prestamos.Update(prestamo);
                            }
                        }
                    }
                    _context.SaveChanges();
                    foreach (var prestamoamma in cliente.Prestamos)
                    {
                        var prestamoexistente = prestamos.FirstOrDefault(x => x.PrestamoNumero == prestamoamma.Id);
                        if (prestamoexistente == null)
                        {
                            Prestamos prestamonuevo = new Prestamos();
                            prestamonuevo.Cliente = _context.Clientes.FirstOrDefault(x => x.Persona.NroDocumento == DNI.ToString());
                            prestamonuevo.PrestamoNumero = prestamoamma.Id;
                            prestamonuevo.CantidadCuotas = prestamoamma.CantidadCuotas;
                            prestamonuevo.Capital = prestamoamma.Capital;
                            prestamonuevo.FechaSolicitado = prestamoamma.Fecha;
                            prestamonuevo.MontoCuota = prestamoamma.MontoCuota;
                            prestamonuevo.Saldo = prestamoamma.Saldo;
                            prestamonuevo.CuotasRestantes = prestamoamma.CuotasRestantes;
                            prestamonuevo.Linea = _context.LineasPrestamos.Find(1);
                            if (prestamoamma.Saldo == 0)
                            {
                                prestamonuevo.EstadoActual = _context.EstadosPrestamos.Find(3);
                            }
                            else
                            {
                                prestamonuevo.EstadoActual = _context.EstadosPrestamos.Find(4);
                            }
                            prestamonuevo.MontoCuota = prestamoamma.MontoCuota;
                            prestamonuevo.CFT = common.CalculaCFT(Convert.ToDouble(prestamoamma.Capital), prestamoamma.CantidadCuotas, Convert.ToDouble(prestamonuevo.MontoCuota));
                            _context.Prestamos.Add(prestamonuevo);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            return true;
        }
        public static decimal ActualizaRenglones(Prestamos prestamo, int PrestamoAMMAId, FINADIContext _context)
        {
            string uat = LoginFINADI();
            decimal ultimacuota = prestamo.MontoCuota;
            var renglones = _context.CuentasCorrientes.Where(x => x.Prestamo.Id == prestamo.Id);
            _context.CuentasCorrientes.RemoveRange(renglones);
            using (var client = new HttpClient())
            {
                PrestamosCuotasDTO cliente = new PrestamosCuotasDTO();
                cliente.UAT = uat;
                cliente.Id = PrestamoAMMAId;
                client.BaseAddress = new Uri(UrlFINADI);
                HttpResponseMessage response = client.PostAsJsonAsync("TraePrestamosCuotas", cliente).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<PrestamosCuotasDTO>();
                    readTask.Wait();
                    cliente = readTask.Result;
                    if (cliente.Status == 500)
                    {
                        return 0;
                    }
                    decimal saldo = 0;
                    foreach (var cuota in cliente.Cuotas)
                    {
                        saldo = saldo + cuota.Importe;
                        var cuotanueva = new CuentasCorrientes();
                        cuotanueva.Cliente = prestamo.Cliente;
                        cuotanueva.Credito = cuota.Importe;
                        cuotanueva.Fecha = cuota.FechaCuota;
                        cuotanueva.Prestamo = prestamo;
                        cuotanueva.Saldo = saldo;
                        cuotanueva.TipoMovimiento = _context.TiposMovimientos.Find(3);
                        if (cuota.Importe > 0)
                        {
                            ultimacuota = cuota.Importe;
                        }
                        _context.CuentasCorrientes.Add(cuotanueva);
                        if (cuota.Pagado > 0)
                        {
                            saldo = saldo - cuota.Pagado;
                            var pagonuevo = new CuentasCorrientes();
                            pagonuevo.Cliente = prestamo.Cliente;
                            pagonuevo.Debito = cuota.Pagado;
                            pagonuevo.Fecha = cuota.FechaPagado;
                            pagonuevo.Prestamo = prestamo;
                            pagonuevo.Saldo = saldo;
                            pagonuevo.TipoMovimiento = _context.TiposMovimientos.Find(2);
                            _context.CuentasCorrientes.Add(pagonuevo);
                        }
                    }
                }
            }
            _context.SaveChanges();
            return ultimacuota;
        }
        public static MTraeListaProvinciasDTO TraeProvinciasCGE(Empresas empresa, FINADIContext _context)
        {
            MTraeListaProvinciasDTO provincia = new MTraeListaProvinciasDTO();
            provincia.Status = 500;
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            provincia.UAT = login.UAT;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeListaProvincias", provincia).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MTraeListaProvinciasDTO>();
                                readTask2.Wait();
                                provincia = readTask2.Result;
                            }
                            else
                            {
                                provincia.Mensaje = "Error de API";
                            }
                        }
                    }
                }
            }
            return provincia;

        }
        public static MTraeListaUnidadesDTO TraeUnidadesCGE(Empresas empresa, FINADIContext _context, int ProvinciaId)
        {
            MTraeListaUnidadesDTO unidad = new MTraeListaUnidadesDTO();
            unidad.Status = 500;
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            unidad.UAT = login.UAT;
                            unidad.ProvinciaId = ProvinciaId;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeListaUnidades", unidad).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MTraeListaUnidadesDTO>();
                                readTask2.Wait();
                                unidad = readTask2.Result;
                            }
                            else
                            {
                                unidad.Mensaje = "Error de API";
                            }
                        }
                    }
                }
            }
            return unidad;

        }
        public static MRegistraPersonaDTO TraeDatosPersonaCGE(Empresas empresa, int DNI, FINADIContext _context, int token)
        {
            MRegistraPersonaDTO cliente = new MRegistraPersonaDTO();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.DNI = DNI;
                            datospersona.Token = token;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosPersonaToken", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    cliente.Apellido = datospersona.Apellido;
                                    cliente.Celular = datospersona.Celular;
                                    cliente.FechaNacimiento = datospersona.FechaNacimiento.ToString();
                                    cliente.Mail = datospersona.eMail;
                                    cliente.Nombres = datospersona.Nombres;
                                    cliente.NumeroDocumento = DNI;
                                    cliente.Status = datospersona.Status;
                                    cliente.Mensaje = datospersona.Mensaje;
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }
        public static MRegistraPersonaDTO TraeDatosPersona20CGE(Empresas empresa, string eMail, FINADIContext _context, int token)
        {
            MRegistraPersonaDTO cliente = new MRegistraPersonaDTO();
            using (var client = new HttpClient())
            {
                cliente.Status = 300;
                cliente.Mensaje = "Persona No Econtrada";
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.eMail = eMail;
                            datospersona.Token = token;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosPersonaToken20", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    cliente.Apellido = datospersona.Apellido;
                                    cliente.Celular = datospersona.Celular;
                                    cliente.FechaNacimiento = datospersona.FechaNacimiento.ToString();
                                    cliente.Mail = datospersona.eMail;
                                    cliente.Nombres = datospersona.Nombres;
                                    cliente.NumeroDocumento = datospersona.DNI;
                                    cliente.Status = datospersona.Status;
                                    cliente.Mensaje = datospersona.Mensaje;
                                    cliente.TipoPersona = datospersona.TipoPersona;
                                    cliente.Token = datospersona.Token;
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }
        public static bool ValidaToken(Empresas empresa, int DNI, int token)
        {
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.DNI = DNI;
                            datospersona.Token = token;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("VerificaTokenPersona", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool ValidaToken20(Empresas empresa, string eMail, int token)
        {
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.eMail = eMail;
                            datospersona.Token = token;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("VerificaTokenPersona20", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosPersonaDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static MTraeDatosCampanaDTO TraeCampana(Empresas empresa, Campanas campana)
        {
            MTraeDatosCampanaDTO datos = new MTraeDatosCampanaDTO();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            datos.MaximoDisponible = campana.MaximoDisponible;
                            datos.MinimoDisponible = campana.MinimoDisponible;
                            datos.ProvinciaId = campana.ProvinciaId;
                            datos.UAT = login.UAT;
                            datos.UnidadId = campana.UnidadId;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosCampana", datos).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MTraeDatosCampanaDTO>();
                                readTask2.Wait();
                                datos = readTask2.Result;
                            }
                        }
                    }
                }
            }
            return datos;
        }
        //public static string EnviarMailEmpresa(DAL.Models.Empresas empresa, string destinatario, string titulo, string texto, string imagenUrl, string remitente = null)
        //{
        //    try
        //    {
        //        //Byte [] Imagen = ResizeImagen(imagen);
        //        var email = new MimeMessage();
        //        if (remitente != null)
        //        {
        //            email.From.Add(MailboxAddress.Parse(remitente));
        //        }
        //        else
        //        {
        //            email.From.Add(MailboxAddress.Parse(empresa.Mail));
        //        }
        //        email.To.Add(MailboxAddress.Parse(destinatario));
        //        email.Subject = titulo;
        //        email.Body = new TextPart(TextFormat.Html) { Text = cuerpoHTMLEmpresa(titulo, texto, "", empresa, imagenUrl) };
        //        var smtp = new SmtpClient();
        //        smtp.Connect(empresa.UrlMail, empresa.PuertoMail, SecureSocketOptions.SslOnConnect);
        //        smtp.Authenticate(empresa.UsernameMail, empresa.PasswordMail);
        //        smtp.Send(email);
        //        smtp.Disconnect(true);

        //        return "ok";
        //    }
        //    catch
        //    {
        //        return "false";
        //    }

        //}
        public static bool  EnviarMailEmpresa(DAL.Models.Empresas empresa, string destinatario, string titulo, string texto, string imagenUrl, string remitente = null)
        {
           
                if (destinatario == null)
                {
                    return false;
                }
                try
                {
                    //string usuario = "novedades@FINADI.org.ar";
                    string usuario = "albarracin_sergio@hotmail.com"; 
                    //string password = "BWSNmr7qGLdHYKz2";
                    string password = "w2cPVg3n9Xq6C7KO";   
                    var origen = new MailAddress("noresponder@FINADI.org.ar", "FINADI");
                    string host = "smtp-relay.sendinblue.com";
                    int puerto = 587;
                    bool ssl = true;
                    NetworkCredential credenciales = new NetworkCredential(usuario, password);
                    MailMessage correo = new MailMessage("noresponder@FINADI.org.ar", destinatario ,titulo, cuerpoHTMLEmpresa(titulo, texto,"",empresa,imagenUrl));
                    correo.From = origen;
                    correo.IsBodyHtml = true;
                    SmtpClient servicio = new SmtpClient(host, puerto);
                    servicio.UseDefaultCredentials = true;
                    servicio.Credentials = credenciales;
                    servicio.EnableSsl = ssl;
                    string token = "";
                    servicio.SendAsync(correo, token);
                }
                catch
                {
                    return false;
                }
                return true;

            

        }
        public static string cuerpoHTMLEmpresa(string titulo, string texto, string cliente, DAL.Models.Empresas empresa, string imagenurl)
        {
            string sHTML = "";
            sHTML += "<html>";
            sHTML += "<meta charset='UTF-8'>";
            sHTML += "<body>";
            sHTML += "<img src=http://emutual.com.ar:91/images/Logoemail.jpeg><br/>";
            //sHTML += "<img src='data:image/jpg;base64," + Convert.ToBase64String(imagen) + "'><br/>";
            sHTML += "<h3>";
            sHTML += titulo;
            sHTML += "</h3><br><br>";
            sHTML += "<div id='Texto' style='font-size:12px; margin-bottom:20px; '>";
            sHTML += texto;
            sHTML += "</div>";
            sHTML += "<div id='footer' style='font-size:12px; text-align:left;'>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>" + empresa.RazonSocial + "</b></p><br><br>";
            sHTML += "<p style='margin-top:0px; margin-bottom:0px;'><b>(2023) FINADI</b></p>";
            sHTML += "</div>";
            sHTML += "</body>";
            sHTML += "</html>";
            return sHTML;
        }
        public static byte[] ResizeImagen(byte[] original, int alto = 0, int ancho = 0)
        {
            if (original == null) return null;

            Image imagen = Image.Load(original);
            imagen.Mutate(i => i.Resize(ancho == 0 ? ConstImagenes.AnchoReporte : ancho, alto == 0 ? ConstImagenes.AltoReporte : alto));
            var salida = new MemoryStream();
            imagen.SaveAsPng(salida);

            return salida?.ToArray();
        }
        public static MTraeListarPersonasDTO TraeClientes(Empresas empresa, string Apellido,string Nombres,Int64 DNI,string UAT)
        {
            MTraeListarPersonasDTO datos = new MTraeListarPersonasDTO();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                if (UAT == null)
                {
                    login.CUIT = empresa.CUIT;
                    login.Password = empresa.PasswordCGE;
                    login.Token = empresa.TokenCGE;
                    client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                    HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                        readTask.Wait();
                        login = readTask.Result;
                    }
                }
                else
                {
                    login.Status = 200;
                    login.UAT = UAT;
                }
                if (login.Status == 200)
                {
                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                        datos.UAT = login.UAT;
                        datos.Nombres = Nombres;
                        datos.Apellido = Apellido;
                        datos.DNI = DNI;
                        HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeListaPersonas", datos).Result;
                        if (response2.IsSuccessStatusCode)
                        {
                            var readTask2 = response2.Content.ReadAsAsync<MTraeListarPersonasDTO>();
                            readTask2.Wait();
                            datos = readTask2.Result;
                        }
                    }
                }
            }
            return datos;
        }
        public static MDatosScoringDTO TraeDatosScoring(Empresas empresa, int DNI, FINADIContext _context)
        {
            MDatosScoringDTO cliente = new MDatosScoringDTO();
            using (var client = new HttpClient())
            {
                MLoginEntidadesDTO login = new MLoginEntidadesDTO();
                login.CUIT = empresa.CUIT;
                login.Password = empresa.PasswordCGE;
                login.Token = empresa.TokenCGE;
                client.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                HttpResponseMessage response = client.PostAsJsonAsync("Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MLoginEntidadesDTO>();
                    readTask.Wait();
                    login = readTask.Result;
                    if (login.Status == 200)
                    {
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                            MDatosScoringDTO datospersona = new MDatosScoringDTO();
                            datospersona.UAT = login.UAT;
                            datospersona.DNI = DNI;
                            HttpResponseMessage response2 = client2.PostAsJsonAsync("TraeDatosScoring", datospersona).Result;
                            if (response2.IsSuccessStatusCode)
                            {
                                var readTask2 = response2.Content.ReadAsAsync<MDatosScoringDTO>();
                                readTask2.Wait();
                                datospersona = readTask2.Result;
                                if (datospersona.Status == 200)
                                {
                                    return datospersona;
                                }
                            }
                        }
                    }
                }
            }
            return cliente;
        }
    }
    public static class ConstImagenes
    {
        public static int AltoReporte = 500;
        public static int AnchoReporte = 500;
    }
}