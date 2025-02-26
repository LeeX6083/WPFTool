using LogViewerTool.Models;
using LogViewerTool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewerTool.ViewModels
{
    public class LogViewModel : INotifyPropertyChanged
    {
        private List<LogEntry> _logs = [];
        public List<LogEntry> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
            }
        }

        public LogViewModel() => LoadLogs();

        private void LoadLogs()
        {
            var logService = new LogService();
            Logs = logService.GetLogs("path/to/logfile.log");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
