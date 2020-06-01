using IHM.ModelNameSpace;
using IHM.ObserverNameSpace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace IHM.ViewModelNameSpace
{
    class ViewModel : INotifyPropertyChanged, IObserver
    {
        private bool _isConnected = false;

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                if (value == _isConnected) return;
                _isConnected = value;
                onPropertyChangedAsync("IsConnected");
            }
        }

        public ObservableCollection<Backup> BackupList { get; set; }
        public ConnectCommand ConnectCommand { get; set; }
        public DisconnectCommand DisconnectCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public Client client;
        public Thread clientThread;

        public ViewModel()
        {
            this.BackupList = new ObservableCollection<Backup>();

            this.ConnectCommand = new ConnectCommand(this);
            this.DisconnectCommand = new DisconnectCommand(this);
 
            client = new Client(this);
            client.attach(this);
                
        }

        //start the startClient method on a thread
        public void connect(string ip)
        {
            clientThread = new Thread(() => client.startClient(ip));
            clientThread.Start();
        }

        //this method uses a dispatcher because the backup list is updated from another thread
        public async void disconnect()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.BackupList.Clear();
            });

            this.client.disconnect();
        }

        //this method uses a dispatcher because the backup list is updated from another thread
        public async void update(IObservable observable, List<Backup> allBackup)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //clear the list of existing backup to updated it
                this.BackupList.Clear();
                
                //add all the backup on the observable collection to display it on the interface
                foreach (Backup element in allBackup)
                {
                    this.BackupList.Add(element);
                }

            });
        }

        //this method uses a dispatcher because the progress of the backup is updated from another thread
        public async void update(IObservable observable, Backup oneBackup)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //update the progress of the backup we just received an update on
                foreach (Backup element in BackupList)
                {
                    if (element.Name == oneBackup.Name)
                    {
                        element.Progress = oneBackup.Progress;
                        element.CurrentFile = oneBackup.CurrentFile;
                    }
                }

            });
        }

        //this method uses a dispatcher because the property is updated from another thread
        public async void onPropertyChangedAsync(string propertyName)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }
    
    }
}
