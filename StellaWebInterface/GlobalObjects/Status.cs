using System.ComponentModel;
using System.Runtime.CompilerServices;
using StellaWebInterface.Annotations;

namespace StellaWebInterface.GlobalObjects
{
    public class Status : INotifyPropertyChanged
    {
        private int _connectedRaspberries;
        public int ConnectedRaspberries
        {
            get => _connectedRaspberries;
            set
            {
                _connectedRaspberries = value;
                OnPropertyChanged();
            }
        }

        private int _connectedClients;
        public int ConnectedClients
        {
            get => _connectedClients;
            set
            {
                _connectedClients = value;
                OnPropertyChanged();
            }
        }

        public Status()
        {
            _connectedClients = 0;
            _connectedRaspberries = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
