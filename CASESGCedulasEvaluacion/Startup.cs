using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CedulasEvaluacion.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CedulasEvaluacion.Services;

namespace CASESGCedulasEvaluacion
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

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddScoped<IRepositorioLogin, RepositorioLogin>();
            services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
            services.AddScoped<IRepositorioPerfiles, RepositorioPerfiles>();
            services.AddScoped<IRepositorioAreas, RepositorioAreas>();
            services.AddScoped<IRepositorioCelular, RepositorioCelular>();
            services.AddScoped<IRepositorioAgua, RepositorioAgua>();
            services.AddScoped<IRepositorioAnalisis, RepositorioAnalisis>();
            services.AddScoped<IRepositorioResiduos, RepositorioResiduos>();
            services.AddScoped<IRepositorioFacturas, RepositorioFacturas>();
            services.AddScoped<IRepositorioDocuments, RepositorioDocuments>();
            services.AddScoped<IRepositorioMuebles, RepositorioMuebles>();
            services.AddScoped<IRepositorioPerfilCelular, RepositorioPerfilCelular>();
            services.AddScoped<IRepositorioFinancieros, RepositorioFinancieros>();
            services.AddScoped<IRepositorioOperacionesPerfil, RepositorioOperacionesPerfil>();
            services.AddScoped<IRepositorioModulos, RepositorioModulos>();
            services.AddScoped<IRepositorioOperaciones, RepositorioOperaciones>();
            services.AddScoped<IRepositorioCatalogoServicios, RepositorioCatalogoServicios>();
            services.AddScoped<IRepositorioContratosServicio, RepositorioContratosServicio>();

            services.AddScoped<IRepositorioInmuebles, RepositorioInmuebles>();
            services.AddScoped<IRepositorioLimpieza, RepositorioLimpieza>();
            services.AddScoped<IRepositorioFumigacion, RepositorioFumigacion>();
            services.AddScoped<IRepositorioMensajeria, RepositorioMensajeria>();
            services.AddScoped<IRepositorioCelular, RepositorioCelular>();
            services.AddScoped<IRepositorioConvencional, RepositorioConvencional>();
            services.AddScoped<IRepositorioIncidencias, RepositorioIncidencias>();
            services.AddScoped<IRepositorioTrasladoExp, RepositorioTrasladoExp>();
            services.AddScoped<IRepositorioTransporte, RepositorioTransporte>();

            services.AddScoped<IRepositorioIncidenciasMensajeria, RepositorioIncidenciasMensajeria>();
            services.AddScoped<IRepositorioIncidenciasAnalisis, RepositorioIncidenciasAnalisis>();
            services.AddScoped<IRepositorioIncidenciasMuebles, RepositorioIncidenciasMuebles>();
            services.AddScoped<IRepositorioIncidenciasTraslado, RepositorioIncidenciasTraslado>();
            services.AddScoped<IRepositorioIncidenciasCelular, RepositorioIncidenciasCelular>();
            services.AddScoped<IRepositorioIncidenciasConvencional, RepositorioIncidenciasConvencional>();
            services.AddScoped<IRepositorioIncidenciasFumigacion, RepositorioIncidenciasFumigacion>();
            services.AddScoped<IRepositorioIncidenciasResiduos, RepositorioIncidenciasResiduos>();
            services.AddScoped<IRepositorioIncidenciasTransporte, RepositorioIncidenciasTransporte>();
            services.AddScoped<IRepositorioIncidenciasAgua, RepositorioIncidenciasAgua>();
            services.AddScoped<IRepositorioIncidenciasAnalisis, RepositorioIncidenciasAnalisis>();
            
            services.AddScoped<IRepositorioEntregables, RepositorioEntregablesLimpieza>();
            services.AddScoped<IRepositorioEntregablesAnalisis, RepositorioEntregablesAnalisis>();
            services.AddScoped<IRepositorioEntregablesTrasladoExp, RepositorioEntregablesTrasladoExp>();
            services.AddScoped<IRepositorioEntregablesMensajeria, RepositorioEntregablesMensajeria>();
            services.AddScoped<IRepositorioEntregablesCelular, RepositorioEntregablesCelular>();
            services.AddScoped<IRepositorioEntregablesConvencional, RepositorioEntregablesConvencional>();
            services.AddScoped<IRepositorioEntregablesResiduos, RepositorioEntregablesResiduos>();
            services.AddScoped<IRepositorioEntregablesFumigacion, RepositorioEntregablesFumigacion>();
            services.AddScoped<IRepositorioEntregablesAgua, RepositorioEntregablesAgua>();
            services.AddScoped<IRepositorioEntregablesTransporte, RepositorioEntregablesTransporte>();
            services.AddScoped<IRepositorioEntregablesAnalisis, RepositorioEntregablesAnalisis>();
            services.AddScoped<IRepositorioEntregablesMuebles, RepositorioEntregablesMuebles>();
            services.AddScoped<IRepositorioEntregablesContrato, RepositorioEntregablesContrato>();
            
            services.AddScoped<IRepositorioReporteCedula, RepositorioReporteCedula>();
            services.AddScoped<IRepositorioEvaluacionServicios, RepositorioEvaluacionServicios>();
            services.AddScoped<IRepositorioEntregablesCedula, RepositorioEntregablesCedula>();
            services.AddScoped<IRepositorioAlcancesEntregables, RepositorioAlcancesEntregables>();

            //Services
            services.AddTransient<ServiceModulos>();
            services.AddTransient<ServicePermisos>();


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.LoginPath = "/login";
                
            });

            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=ReporteCedula}/{action=Print}/{id?}");
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
