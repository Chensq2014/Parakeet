// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parakeet.Net.ConsoleApp;

Console.WriteLine($"Hello, World!\r\n");

await CreateHostBuilder(args).RunConsoleAsync();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .AddAppSettingsSecretsJson()
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHostedService<ConsoleHostedService>();
        });