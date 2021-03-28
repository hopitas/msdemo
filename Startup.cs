using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MS.Function
{
public class Startup : FunctionsStartup
    {  
        public override void Configure(IFunctionsHostBuilder builder)
        {      
            var config = builder.GetContext().Configuration;
            var sqlConn = config.GetConnectionString("mycs");
            var tableConn = config.GetConnectionString("TableStorage");
            var apiKey = config.GetConnectionString("ApiKey");
            builder.Services.AddSingleton<IConfiguration>((s)=>config);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}