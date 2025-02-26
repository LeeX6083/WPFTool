using LogViewer.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace LogViewer.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 设置 DataContext 为 LogViewModel 的实例
            DataContext = new LogViewModel();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*",
                Title = "选择日志"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                //FilePathTextBox.Text = openFileDialog.FileName;
                // 获取 LogViewModel 实例
                var viewModel = DataContext as LogViewModel;
                if (viewModel != null)
                {
                    // 调用 LoadLogs 方法并传入文件路径
                    viewModel.LoadLogsCommand.Execute(openFileDialog.FileName);
                }
            }
        }
    }
}
