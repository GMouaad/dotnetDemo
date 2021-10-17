

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

        public void run()
        {
            while (true)
            {
                try
                {
                    var payload = Encoding.UTF8.GetBytes(generateDummySensorValue());
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(this.config.Topic)
                        .WithPayload(payload)
                        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                        .WithRetainFlag()
                        .Build();

                    if (this.managedMqttClientPublisher != null)
                    {
                        this.managedMqttClientPublisher.PublishAsync(message);
                    }
                    Thread.Sleep(this.config.PublishInterval);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Occurs" + ex.Message);

                    // TODO: add stop and restart mechanism?
                    // stop();
                    // Thread.CurrentThread.Suspend();
                }
            }
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

        private void OnPublisherConnected(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine($"Publisher connected to the broker and running on Thread {Thread.CurrentThread.Name}");
        }

        private void OnPublisherDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine("Publisher disconnected from the broker");
            // Console.WriteLine($"Shutting down Thread {Thread.CurrentThread.Name}");
        }

        public void start()
        {
            this.managedMqttClientPublisher.StartAsync(
                new ManagedMqttClientOptions { ClientOptions = this.options });
        }

        public async void stop()
        {
            if (this.managedMqttClientPublisher == null)
                return;
            await this.managedMqttClientPublisher.StopAsync();
            this.managedMqttClientPublisher = null;
        }

        public String generateDummySensorValue()
        {
            var sensorValue = rnd.Next(20,30);
            return sensorValue.ToString();
        }

    }
}