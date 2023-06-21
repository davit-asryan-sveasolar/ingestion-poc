using Amazon.SimpleNotificationService;
using Inverters.Ingestion.Huawei.Jobs.Live.Events;
using Inverters.Ingestion.Huawei.Jobs.Live.Hosting;
using Inverters.Ingestion.Huawei.Jobs.Live.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vendors.Huawei.Client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole(f => f.TimestampFormat = "yyyy/MM/dd HH:mm:ss.ff");
builder.Services.AddHostedService<MainService>();
builder.Services.AddHuaweiClient("https://eu5.fusionsolar.huawei.com:27200");
builder.Services.AddSingleton<TotalToClockAlignedConverter>();
builder.Services.AddSingleton<HuaweiSiteDataRepository>();
builder.Services.AddSingleton(sp
    => sp.GetRequiredService<IConfiguration>().GetAWSOptions().CreateServiceClient<IAmazonSimpleNotificationService>());
builder.Services.AddSingleton<EventPublisher>();

IHost host = builder.Build();
host.Run();