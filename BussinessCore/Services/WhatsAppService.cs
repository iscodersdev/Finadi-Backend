using DAL.DTOs.WhatsApp;
using RestSharp;

namespace FINADICore.Services
{
    public class WhatsAppService
    {
        private static string Instance = "instance30528";
        private static string Token = "j9la6hpxn0yvflzs";
        public static void EnviaWhatsAppTexto(WhatsAppDTO modelo)
        {
            var client = new RestClient("https://api.ultramsg.com/" + Instance + "/messages/chat");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "&token=" + Token + "&to=" + modelo.Telefono + "&body=" + modelo.Texto + "&priority=1&referenceId=", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            return;
        }
        public static void EnviaWhatsAppImagen(WhatsAppDTO modelo)
        {
            var client = new RestClient("https://api.ultramsg.com/" + Instance + "/messages/image");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "&token=" + Token + "&to=" + modelo.Telefono + "&image=" + modelo.ImagenUrl + "&caption=" + modelo.Texto + "&priority=1&referenceId=", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            return;
        }
        public static void EnviaWhatsAppLink(WhatsAppDTO modelo)
        {
            var client = new RestClient("https://api.ultramsg.com/" + Instance + "/messages/link");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", "&token=" + Token + "&to=" + modelo.Telefono + "&link=" + modelo.Texto + "&priority=1&referenceId=", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            return;
        }
    }
}
