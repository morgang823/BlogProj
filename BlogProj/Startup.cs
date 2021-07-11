using BlogProj.Data;
using BlogProj.Models;
using BlogProj.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Connection.GetConnectionString(Configuration)));
            //Configure Cross Origin Resource Sharing (CcORS) policy
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultPolicy",
                    builder => builder.AllowAnyOrigin()
                                        .AllowAnyMethod()
                                        .AllowAnyHeader());
            });


            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddDefaultUI()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped<IBlogFileService, BasicFileService>();
            services.AddScoped<DataService>();
            services.AddScoped<BasicSlugService>();
            services.AddScoped<IEmailSender, GmailEmailService>();
            services.AddScoped<SearchService>();

            //Swagger.Service reference
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments($"{Directory.GetCurrentDirectory()}/wwwroot/BlogProj.xml", true);
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Blog Api",
                    Version = "v1",
                    Description = "Serving up the Blog data using .Net Core",
                    Contact = new OpenApiContact
                    {
                        Name = "Gianni Morgan",
                        Email = "morgang823@gmail.com",
                        Url = new System.Uri("https://www.linkedin.com/in/gianni-morgan-b1398b76/")
                    }
                });
            });

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("DefaultPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestAPI v1");
                c.InjectJavascript("/Swagger/Swagger.js");
                c.InjectStylesheet("/Swagger/Swagger.css");
                c.DocumentTitle = "Blog Public API";
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
               name: "SEORoute",
               pattern: "BlogPost/Details/{slug}",
                    defaults: new { controller = "Posts", action = "Details" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
