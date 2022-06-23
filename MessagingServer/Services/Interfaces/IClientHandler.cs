namespace MessagingServer.Services.Interfaces
{
    public interface IClientHandler
    {
        Task Ack(string clientId, int messageIndex, string messageId);
        Task SendMessagedToClient(string clientId, int max, Func<MessageEvent, Task<bool>> funcCallBack);
    }
}