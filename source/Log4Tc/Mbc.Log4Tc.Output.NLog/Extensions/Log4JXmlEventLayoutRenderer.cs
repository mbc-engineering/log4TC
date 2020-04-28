using NLog;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Mbc.Log4Tc.Output.NLog.Extensions
{
    /// <summary>
    /// XML event description compatible with log4j, Chainsaw and NLogViewer.
    /// Adapted from NLog.LayoutRenderers.Log4JXmlEventLayoutRenderer for usage
    /// with log4TC.
    /// </summary>
    [LayoutRenderer("mbclog4jxmlevent")]
    [ThreadSafe]
    [MutableUnsafe]
    public class Log4JXmlEventLayoutRenderer : LayoutRenderer
    {
        /*
        <log4j:event
            logger="FischerAg.Ui.Common.Caliburn.LoggerBootstrapper"
            level="INFO"
            timestamp="1587645295261"
            thread="1">
            <log4j:message>Startup Application with Version: '20.4.14.0' on host: 'STGM-NB2'.</log4j:message>
            <log4j:properties>
                <log4j:data name="log4japp" value="FischerAg.Ui.Nsp.exe(68804)" />
                <log4j:data name="log4jmachinename" value="STGM-NB2" />
            </log4j:properties>
         </log4j:event>


        */
        private static readonly DateTime Log4jDateBase = new DateTime(1970, 1, 1);

        private static readonly string DummyNamespace = "http://nlog-project.org/dummynamespace/" + Guid.NewGuid();
        private static readonly string DummyNamespaceRemover = " xmlns:log4j=\"" + DummyNamespace + "\"";

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4JXmlEventLayoutRenderer" /> class.
        /// </summary>
        public Log4JXmlEventLayoutRenderer()
            : this(LogFactory.CurrentAppDomain)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4JXmlEventLayoutRenderer" /> class.
        /// </summary>
        public Log4JXmlEventLayoutRenderer(IAppDomain appDomain)
        {
            Parameters = new List<NLogViewerParameterInfo>();
        }

        /// <summary>
        /// Initializes the layout renderer.
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            _xmlWriterSettings = new XmlWriterSettings
            {
                Indent = IndentXml,
                ConformanceLevel = ConformanceLevel.Fragment,
                NamespaceHandling = NamespaceHandling.OmitDuplicates,
                IndentChars = "  ",
            };
        }

        /// <summary>
        /// Gets or sets a value indicating whether the XML should use spaces for indentation.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IndentXml { get; set; }

        /// <summary>
        /// Gets or sets the AppInfo field. By default it's the source field.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public Layout AppInfo { get; set; }

        public Layout Message { get; set; }

        /// <summary>
        /// Gets or sets the option to include all properties from the log events
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeAllProperties { get; set; }

        /// <summary>
        /// Gets or sets the log4j:event logger-xml-attribute (Default ${logger})
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public Layout LoggerName { get; set; }

        private XmlWriterSettings _xmlWriterSettings;

        internal IList<NLogViewerParameterInfo> Parameters { get; set; }

        private int GetTaskId(LogEventInfo logEventInfo)
        {
            if (logEventInfo.Properties.TryGetValue("_TcTaskIdx_", out object value) && value != null)
            {
                try
                {
                    return Convert.ToInt32(value);
                }
                catch (Exception)
                {
                }
            }

            return 0;
        }

        private string GetSource(LogEventInfo logEventInfo)
        {
            if (logEventInfo.Properties.TryGetValue("_TcLogSource_", out object value) && value != null)
            {
                return value.ToString();
            }

            return string.Empty;
        }

        private string GetApplication(LogEventInfo logEventInfo)
        {
            if (logEventInfo.Properties.TryGetValue("_TcAppName_", out object appName) && appName != null && logEventInfo.Properties.TryGetValue("_TcProjectName_", out object projectName) && projectName != null)
            {
                return $"{appName}-{projectName}";
            }

            return string.Empty;
        }

        private string RenderLogEvent(Layout layout, LogEventInfo logEvent)
        {
            if (layout == null || logEvent == null)
                return null;    // Signal that input was wrong

            SimpleLayout simpleLayout = layout as SimpleLayout;
            if (simpleLayout != null && simpleLayout.IsFixedText)
            {
                return simpleLayout.Render(logEvent);
            }

            return layout.Render(logEvent);
        }

        internal void AppendToStringBuilder(StringBuilder sb, LogEventInfo logEvent)
        {
            Append(sb, logEvent);
        }

        /// <summary>
        /// Renders the XML logging event and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xtw = XmlWriter.Create(sb, _xmlWriterSettings))
            {
                xtw.WriteStartElement("log4j", "event", DummyNamespace);
                xtw.WriteAttributeSafeString("logger", LoggerName != null ? LoggerName.Render(logEvent) : logEvent.LoggerName);
                xtw.WriteAttributeString("level", logEvent.Level.Name.ToUpperInvariant());
                xtw.WriteAttributeString("timestamp", Convert.ToString((long)(logEvent.TimeStamp.ToUniversalTime() - Log4jDateBase).TotalMilliseconds, CultureInfo.InvariantCulture));
                xtw.WriteAttributeString("thread", Convert.ToString(GetTaskId(logEvent)));

                xtw.WriteElementSafeString("log4j", "message", DummyNamespace, Message != null ? RenderLogEvent(Message, logEvent) : logEvent.FormattedMessage);

                xtw.WriteStartElement("log4j", "properties", DummyNamespace);

                if (IncludeAllProperties)
                {
                    AppendProperties("log4j", DummyNamespace, xtw, logEvent);
                }

                AppendParameters(logEvent, xtw);

                xtw.WriteStartElement("log4j", "data", DummyNamespace);
                xtw.WriteAttributeString("name", "log4japp");
                xtw.WriteAttributeSafeString("value", AppInfo?.Render(logEvent) ?? GetApplication(logEvent));
                xtw.WriteEndElement();

                xtw.WriteStartElement("log4j", "data", DummyNamespace);
                xtw.WriteAttributeString("name", "log4jmachinename");
                xtw.WriteAttributeSafeString("value", GetSource(logEvent));
                xtw.WriteEndElement();

                xtw.WriteEndElement();  // properties

                xtw.WriteEndElement();  // event
                xtw.Flush();

                // get rid of 'nlog' and 'log4j' namespace declarations
                sb.Replace(DummyNamespaceRemover, string.Empty);

                // copy to builder
                builder.Append(sb);
            }
        }

        private void AppendParameters(LogEventInfo logEvent, XmlWriter xtw)
        {
            for (int i = 0; i < Parameters?.Count; ++i)
            {
                var parameter = Parameters[i];
                if (string.IsNullOrEmpty(parameter?.Name))
                    continue;

                var parameterValue = parameter.Layout?.Render(logEvent) ?? string.Empty;
                if (!parameter.IncludeEmptyValue && string.IsNullOrEmpty(parameterValue))
                    continue;

                xtw.WriteStartElement("log4j", "data", DummyNamespace);
                xtw.WriteAttributeSafeString("name", parameter.Name);
                xtw.WriteAttributeSafeString("value", parameterValue);
                xtw.WriteEndElement();
            }
        }

        private void AppendProperties(string prefix, string propertiesNamespace, XmlWriter xtw, LogEventInfo logEvent)
        {
            if (logEvent.HasProperties)
            {
                foreach (var contextProperty in logEvent.Properties)
                {
                    string propertyKey = Convert.ToString(contextProperty.Key, CultureInfo.InvariantCulture);
                    if (string.IsNullOrEmpty(propertyKey))
                        continue;

                    string propertyValue = Convert.ToString(contextProperty.Value, CultureInfo.InvariantCulture);
                    if (propertyValue == null)
                        continue;

                    xtw.WriteStartElement(prefix, "data", propertiesNamespace);
                    xtw.WriteAttributeSafeString("name", propertyKey);
                    xtw.WriteAttributeSafeString("value", propertyValue);
                    xtw.WriteEndElement();
                }
            }
        }
    }
}
