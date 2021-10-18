using System;

namespace MqttConsumer
{
    using System;
    using System.Text;

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
            var envPersistencePath = System.Environment
                .GetEnvironmentVariable("PERSISTENCE_DIR", EnvironmentVariableTarget.Machine) ?? "/Users/MouaadGssair/Desktop/_Workspace/dotnetDemo/Projects/Persistence/";

            var envPersistenceFilename = System.Environment
                .GetEnvironmentVariable("PERSISTENCE_FILENAME", EnvironmentVariableTarget.Machine) ?? "SimpleDB.txt"; // TODO: Use JSON instead ?

            var config = new ConsumerConfig
            (
                envBrokerHostname,
                Int32.Parse(envBrokerPort),
                topic,
                envPersistencePath + envPersistenceFilename
            );

            Console.WriteLine($"Available configuration: \n\t Broker: {config.BrokerHostname}:{config.BrokerPort}");
            Console.WriteLine($"\t Topic: {config.Topic}");
            Console.WriteLine($"\t Persistence file: {config.PersistenceFile}");

            Console.WriteLine("Initializing ...");
            var consumer = new Consumer(config);

            consumer.init();

            Console.WriteLine("Starting the consumer ...");
            consumer.startClient();

            Console.WriteLine("Listening to messages ...");
            while (true)
            {

            }
        }



    }
}
