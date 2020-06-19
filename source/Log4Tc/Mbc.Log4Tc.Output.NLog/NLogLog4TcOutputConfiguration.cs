using System.ComponentModel.DataAnnotations;

namespace Mbc.Log4Tc.Output.NLog
{
    public class NLogLog4TcOutputConfiguration
    {
        [Required]
        public string Name { get; set; }
    }
}
