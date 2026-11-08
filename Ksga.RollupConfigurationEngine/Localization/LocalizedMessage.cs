namespace Ksga.RollupConfigurationEngine.Core.Localization
{
    public sealed class LocalizedMessage
    {
        public string Code { get; }
        public IReadOnlyList<object?> Args { get; }

        public LocalizedMessage(string code, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            Code = code;
            Args = args ?? Array.Empty<object?>();
        }

        public string ToString(string? language) => MessageCatalog.Format(this, language);

        public override string ToString() => ToString(null);
    }
}
