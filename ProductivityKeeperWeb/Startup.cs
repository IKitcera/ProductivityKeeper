using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Hubs;
using ProductivityKeeperWeb.Services;
using ProductivityKeeperWeb.Services.Repositories;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb
{
    public class Startup
    {
        private readonly string AllowedClient = string.Empty;
        private const string Policy = "Single";
        private readonly bool _enableHangfire;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            AllowedClient = Configuration.GetValue<string>("AllowedClients");
            _enableHangfire = !environment.IsDevelopment() || Configuration.GetValue<bool>("Hangfire:Enabled");
        }

        public IConfiguration Configuration { get; }

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
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
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

                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    (path.StartsWithSegments("/chart-hub")))
                                {
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

            if (_enableHangfire)
            {
                services.AddHangfire(configuration =>
                {
                    configuration.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
                });
                services.AddHangfireServer();
            }
            else
            {
                services.AddSingleton<IBackgroundJobClient, NoOpBackgroundJobClient>();
            }

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
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                });

                c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", null, null),
                        new List<string>()
                    }
                });
            });

            services.AddScoped<ITasksReadService, TasksReadService>();
            services.AddScoped<ITasksWriteService, TasksWriteService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IStatistics, StatisticsService>();
            services.AddScoped<ITimerService, TimerService>();

            // Diary feature Clean Architecture registrations
            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<DiaryService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(Policy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChartHub>("/chart-hub");
                endpoints.MapControllers();
            });

            if (_enableHangfire)
            {
                app.UseHangfireDashboard("/hangfire");
            }
        }

        private sealed class NoOpBackgroundJobClient : IBackgroundJobClient
        {
            public string Create(Job job, IState state) => string.Empty;

            public bool ChangeState(string jobId, IState state, string expectedState) => false;
        }
    }
}
