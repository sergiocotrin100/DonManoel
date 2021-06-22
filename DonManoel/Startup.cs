using Core.Entities;
using Core.Interfaces;
using DonManoel.Data;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repository;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace DonManoel
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
            services.AddScoped<IUserSession, UserSession>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IMesaRepository, MesaRepository>();
            services.AddTransient<IPedidoItemRepository, PedidoItemRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<IDonConnection, DonConnection>();
            services.Configure<EmailConfiguracao>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmail, Email>();
            services.AddTransient<IResetSenhaRepository, ResetSenhaRepository>();
            MemoryCacheTicketStore memoryCacheTicketStore = new MemoryCacheTicketStore();
            services.AddSingleton(memoryCacheTicketStore);

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddRazorPagesOptions(options => { }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //services.AddSignalR();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
           .AddCookie(opt =>
           {
               opt.LoginPath = new PathString("/AccessUser/LoginAsync");
               opt.LogoutPath = new PathString("/AccessUser/LogoutAsync");
               opt.AccessDeniedPath = new PathString("/Erro/AcessoNegado");
               opt.ExpireTimeSpan = TimeSpan.FromMinutes(600);
               opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
               //opt.Cookie = new CookieBuilder()
               //{
               //    Name = ".DonManuelCookie",
               //    //Expiration = new System.TimeSpan(0, 120, 0),

               //    //Se tiver um dom�nio...
               //    //Domain = ".site.com.br",
               //};
           });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
            });

            //services.AddControllersWithViews();

            services.AddProgressiveWebApp();
            //services.AddProgressiveWebApp(new PwaOptions
            //{ 
            //    CacheId = "Worker 1.1",
            //    Strategy = ServiceWorkerStrategy.CacheFirst,
            //    RoutesToPreCache = "/Home/Index, /Shared/_Layout,/Shared/Error",

            //    OfflineRoute = "index.html",
                
            //});
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
                app.UseExceptionHandler("/Erro/PaginaErro");
                app.UseHsts();
            }

            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
