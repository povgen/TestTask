namespace TestTask.Server.Services;

public class Storage(string basePathToStorage): IStorage
{
    private string GetFilePath(string relativePath, out string pathToDirectory)
    {
        var path = Path.Combine(basePathToStorage, relativePath);
        var pathParts = path.Split('/').ToList();
        pathParts.RemoveAt(pathParts.Count - 1);
        pathToDirectory = string.Join('/', pathParts);
        return path;
    }
    
    /// <summary>
    /// Save file to storage
    /// </summary>
    /// <param name="relativePath">relative file path from basePathToStorage</param>
    /// <param name="file"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async void SaveFile(string relativePath, IFormFile file)
    {
        var path = GetFilePath(relativePath, out var pathToDirectory);
        
        Directory.CreateDirectory(pathToDirectory);

        await using var fileStream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(fileStream);
        fileStream.Close();
    }

    public string GetAbsoluteFilePath(string relativePath)
    {
        var path = GetFilePath(relativePath, out var pathToDirectory);
        return Path.GetFullPath(path);
    }

    public bool IsFileExists(string relativePath)
    {
        var path = GetFilePath(relativePath, out var pathToDirectory);
        return File.Exists(path);
    }

    public void DeleteFile(string relativePath)
    {
        File.Delete(GetFilePath(relativePath, out var pathToDirectory));
    }
}