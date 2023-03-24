// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Hosting;
using CommandLine;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging=> logging.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Trace))
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
//logger.LogInformation("Host created.");

//logger.LogInformation("Hello! This is log message from .NET console app...");

var worker = new SqlExporter.Worker(logger);


CommandLine.Parser.Default.ParseArguments<SqlExporter.Options>(args)
          .WithParsed<SqlExporter.Options>(opts => worker.Run(opts))
          .WithNotParsed <SqlExporter.Options >((errs) => HandleParseError(errs,logger));

static void HandleParseError(IEnumerable<Error> errs,ILogger log)
{
    foreach (var error in errs)
    {
       // log.LogError(error.ToString());
    }
    
}

