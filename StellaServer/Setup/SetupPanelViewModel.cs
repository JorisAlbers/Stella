using System;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Network;

namespace StellaServer.Setup
{
    public class SetupPanelViewModel : ReactiveObject
    {
        [Reactive] public string ServerIp { get; set; }
        [Reactive] public int ServerTcpPort { get; set; }
        [Reactive] public int ServerUdpPort { get; set; }
        [Reactive] public string MappingFilePath { get; set; }
        [Reactive] public string ErrorText { get; set; }
       
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        
        
        public SetupPanelViewModel()
        {
            var canStartServer = this.WhenAnyValue(
                x => x.ServerIp,
                x => x.ServerTcpPort,
                x => x.ServerUdpPort,
                x => x.MappingFilePath,
                (serverIp, tcp, udp, configFile) =>
                    !String.IsNullOrWhiteSpace(serverIp) &&
                    !String.IsNullOrWhiteSpace(MappingFilePath) &&
                    tcp != 0 &&
                    udp != 0
                );

            StartCommand = ReactiveCommand.Create(() =>
            {
                BitmapRepository bitmapRepository = new BitmapRepository("fake_path");
                StellaServerLib.StellaServer stellaServer =
                    new StellaServerLib.StellaServer(MappingFilePath, ServerIp, ServerTcpPort, ServerUdpPort, 1, bitmapRepository, new Server());
                stellaServer.Start();
            }, canStartServer);

            StartCommand.ThrownExceptions.Subscribe(error => ErrorText = $"Failed to start server: {error.Message}");
        }
    }
}