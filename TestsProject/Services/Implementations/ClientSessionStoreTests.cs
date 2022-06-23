using DomainModels;

using MessagingServer.Services.Implementations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace TestsProject.Services.Implementations
{
    [TestClass]
    public class ClientSessionStoreTests
    {
        private MockRepository mockRepository;

        [TestMethod]
        public async Task GetOrSetClientInfo_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var clientSessionStore = this.CreateClientSessionStore();
            string clientId = "mock";
            ClientInfoDoM data = new ClientInfoDoM { ClientId=clientId };

            // Act
            var result = await clientSessionStore.GetOrSetClientInfo(clientId,data);

            // Assert
            Assert.AreEqual(clientId, result.ClientId);
        }

        [TestMethod]
        public async Task RemoveSession_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var clientSessionStore = this.CreateClientSessionStore();
            string clientId = "mock";
            ClientInfoDoM clientInfo = new ClientInfoDoM { ClientId = clientId };

            // Act
            await clientSessionStore.RemoveSession(clientInfo);
            clientInfo.ClientId = "1234";
            var reuslt = await clientSessionStore.GetOrSetClientInfo(clientInfo.ClientId, clientInfo);
            // Assert
            Assert.AreEqual("1234",clientInfo.ClientId);
            this.mockRepository.VerifyAll();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        [TestMethod]
        public async Task UpdateSession_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var clientSessionStore = this.CreateClientSessionStore();
            string clientId = "mock";
            ClientInfoDoM clientInfo = new ClientInfoDoM { ClientId=clientId };
            var result = await clientSessionStore.GetOrSetClientInfo(clientId, clientInfo);

            // Act
            result.MessagesSendSoFar = 10;
            await clientSessionStore.UpdateSession(clientInfo);
            result = await clientSessionStore.GetOrSetClientInfo(clientId, clientInfo);

            // Assert
            Assert.AreEqual(10,result.MessagesSendSoFar);
        }

        private ClientSessionStore CreateClientSessionStore()
        {
            return new ClientSessionStore();
        }
    }
}