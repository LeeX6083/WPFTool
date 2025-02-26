using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; } // 时间戳
        public string Level { get; set; } = string.Empty;      // 日志级别
        public int ThreadId { get; set; }       // 线程ID
        public string Module { get; set; } = string.Empty;   // 模块名称
        public string Version { get; set; } = string.Empty;   // 版本号
        public string Method { get; set; } = string.Empty;    // 方法名称
        public string Content { get; set; } = string.Empty;  // 日志内容
    }
}
