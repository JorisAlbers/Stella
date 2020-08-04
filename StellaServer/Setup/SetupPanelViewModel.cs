﻿using System;
using System.Collections.Generic;
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
        [Reactive] public List<string> Errors { get; set; }
       
        public ReactiveCommand<Unit, Unit> StartCommand { get; }
        
        
        public SetupPanelViewModel()
        {
            var canStartServer = this.WhenAnyValue(
                x => x.ServerIp,
                x => x.ServerTcpPort,
                x => x.ServerUdpPort,
                x => x.MappingFilePath,
                x => x.BitmapFolder,
                (serverIp, tcp, udp, configFile, bitmapFolder) =>
                    !String.IsNullOrWhiteSpace(serverIp) &&
                    !String.IsNullOrWhiteSpace(MappingFilePath) &&
                    tcp != 0 &&
                    udp != 0 &&
                    !String.IsNullOrWhiteSpace(bitmapFolder) 
                );

            StartCommand = ReactiveCommand.Create(() =>
            {
                BitmapRepository bitmapRepository = new BitmapRepository(BitmapFolder);
                StellaServerLib.StellaServer stellaServer =
                    new StellaServerLib.StellaServer(MappingFilePath, ServerIp, ServerTcpPort, ServerUdpPort, 1, bitmapRepository, new Server());
                stellaServer.Start();
                


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
}