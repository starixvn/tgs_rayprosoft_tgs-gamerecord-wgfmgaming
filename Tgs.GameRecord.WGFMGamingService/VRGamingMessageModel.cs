namespace Tgs.GameRecord.WGFMGamingService
{
    public record VRGamingMessageModel
    {
        public string GameProviderCode { get; init; } = default!;
        public DateTimeOffset SendDateTime { get; init; }
    }
}
