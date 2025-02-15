using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Test.Configurations.Models;
using Test.Data;
using Test.Entities;

namespace Test.Extensions
{
    public static class ServiceExtensions
    {


        public static void ConfigureSerilog(this IServiceCollection services)
        {
            services.AddSerilog(options =>
                       {
                           options.MinimumLevel.Information()
                           .WriteTo.Console(
                               restrictedToMinimumLevel: LogEventLevel.Debug,
                               outputTemplate: "{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                           ).WriteTo.File("Logs/log.txt",
                               rollOnFileSizeLimit: true,
                               rollingInterval: RollingInterval.Day,
                               fileSizeLimitBytes: 1000000,
                               outputTemplate: "{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                               restrictedToMinimumLevel: LogEventLevel.Warning
                           );
                       });
        }
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidateAudience = false,
                    ValidAudience = config["JWT:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
                };
            });
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SudoPolicy", p => p.RequireClaim("Role", "Sudo"));
            });
        }


        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SudoData>(configuration.GetSection(SudoData.Section));
        }

        public static void AddConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Default"));
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Usuario, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppDbContext>();
        }

        public static void AddDependencyInjections(this IServiceCollection services)
        {
            services.AddScoped<DbSeederInitializer>();
        }
    }
}