using System.ComponentModel;
using DotNetify;
using StellaWebInterface.GlobalObjects;

namespace StellaWebInterface.Controllers
{
    public class StatusBlock : BaseVM
    {
        public int ConnectedClients
        {
            get => _context.Status.ConnectedClients;
            set => _context.Status.ConnectedClients = value;
        }

        public int ConnectedRaspberries
        {
            get => _context.Status.ConnectedRaspberries;
            set => _context.Status.ConnectedRaspberries = value;
        }

        private Context _context;

        public StatusBlock()
        {
            _context = Context.Instance;
            _context.Status.PropertyChanged += ContextOnPropertyChanged;
        }

        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Status.ConnectedRaspberries):
                    Changed(nameof(ConnectedRaspberries));
                    break;
                case nameof(Status.ConnectedClients):
                    Changed(nameof(ConnectedClients));
                    break;
            }

            PushUpdates();
        }
    }
}