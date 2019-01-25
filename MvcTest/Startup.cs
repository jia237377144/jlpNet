using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcTest.Middleware;
using Newtonsoft.Json.Serialization;

namespace MvcTest
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options => {
                //https://github.com/aspnet/Mvc/issues/4842
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddMemoryCache();//等同于WebCache

            //跨域设置
            services.AddCors(options =>
            {
                options.AddPolicy("defaultCors",builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyOrigin();

                    /*
                    builder.WithOrigins("http://www.baidu.com","http://www.163.com")
                    .AllowAnyHeader()
                    .AllowAnyHeader();
                    */
                });
            });

            services.AddMvcCore(options=> {
            }).AddJsonFormatters(setting =>
            {
                setting.Formatting = Newtonsoft.Json.Formatting.Indented;
                setting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
            })
            .AddApiExplorer();

            //忽略url大小写
            services.AddRouting(options => options.LowercaseUrls = true);

            //权限认证
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                    {
                        HttpOnly = true
                     ,
                        Expiration = System.TimeSpan.FromSeconds(2880)
                     ,
                        Name = "BasicAdmin"
                    };
                    options.LoginPath = new PathString("/account/login");
                    options.AccessDeniedPath = new PathString("/account/login");
                });
  

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCors("defaultCors");

            app.UsePermission(new PermissionMiddlewareOption()
            {
                LoginAction = @"/Account/Login",
                NoPemission = @"/Account/Login",
                UserPermissions = new List<UserPermission> {
                     new UserPermission { Url="/Home/Index", UserName="Admin"},
                      new UserPermission { Url="/home/contact", UserName="Admin"},
                      new UserPermission { Url="/home/about", UserName="System"},
                      new UserPermission { Url="/Home/Index", UserName="System"}
                }
            });


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
