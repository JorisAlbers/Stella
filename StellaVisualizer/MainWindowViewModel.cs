﻿using System.Windows.Controls;
using StellaVisualizer.Client;
using StellaVisualizer.Model;
using StellaVisualizer.Model.Client;
using StellaVisualizer.Server;

namespace StellaVisualizer
{
    public class MainWindowViewModel
    {
        public ServerControlViewModel ServerViewModel { get; set; }

        public ClientsControlViewModel ClientsControlViewModel { get; set; }


        private MemoryNetworkController _memoryNetworkController;

        private MemoryLedStrip[] _memoryLedStrips;

        public MainWindowViewModel()
        {
            // Start NetworkController which shortcuts the network connection between the server and each client
            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(1200);
            _memoryLedStrips[1] = new MemoryLedStrip(1200);
            _memoryLedStrips[2] = new MemoryLedStrip(1200);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, 1200, 20, 255);
            
            ServerViewModel = new ServerControlViewModel(_memoryNetworkController);

            Orientation orientation = Orientation.Horizontal;


            ClientViewerViewModel[] clientViewModels = new ClientViewerViewModel[3];
            clientViewModels[0] = new ClientViewerViewModel(1200, orientation);
            clientViewModels[1] = new ClientViewerViewModel(1200, orientation);
            clientViewModels[2] = new ClientViewerViewModel(1200, orientation);

            _memoryLedStrips[0].RenderRequested += (sender, colors) => clientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => clientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => clientViewModels[2].DrawFrame(colors);

            ClientsControlViewModel = new ClientsControlViewModel(clientViewModels, orientation);
        }
    }
}
