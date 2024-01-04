using System.Text;
using PuppeteerSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestTask.Consumer;

using var host = Host.CreateApplicationBuilder(args).Build();

var config = host.Services.GetRequiredService<IConfiguration>();

RabbitMQSettings settings = new();
config.GetSection(nameof(RabbitMQSettings)).Bind(settings);

var pathToStorage = config.GetValue<string>("PathToStorage") ??
                    throw new Exception("PathToStorage is undefined in appsettings.json");


var pathToPdfStorage = Path.Combine(pathToStorage, "GeneratedPdf");
var pathToHtmlStorage = Path.Combine(pathToStorage, "UploadedHtml");

Directory.CreateDirectory(pathToPdfStorage);

var factory = new ConnectionFactory
{
    HostName = settings.Host,
};
if (settings.Port.HasValue)
{
    factory.Port = (int)settings.Port;
}

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(
    queue: "task_queue",
    exclusive: false,
    durable: true,
    autoDelete: false,
    arguments: null
);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += async (model, es) =>
{
    var body = es.Body.ToArray();
    var fileName = Encoding.UTF8.GetString(body);

    Converter.convertHtmlToPdf($"{pathToHtmlStorage}/{fileName}.html", 
        $"{pathToPdfStorage}/{fileName}.pdf");
};

channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);

await host.RunAsync();