using System;
using System.Threading.Tasks;
using Confluent.Kafka.Admin;
using Confluent.Kafka;

public static class KafkaTopicCreator
{
    public static async Task CreateTopicIfNotExistsAsync(string bootstrapServers, string topicName)
    {
        var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

        using var adminClient = new AdminClientBuilder(config).Build();

        var topicSpecification = new TopicSpecification
        {
            Name = topicName,
            NumPartitions = 3,
            ReplicationFactor = 1
        };

        try
        {
            await adminClient.CreateTopicsAsync(new[] { topicSpecification });
            Console.WriteLine("Tópico criado com sucesso!");
        }
        catch (CreateTopicsException e)
        {
            if (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                Console.WriteLine("Tópico já existe.");
            }
            else
            {
                Console.WriteLine($"Erro ao criar tópico: {e.Results[0].Error.Reason}");
                throw;
            }
        }
    }
}