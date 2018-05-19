using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prode.API.Services;
using StackExchange.Profiling;
using Swashbuckle.AspNetCore.Swagger;

namespace Prode.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;

        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton(typeof(IDbService), typeof(DbService));
            services.AddSingleton(typeof(IUserService), typeof(UserService));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
            {
                o.Cookie.Path = "/app";
                o.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                o.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(TraducirPolicy.CanSuggest, policy => policy.RequireClaim(ClaimType.CanSuggest));
            //    options.AddPolicy(TraducirPolicy.CanReview, policy => policy.RequireClaim(ClaimType.CanReview));
            //    options.AddPolicy(TraducirPolicy.CanManageUsers, policy => policy.RequireClaim(ClaimType.IsModerator));
            //});

            services.AddExceptional(settings =>
            {
                settings.UseExceptionalPageOnThrow = HostingEnvironment.IsDevelopment();
                settings.OnBeforeLog += (sender, args) =>
                {
                    var match = Regex.Match(args.Error.FullUrl, "^(([^/]+)//([^/]+))/", RegexOptions.Compiled);
                    var miniProfilerUrl = match.Groups[1].Value + "/app/mini-profiler-resources/results?id=" + MiniProfiler.Current.Id.ToString();

                    args.Error.CustomData = args.Error.CustomData ?? new Dictionary<string, string>();
                    args.Error.CustomData.Add("MiniProfiler", miniProfilerUrl);
                };
                settings.LogFilters.Cookie[".AspNetCore.Cookies"] = "hidden";
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseMiniProfiler();
            app.UseExceptional();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc();
            
        }
    }
}
