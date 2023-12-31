namespace PingService;

public class PingService : IPingService
{
    public async Task<string> GetData(int value)
    {
        await Task.Delay(500);
        return $"Replying to your Ping - {value} with Pong.";
    }
}