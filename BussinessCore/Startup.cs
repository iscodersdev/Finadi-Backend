using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FINADICore.Services;
using Newtonsoft.Json.Serialization;
using BusinessCore.Services;
using Commons.Identity.DummyData;
using Commons.Extensions;
using Commons.Identity.Services;

namespace FINADICore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddDbContext<FINADIContext>(options =>
                options.UseLazyLoadingProxies()
                        .UseSqlServer(
                    Configuration.GetConnectionString("FINADI"),
                    //Configuration.GetConnectionString("sql_local"),

                    x => x.MigrationsAssembly("DAL")));
            
            services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;

            });

            services.AddCommonsServices<Usuario, FINADIContext>();
            services.AddDistributedMemoryCache();
            services.AddCommonsLibraryViews();
            services.AddHttpContextAccessor();
            services.AddTransient<CGEService>();
            services.AddTransient<NotificacionAPIService>();
            services.AddTransient<PlenarioService>();

            services.AddSession();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcOptions(options=> {
                    options.MaxModelValidationErrors = 50;
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                        _ => "El campo es obligatorio.");
                })
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                }
                ).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                ;

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserService<Usuario> userService, FINADIContext context)
        {
            // Asignacion de administradores por defecto
            //var adminUserResult = context.Usuarios.Where(x => x.UserName == "ramperez" || x.UserName == "pperez" || x.UserName == "mkucsera" || x.UserName == "aballa").ToList();
            //if (adminUserResult != null)
            //{
            //    foreach (Usuario a in adminUserResult)
            //    {
            //        if (usuario.GetClaimsAsync(a).Result.All(x => x.Type != "IsAdmin"))
            //        {
            //            usuario.AddClaimAsync(a, new Claim("IsAdmin", a.UserName));
            //        }
            //    }
            //}

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                DummyAdmin.Initialize<Usuario>(userService).Wait();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseHsts();                
                //Provisorio
                DummyAdmin.Initialize<Usuario>(userService).Wait();
            }

            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseSession();
            app.UseCommonsLibraryScripts();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
              );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();
        }
    }
}
