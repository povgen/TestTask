using Microsoft.AspNetCore.Mvc;
using TestTask.Consumer;
using TestTask.Server.Services;

namespace TestTask.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class FileController : Controller
{
    private readonly IStorage _storage;

    private readonly long _maxSizeOfFiles; // 10 mb 

    private readonly IPublisher _publisher;
    public FileController(IConfiguration config)
    {
        RabbitMQSettings settings = new();
        config.GetSection(nameof(RabbitMQSettings)).Bind(settings);

        var pathToStorage = config.GetValue<string>("PathToStorage") ??
                            throw new Exception("PathToStorage is undefined in appsettings.json");
        
        _storage = new Storage(pathToStorage);
        _maxSizeOfFiles  = config.GetValue("MaxFilesSize", 10240 * 1024);

        _publisher = new Publisher(settings);
    }
  

    private bool IsValidFiles(IEnumerable<IFormFile> files, out string error)
    {
        long size = 0;

        foreach (var formFile in files)
        {
            if (formFile.ContentType != "text/html")
            {
                error = "Incorrect format file, allowed only html";
                return false;
            }

            size += formFile.Length;
        }

        var isTooBig = size >= _maxSizeOfFiles;
        var max = _maxSizeOfFiles / (1024 * 1024);
        error = isTooBig ? $"Too big data max is {max} Mb" : "";
        return !isTooBig;
    }

    private static string GetPdfFilePath(string fileToken)
    {
        return $"GeneratedPdf/{fileToken}.pdf";
    }

    private string GetHtmlFilePath(string fileToken)
    {
        return $"UploadedHtml/{fileToken}.html";
    }

    [HttpPost]
    public IActionResult UploadFile(IFormFile[] files)
    {
        if (!IsValidFiles(files, out var error))
        {
            return BadRequest(error);
        }

        var fileTokens = new List<string>();

        foreach (var file in files)
        {
            var fileToken = Path.GetRandomFileName().Replace('.', '_');
            _storage.SaveFile($"UploadedHtml/{fileToken}.html", file);
            if (!_storage.IsFileExists($"UploadedHtml/{fileToken}.html"))
            {
                throw new Exception(
                    "The file was not saved. Check the availability "
                    + "of the directory specified in the \"Storage Path\"");
            }

            fileTokens.Add(fileToken);
            _publisher.AddToQueue(fileToken);
        }


        return Ok(new { fileTokens });
    }

    [Route("status")]
    [HttpGet]
    public IActionResult GetStatusOfPdf(string fileToken)
    {
        var t = _storage.IsFileExists($"UploadedHtml/{fileToken}.html");
        if (!t)
        {
            return BadRequest("File not found");
        }

        var isExist = _storage.IsFileExists(GetPdfFilePath(fileToken));

        return Ok(new
        {
            status = isExist ? "completed" : "processing"
        });
    }

    [HttpGet]
    public IActionResult GetPdf(string fileToken)
    {
        var path = GetPdfFilePath(fileToken);
        if (!_storage.IsFileExists(path)) // It's necessary for correct work with cloud file services
        {
            return BadRequest("File not found");
        }

        path = _storage.GetAbsoluteFilePath(path);

        return PhysicalFile(path, "application/pdf", "test.pdf");
    }

    [HttpDelete]
    public IActionResult DeleteFiles(string fileToken)
    {
        var pdfPath = GetPdfFilePath(fileToken);
        if (!_storage.IsFileExists(pdfPath))
        {
            return BadRequest("File not found");
        }

        ;

        _storage.DeleteFile(pdfPath);
        _storage.DeleteFile(GetHtmlFilePath(fileToken));

        return Ok();
    }
}