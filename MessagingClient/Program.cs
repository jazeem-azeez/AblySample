using MessagingServer.ClientLibrary;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Polly;

namespace MessagingServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {


            string UriString = "http://localhost:" + args[0];
            int max = Convert.ToInt32(args[1]);
            string clientId = args.Length > 2 ? args[2] : Guid.NewGuid().ToString();

            IServiceCollection services = new ServiceCollection();
            services.AddLogging(cfg => cfg.AddConsole().AddFilter("MessagingServer", LogLevel.Information).AddFilter("Default", LogLevel.Warning));
            services.AddScoped<MessagingClient>();

            services.AddGrpcClient<MessagingServer.MessagingServerClient>((ctx, client) =>
            {
                client.Address = new Uri(UriString);
            });
            var provider = services.BuildServiceProvider();

            var policy = Policy.Handle<Exception>()
                          .WaitAndRetryAsync(
                            5,
                            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            (exception, timeSpan, context) =>
                            {
                                var logger = provider.GetService<ILogger<Program>>();
                                logger.LogError(exception.Message, timeSpan);
                            }
                          ); ;
            using (var scope = provider.CreateScope())
            {
                var client = scope.ServiceProvider.GetService<MessagingClient>();
                client.Setup(clientId, max);
                await policy.ExecuteAsync(async () =>
                {
                    await client.Recieve();
                });
            };
            Console.ReadLine();
        }
    }
}