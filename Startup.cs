using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotNetSpaTemplateAnalysis {
    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) => {
                logRequest(logger, context, "app");
                await next();
                logResponse(logger, context, "app");
            });

            //app.UseStaticFiles();

            // this should really be wrapped in if (!env.IsDevelopment())
            app.UseSpaStaticFiles();

            app.Use(async (context, next) => {
                logRequest(logger, context, "app after UseSpaStaticFiles");
                await next();
                logResponse(logger, context, "app after UseSpaStaticFiles");
            });

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            // UseSpa actually uses the internal class SpaDefaultPageMiddleware
            // The SpaDefaultPageMiddleware unconditionally rewrites context.Request.Path to the value of spa.Options.DefaultPage
            // It then attempts to serve the modified Request Path using the UseSpaStaticFilesInternal
            // Under the hood, UseSpaStaticFiles/UseSpaStaticFilesInternal just uses the UseStaticFiles middleware with a custom FileProvider of type ISpaStaticFileProvider
            // An ISpaStaticFileProvider service is added when AddSpaStaticFiles() method is used (configured to 'ClientApp/dist')
            // If AddSpaStaticFiles() is not used (no ISpaStaticFileProvider registered), it falls back on serving files from wwwroot
            app.UseSpa(spa => {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                // spa.Options.SourcePath is used to determine the folder where npm scripts should be executed from
                // only relevant when using spa.UseAngularCliServer(npmScript: "start");
                // spa.Options.SourcePath does nothing if you use UseProxyToSpaDevelopmentServer
                spa.Options.SourcePath = "ClientApp";
                spa.Options.DefaultPage = "/index.html"; // default is '/index.html'

                app.Use(async(context, next) => {
                    logRequest(logger, context, "inside app.UseSpa");
                    await next();
                    logResponse(logger, context, "inside app.UseSpa");
                });

                if (env.IsDevelopment()) {
                    // Due to the way that the UseSpa extension method works
                    // the following middleware is actually added before
                    // the middleware the UseSpa extension method adds to the pipeline
                    // This means that the proxy middleware runs first
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer(baseUri: "http://localhost:8080");
                }
            });

            app.Use(async(context, next) => {
                logRequest(logger, context, "app after app.UseSpa");
                await next();
                logResponse(logger, context, "app after app.UseSpa");
            });
        }

        private void logRequest(ILogger logger, HttpContext context, string description) {
            if (context.Request != null) {
                var builder = new System.Text.StringBuilder()
                    .AppendLine($"{description} context.Request.Protocol: {context.Request.Protocol}")
                    .AppendLine($"{description} context.Request.Method: {context.Request.Method}")
                    .AppendLine($"{description} context.Request.Scheme: {context.Request.Scheme}")
                    .AppendLine($"{description} context.Request.Host: {context.Request.Host}")
                    .AppendLine($"{description} context.Request.PathBase: {context.Request.PathBase}")
                    .AppendLine($"{description} context.Request.Path: {context.Request.Path}")
                    .AppendLine($"{description} context.Request.QueryString: {context.Request.QueryString}");
                logger.LogInformation(builder.ToString());
            } else {
                logger.LogInformation($"{description} context.Request is null");
            }
        }

        private void logResponse(ILogger logger, HttpContext context, string description) {
            if (context.Response != null) {
                var builder = new System.Text.StringBuilder()
                    .Append($"{description} context.Response.StatusCode: {context.Response.StatusCode}")
                    .AppendLine($" Url: {context.Request?.Path.Value}");
                logger.LogInformation(builder.ToString());
            } else {
                logger.LogInformation($"{description} context.Response is null");
            }
        }
    }
}