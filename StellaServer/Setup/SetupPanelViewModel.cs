using System;
using System.Collections.Generic;
using System.IO.Abstractions;
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
        [Reactive] public string BitmapFolder { get; set; }                                                                                                                        
        [Reactive] public string StoryboardFolder { get; set; }                                                                                                                        
        [Reactive] public int MaximumFrameRate { get; set; }                                                                                                                        
        [Reactive] public List<string> Errors { get; set; }

        public EventHandler<ServerCreatedEventArgs> ServerCreated;
       
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        
        
        public SetupPanelViewModel(ServerSetupSettings settings)
        {
            if (settings != null)
            {
                ServerIp = settings.ServerIp;
                ServerTcpPort = settings.ServerTcpPort;
                ServerUdpPort = settings.ServerUdpPort;
                MappingFilePath = settings.MappingFilePath;
                BitmapFolder = settings.BitmapFolder;
                StoryboardFolder = settings.StoryboardFolder;
                MaximumFrameRate = settings.MaximumFrameRate;
            }

            var canStartServer = this.WhenAnyValue(
                x => x.ServerIp,
                x => x.ServerTcpPort,
                x => x.ServerUdpPort,
                x => x.MappingFilePath,
                x => x.BitmapFolder,
                x=> x.StoryboardFolder,
                x => x.MaximumFrameRate,
                (serverIp, tcp, udp, configFile, bitmapFolder, storyboardFolder, maximumFrameRate) =>
                    !String.IsNullOrWhiteSpace(serverIp) &&
                    !String.IsNullOrWhiteSpace(MappingFilePath) &&
                    tcp != 0 &&
                    udp != 0 &&
                    !String.IsNullOrWhiteSpace(bitmapFolder) &&
                    !String.IsNullOrWhiteSpace(storyboardFolder) &&
                    maximumFrameRate > 1 && maximumFrameRate < 1000
            );

            StartCommand = ReactiveCommand.Create(() =>
            {
                BitmapRepository bitmapRepository = new BitmapRepository(new FileSystem(),BitmapFolder);
                StellaServerLib.StellaServer stellaServer =
                    new StellaServerLib.StellaServer(MappingFilePath, ServerIp, ServerTcpPort, ServerUdpPort, 1, MaximumFrameRate, bitmapRepository, new Server());
                stellaServer.Start();
                
                ServerCreated?.Invoke(this, new ServerCreatedEventArgs(new ServerSetupSettings()
                {
                    ServerIp = ServerIp,
                    ServerTcpPort = ServerTcpPort,
                    ServerUdpPort = ServerUdpPort,
                    MappingFilePath = MappingFilePath,
                    BitmapFolder = BitmapFolder,
                    StoryboardFolder = StoryboardFolder,
                    MaximumFrameRate = MaximumFrameRate,
                }, stellaServer));
            }, canStartServer);

            StartCommand.ThrownExceptions.Subscribe(error => Errors = GetAllErrorMessages(error));
        }

        private List<string> GetAllErrorMessages(Exception e)
        {
            List<string> errorMessages = new List<string>();
            do
            {
                errorMessages.Add(e.Message);
                e = e.InnerException;
            } while (e != null);

            return errorMessages;
        }
    }

    public class ServerCreatedEventArgs: EventArgs
    {
        public ServerSetupSettings Settings { get; }
        public StellaServerLib.StellaServer StellaServer { get; }

        public ServerCreatedEventArgs(ServerSetupSettings settings, StellaServerLib.StellaServer stellaServer)
        {
            Settings = settings;
            StellaServer = stellaServer;
        }
    }
}