using System;
using System.Threading;

namespace MqttProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading configuration ...");
            var envBrokerHostname = System.Environment
                .GetEnvironmentVariable("BROKER_HOSTNAME", EnvironmentVariableTarget.Machine) ?? "localhost"; // Machine/Process/User
            var envBrokerPort = System.Environment
                .GetEnvironmentVariable("BROKER_PORT", EnvironmentVariableTarget.Machine) ?? "9883";
            var topic = "floor1/temperature/sensor1";
            var envPublishInterval = System.Environment
                .GetEnvironmentVariable("PUBLISH_INTERVAL", EnvironmentVariableTarget.Machine) ?? "2000";

            var config = new ProducerConfig 
            (
                envBrokerHostname,
                Int32.Parse(envBrokerPort),
                topic,
                Int32.Parse(envPublishInterval)
            );
            Console.WriteLine($"Available configuration: \n\t Broker: {config.BrokerHostname}:{config.BrokerPort}");
            Console.WriteLine($"\t Topic: {config.Topic}");
            Console.WriteLine($"\t PublishInterval: {config.PublishInterval}");

            Console.WriteLine("Initializing ...");
            var producer = new SimulatedSensor(config);
            producer.init();

            var producerThread = new Thread(producer.run);
            
            Console.WriteLine("Starting the publisher ...");
            producer.startClient();
            producerThread.Start();

            // TODO Monitor the thread
            while (producerThread.ThreadState != ThreadState.Stopped)
            {
                Thread.Sleep(5000);
            }

            Console.WriteLine("Shutting down ...");
        }

        


    }
}
