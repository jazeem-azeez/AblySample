using Grpc.Core;

using MessagingServer.Services.Interfaces;

using Microsoft.Extensions.Logging;

namespace MessagingServer.Services.Implementations
{
    public class MessagingServerService : MessagingServer.MessagingServerBase
    {
        private readonly ILogger<MessagingServerService> logger;
        private readonly IClientHandler clientHandler;

        public MessagingServerService(ILogger<MessagingServerService> logger, IClientHandler clientHandler)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.clientHandler = clientHandler ?? throw new ArgumentNullException(nameof(clientHandler));
        }

        public override async Task<Noop> Ack(AckInfo request, ServerCallContext context)
        {
            logger.LogInformation($"Ack of message {request.MessageId} from client ");
            await clientHandler.Ack(request.ClientId, request.MessageIndex, request.MessageId);
            return new Noop();
        }

        public override async Task Recieve(ConnectionRequest request, IServerStreamWriter<MessageEvent> responseStream, ServerCallContext context)
        {
            await clientHandler.SendMessagedToClient(request.ClientId, request.Max, async message => { 
                logger.LogInformation($"sending message {message.MessageId} to client ");
                await responseStream.WriteAsync(message);
                await Task.Delay(1000);
                return true;
            });
        }
         
    }
}