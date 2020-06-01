using System.ComponentModel;

namespace IHM.ModelNameSpace
{
    //this class is used to deserialize the received data objet and use it on the view model
    class Backup : INotifyPropertyChanged
    {
        private int _progress = 0;
        private string _currentFile = "";

        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public string Target { get; set; } = "";
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (value == _progress) return;
                _progress = value;
                onPropertyChanged("Progress");
            }
        }
        public string CurrentFile
        {
            get
            {
                return _currentFile;
            }
            set
            {
                if (value == _currentFile) return;
                _currentFile = value;
                onPropertyChanged("CurrentFile");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Backup()
        {

        }

        //notifies a client that a static property value has changed
        public void onPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
