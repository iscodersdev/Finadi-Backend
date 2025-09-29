using Commons.Identity.Services;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace FINADICore.Services
{
    public class CGEService
    {

        static HttpClient clientToken = new HttpClient();
        string UrlCGEApi = "https://www.cge.mil.ar:81/api/mentidades/";
        private FINADIContext _context;

        public CGEService(FINADIContext context)
        {
            _context = context;
        }

        #region ApiCGE
        public MTraePrestamosAprobadosDTO MTraePrestamosAprobadosDTO(MTraePrestamosAprobadosDTO Uat)
        {
            using (var client = new HttpClient())
            {
                Uat.Prestamos = new List<MPrestamosAprobadosDTO>();
                client.BaseAddress = new Uri(UrlCGEApi);
                //HttpResponseMessage response = client.PostAsJsonAsync("TraePrestamosAprobados", Uat).Result;
                HttpResponseMessage response = client.PostAsJsonAsync("TraePrestamosAConfirmar", Uat).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<MTraePrestamosAprobadosDTO>();
                    readTask.Wait();
                    Uat = readTask.Result;
                    if (Uat.Status == 200)
                    {
                        return Uat;
                    }
                }
                return Uat;
            }
        }

        public Clientes TraePersonaCGE(Empresas empresa, int DNI, string Codigo, FINADIContext _context)
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

                                    cliente.Usuario.Mail = datospersona.eMail;
                                    cliente.Usuario.Password = DNI.ToString();
                                    cliente.Usuario.UserName = datospersona.eMail;

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

        public Clientes TraeDatosPersonaCGE(string UAT, int DNI, string Codigo = "")
        {
            Clientes cliente = new Clientes();            
            using (var client2 = new HttpClient())
            {
                client2.BaseAddress = new Uri("https://www.cge.mil.ar:81/api/mentidades/");
                MDatosPersonaDTO datospersona = new MDatosPersonaDTO();
                datospersona.UAT = UAT;
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
                        };

                        cliente.Usuario = new Usuario();
                        cliente.Usuario.Mail = datospersona.eMail;
                        cliente.Usuario.Password = DNI.ToString();
                        cliente.Usuario.UserName = datospersona.eMail;
                        cliente.Usuario.NormalizedUserName = datospersona.eMail;
                        cliente.Usuario.Personas = cliente.Persona;

                        cliente.CategoriaLaboral = datospersona.Categoria;
                        cliente.Celular = datospersona.Celular;
                        cliente.EsMilitar = true;                        

                        cliente.NumeroLegajoLaboral = datospersona.NOU.ToString();
                        cliente.NumeroCliente = Codigo;
                        cliente.RazonSocial = datospersona.Apellido + ", " + datospersona.Nombres;
                        cliente.FechaIngreso = DateTime.Now;
                        cliente.FechaIngresoLaboral = datospersona.FechaIngreso;
                    }
                }
            }       
            return cliente;
        }


        public string LoginCGE(Empresas empresa)
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
                        return login.UAT;
                    }
                }
                return response.ToString();
            }
        }

        #endregion
    }
}