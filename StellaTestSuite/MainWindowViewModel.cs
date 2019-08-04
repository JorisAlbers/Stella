﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using StellaTestSuite.Client;
using StellaTestSuite.Model;
using StellaTestSuite.Model.Server;
using StellaTestSuite.Server;

namespace StellaTestSuite
{
    public class MainWindowViewModel
    {
        public ServerControlViewModel ServerViewModel { get; set; }

        public ClientViewerViewModel[] ClientViewModels { get; set; }

        private MemoryNetworkController _memoryNetworkController;

        public MainWindowViewModel()
        {
            _memoryNetworkController = new MemoryNetworkController(3, 1200, 20, 255);
            _memoryNetworkController.FrameSend += MemoryNetworkControllerOnFrameSend;

            ServerViewModel = new ServerControlViewModel(_memoryNetworkController);

            ClientViewModels = new ClientViewerViewModel[3];
            ClientViewModels[0] = new ClientViewerViewModel(1200);
            ClientViewModels[1] = new ClientViewerViewModel(1200);
            ClientViewModels[2] = new ClientViewerViewModel(1200);
        }

        private void MemoryNetworkControllerOnFrameSend(object sender, MessageSendEventArgs e)
        {
            // The server sends a frame to a client.
            // Update the GUI.

            ClientViewModels[e.ID].DrawFrame(e.frame);
        }
    }
}
