using UiDesktopApp1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wpf.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace UiDesktopApp1.Services
{
    public class LogService : IPageService
    {
        private readonly IServiceProvider _serviceProvider;
        public LogService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public static List<LogEntry> ReadLogs(string[] filePaths)
        {
            var logEntries = new List<LogEntry>();

            foreach (string filePath in filePaths)
            {
                if (!File.Exists(filePath))
                    return logEntries;
                // 注册 CodePagesEncodingProvider 以支持 GB2312 等编码
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // 定义日志行的正则表达式模式
                string pattern = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d+)\|([A-Z]+)\[(\d+)\]\[([^\]]+)\]\s(.*)$";
                Regex regex = new Regex(pattern);
                Encoding logEncoding = DetectEncodingFromBom(filePath);
                foreach (var line in File.ReadAllLines(filePath, logEncoding))
                {
                    Match match = regex.Match(line);
                    if (match.Success)
                    {
                        // 提取匹配的组
                        string timestampStr = match.Groups[1].Value;
                        string level = match.Groups[2].Value;
                        string threadIdStr = match.Groups[3].Value;
                        string moduleContent = match.Groups[4].Value;
                        string content = match.Groups[5].Value;

                        // 解析时间戳和线程ID
                        if (DateTime.TryParse(timestampStr, out DateTime timestamp) && int.TryParse(threadIdStr, out int threadId))
                        {
                            // 进一步处理 moduleContent 以提取 Module 和 Method
                            string[] moduleParts = moduleContent.Split('.');
                            string module = moduleParts[0];
                            string method = moduleParts.Length > 1 ? moduleContent.Substring(module.Length + 1) : string.Empty;

                            // 处理 content 以提取 Version 和 真正的 content
                            string version = string.Empty;
                            string realContent = content;
                            // 假设版本号的格式为 x.x.x.x
                            Regex versionRegex = new Regex(@"\[(\d+(\.\d+){3})\]");
                            Match versionMatch = versionRegex.Match(content);
                            if (versionMatch.Success)
                            {
                                version = versionMatch.Groups[1].Value;
                                realContent = versionRegex.Replace(content, "").TrimEnd('|');
                            }

                            logEntries.Add(new LogEntry
                            {
                                Timestamp = timestamp,
                                Level = level,
                                ThreadId = threadId.ToString(),
                                Module = module,
                                Version = version,
                                Method = method,
                                Content = realContent
                            });
                        }

                    }
                }
            }
            logEntries = [.. logEntries.OrderBy(a => a.Timestamp)];
            CalculateDurations(logEntries);
            return logEntries;
        }

        public static void CalculateDurations(List<LogEntry> logEntries)
        {
            // 按线程ID和方法名称分组
            var groups = logEntries.GroupBy(le => new { le.ThreadId, le.Method });

            foreach (var group in groups)
            {
                var sortedLogs = group.OrderBy(le => le.Timestamp).ToList();
                ProcessServiceSegments(sortedLogs);
            }
        }

        private static void ProcessServiceSegments(List<LogEntry> logs)
        {
            int i = 0;
            while (i < logs.Count)
            {
                // 查找下一个 Service Start
                int startIndex = FindNextServiceStart(logs, i);
                if (startIndex == -1)
                    break; // 没有更多的 Service Start

                // 查找对应的闭合口
                int endIndex = FindServiceEnd(logs, startIndex);

                // 处理服务段
                ProcessSegment(logs, startIndex, endIndex);

                // 移动到下一个可能的 Service Start
                i = endIndex + 1;
            }
        }

        private static int FindNextServiceStart(List<LogEntry> logs, int startIndex)
        {
            for (int i = startIndex; i < logs.Count; i++)
            {
                if (logs[i].Content.Contains("Service Start"))
                    return i;
            }
            return -1; // 没有找到
        }

        private static int FindServiceEnd(List<LogEntry> logs, int startIndex)
        {
            // 查找下一个 Service Start 的上一行
            for (int i = startIndex + 1; i < logs.Count; i++)
            {
                if (logs[i].Content.Contains("Service Start"))
                    return i - 1; // 返回上一个日志作为闭合口
            }
            // 如果没有找到下一个 Service Start，则用最后一行作为闭合口
            return logs.Count - 1;
        }

        private static void ProcessSegment(List<LogEntry> logs, int startIndex, int endIndex)
        {
            DateTime startTime = logs[startIndex].Timestamp;
            DateTime prevTime = startTime;

            for (int i = startIndex; i <= endIndex; i++)
            {
                LogEntry currentLog = logs[i];
                if (i == startIndex)
                {
                    currentLog.Duration = 0;
                    currentLog.TotalDuration = 0;
                }
                else
                {
                    TimeSpan duration = currentLog.Timestamp - prevTime;
                    TimeSpan totalDuration = currentLog.Timestamp - startTime;
                    currentLog.Duration = (decimal)duration.TotalSeconds;
                    currentLog.TotalDuration = (decimal)totalDuration.TotalSeconds;
                    prevTime = currentLog.Timestamp;
                }
            }
        }

        public static Encoding DetectEncodingFromBom(string filePath)
        {
            // 读取文件的前4个字节
            byte[] bom = new byte[4];
            using (FileStream file = new(filePath, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // 检查UTF-8 BOM（EF BB BF）
            if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
            {
                return Encoding.UTF8;
            }
            // 检查UTF-32 Big Endian BOM（00 00 FE FF）
            else if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
            {
                return Encoding.UTF32;
            }
            // 检查UTF-32 Little Endian BOM（FF FE 00 00）
            else if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
            {
                return Encoding.GetEncoding(12000);
            }
            // 检查UTF-16 Big Endian BOM（FE FF）
            else if (bom[0] == 0xFE && bom[1] == 0xFF)
            {
                return Encoding.BigEndianUnicode;
            }
            // 检查UTF-16 Little Endian BOM（FF FE）
            else if (bom[0] == 0xFF && bom[1] == 0xFE)
            {
                return Encoding.Unicode;
            }

            // 如果没有检测到BOM，返回默认编码（这里假设为UTF-8）
            return Encoding.UTF8;
        }

        public T? GetPage<T>()
          where T : class
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException("The page should be a WPF control.");

            return (T?)_serviceProvider.GetService(typeof(T));
        }

        /// <inheritdoc />
        public FrameworkElement? GetPage(Type pageType)
        {
            if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
                throw new InvalidOperationException("The page should be a WPF control.");

            return _serviceProvider.GetService(pageType) as FrameworkElement;
        }
    }
}
