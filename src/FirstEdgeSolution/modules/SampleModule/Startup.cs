using Microsoft.Data.SqlClient;
using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Functions.Samples.Startup))]
namespace Functions.Samples
{
    public class Startup : FunctionsStartup
    {
        private IConfigurationRoot configuration;
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            configuration = builder.ConfigurationBuilder.Build();
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped((s) =>
            {
                var connectionString = configuration.GetConnectionString("Database");
                var conn = new SqlConnection(connectionString);
                // TODO: use openAsync later. This will be a blocking call every time. This is bad scaling
                conn.Open();
                return conn;
            });
        }
    }
}