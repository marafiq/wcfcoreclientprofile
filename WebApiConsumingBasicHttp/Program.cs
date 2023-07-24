using System.ServiceModel;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.ObjectPool;
using Nito.AsyncEx;
using WebApiConsumingBasicHttp.PingService;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
    options.ListenAnyIP(80);
});

builder.Services.AddSingleton<ObjectPool<PingServiceClient>>(_ =>
    new DefaultObjectPool<PingServiceClient>(new PoolPolicy()));
builder.Services.AddSingleton<PingServiceClient>(_ =>
    new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            SendTimeout = TimeSpan.FromMinutes(5),
            ReceiveTimeout = TimeSpan.FromMinutes(5)
        },
        new EndpointAddress("http://pingservice/ping")));
var app = builder.Build();
ClientBase<IPingService>.CacheSetting = CacheSetting.AlwaysOn;
app.MapGet("/", () => "Hello World!");
app.MapGet("/singleton",
    (PingServiceClient pingServiceClient) =>
        pingServiceClient.GetData(new GetDataRequest(Random.Shared.Next())).GetDataResult);
app.MapGet("/asyncsingleton",
    (PingServiceClient pingServiceClient) =>
        AsyncContext.Run(() => pingServiceClient.GetData(new GetDataRequest(Random.Shared.Next()))).GetDataResult);
app.MapGet("/pooled", (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = client.GetData(new GetDataRequest(Random.Shared.Next()));
    pool.Return(client);
    return data.GetDataResult;
});

app.MapGet("/asyncpooled", (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = AsyncContext.Run(() => client.GetData(new GetDataRequest(Random.Shared.Next())));
    pool.Return(client);
    return data.GetDataResult;
});
app.MapGet("/pureasync", async (ObjectPool<PingServiceClient> pool) =>
{
    var client = pool.Get();
    var data = await client.GetDataAsync(new GetDataRequest(Random.Shared.Next()));
    pool.Return(client);
    return data.GetDataResult;
});
app.MapGet("/healthy", () => "Healthy");
app.Run();

public class PoolPolicy : IPooledObjectPolicy<PingServiceClient>
{
    public PingServiceClient Create()
    {
        return new PingServiceClient(new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            SendTimeout = TimeSpan.FromMinutes(5),
            ReceiveTimeout = TimeSpan.FromMinutes(5)
        }, new EndpointAddress("http://pingservice/ping"));
    }

    public bool Return(PingServiceClient obj)
    {
        if (obj.State == CommunicationState.Faulted)
        {
            return false;
        }

        return true;
    }
}