namespace Ksga.RollupConfigurationEngine.Core.Localization
{
    public sealed class RollupException : InvalidOperationException
    {
        public LocalizedMessage Localized { get; }

        public string Code => Localized.Code;

        public RollupException(LocalizedMessage localized)
            : base((localized ?? throw new ArgumentNullException(nameof(localized))).ToString())
        {
            Localized = localized;
        }

        public static RollupException Create(string code, params object?[] args)
            => new RollupException(new LocalizedMessage(code, args));

        public string GetMessage(string? language) => Localized.ToString(language);
    }
}
