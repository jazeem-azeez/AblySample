using DomainModels;

using MessagingServer.Services.Interfaces;

namespace MessagingServer.Services.Implementations
{
    public class ClientSessionStore : IClientSessionStore
    {
        Dictionary<string, ClientInfoDoM> cache = new Dictionary<string, ClientInfoDoM>();
        

        public Task<ClientInfoDoM> GetOrSetClientInfo(string clientId, ClientInfoDoM data)
        {
            if (cache.ContainsKey(clientId))
            {
                return Task.FromResult(cache[clientId]);
            }
            cache[clientId] = data;
            return Task.FromResult(data);
        }

        public Task RemoveSession(ClientInfoDoM clientInfo)
        {
            cache.Remove(clientInfo.ClientId);
            return Task.CompletedTask;
        }

        public Task UpdateSession(ClientInfoDoM clientInfo)
        {
            cache[clientInfo.ClientId]=clientInfo;
            return Task.CompletedTask;
        }
    }
}
