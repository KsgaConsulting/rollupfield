using System.Collections;
using System.Globalization;
using Ksga.RollupConfigurationEngine.Core.Localization.Languages;

namespace Ksga.RollupConfigurationEngine.Core.Localization
{
    public static class MessageCatalog
    {
        public const string DefaultLanguage = "en";

        private static readonly Dictionary<string, IReadOnlyDictionary<string, string>> Catalogs =
            new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal)
            {
                ["en"] = EnglishMessages.Values,
                ["fr"] = FrenchMessages.Values,
                ["es"] = SpanishMessages.Values,
                ["it"] = ItalianMessages.Values,
                ["pt"] = PortugueseMessages.Values,
                ["de"] = GermanMessages.Values,
                ["nl"] = DutchMessages.Values,
                ["ar"] = ArabicMessages.Values
            };

        public static IReadOnlyCollection<string> SupportedLanguages => Catalogs.Keys;

        public static IReadOnlyDictionary<string, string> GetCatalog(string language)
            => Catalogs[NormalizeLanguage(language)];

        public static string NormalizeLanguage(string? language)
        {
            if (string.IsNullOrWhiteSpace(language))
                return DefaultLanguage;

            var trimmed = language!.Trim();
            var separator = trimmed.IndexOfAny(new[] { '-', '_' });
            var code = (separator > 0 ? trimmed.Substring(0, separator) : trimmed).ToLowerInvariant();

            return Catalogs.ContainsKey(code) ? code : DefaultLanguage;
        }

        public static string Format(LocalizedMessage message, string? language)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var normalized = NormalizeLanguage(language);

            if (!Catalogs[normalized].TryGetValue(message.Code, out var template)
                && !Catalogs[DefaultLanguage].TryGetValue(message.Code, out template))
            {
                template = message.Code;
            }

            var args = new object?[message.Args.Count];

            for (var i = 0; i < args.Length; i++)
                args[i] = Resolve(message.Args[i], normalized);

            return args.Length == 0
                ? template
                : string.Format(CultureInfo.InvariantCulture, template, args);
        }

        private static object? Resolve(object? argument, string language)
        {
            if (argument is LocalizedMessage nested)
                return Format(nested, language);

            if (argument is IEnumerable sequence && !(argument is string))
            {
                var parts = new List<string>();

                foreach (var item in sequence)
                    parts.Add(item is LocalizedMessage localized ? Format(localized, language) : Convert.ToString(item, CultureInfo.InvariantCulture) ?? string.Empty);

                return string.Join(" ", parts);
            }

            return argument;
        }
    }
}
