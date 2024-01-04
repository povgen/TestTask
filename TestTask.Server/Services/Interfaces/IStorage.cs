namespace TestTask.Server.Services;

public interface IStorage
{
    public void SaveFile(string relativePath, IFormFile file);

    public string GetAbsoluteFilePath(string relativePath);
    
    public bool IsFileExists(string relativePath);

    public void DeleteFile(string relativePath);


}