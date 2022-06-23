
namespace MessagingServer.ClientLibrary.Common
{
    public static class Helpers
    {

        /// <summary>
        /// Adler32s the specified string.
        ///README:
        ///https://en.wikipedia.org/wiki/Adler-32
        ///https://gist.github.com/i-e-b/c37cc2d728fe5e5a56205cd7e62d682c
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static uint Adler32(string str)
        {
            const int mod = 65521;
            uint a = 1, b = 0;
            foreach (char c in str)
            {
                a = (a + c) % mod;
                b = (b + a) % mod;
            }
            return (b << 16) | a;
        }

        public static string GetClientsChannelInfoKey(string clientId, string channelName)
        {
            return $"{clientId}-+-{channelName}";
        }
    }
}