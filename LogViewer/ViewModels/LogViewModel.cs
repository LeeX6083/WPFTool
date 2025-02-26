using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LogViewer.Models;
using LogViewer.Services;

namespace LogViewer.ViewModels
{
    public class LogViewModel : INotifyPropertyChanged
    {
        private List<LogEntry> _logs;
        private SeriesCollection _logLevelStatistics;
        private readonly LogService _logService;

        public LogViewModel()
        {
            _logService = new LogService();
            LoadLogsCommand = new RelayCommand(LoadLogs);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            NextPageCommand = new RelayCommand(NextPage);
        }

        public List<LogEntry> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                OnPropertyChanged();
                UpdateLogLevelStatistics(); // 更新统计图表
            }
        }
        public SeriesCollection LogLevelStatistics
        {
            get => _logLevelStatistics;
            set
            {
                _logLevelStatistics = value;
                OnPropertyChanged();
                // 输出调试信息，检查数据是否正确绑定
                Debug.WriteLine($"LogLevelStatistics updated: {_logLevelStatistics != null}");
            }
        }

        public ICommand LoadLogsCommand { get; }

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="parameter"></param>
        private void LoadLogs(object parameter)
        {
            var filePath = parameter as string;
            if (string.IsNullOrEmpty(filePath))
                return;

            _allLogs = _logService.ReadLogs(filePath);
            CurrentPage = 1; // 重置到第一页
            UpdatePagedLogs();
            UpdateLogLevelStatistics();

            // 通知 TotalPages 和 TotalCount 属性已经改变
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(TotalCount));
        }

        /// <summary>
        /// 加载饼图
        /// </summary>
        private void UpdateLogLevelStatistics()
        {
            LogLevelStatistics = new SeriesCollection(); // 初始化空集合
            if (Logs == null || !Logs.Any())
                return;

            var levelCounts = Logs
                .GroupBy(log => log.Level)
                .Select(group => new PieSeries
                {
                    Title = group.Key,
                    Values = new ChartValues<int> { group.Count() },
                    DataLabels = true
                }).ToList();
            LogLevelStatistics.AddRange(levelCounts);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region 分页
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        private const int PageSize = 50; // 每页显示的日志条数
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _totalCount = 1;
        private List<LogEntry> _allLogs;

        private void PreviousPage(object parameter)
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        private void NextPage(object parameter)
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        }
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdatePagedLogs();
            }
        }

        public int TotalPages => (_allLogs?.Count ?? 0) / PageSize + 1;
        public int TotalCount => (_allLogs?.Count ?? 0);

        private void UpdatePagedLogs()
        {
            if (_allLogs == null)
                return;

            Logs = _allLogs
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
        #endregion

    }
}