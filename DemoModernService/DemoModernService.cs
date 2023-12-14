
using Microsoft.Extensions.Hosting;
using murrayju.ProcessExtensions;

namespace DemoModernService;

internal class DemoModernService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ProcessExtensions.StartProcessAsCurrentUser("calc.exe");
    }
}