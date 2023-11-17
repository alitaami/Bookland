using NLog;
using NLog.Web;
using WebFramework.Configuration;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    WebApplication
        .CreateBuilder(args)
        .ConfigureServices()
        .Build()
        .Configure()
        .Run();
}
catch (Exception ex)
{
    logger.Error(ex);

    throw;
}

