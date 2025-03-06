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
using System.Globalization;
using System.Windows.Data;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class LogViewModel : ObservableObject, INavigationAware
    {
        #region 属性
        // 日志相关
        [ObservableProperty]
        private List<LogEntry> _logs = [];
        [ObservableProperty]
        private List<LogEntry> _filteredLogs = [];
        private List<LogEntry>? _allLogs;
        private readonly LogService _logService;

        // 搜索相关
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
        [ObservableProperty]
        private List<string>? _searchMethods;

        // 分页相关
        private const int PageSize = 50; // 每页显示的日志条数
        [ObservableProperty]
        private int _currentPage = 1;
        [ObservableProperty]
        private int _totalPages = 1;
        [ObservableProperty]
        private int _totalCount = 1;
        #endregion

        #region 构造方法
        public LogViewModel(LogService logService)
        {
            _logService = logService;
        }
        #endregion

        #region 导航方法
        public void OnNavigatedTo()
        {

        }

        public void OnNavigatedFrom()
        {

        }
        #endregion

        #region 命令处理
        /// <summary>
        /// 加载日志
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private void LoadLogs(object parameter)
        {
            if (parameter is not string[] filePaths) return;

            try
            {
                _allLogs = LogService.ReadLogs(filePaths);
                ResetPagination();
                InitializeSearchMethods();
                UpdateViewData(_allLogs);
            }
            catch (Exception ex)
            {
                // 实际项目中应添加日志记录
                Debug.WriteLine($"加载日志失败: {ex.Message}");
            }
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private void Search(object parameter)
        {
            if (_allLogs == null) return;

            var filtered = ApplySearchFilters(_allLogs.AsQueryable());
            CurrentPage = 1;
            UpdateViewData(filtered.ToList());
        }
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private void PreviousPage(object parameter)
        {
            if (CurrentPage > 1) CurrentPage--;
            UpdatePagedLogs();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="parameter"></param>
        [RelayCommand]
        private void NextPage(object parameter)
        {
            if (CurrentPage < TotalPages) CurrentPage++;
            UpdatePagedLogs();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private IQueryable<LogEntry> ApplySearchFilters(IQueryable<LogEntry> query)
        {
            if (SearchStartDate.HasValue)
                query = query.Where(log => log.Timestamp >= SearchStartDate.Value);

            if (SearchEndDate.HasValue)
                query = query.Where(log => log.Timestamp <= SearchEndDate.Value);

            if (!string.IsNullOrEmpty(SearchThreadId))
                query = query.Where(log => log.ThreadId == SearchThreadId);

            if (!string.IsNullOrEmpty(SearchContent))
                query = query.Where(log => log.Content.Contains(SearchContent));

            if (!string.IsNullOrEmpty(SelectedSearchMethod))
                query = query.Where(log => log.Method == SelectedSearchMethod);

            return query;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="logs"></param>
        private void UpdateViewData(List<LogEntry> logs)
        {
            Logs = logs;
            UpdatePaginationValues();
            UpdatePagedLogs();
        }

        /// <summary>
        /// 更新分页信息
        /// </summary>
        private void UpdatePaginationValues()
        {
            TotalCount = Logs.Count;
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
            NotifyPaginationChanges();
        }

        private void ResetPagination()
        {
            CurrentPage = 1;
            UpdatePaginationValues();
        }

        private void NotifyPaginationChanges()
        {
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(TotalCount));
        }

        private void UpdatePagedLogs()
        {
            FilteredLogs = Logs?
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList() ?? [];
        }
        /// <summary>
        /// 初始化控件数据源
        /// </summary>
        private void InitializeSearchMethods()
        {
            SearchMethods = _allLogs?
                .Select(log => log.Method)
                .Distinct().Order()
                .ToList();
        }

        #endregion

    }
}
