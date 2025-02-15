using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Data;
using Test.Extensions.Middlewares;

namespace Test.Extensions
{
    public static class AppPipelineExtension
    {
        public static async Task AddPipeline(this WebApplication app)
        {
            await app.InitialiseDatabaseAsync();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
        }
    }
}