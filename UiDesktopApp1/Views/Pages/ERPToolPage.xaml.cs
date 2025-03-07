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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UiDesktopApp1.Services;
using UiDesktopApp1.ViewModels.Pages;

namespace UiDesktopApp1.Views.Pages
{
    /// <summary>
    /// ERPToolPage.xaml 的交互逻辑
    /// </summary>
    public partial class ERPToolPage : Page
    {
        public ERPToolModel ViewModel { get; }
        public ERPToolPage(ERPToolService ERPToolService)
        {
            ViewModel = new ERPToolModel(ERPToolService);
            InitializeComponent();
        }

        private void BtnSendRequest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnStartListen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnStopListen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
