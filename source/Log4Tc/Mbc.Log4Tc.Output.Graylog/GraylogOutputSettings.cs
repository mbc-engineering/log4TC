namespace Mbc.Log4Tc.Output.Graylog
{
    internal class GraylogOutputSettings
    {
        public string GraylogHostname { get; set; } = "localhost";

        public ushort GraylogPort { get; set; } = 12201;

        public GelfCompressionType GelfCompression { get; set; } = GelfCompressionType.Gzip;

        internal enum GelfCompressionType
        {
            None,
            Gzip,
        }
    }
}
