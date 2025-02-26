using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UiDesktopApp1.Models;
using UiDesktopApp1.Services;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Controls;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class LogViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private List<LogEntry> _logs = [];
        [ObservableProperty]
        private List<LogEntry> _filteredLogs = [];
        //private SeriesCollection _logLevelStatistics;
        private readonly LogService _logService;

        public LogViewModel(LogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 读取日志
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private void LoadLogs(object parameter)
        {
            if (parameter is not string[] filePaths)
                return;

            _allLogs = LogService.ReadLogs(filePaths);
            Logs = _allLogs;
            CurrentPage = 1; // 重置到第一页
            TotalCount = _allLogs.Count;
            TotalPages = _allLogs.Count / PageSize + 1;
            UpdatePagedLogs();
            InitCheckBox();

            // 通知 TotalPages 和 TotalCount 属性已经改变
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(TotalCount));
        }
        [ObservableProperty]
        private string _selectedSearchMethod = string.Empty;
        [ObservableProperty]
        private DateTime? _searchStartDate;
        [ObservableProperty]
        private DateTime? _searchEndDate;
        [ObservableProperty]
        private string _searchThreadId = string.Empty;
        [ObservableProperty]
        private string _searchContent = string.Empty;
        [RelayCommand]
        private void Search(object parameter)
        {
            if (_allLogs is null)
                return;
            var query = _allLogs.AsQueryable();

            if (SearchStartDate.HasValue)
            {
                query = query.Where(log => log.Timestamp >= SearchStartDate.Value);
            }
            if (SearchEndDate.HasValue)
            {
                query = query.Where(log => log.Timestamp <= SearchEndDate.Value);
            }
            if (!string.IsNullOrEmpty(SearchThreadId))
            {
                query = query.Where(log => log.ThreadId == SearchThreadId);
            }
            if (!string.IsNullOrEmpty(SearchContent))
            {
                query = query.Where(log => log.Content.Contains(SearchContent));
            }
            if (!string.IsNullOrEmpty(SelectedSearchMethod))
            {
                query = query.Where(log => log.Method == SelectedSearchMethod);
            }

            Logs = new List<LogEntry>([.. query]);
            TotalCount = Logs.Count;
            TotalPages = Logs.Count / PageSize + 1;
            UpdatePagedLogs();
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(TotalCount));
        }
        [ObservableProperty]
        private List<string>? _searchMethods;
        private void InitCheckBox()
        {
            if (_allLogs != null)
            {
                SearchMethods = _allLogs.Select(fl => fl.Method).Distinct().ToList();
                OnPropertyChanged(nameof(SearchMethods));
            }
        }

        #region 分页
        private const int PageSize = 50; // 每页显示的日志条数
        [ObservableProperty]
        private int _currentPage = 1;
        [ObservableProperty]
        private int _totalPages = 1;
        [ObservableProperty]
        private int _totalCount = 1;
        private List<LogEntry>? _allLogs;

        [RelayCommand]
        private void PreviousPage(object parameter)
        {
            if (CurrentPage > 1)
                CurrentPage--;
            UpdatePagedLogs();
        }

        [RelayCommand]
        private void NextPage(object parameter)
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
            UpdatePagedLogs();
        }


        private void UpdatePagedLogs()
        {
            if (Logs == null)
                return;

            FilteredLogs = Logs
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            //FilteredLogs = Logs;
        }

        public void OnNavigatedTo()
        {

        }

        public void OnNavigatedFrom()
        {

        }
        #endregion

    }
}
