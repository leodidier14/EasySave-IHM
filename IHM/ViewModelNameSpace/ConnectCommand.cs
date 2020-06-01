using System;
using System.Windows.Input;

namespace IHM.ViewModelNameSpace
{
    class ConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public ViewModel ViewModel { get; set; }

        public ConnectCommand(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.ViewModel.connect(parameter as string);
        }
    }
}
