using System.ComponentModel;
using System.Runtime.CompilerServices;
using StellaWebInterface.Annotations;
using DotNetify;

namespace StellaWebInterface.GlobalObjects
{
    public class Context : INotifyPropertyChanged
    {
        private static Context _instance;
        public Status Status { get; }

        private Context()
        {
            Status = new Status();
        }

        public static Context Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Context();
                }

                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
