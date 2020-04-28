using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mbc.Log4Tc.Output.NLog.Extensions
{

    /// <summary>
    /// Log event context data. Copy of NLog version. Extended to exclude internal
    /// properites "_<*>_".
    /// </summary>
    [LayoutRenderer("mbc-all-event-properties")]
    [ThreadAgnostic]
    [ThreadSafe]
    [MutableUnsafe]
    public class AllEventPropertiesLayoutRenderer : LayoutRenderer
    {
        private string _format;
        private string _beforeKey;
        private string _afterKey;
        private string _afterValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllEventPropertiesLayoutRenderer"/> class.
        /// </summary>
        public AllEventPropertiesLayoutRenderer()
        {
            Separator = ", ";
            Format = "[key]=[value]";
            Exclude = new HashSet<string>(
                new string[0],
                StringComparer.OrdinalIgnoreCase);
            ExcludeStandard = true;
        }

        /// <summary>
        /// Gets or sets string that will be used to separate key/value pairs.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Separator { get; set; }

        /// <summary>
        /// Get or set if empty values should be included.
        ///
        /// A value is empty when null or in case of a string, null or empty string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultValue(false)]
        public bool IncludeEmptyValues { get; set; } = false;

        /// <summary>
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public ISet<string> Exclude { get; set; }

        public bool ExcludeStandard { get; set; }

        /// <summary>
        /// Gets or sets how key/value pairs will be formatted.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Format
        {
            get => _format;
            set
            {
                if (!value.Contains("[key]"))
                    throw new ArgumentException("Invalid format: [key] placeholder is missing.");

                if (!value.Contains("[value]"))
                    throw new ArgumentException("Invalid format: [value] placeholder is missing.");

                _format = value;

                var formatSplit = _format.Split(new[] { "[key]", "[value]" }, StringSplitOptions.None);
                if (formatSplit.Length == 3)
                {
                    _beforeKey = formatSplit[0];
                    _afterKey = formatSplit[1];
                    _afterValue = formatSplit[2];
                }
                else
                {
                    _beforeKey = null;
                    _afterKey = null;
                    _afterValue = null;
                }
            }
        }

        /// <summary>
        /// Renders all log event's properties and appends them to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (!logEvent.HasProperties)
                return;

            var formatProvider = GetFormatProvider(logEvent);
            bool checkForExclude = Exclude?.Count > 0;
            bool nonStandardFormat = _beforeKey == null || _afterKey == null || _afterValue == null;

            bool first = true;
            foreach (var property in logEvent.Properties)
            {
                if (!IncludeEmptyValues && IsEmptyPropertyValue(property.Value))
                    continue;

                if (property.Key is string propertyKey)
                {
                    if (ExcludeStandard && propertyKey.StartsWith("_") && propertyKey.EndsWith("_"))
                        continue;

                    if (checkForExclude && Exclude.Contains(propertyKey))
                        continue;
                }

                if (!first)
                {
                    builder.Append(Separator);
                }

                first = false;

                if (nonStandardFormat)
                {
                    var key = Convert.ToString(property.Key, formatProvider);
                    var value = Convert.ToString(property.Value, formatProvider);
                    var pair = Format.Replace("[key]", key)
                                        .Replace("[value]", value);
                    builder.Append(pair);
                }
                else
                {
                    builder.Append(_beforeKey);
                    builder.Append(property.Key);
                    builder.Append(_afterKey);
                    builder.Append(property.Value);
                    builder.Append(_afterValue);
                }
            }
        }

        private static bool IsEmptyPropertyValue(object value)
        {
            if (value is string s)
            {
                return string.IsNullOrEmpty(s);
            }

            return value == null;
        }
    }
}
