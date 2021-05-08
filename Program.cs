using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app_task_azureweekend
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("PROGRAM MAIN");
            try
            {
                var builder = new HostBuilder()
                 .ConfigureServices((hostContext, services) =>
                 {
                     IServiceCollection serviceCollection = services.AddHostedService<Service>();
                 });

                await builder.RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}
