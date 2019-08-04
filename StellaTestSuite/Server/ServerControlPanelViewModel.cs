using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaServerLib.Animation;

namespace StellaTestSuite.Server
{
    public class ServerControlPanelViewModel
    {
        public List<Storyboard> Storyboards { get; private set; }

        public ServerControlPanelViewModel(List<Storyboard> storyboards)
        {
            Storyboards = storyboards;
        }
    }
}
