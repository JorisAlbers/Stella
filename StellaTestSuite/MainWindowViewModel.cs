using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaTestSuite.Client;
using StellaTestSuite.Server;

namespace StellaTestSuite
{
    public class MainWindowViewModel
    {
        public ServerControlViewModel ServerViewModel { get; set; }

        public ClientViewerViewModel Client1 { get; set; }
        public ClientViewerViewModel Client2 { get; set; }
        public ClientViewerViewModel Client3 { get; set; }

        public MainWindowViewModel()
        {
            ServerViewModel = new ServerControlViewModel();
            Client1 = new ClientViewerViewModel(1200);
            Client2 = new ClientViewerViewModel(1200);
            Client3 = new ClientViewerViewModel(1200);
        }
    }
}
