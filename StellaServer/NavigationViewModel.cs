using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows.Input;
using ReactiveUI;

namespace StellaServer
{
    public class NavigationViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> NavigateToCreateAnimation { get; } = ReactiveCommand.Create(() => { });
        public ReactiveCommand<Unit, Unit> NavigateToMidiPanel { get; } = ReactiveCommand.Create(() => { });

        public NavigationViewModel()
        {
        }
    }
}
