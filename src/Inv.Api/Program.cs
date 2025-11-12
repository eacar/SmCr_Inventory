using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Inv.Api;

public class Program
{
    public static Task Main(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.AddJsonFile($"AppSettings.{ctx.HostingEnvironment.EnvironmentName}.json",
                    optional: true, reloadOnChange: true);
            })
            .ConfigureWebHostDefaults(w => w.UseStartup<Startup>())
            .Build()
            .RunAsync();
}