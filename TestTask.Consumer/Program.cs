// See https://aka.ms/new-console-template for more information

using System.Text;
using PuppeteerSharp;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var pathToPdfStorage = "Z:/Storage/GeneratedPdf";
var pathToHtmlStorage = "Z:/Storage/UploadedHtml";
 
var factory = new ConnectionFactory()
{
    HostName = "127.0.0.1",
};

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
    Directory.CreateDirectory(pathToPdfStorage);
    Console.WriteLine($"{pathToHtmlStorage}/{fileName}.html");
    var reader = new StreamReader($"{pathToHtmlStorage}/{fileName}.html");
    var text = reader.ReadToEnd();
    reader.Close();
    //
            
    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
    await using var page = await browser.NewPageAsync();
    await page.SetContentAsync(text);
    //
    await page.PdfAsync($"{pathToPdfStorage}/{fileName}.pdf");
    await page.CloseAsync();
    await browser.CloseAsync();


};

channel.BasicConsume(queue: "task_queue", autoAck: true, consumer: consumer);

Console.ReadKey();