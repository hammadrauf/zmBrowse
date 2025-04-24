using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace zmBrowse;


class Program
{
    private static ILoggerFactory factory;
    private static ILogger logger;


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
                .AddConsole()
                .SetMinimumLevel(LogLevel.Debug);
        });
        logger = factory.CreateLogger<Program>();
        logger.LogDebug("Started Logger for Program.");

        ApplicationConfiguration.Initialize();
        Application.Run(new mainForm(logger));
    }    
}