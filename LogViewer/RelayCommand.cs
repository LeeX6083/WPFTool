using System;
using System.Windows.Input;

namespace LogViewer
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute; // 执行命令的逻辑
        private readonly Func<object, bool> _canExecute; // 判断命令是否可以执行的逻辑

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execute">命令的执行逻辑</param>
        /// <param name="canExecute">命令的可执行条件（可选）</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// 判断命令是否可以执行
        /// </summary>
        /// <param name="parameter">命令参数</param>
        /// <returns>是否可以执行</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// 当命令的可执行状态发生变化时触发
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}