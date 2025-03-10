using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;
using WpfTool.LogViewerModule.Services;
using WpfTool.LogViewerModule.ViewModels.Pages;
using WpfTool.SharedInterfaces.Models;
using WpfTool.SharedInterfaces.Services;

namespace WpfTool.LogViewerModule.Views.Pages
{
    /// <summary>
    /// LogViewerPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogViewerPage : INavigableView<LogViewModel>
    {
        public LogViewModel ViewModel { get; }
        public LogViewerPage(ILogService logService)
        {
            ViewModel = new LogViewModel(logService);
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*",
                Title = "选择日志",
                // 设置支持多选文件
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                //FilePathTextBox.Text = openFileDialog.FileName;
                // 获取 LogViewModel 实例
                var viewModel = DataContext as LogViewModel;
                if (viewModel != null)
                {
                    // 调用 LoadLogs 方法并传入文件路径
                    viewModel.LoadLogsCommand.Execute(openFileDialog.FileNames);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取所属的窗口
            var window = Window.GetWindow(this);
            gridLog.MaxHeight = window.ActualHeight * 0.6;
            if (window != null)
            {
                // 监听窗口的 SizeChanged 事件
                window.SizeChanged += Window_SizeChanged;
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 设置 DataGrid 的最大高度为窗口高度的 80%
            var window = sender as Window;
            if (window != null)
            {
                gridLog.MaxHeight = window.ActualHeight * 0.6;
            }
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 获取双击的单元格
            var cell = sender as DataGridCell;
            if (cell != null)
            {
                // 获取单元格的数据上下文
                var dataContext = cell.DataContext as LogEntry;
                if (dataContext != null)
                {
                    // 打开新窗口显示完整内容
                    var popupWindow = new ContentPage(dataContext.Content);
                    popupWindow.Show();
                }
            }
        }

        private void gridLog_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            // 获取当前行的索引（从0开始）
            int rowIndex = e.Row.GetIndex();

            // 设置行号（从1开始）
            e.Row.Header = (rowIndex + 1).ToString();
        }
    }
}
