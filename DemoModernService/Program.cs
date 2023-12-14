
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoModernService;

internal class Program
{
    static async Task Main(string[] args)
    {
         var host = Host.CreateDefaultBuilder(args)
            .UseWindowsService(opt =>
            {
                opt.ServiceName = "DemoModernService";
            })
            .ConfigureServices(svc =>
            {
                svc.AddHostedService<DemoModernService>();
            })
            .Build();

        await host.RunAsync();
    }
}