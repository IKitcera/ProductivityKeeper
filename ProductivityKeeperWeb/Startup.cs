using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Hubs;
using ProductivityKeeperWeb.Middleware;
using ProductivityKeeperWeb.Services;
using ProductivityKeeperWeb.Services.Repositories;
using System.Net;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb
{
    public class Startup
    {
        string AllowedClient = string.Empty;
        string Policy = "Single";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AllowedClient = Configuration.GetValue<string>("AllowedClients");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                        {
                            options.AddPolicy(Policy,
                                builder => builder
                                .WithOrigins(AllowedClient)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                             );
                        });
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                    {
                        //     options.Authority = host;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // ValidateIssuer = true,
                            //  ValidateAudience = true,
                            //   ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = AuthOptions.ISSUER,
                            ValidAudience = AuthOptions.AUDIENCE,
                            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/chart-hub")))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };

                    });

            services.AddMvcCore()
              .AddNewtonsoftJson(opt =>
              {
                  opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
              })
              .AddMvcOptions(options =>
              {
                  options.SuppressOutputFormatterBuffering = true;
                  options.Filters.Add(typeof(GlobalExceptionFilter));
              }).AddApiExplorer()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddViews()
                .AddRazorViewEngine()
                .AddCacheTagHelper()
                .AddDataAnnotations();

            services.AddDbContext<ApplicationContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            });

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            services.AddHangfire(configuration => 
            {
                configuration.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddHangfireServer();

            services.AddSignalR();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddHttpContextAccessor();

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
            });

            services.AddScoped<ITasksReadService, TasksReadService>();
            services.AddScoped<ITasksWriteService, TasksWriteService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IStatistics, StatisticsService>();
            services.AddScoped<ITimerService, TimerService>();

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
                //app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseCors(Policy);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChartHub>("/chart-hub");
                endpoints.MapControllers();
            });
             
            app.UseHangfireDashboard("/hangfire");
        }
    }
}
