using Autofac;
using Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NToastNotify;
using System;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
//using WebFramework.Middlewares;
using WebFramework.Swagger;

namespace Secretary
{
    public class Startup
    {
        private readonly SiteSettings _siteSetting;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {




            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));

            services.InitializeAutoMapper();

            services.AddDbContext(Configuration);

            services.AddCustomIdentity(_siteSetting.IdentitySettings);

            //services.AddMinimalMvc();
            services.AddControllersWithViews();

            services.AddElmahCore(Configuration, _siteSetting);

            services.ConfigureApplicationCookie(delegate (CookieAuthenticationOptions options)
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.LoginPath = "/auth/Login";
                options.LogoutPath = "/auth/Logout";
                options.AccessDeniedPath = "/auth/Logout";
            });

            //services.AddJwtAuthentication(_siteSetting.JwtSettings);

            //services.AddCustomApiVersioning();

            //services.AddSwagger();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddNToastNotifyNoty(new NotyOptions
            {
                Timeout = 850,
                ProgressBar = true,
                Modal = true,
                // Change Custome Layout : topRight", "top", "topLeft", "topCenter", "center", "centerLeft", "centerRight","bottom","bottomLeft","bottomCenter","bottomRight"
                Layout = "topCenter",
                // Change Custome Theme : metroui,relax,nest,semanticui,sunset,mint,bootstrap-v3,light,bootstrap-v4,
                Theme = "metroui",
            })
                .AddControllersAsServices().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            // Don't create a ContainerBuilder for Autofac here, and don't call builder.Populate()
            // That happens in the AutofacServiceProviderFactory for you.
        }

        // ConfigureContainer is where you can register things directly with Autofac. 
        // This runs after ConfigureServices so the things ere will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Register Services to Autofac ContainerBuilder
            builder.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.IntializeDatabase();

            //app.UseCustomExceptionHandler();

            app.UseHsts(env);

            app.UseHttpsRedirection();

            app.UseElmahCore(_siteSetting);

            //app.UseSwaggerAndUI();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseNToastNotify();


            //Use this config just in Develoment (not in Production)
            //app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseEndpoints(config =>
            {
                //config.MapControllers(); // Map attribute routing
                //    .RequireAuthorization(); Apply AuthorizeFilter as global filter to all endpoints
                config.MapControllerRoute(
                       name: "default",
    pattern: "{controller=auth}/{action=login}/{id?}"); // Map default route {controller=Home}/{action=Index}/{id?}
            });

            //Using 'UseMvc' to configure MVC is not supported while using Endpoint Routing.
            //To continue using 'UseMvc', please set 'MvcOptions.EnableEndpointRouting = false' inside 'ConfigureServices'.
            app.UseMvc();
        }
    }
}
