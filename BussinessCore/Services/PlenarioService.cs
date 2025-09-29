using System;  
using DAL.Data;
using System.Net.Http;
using System.Linq;
using DAL.DTOs.AgilPagosDTO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DAL.DTOs.Plenario;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace FINADICore
{

    [AllowAnonymous]
    public class PlenarioService : ControllerBase
    {
        public FINADIContext _context;
        public string Url = "https://qa.FINADI.org.ar/PlenarioServices.svc/json/"; //Testing
        //public string Url = "https://produccion.FINADI.org.ar/PlenarioServices.svc/json/"; //Produccion
        public PlenarioService(FINADIContext context)
        {
            _context = context;
        }

        #region Endpoints Plenario


        [HttpPost]
        public LoginResponse Login(Login login)
        {
            LoginResponse loginResponse = new LoginResponse();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Url);
                HttpResponseMessage response = client.PostAsJsonAsync("Usuarios/Login", login).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<LoginResponse>();
                    readTask.Wait();                    
                    loginResponse = readTask.Result;
                    if (loginResponse.statusCode == 200)
                    {
                        return loginResponse;
                    }
                    return loginResponse;
                }
            }
            return loginResponse;
        }

        
        [HttpPost]
        public OperationResult PrestamoSolicitudSave(PrestamosSolicitud prestamoSolicitud, TokenValoresPosibles tokenValoresPosibles)
        {
            string valoresUrl = $"{tokenValoresPosibles.Token}/{Convert.ToInt32(tokenValoresPosibles.boolGenerarCredito)}/{Convert.ToInt32(tokenValoresPosibles.boolOtorgarCredito)}/{Convert.ToInt32(tokenValoresPosibles.boolGenerarOrdendePago)}/{Convert.ToInt32(tokenValoresPosibles.boolLiquidarOrdenPago)}/{tokenValoresPosibles.Id_Banco}/{Convert.ToInt32(tokenValoresPosibles.IsWorkFlow)}/{Convert.ToInt32(tokenValoresPosibles.updateWithModel)}";

            OperationResult responseNotificion = new OperationResult();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(Url);
                string completa = "Prestamos/Solicitudes/Save/" + valoresUrl;
                HttpResponseMessage response = client.PostAsJsonAsync(completa, prestamoSolicitud).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsAsync<OperationResult>();
                    readTask.Wait();
                    responseNotificion = readTask.Result;
                    if (responseNotificion.statusCode == HttpStatusCode.OK)
                    {
                        return responseNotificion;
                    }
                    return responseNotificion;
                }
            }
            return responseNotificion;
        }
        

        #endregion
    }
}

