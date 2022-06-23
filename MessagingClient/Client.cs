using Grpc.Core;

using MessagingServer.ClientLibrary.Common;

using Microsoft.Extensions.Logging;

namespace MessagingServer.ClientLibrary
{
    public class MessagingClient
    {
        private readonly ILogger<MessagingClient> logger;
        private readonly MessagingServer.MessagingServerClient messagingServerClient;
        private string clientId;
        private int max;

        public MessagingClient(ILogger<MessagingClient> logger, MessagingServer.MessagingServerClient messagingServerClient)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.messagingServerClient = messagingServerClient ?? throw new ArgumentNullException(nameof(messagingServerClient));
        }

        public async Task<bool> Recieve()
        {
            try
            {
                string checkSum = File.Exists(clientId) ? File.ReadAllText(clientId) : string.Empty;
                using (var stream = messagingServerClient.Recieve(new ConnectionRequest { ClientId = clientId, Max = max }))
                {
                    var currentMessage = new MessageEvent();
                    while (await stream.ResponseStream.MoveNext())
                    {
                        currentMessage = stream.ResponseStream.Current;

                        logger.LogInformation($" from  client {clientId} got Messages {currentMessage.MessageId}");
                        if (currentMessage.Header == "checksum")
                        {
                            break;
                        }
                        checkSum = Helpers.Adler32(checkSum + currentMessage.MessageData).ToString();
                        await messagingServerClient.AckAsync(new AckInfo()
                        {
                            MessageIndex = currentMessage.MessageIndex,
                            ClientId = clientId,
                            MessageId = currentMessage.MessageId
                        });
                        File.WriteAllText(clientId, checkSum);
                        logger.LogInformation($"Acknowledged for  client {clientId} on Messages {currentMessage.MessageId} ");
                    }
                    if (checkSum == currentMessage.MessageData && currentMessage.Header == "checksum")
                    {
                        logger.LogInformation("Message checkSum Passed");

                        return true;
                    }
                    else
                    {
                        logger.LogError("Message checkSum Failed ");

                    }
                }
            }
            catch (RpcException e)
            {
                logger.LogError(e.Message);
                throw ;
            }
             
            return false;
        }

        public void Setup(string clientId, int max)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
            }


            this.max = max < 65535 && max >= 0 ? max : throw new ArgumentNullException(nameof(max));

            this.clientId = clientId;

        }

    }
}