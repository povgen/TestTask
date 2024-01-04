using PuppeteerSharp;

namespace TestTask.Consumer;

public static class Converter
{
    /// <summary>
    /// generates a pdf of the page with PuppeteerSharp
    /// </summary>
    /// <param name="htmlFilePath">The file path to load the HTML to.</param>
    /// <param name="pdfFilePath">The file path to save the PDF to.</param>
    /// <remarks>
    /// Generating a pdf is currently only supported in Chrome headless.
    /// </remarks>
    public static async void convertHtmlToPdf(string htmlFilePath, string pdfFilePath)
    {
        var reader = new StreamReader(htmlFilePath);
        var text = await reader.ReadToEndAsync();
        reader.Close();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(text);

        await page.PdfAsync(pdfFilePath);
        await page.CloseAsync();
        await browser.CloseAsync();
    }
}