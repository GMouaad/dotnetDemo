namespace MqttConsumer
{
    using System;
    using System.Text;
    using MQTTnet;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Receiving;
    using MQTTnet.Client.Options;
    using MQTTnet.Extensions.ManagedClient;
    using MQTTnet.Protocol;
    using MQTTnet.Formatter;
    using System.Threading;
    using System.IO;

    public class Consumer
    {
        /// <summary>
        /// The managed subscriber client.
        /// </summary>
        private IManagedMqttClient managedMqttClientSubscriber;
        private MqttFactory mqttFactory;
        private MqttClientOptions options;
        private ConsumerConfig config;

        public Consumer(ConsumerConfig config)
        {
            this.config = config;
        }
        public void init()
        {
            this.mqttFactory = new MqttFactory();

            var tlsOptions = new MqttClientTlsOptions
            {
                UseTls = false,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                AllowUntrustedCertificates = true
            };

            this.options = new MqttClientOptions
            {
                ClientId = "ConsumerClient" + Thread.CurrentThread.Name, // will take name of main thread ???!!
                ProtocolVersion = MqttProtocolVersion.V311,
                ChannelOptions = new MqttClientTcpOptions
                {
                    Server = this.config.BrokerHostname,
                    Port = this.config.BrokerPort,
                    TlsOptions = tlsOptions
                }
            };

            // Check if the arguments were parsed correctly and injected 
            if (options.ChannelOptions == null)
            {
                throw new InvalidOperationException();
            }

            options.Credentials = new MqttClientCredentials
            {
                Username = this.config.Username,
                Password = Encoding.UTF8.GetBytes(this.config.Password)
            };

            options.CleanSession = true;
            options.KeepAlivePeriod = TimeSpan.FromSeconds(5);
            this.managedMqttClientSubscriber = mqttFactory.CreateManagedMqttClient();
            this.managedMqttClientSubscriber.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnPublisherConnected);
            this.managedMqttClientSubscriber.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnPublisherDisconnected);
            this.managedMqttClientSubscriber.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(this.OnSubscriberMessageReceived);
        }

        /// <summary>
        /// Handles the received subscriber message event.
        /// </summary>
        /// <param name="x">The MQTT application message received event args.</param>
        private void OnSubscriberMessageReceived(MqttApplicationMessageReceivedEventArgs x)
        {
            var payload = x.ApplicationMessage.ConvertPayloadToString();
            var item = $"Timestamp of arrival: {DateTime.Now:O} | Topic: {x.ApplicationMessage.Topic} | Payload: {payload} | QoS: {x.ApplicationMessage.QualityOfServiceLevel}";
            Console.WriteLine(item);

            // Persiste data on file
            using StreamWriter file = new(this.config.PersistenceFile, append: true);
            file.WriteLine(payload);
        }

        /// <summary>
        /// Handle called when client gets connected
        /// </summary>
        /// <param name="obj"></param>
        private void OnPublisherConnected(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine($"Subscriber connected to the broker "); // and running on Thread {Thread.CurrentThread.Name}
        }

        /// <summary>
        /// Handle called when client gets disconnected
        /// </summary>
        /// <param name="obj"></param>
        private void OnPublisherDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine("Subscriber disconnected from the broker");
        }

        /// <summary>
        /// Help function to start the client
        /// </summary>
        public void startClient()
        {
            this.managedMqttClientSubscriber.StartAsync(
                new ManagedMqttClientOptions { ClientOptions = this.options });
            var topicFilter = new MqttTopicFilter { Topic = config.Topic };
            Console.WriteLine($"Subscripbing to topic {config.Topic}");
            this.managedMqttClientSubscriber.SubscribeAsync(topicFilter);
        }

        /// <summary>
        /// Help function to stop the client
        /// </summary>
        public async void stopClient()
        {
            if (this.managedMqttClientSubscriber == null)
                return;
            Console.WriteLine("Stopping the client");
            await this.managedMqttClientSubscriber.StopAsync();
            this.managedMqttClientSubscriber = null;
        }
    }
}