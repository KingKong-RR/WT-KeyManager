using System;
using System.ComponentModel;

namespace KeyManager.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            // note: the PropertyChanged event is not null, if at least one subscription has been made
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        // async invoke
        public void DispatcherBeginInvoke(Action action)
        {
            System.Windows.Application.Current?.Dispatcher.BeginInvoke(action);
        }

        public void DispatcherInvoke(Action action)
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(action);
        }

    }
}
