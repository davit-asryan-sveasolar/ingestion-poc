using Microsoft.Extensions.DependencyInjection;

namespace Vendors.Huawei.Client;

public static class HuaweiServiceCollectionExtensions
{
    public static IServiceCollection AddHuaweiClient(this IServiceCollection services, string baseAddress)
    {
        services.AddHttpClient("huawei", client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });
        return services.AddSingleton<HuaweiApiClient>();
    }
}