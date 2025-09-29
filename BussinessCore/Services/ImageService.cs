using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace FINADICore.Services
{
    public class ImageService
    {
        private IHostingEnvironment _env;
        public ImageService(IHostingEnvironment env)
        {
            _env = env;
        }
        
        public string Upload(int id, IFormFile archivo)
        {
            string ruta = _env.WebRootPath + "/images/campana/" + id + ".jpg";
            string rutaServer = "http://emutual.com.ar:91/images/campana/" + id+".jpg";
            FileStream fs = File.Create(ruta);
            archivo.CopyTo(fs);
            fs.Close();
            return rutaServer;
        }
    }
}
