

using DomainModels;

namespace MessagingServer.Services.Interfaces
{
    public interface IClientSessionStore
    {
        Task<ClientInfoDoM> GetOrSetClientInfo(string clientId, ClientInfoDoM clientInfo);

        Task UpdateSession(ClientInfoDoM clientInfo);
        Task RemoveSession(ClientInfoDoM clientInfo);
    }
}