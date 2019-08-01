using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaTestSuite.Server;

namespace StellaTestSuite
{
    public class MainWindowViewModel
    {
        public ServerControlViewModel ServerViewModel { get; set; }

        public MainWindowViewModel()
        {
            ServerViewModel = new ServerControlViewModel();
        }
    }
}
