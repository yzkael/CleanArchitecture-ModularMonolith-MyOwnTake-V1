using Serilog;
using Test.Data;
using Test.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: "./Logs/bootstrap/log.txt",
        rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    // Add services to the container.
    builder.AddServices(builder.Configuration);
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    await app.InitialiseDatabaseAsync();
    app.AddPipeline();
    app.Run();



}
catch (System.Exception exception)
{

    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Closing and flushing logger");
    Log.CloseAndFlush();
}