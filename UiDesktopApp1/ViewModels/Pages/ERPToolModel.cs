using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiDesktopApp1.Services;
using Wpf.Ui.Controls;

namespace UiDesktopApp1.ViewModels.Pages
{
    public partial class ERPToolModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private List<string>? _serviceNames;
        private ERPToolService _ERPToolService;
        public ERPToolModel(ERPToolService ERPToolService)
        {
            _ERPToolService = ERPToolService;
            InitializeServiceNames();
        }

        #region 导航方法

        public void OnNavigatedTo()
        {
            throw new NotImplementedException();
        }
        public void OnNavigatedFrom()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化控件数据源
        /// </summary>
        private void InitializeServiceNames()
        {
            ServiceNames = _ERPToolService.GetServiceNames();
        }
        #endregion
    }
}
