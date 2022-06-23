namespace DomainModels
{
    public class ClientInfoDoM
    {
        public string ClientId { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }
        public int NumberOfMessagesToSend { get; set; }
        public int MessagesSendSoFar { get; set; }
        public string CheckSum { get; set; }

    }
}