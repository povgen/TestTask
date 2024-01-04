using System.Text;
using RabbitMQ.Client;

namespace TestTask.Server.Services;

public class Publisher: IPublisher
{
    private ConnectionFactory _factory;
    public Publisher(string hostName, int? port)
    {
        _factory = new ConnectionFactory {
            HostName = hostName
        };
        if (port.HasValue)
        {
            _factory.Port = (int)port;
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