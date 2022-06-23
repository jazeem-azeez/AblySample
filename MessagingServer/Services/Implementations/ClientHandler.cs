

using System;

using DomainModels;

using Grpc.Core;

using MessagingServer.Common;
using MessagingServer.Services.Interfaces;

namespace MessagingServer.Services.Implementations
{
    public class ClientHandler : IClientHandler
    {
        private readonly ILogger<ClientHandler> logger;
        private readonly IClientSessionStore sessionStore;

        public ClientHandler(ILogger<ClientHandler> logger, IClientSessionStore sessionStore)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));
        }
        public Task Ack(string clientId, int messageIndex, string messageId)
        {
            //NOT implementing extensively with transport semantics s it may be an overkill
            return Task.CompletedTask;
        }

        public async Task SendMessagedToClient(string clientId, int max, Func<MessageEvent, Task<bool>> funcCallBack)
        {
            Random random = new Random();
            var clientInfo =  await sessionStore.GetOrSetClientInfo(clientId, new ClientInfoDoM
                {
                    ClientId = clientId,
                    LastUpdateTimestamp = DateTime.UtcNow,
                    MessagesSendSoFar = 0,
                    NumberOfMessagesToSend = max
                });
            if (!IsClientSessionValid(clientInfo))
            {
                new RpcException(Status.DefaultCancelled, "client Session is Invalid - please use a different client Id");
            }
            try
            {
                while (clientInfo.MessagesSendSoFar < clientInfo.NumberOfMessagesToSend)
                {
                    var msg = new MessageEvent
                    {
                        MessageData = random.Next(0, 65535).ToString(),
                        MessageId = clientInfo.MessagesSendSoFar.ToString(),
                        MessageIndex = clientInfo.MessagesSendSoFar,
                        Header = ""
                    };

                    if (await funcCallBack(msg))
                    {
                        clientInfo = UpdateCheckSum(clientInfo, msg);
                        clientInfo.MessagesSendSoFar++;
                        await UpdateClientInfo(clientInfo);
                    }

                }

                await SendCheckSum(clientInfo, funcCallBack);
                await RemoveClientInfo(clientInfo);

            }
            catch (RpcException e)
            {
                logger.LogError(e.Message);
            }

        }

        private async Task RemoveClientInfo(ClientInfoDoM clientInfo)
        {
            await sessionStore.RemoveSession(clientInfo);
        }

        private async Task SendCheckSum( ClientInfoDoM clientInfo, Func<MessageEvent, Task<bool>> funcCallBack)
        {
            logger.LogInformation("sending checksum");
            var msg = new MessageEvent
{
                MessageData = clientInfo.CheckSum,
                MessageId = clientInfo.ClientId,
                MessageIndex = clientInfo.MessagesSendSoFar,
                Header = "checksum"
            };
            await funcCallBack(msg);

        }

        private async Task UpdateClientInfo(ClientInfoDoM clientInfo)
        {
            clientInfo.LastUpdateTimestamp = DateTime.UtcNow;
            await sessionStore.UpdateSession(clientInfo);
        }

        private ClientInfoDoM UpdateCheckSum(ClientInfoDoM clientInfo, MessageEvent msg)
        {
            clientInfo.CheckSum = Helpers.Adler32(clientInfo.CheckSum + msg.MessageData).ToString();
            return clientInfo;
        }

        private bool IsClientSessionValid(ClientInfoDoM clientInfo)
        {
            if (Math.Abs(clientInfo.LastUpdateTimestamp.Subtract(DateTime.UtcNow).Seconds) > 30)
            {
                logger.LogError("Failed as last Client Session expired ");
                return false;
            }
            return true;
        }
    }
}