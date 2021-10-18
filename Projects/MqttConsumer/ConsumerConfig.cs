namespace MqttConsumer
{
    /// <summary>
    /// Object to store the application configuration
    /// </summary>
    public class ConsumerConfig
    {
        public string BrokerHostname { get; set; }
        public int BrokerPort { get; set; }
        public string Topic { get; set; }
        public string PersistenceFile { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ConsumerConfig(string brokerHostname, int brokerPort, string topic, string persistenceFile)
        {
            this.BrokerHostname = brokerHostname;
            this.BrokerPort = brokerPort;
            this.Topic = topic;
            this.PersistenceFile = persistenceFile;
            this.Username = "username";
            this.Password = "password";
        }
    }
}