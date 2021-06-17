using AutoMapper;
using Core.Interfaces;
using DonManoel.Data;
using Infrastructure;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEssentials.AspNetCore.Pwa;

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
            MemoryCacheTicketStore memoryCacheTicketStore = new MemoryCacheTicketStore();
            services.AddSingleton(memoryCacheTicketStore);

            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddRazorPagesOptions(options => { }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LogoutPath = new PathString("/Access/LogoutAsync");
                options.LoginPath = new PathString("/Access/LoginAsync");

               // options.LoginPath = "/Access/LoginAsync/";
              //  options.AccessDeniedPath = new PathString("/Access/LoginAsync/");
             //   options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

               options.SessionStore = memoryCacheTicketStore;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(180);
            });

            services.AddDistributedMemoryCache();
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
                app.UseExceptionHandler("/Home/Error");
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
            app.UseAuthorization();

            app.UseSession();
            app.UseAuthentication();

           // app.UseCors("AllowSpecificOrigin");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
