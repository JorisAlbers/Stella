using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Midi;
using StellaServerLib;
using StellaServerLib.Network;
using StellaServerLib.Serialization.Mapping;

namespace StellaServer.Setup
{
    public class SetupPanelViewModel : ReactiveObject
    {
        [Reactive] public string ServerIp { get; set; }
        [Reactive] public int ServerTcpPort { get; set; }
        [Reactive] public int ServerUdpPort { get; set; }
        [Reactive] public int RemoteUdpPort { get; set; }
        [Reactive] public string MappingFilePath { get; set; }
        [Reactive] public string BitmapFolder { get; set; }                                                                                                                        
        [Reactive] public string StoryboardFolder { get; set; }                                                                                                                        
        [Reactive] public int MaximumFrameRate { get; set; }           
        [Reactive] public List<MidiWithIndex> MidiDevices { get; set; }
        [Reactive] public int SelectedMidiDevice { get; set; }
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
                RemoteUdpPort = settings.RemoteUdpPort;
                MappingFilePath = settings.MappingFilePath;
                BitmapFolder = settings.BitmapFolder;
                StoryboardFolder = settings.StoryboardFolder;
                MaximumFrameRate = settings.MaximumFrameRate;
            }

            var canStartServer = this.WhenAnyValue(
                x => x.ServerIp,
                x => x.ServerTcpPort,
                x => x.ServerUdpPort,
                x => x.RemoteUdpPort,
                x => x.MappingFilePath,
                x => x.BitmapFolder,
                x=> x.StoryboardFolder,
                x => x.MaximumFrameRate,
                (serverIp, tcp, udp, remoteUdpPort, configFile, bitmapFolder, storyboardFolder, maximumFrameRate) =>
                    !String.IsNullOrWhiteSpace(serverIp) &&
                    !String.IsNullOrWhiteSpace(MappingFilePath) &&
                    tcp != 0 &&
                    udp != 0 &&
                    remoteUdpPort != 0 &&
                    !String.IsNullOrWhiteSpace(bitmapFolder) &&
                    !String.IsNullOrWhiteSpace(storyboardFolder) &&
                    maximumFrameRate > 1 && maximumFrameRate < 1000
            );

            MidiDevices = GetMidiDevices();

            StartCommand = ReactiveCommand.Create(() =>
            {
               // Read mapping
                MappingLoader mappingLoader = new MappingLoader();
                using var reader = new StreamReader(MappingFilePath);
                var mapping = mappingLoader.Load(reader);


                BitmapRepository bitmapRepository = new BitmapRepository(new FileSystem(), BitmapFolder);

                string resizedRepositoryPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "StellaServer", "Bitmaps",
                        $"{mapping.Columns}");
                BitmapRepository resizedBitmapRepository = new BitmapRepository(new FileSystem(), resizedRepositoryPath);


                StellaServerLib.StellaServer stellaServer =
                    new StellaServerLib.StellaServer(ServerIp, ServerTcpPort, ServerUdpPort, RemoteUdpPort, 1, MaximumFrameRate, new Server());

                stellaServer.Start(mapping, resizedBitmapRepository);


                MidiInputManager midiInputManager = null;
                if (SelectedMidiDevice > 0)
                {
                    // the 0 index indicates no midi device should be used.
                    midiInputManager = new MidiInputManager(SelectedMidiDevice - 1);
                    midiInputManager.Start(stellaServer);
                }
                

                ServerCreated?.Invoke(this, new ServerCreatedEventArgs(new ServerSetupSettings()
                {
                    ServerIp = ServerIp,
                    ServerTcpPort = ServerTcpPort,
                    ServerUdpPort = ServerUdpPort,
                    RemoteUdpPort = RemoteUdpPort,
                    MappingFilePath = MappingFilePath,
                    BitmapFolder = BitmapFolder,
                    StoryboardFolder = StoryboardFolder,
                    MaximumFrameRate = MaximumFrameRate,
                }, 
                    stellaServer,
                    mapping,
                    midiInputManager,
                    bitmapRepository,
                    resizedBitmapRepository));
            }, canStartServer);

            StartCommand.ThrownExceptions.Subscribe(error => Errors = GetAllErrorMessages(error));
        }

        private List<MidiWithIndex> GetMidiDevices()
        {
            List<MidiWithIndex> devices = new List<MidiWithIndex>();
            devices.Add(null);
            devices.AddRange(MidiInputManager.GetDevices());
            return devices;
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
        public MappingLoader.Mapping Mapping { get; }
        public MidiInputManager MidiInputManager { get; }
        public BitmapRepository BitmapRepository { get; set; }
        public BitmapRepository ResizedBitmapRepository { get; set; }

        public ServerCreatedEventArgs(ServerSetupSettings settings, StellaServerLib.StellaServer stellaServer,
            MappingLoader.Mapping mapping,
            MidiInputManager midiInputManager, BitmapRepository bitmapRepository, BitmapRepository resizedBitmapRepository)
        {
            Settings = settings;
            StellaServer = stellaServer;
            Mapping = mapping;
            MidiInputManager = midiInputManager;
            BitmapRepository = bitmapRepository;
            ResizedBitmapRepository = resizedBitmapRepository;
        }
    }
}