using LogViewerTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewerTool.Services
{
    public class LogService
    {
        public List<LogEntry> GetLogs(string filePath)
        {
            var logs = new List<LogEntry>();
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        logs.Add(new LogEntry(
                            timestamp: DateTime.Parse(parts[0]),
                            level: parts[1],
                            message: parts[2],
                            source: parts[3]
                        ));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading log file: {ex.Message}");
            }
            return logs;
        }
    }
}
