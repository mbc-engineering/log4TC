using Mbc.Log4Tc.Model;
using System.Threading.Tasks;

namespace Mbc.Log4Tc.Output
{
    public interface IOutputHandler
    {
        Task ProcesLogEntry(LogEntry logEntry);
    }
}
