namespace WpfTool.MainApp.Models
{
    public class AppConfig
    {
        public string ConfigurationsFolder { get; set; } = string.Empty;

        public string AppPropertiesFileName { get; set; } = string.Empty;

        /// <summary>
        /// 数据库文件路径
        /// </summary>
        public string DatabaseFilePath { get; set; } = string.Empty; 
    }
}
