var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel((b,options) =>
{
    
    options.ListenAnyIP(80);
});
// Your previous configuration code
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<PingService.PingService>();
    serviceBuilder.AddServiceEndpoint<PingService.PingService, IPingService>(
        new BasicHttpBinding(BasicHttpSecurityMode.None), "http://pingservice:80/ping");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = false;
});
app.UseHttpLogging();
app.Urls.Add("http://pingservice:80");
app.Run();