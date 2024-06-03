using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BUDSharedCore.Persistence.Context;
using eBauGISTriageApi.Helper.Exceptions;
using eBauGISTriageApi.Models;
using eBauGISTriageApi.Persistence;
using eBauGISTriageApi.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using NSwag;
using NSwag.Examples;
using NSwag.Generation.Processors.Security;

namespace eBauGISTriageApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.CurrentEnvironment = env;
        }

        internal static IConfiguration Configuration { get; private set; }

        private IWebHostEnvironment CurrentEnvironment { get; set; }

        private const string c_pathToQueriesFolder = "/Data/GISQueries";
        private const string c_pathToWorkflowsFolder = "/Data/Workflows";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (CurrentEnvironment.IsDevelopment() || CurrentEnvironment.IsStaging())
            {
                services.AddExampleProviders(typeof(Request).Assembly); // GeoJson example from OpenApiRequestResponseExamples
                services.AddExampleProviders(typeof(Response).Assembly); // Response example from OpenApiRequestResponseExamples

                services.AddOpenApiDocument((settings, provider) =>
                {
                    settings.AddSecurity("Bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                        Description = "Type into the textbox: {your JWT token}.",
                    });

                    settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
                    settings.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "GIS Triage API";
                        document.Info.Description = "API to execute the gis triage";
                    };

                    settings.AddExamples(provider); // OpenApiRequestResponseExamples

                });
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .SetIsOriginAllowed(origin =>
                        {
                            Uri uri = new Uri(origin);
                            return uri.Host == "localhost" || uri.Host.EndsWith("*.bl.ch");
                        })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition")
                        );
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        .WithOrigins("https://*.bl.ch")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition")
                        );
                });
            }

            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
            {
                options.Authority = Configuration["ServerConfig:IdentityServerURL"];
                options.RequireHttpsMetadata = false;
                options.Audience = "eBauWeb";
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Add the access_token as a claim, as we may actually need it
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };

            });

            services.AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(Configuration);
            });

            List<DbContextInfo> dbContextInfo = new List<DbContextInfo>();
            services.AddSingleton(x => new DbContextInfoRegistry(dbContextInfo));
            services.AddSingleton(new DbContextConfig<GdwhCtx>(string.Empty));

            services.AddDbContextFactory<GdwhCtx>(options =>
            {
                options.UseNpgsql(Configuration["ConnectionStrings:GDWHConnection"]);
            });

            string absolutePathToQueriesFolder = CurrentEnvironment.ContentRootPath + c_pathToQueriesFolder;

            services.AddSingleton<QueriesService>(serviceProvider => new QueriesService(
                serviceProvider.GetService<IDbContextFactory<GdwhCtx>>(),
                serviceProvider.GetService<ILogger<QueriesService>>(),
                new QueriesRepository(
                    absolutePathToQueriesFolder,
                    serviceProvider.GetService<ILogger<QueriesService>>(),
                    new FileSystem())));

            services.AddSingleton<TriageService>(serviceProvider =>
            {
                ILogger? triageServiceLogger = serviceProvider.GetService<ILogger<TriageService>>();
                ILogger? workflowsRepositoryLogger =
                    serviceProvider.GetService<ILogger<FileSystemWorkflowsRepository>>();
                IWorkflowsRepository workflowsRepository = new FileSystemWorkflowsRepository(
                    workflowsRepositoryLogger,
                    CurrentEnvironment.ContentRootPath + c_pathToWorkflowsFolder,
                    new FileSystem());

                return new TriageService(triageServiceLogger, workflowsRepository);
            });

        services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(); // serve OpenAPI/Swagger documents
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(); // serve OpenAPI/Swagger documents
                app.UseSwaggerUi3(); // serve Swagger UI
            }
        }
    }
}
