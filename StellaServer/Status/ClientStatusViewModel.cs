using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Status
{
    public class ClientStatusViewModel : ReactiveObject

    {
    public string Name { get; }

    [Reactive] public bool IsConnected { get; set; }

    public ClientStatusViewModel(string name)
    {
        Name = name;
    }
    }
}