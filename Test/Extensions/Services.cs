using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace Test.Extensions
{
    public static class Services
    {
        public static void AddServices(this IHostApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();
            builder.Services.ConfigureSerilog();
            builder.Services.ConfigureAuthentication(configuration);
            builder.Services.AddConfigurationOptions(configuration);
            builder.Services.AddConfigureDbContext(configuration);
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureAuthorization();
            builder.Services.AddDependencyInjections();
        }
    }
}