namespace TestTask.Server.Services;

public interface IPublisher
{
    public void AddToQueue(string fileToken);
}