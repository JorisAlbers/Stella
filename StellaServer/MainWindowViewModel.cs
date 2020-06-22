using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive] public AnimationsPanelViewModel AnimationsPanelViewModel { get; set; }

        public MainWindowViewModel(string bitmapFolder)
        {
            AnimationsPanelViewModel = new AnimationsPanelViewModel(bitmapFolder);
        }
    }
}
