using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ChoresHub.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();
            services.AddSwaggerDocumentation();
            services.ConfigureLowercaseUrls();
            services.AddCustomCors(configuration);
            return services;
        }



        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var key = jwtSettings["SecretKey"];
            var audience = jwtSettings["Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["atk"];
                            return Task.CompletedTask;
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ClockSkew = TimeSpan.FromMinutes(5), 
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
                    };
                });
            return services;
        }


        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security"
            };
            var securityRequirement = new OpenApiSecurityRequirement(){
                {
                     new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference
                          {
                             Type = ReferenceType.SecurityScheme,
                              Id = "Bearer"
                         }
                     },
                        new string[] {}
                 }
            };
            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "ChoresHub"
            };

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", info);
                swagger.AddSecurityDefinition("Bearer", securityScheme);
                swagger.AddSecurityRequirement(securityRequirement);
            });
            return services;
        }

        public static IServiceCollection ConfigureLowercaseUrls(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });
            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3030" };
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                        .AllowCredentials()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            return services;
        }

    }
}