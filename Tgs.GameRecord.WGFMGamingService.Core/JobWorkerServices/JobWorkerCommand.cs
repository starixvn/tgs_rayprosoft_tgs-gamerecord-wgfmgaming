namespace Tgs.GameRecord.WGFmGamingService.Core.JobWorkerServices
{
    public record JobWorkerCommand
    {
        public string GameProviderCode { get; init; } = default!;
        public DateTimeOffset SendOn { get; init; }

        public DateTimeOffset ReceiveOn { get; init; }
    }
}
