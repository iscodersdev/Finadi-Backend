using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(FINADICore.Areas.Identity.IdentityHostingStartup))]
namespace FINADICore.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}