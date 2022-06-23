using MessagingServer.ClientLibrary.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestsProject
{
    [TestClass]
    public class HelpersTests
    {
        [TestMethod]
        public void Adler32_StateUnderTest_ExpectedBehavior()
        {
            // Act
            var result = Helpers.Adler32("str") + "";

            // Assert
            Assert.AreEqual("45482330", result);
        }

        [TestMethod]
        public void GetClientsChannelInfoKey_StateUnderTest_ExpectedBehavior()
        {
            // Act
            var result = Helpers.GetClientsChannelInfoKey("clientId", "channelName");

            // Assert
            Assert.AreEqual("clientId-+-channelName", result);
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }
    }
}