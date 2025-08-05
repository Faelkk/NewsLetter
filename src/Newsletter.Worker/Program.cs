using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newsletter.Application.Interfaces;
using Newsletter.Application.UseCases;
using Newsletter.Domain.Interfaces;
using Newsletter.Infrastructure.Context;
using Newsletter.Infrastructure.Repository;
using Newsletter.Worker.Consumers;
using Newsletter.Worker.Handlers;

class Program
{
    public static async Task Main(string[] args)
    {
        await KafkaTopicCreator.CreateTopicIfNotExistsAsync("broker:9092", "subscription-status-updated");


        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddScoped<IDatabaseContext, DatabaseContext>();
                services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
                services.AddScoped<IConfirmSubscriptionStatus, ConfirmSubscriptionStatus>();
                services.AddScoped<SubscriptionStatusUpdateHandler>();
                services.AddHostedService<SubscriptionStatusConsumer>();
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build()
            .RunAsync(); 
    }
}