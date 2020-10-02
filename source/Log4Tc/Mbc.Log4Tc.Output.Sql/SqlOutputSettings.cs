using Mbc.Log4Tc.Service.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace Mbc.Log4Tc.Output.Sql
{
    internal class SqlOutputSettings
    {
        /// <summary>
        /// Typ der die Klasse <see cref="DbConnection"/> implementiert.
        /// </summary>
        [Required]
        [TypeClassValidation]
        public string ConnectionType { get; set; }

        [Required]
        public string ConnectionString { get; set; }
    }
}
