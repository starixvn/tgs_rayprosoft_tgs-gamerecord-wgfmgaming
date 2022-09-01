using System.Reflection;
using StackExchange.Redis;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.WGFmGamingService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
       this IServiceCollection services,
       Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
       Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
       Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions,
       Func<IServiceProvider, IDatabase> redisDatabaseResolver)
        => services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
            .AddGameInterfaceWGFmGamingServiceClient(gameInterfaceWGFmGamingClientOptions)
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, redisDatabaseResolver(sp)))
            .AddTransient<IUtensil, UtensilRepository>()
            .Decorate<IUtensil>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UtensilCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
}
