using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class ProducerTests
{
    [Fact]
    public void Test_Configuration_Loads_Correctly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Kafka:BootstrapServers", "test-server"},
            {"Kafka:SaslUsername", "test-user"},
            {"Kafka:SaslPassword", "test-password"}
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var kafkaConfig = configuration.GetSection("Kafka");

        // Assert
        Assert.Equal("test-server", kafkaConfig["BootstrapServers"]);
        Assert.Equal("test-user", kafkaConfig["SaslUsername"]);
        Assert.Equal("test-password", kafkaConfig["SaslPassword"]);
    }
}