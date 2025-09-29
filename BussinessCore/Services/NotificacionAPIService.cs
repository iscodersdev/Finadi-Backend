using System;  
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using DAL.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DAL.Models.API;
using FINADICore.Areas.Core.Controllers;

namespace BusinessCore.Services
{
    public class NotificacionAPIService
    {
        public NotificacionAPIService(){}

        public string Envia_Push(string push_id, string subtitulo, string texto)
        {
            var httpClient = new HttpClient();
            Push push = new Push();
            //Actualizar app_id para Billetera
            push.app_id = "ZTBiYzY1NDAtN2RmNC00ZDM3LThkZTYtYjlmYjgxYTZkNTEx";
            List<string> id = new List<string> { push_id };
            push.include_player_ids = id;
            Idiomas idiomaheading = new Idiomas();
            idiomaheading.en = subtitulo;
            idiomaheading.es = subtitulo;
            push.headings = idiomaheading;
            Idiomas idiomacontent = new Idiomas();
            idiomacontent.en = texto;
            idiomacontent.es = texto;
            push.contents = idiomacontent;
            push.android_group = push_id;
            HttpResponseMessage result = httpClient.PostAsync("https://onesignal.com/api/v1/notifications", push.AsJson()).Result;
            string test = result.Headers.ToString();
            return test;
        }

    }
}