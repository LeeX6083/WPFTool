using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfTool.SharedInterfaces.Models;

namespace WpfTool.SharedInterfaces.Services
{
    public interface ILogService
    {
        List<LogEntry> ReadLogs(string[] filePaths);
    }
}
