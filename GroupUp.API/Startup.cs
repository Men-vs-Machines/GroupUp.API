using System;
using AspNetCoreRateLimit;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using GroupUp.API.Domain;
using GroupUp.API.Domain.Interfaces;
using GroupUp.API.Firestore;
using GroupUp.API.Firestore.Utility;
using GroupUp.API.Firestore.Workflows;

namespace GroupUp.API
{
    public class Startup
    {
        // Dependant on the computer
        private const string Filepath = "../../../secret-santa-api-key.json";
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IWebHostEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Startup>(true, true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Filepath);
                        
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroupUp.API", Version = "v1" });
            });

            services.AddSwaggerGenNewtonsoftSupport();
            
            services.AddOptions();
            services.AddMemoryCache();
            services.AddInMemoryRateLimiting();
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            var config = Configuration.GetSection("Firestore").Get<FirestoreConfig>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton(config);            
            services.AddSingleton<IGroupRepository, GroupRepository>();
            services.AddSingleton<IGroupCreationWorkflow, GroupCreationWorkflow>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var firebaseProjectName = config.ProjectId;
                    options.Authority = "https://securetoken.google.com/" + firebaseProjectName;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/" + firebaseProjectName,
                        ValidateAudience = true,
                        ValidAudience = firebaseProjectName,
                        ValidateLifetime = true
                    };
                });
            
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });

            services.AddAutoMapper(typeof(MappingProfile));
        }
            

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIpRateLimiting();
            
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroupUp.API v1"));

            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
