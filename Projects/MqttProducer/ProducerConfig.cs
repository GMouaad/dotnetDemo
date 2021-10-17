namespace MqttProducer
{
    class ProducerConfig
    {
        public string BrokerHostname { get; set; }
        public int BrokerPort { get; set; }
        public string Topic { get; set; }
        public int PublishInterval { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ProducerConfig(string brokerHostname, int brokerPort, string topic, int publishInterval)
        {
            this.BrokerHostname = brokerHostname;
            this.BrokerPort = brokerPort;
            this.Topic = topic;
            this.PublishInterval = publishInterval;
            this.Username = "username";
            this.Password = "password"
        }
    }
}