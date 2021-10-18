

namespace MqttProducer
{
    using System;
    using System.Text;
    using System.Threading;
    using MQTTnet;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Options;
    using MQTTnet.Extensions.ManagedClient;
    using MQTTnet.Protocol;
    using MQTTnet.Formatter;

    /// <summary>
    /// 
    /// </summary>
    class SimulatedSensor
    {
        private IManagedMqttClient managedMqttClientPublisher;
        private MqttFactory mqttFactory;
        private MqttClientOptions options;

        private ProducerConfig config;

        static Random rnd = new Random();

        public SimulatedSensor(ProducerConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// The run method is the main method that will given as a handle to the thread
        /// </summary>
        public void run()
        {
            while (true)
            {
                try
                {
                    if (!this.managedMqttClientPublisher.IsStarted)
                    {
                        this.startClient();
                    }
                    var sensorValue = DateTimeOffset.Now.ToUnixTimeSeconds() +":"+ generateDummySensorValue();
                    var payload = Encoding.UTF8.GetBytes(sensorValue);
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(this.config.Topic)
                        .WithPayload(payload)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .WithRetainFlag()
                        .Build();

                    if (this.managedMqttClientPublisher != null)
                    {
                        Console.WriteLine($"Publishing timestamp and sensor value: {sensorValue}");
                        this.managedMqttClientPublisher.PublishAsync(message);
                    }
                    Thread.Sleep(this.config.PublishInterval);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occurs" + ex.Message);

                    // TODO: add stop and restart mechanism
                    stopClient();
                    Thread.CurrentThread.Interrupt();
                }
            }
        }

        /// <summary>
        /// Helping method to initialize the MQTT client and other properties
        /// </summary>
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
                ClientId = "ClientPublisher" + Thread.CurrentThread.Name, // will take name of main thread ???!!
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
            this.managedMqttClientPublisher = mqttFactory.CreateManagedMqttClient();
            // this.managedMqttClientPublisher.UseApplicationMessageReceivedHandler(this.HandleReceivedApplicationMessage);
            this.managedMqttClientPublisher.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnPublisherConnected);
            this.managedMqttClientPublisher.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnPublisherDisconnected);

        }

        /// <summary>
        /// Handle called when client gets connected
        /// </summary>
        /// <param name="obj"></param>
        private void OnPublisherConnected(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine($"Publisher connected to the broker and running on Thread {Thread.CurrentThread.Name}");
        }

        /// <summary>
        /// Handle called when client gets disconnected
        /// </summary>
        /// <param name="obj"></param>
        private void OnPublisherDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine("Publisher disconnected from the broker");
            // Console.WriteLine($"Shutting down Thread {Thread.CurrentThread.Name}");
        }

        /// <summary>
        /// Help function to start the client
        /// </summary>
        public void startClient()
        {
            this.managedMqttClientPublisher.StartAsync(
                new ManagedMqttClientOptions { ClientOptions = this.options });
        }

        /// <summary>
        /// Help function to stop the client
        /// </summary>
        public async void stopClient()
        {
            if (this.managedMqttClientPublisher == null)
                return;
            await this.managedMqttClientPublisher.StopAsync();
            this.managedMqttClientPublisher = null;
        }

        /// <summary>
        /// Help method to generate a random number to simulate sensor values
        /// </summary>
        /// <returns></returns>
        public String generateDummySensorValue()
        {
            var sensorValue = rnd.Next(20,30);
            return sensorValue.ToString();
        }

    }
}