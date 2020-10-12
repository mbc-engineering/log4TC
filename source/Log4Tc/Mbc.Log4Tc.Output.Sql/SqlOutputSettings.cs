using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SqlOutputSettings
    {
        public enum DbDriver
        {
            MySql,
            Postgres,
            SqlServer,
        }

        public enum DbScheme
        {
            SimpleFlat,
            FullFlat,
        }

        [Required]
        public DbDriver Driver { get; set; }

        [Required]
        public string ConnectionString { get; set; }

        public DbScheme Scheme { get; set; } = DbScheme.SimpleFlat;
    }
}
