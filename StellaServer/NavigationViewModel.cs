using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using ReactiveUI;

namespace StellaServer
{
    public class NavigationViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> NavigateToCreateAnimation { get; } = ReactiveCommand.Create(() => { });

        public NavigationViewModel()
        {
        }
    }
}
