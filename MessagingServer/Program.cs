

using MessagingServer.Services.Implementations;
using MessagingServer.Services.Interfaces;

namespace MessagingServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.ConfigureKestrel(opts =>
            {
                args = args == null || args.Length == 0 ? new[] { "5500" } : args;
                opts.ListenAnyIP(Convert.ToInt32(args[0]));
            }); 
            builder.Services.AddSingleton<IClientHandler, ClientHandler>();
            builder.Services.AddSingleton<IClientSessionStore, ClientSessionStore>();
            builder.Services.AddGrpc(); 

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            app.MapGrpcService<MessagingServerService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");
            app.Run();
        }
    }
}