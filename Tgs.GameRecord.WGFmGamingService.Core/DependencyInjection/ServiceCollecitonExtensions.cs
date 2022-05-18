using System.Reflection;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecordSchedules.Client;
using Tgs.GameRecord.WGFmGamingService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRecordCore(
       this IServiceCollection services,
       Action<GameRecordInfoOptions, IServiceProvider> configure,
       Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
       Action<GameRecordSchedulesClientOptions, IServiceProvider> gameRecordSchedulesClientOptions,
       Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions)
        => services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(configure)
        .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .Scan(scan =>
        {
            _ = scan.FromCallingAssembly()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("repository", StringComparison.OrdinalIgnoreCase)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
        })
        .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
        .AddGameRecordSchedulesClient(gameRecordSchedulesClientOptions)
        .AddGameProvidersClient(gameProvidersClientOptions)
        .AddScoped<ISystemClock, SystemClock>()
        .AddTransient<IWGFMGaming, WGFMGamingRepository>()
        .Decorate<IWGFMGaming>((innerRepository, sp) => ActivatorUtilities.CreateInstance<WGFMGamingMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
}
