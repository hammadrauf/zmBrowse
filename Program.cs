using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.DependencyInjection;

namespace zmBrowse;


class Program
{
    private static ILoggerFactory factory = null!;
    private static ILogger logger = null!;


    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        factory = LoggerFactory.Create(static builder =>
        {
            builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("zmBrowse.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
        });
        logger = factory.CreateLogger<Program>();
        logger.LogDebug("==> Started Logger for Program zmBrowse.....");

        ApplicationConfiguration.Initialize();
        Application.Run(new mainForm(logger));
    }    
}