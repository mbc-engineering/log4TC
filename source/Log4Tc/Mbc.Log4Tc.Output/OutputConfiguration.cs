using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output
{
    public class OutputConfiguration
    {
        [Required]
        public string Type { get; set; }

        public object Filter { get; set; }

        public IConfigurationSection ConfigSection { get; set; }
    }
}
