using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;

class Consumer {

    static void Main(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var kafkaConfig = configuration.GetSection("Kafka");

        var config = new ConsumerConfig
        {
            // Read properties from appsettings.json
            BootstrapServers = kafkaConfig["BootstrapServers"],
            SaslUsername     = kafkaConfig["SaslUsername"],
            SaslPassword     = kafkaConfig["SaslPassword"],

            // Fixed properties
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism    = SaslMechanism.Plain,
            GroupId          = "kafka-dotnet-getting-started",
            AutoOffsetReset  = AutoOffsetReset.Earliest
        };

        const string topic = "purchases";

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        using (var consumer = new ConsumerBuilder<string, string>(config).Build())
        {
            consumer.Subscribe(topic);
            try {
                while (true) {
                    var cr = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key,-10} value = {cr.Message.Value}");
                }
            }
            catch (OperationCanceledException) {
                // Ctrl-C was pressed.
            }
            finally{
                consumer.Close();
            }
        }
    }
}