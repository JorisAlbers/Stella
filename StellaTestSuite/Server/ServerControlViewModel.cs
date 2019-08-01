using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellaTestSuite.Server
{
    public class ServerControlViewModel
    {
        public ServerConfigurationViewModel ServerConfigurationViewModel { get; set; }
        
        public ServerControlViewModel()
        {
            ServerConfigurationViewModel = new ServerConfigurationViewModel();
        }
    }
}
