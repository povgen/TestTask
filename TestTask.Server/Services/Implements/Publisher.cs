using System.Text;
using RabbitMQ.Client;
using TestTask.Consumer;

namespace TestTask.Server.Services;

public class Publisher: IPublisher
{
    private readonly ConnectionFactory _factory;
    public Publisher(RabbitMQSettings settings)
    {
        _factory = new ConnectionFactory {
            HostName = settings.Host
        };
        if (settings.Port.HasValue)
        {
            _factory.Port = (int)settings.Port;
        }
    }
    public void AddToQueue(string fileToken)
    {
        using (var connection = _factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(
                    queue: "task_queue",
                    exclusive: false,
                    durable: true,
                    autoDelete: false,
                    arguments: null
                );


                channel.BasicPublish(
                    exchange: "",
                    routingKey: "task_queue",
                    basicProperties: null,
                    body: Encoding.UTF8.GetBytes(fileToken)
                );
            }
        }
    }
}