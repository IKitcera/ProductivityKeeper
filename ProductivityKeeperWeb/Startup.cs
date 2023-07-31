using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ProductivityKeeperWeb.Data;
using ProductivityKeeperWeb.Domain.Interfaces;
using ProductivityKeeperWeb.Domain.Models;
using ProductivityKeeperWeb.Hubs;
using ProductivityKeeperWeb.Services;
using ProductivityKeeperWeb.Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductivityKeeperWeb
{
    public class Startup
    {
        const string host = "http://localhost4200";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                        {
                            options.AddPolicy(host,
                                builder => builder
                                .WithOrigins(host, "http://localhost:51729")
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

            services.AddHangfire(configuration => configuration.UseSqlServerStorage(
                Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            services.AddSignalR();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductivityKeeperWeb", Version = "v1" });

                // Define the security scheme for JWT authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                // Add the security requirement for JWT authentication
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            services.AddDbContext<ApplicationContext>();
            services.AddHttpContextAccessor();

            services.AddControllers();
            services.AddScoped<ITasksReadService, TasksReadService>();
            services.AddScoped<ITasksWriteService, TasksWriteService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IAnalytics, AnalyticService>();
            services.AddScoped<ITimerService, TimerService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductivityKeeperWeb v1");

                });
            }


            app.UseCors(host);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChartHub>("/chart-hub");
                endpoints.MapControllers();
            });
        }
    }
}
