using CoreBB.Web.Data;
using CoreBB.Web.Helpers;
using CoreBB.Web.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CoreBB
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<CoreBBContext>();
            services.AddTransient<IRepository, CoreBBRepository>();
            services.AddTransient<IHasher, Hasher>();
            services.AddTransient<ILogin, LoginHelper>();
            services.AddTransient<IRegister, RegistrationHelper>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => {
                    option.LoginPath = "/User/LogIn";
                    option.AccessDeniedPath = "/Error/AccessDenied";
                    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler("/Error/Index");
            app.UseStaticFiles();   
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();  //The default URL routing template is {controller=Home}/{action=Index}/{id?}
            app.UseDeveloperExceptionPage();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
        }
    }
}
