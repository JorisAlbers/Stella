using System.Linq;
using System.Windows.Controls;
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
            if (false) // Toggle for the cloud setup
            {
                InitAsCloudSetup();
            }

            if (false) // Toggle for the Sterk setup
            {
                InitAsSterkSetup();
            }

            if (false) // Toggle for the Clud 2020 setup (6 rows, 4 tubes per row)
            {
                InitAsCloud2020Setup();
            }

            if (true) // 4 rows per pi, 2 columns per row
            {
                InitAsCloud2024Setup();
            }
        }

        private void InitAsCloud2024Setup()
        {
            // row = a line of tubes
            // column =  a tube on a line
            int pixelsPerColumn = 120;
            int columns = 2;
            int rows = 4;
            Orientation orientation = Orientation.Vertical;

            int pixelsPerRow = pixelsPerColumn * columns;
            int pixelsPerPi = pixelsPerRow * rows;

            
            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[1] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[2] = new MemoryLedStrip(pixelsPerPi);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, pixelsPerPi, 255);

            ServerViewModel = new ServerControlViewModel(_memoryNetworkController, pixelsPerRow, pixelsPerPi * 3);

            ClientViewerViewModel[] clientViewModels = Enumerable.Range(0, 3).Select(_ => new ClientViewerViewModel(pixelsPerPi, orientation, pixelsPerColumn, columns, rows)).ToArray();

            _memoryLedStrips[0].RenderRequested += (sender, colors) => clientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => clientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => clientViewModels[2].DrawFrame(colors);

            ClientsControlViewModel = new ClientsControlViewModel(clientViewModels, orientation);
        }

        private void InitAsCloud2020Setup()
        {
            // Start NetworkController which shortcuts the network connection between the server and each client
            int pixelsPerColumn = 120;
            int pixelsPerPi = 960;
            int pixelsPerRow = pixelsPerPi / 2;

            int columnsPerClient = 1;
            int rowsPerClient = 2;

            Orientation orientation = Orientation.Horizontal;


            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[1] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[2] = new MemoryLedStrip(pixelsPerPi);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, pixelsPerPi, 255);

            ServerViewModel = new ServerControlViewModel(_memoryNetworkController, pixelsPerRow, pixelsPerRow * 6);

            ClientViewerViewModel[] clientViewModels = Enumerable.Range(0,3).Select(_=> new ClientViewerViewModel(pixelsPerPi, orientation, pixelsPerColumn,columnsPerClient, rowsPerClient)).ToArray();

            _memoryLedStrips[0].RenderRequested += (sender, colors) => clientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => clientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => clientViewModels[2].DrawFrame(colors);
            
            ClientsControlViewModel = new ClientsControlViewModel(clientViewModels, orientation);
        }


        private void InitAsCloudSetup()
        {
            // Start NetworkController which shortcuts the network connection between the server and each client
            int pixelsPerColumn = 120;

            int pixelsPerPi = 1200;
            int pixelsPerRow = pixelsPerPi / 2;
            Orientation orientation = Orientation.Vertical;
            int columnsPerClient = 2;
            int rowsPerClient = 1;

            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[1] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[2] = new MemoryLedStrip(pixelsPerPi);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, pixelsPerPi, 255);

            ServerViewModel = new ServerControlViewModel(_memoryNetworkController, pixelsPerRow, pixelsPerRow * 6);


            ClientViewerViewModel[] clientViewModels = Enumerable.Range(0, 3).Select(_ => new ClientViewerViewModel(pixelsPerPi, orientation, pixelsPerColumn, columnsPerClient, rowsPerClient)).ToArray();


            _memoryLedStrips[0].RenderRequested += (sender, colors) => clientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => clientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => clientViewModels[2].DrawFrame(colors);

            ClientsControlViewModel = new ClientsControlViewModel(clientViewModels, orientation);
        }

        private void InitAsSterkSetup()
        {
            // Start NetworkController which shortcuts the network connection between the server and each client
            int pixelsPerColumn = 120;
            int pixelsPerPi  = 720;
            int pixelsPerRow = pixelsPerPi / 2;
            Orientation orientation = Orientation.Horizontal;
            int columnsPerClient = 1;
            int rowsPerClient = 2;


            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[1] = new MemoryLedStrip(pixelsPerPi);
            _memoryLedStrips[2] = new MemoryLedStrip(pixelsPerPi);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, pixelsPerPi, 255);

            ServerViewModel = new ServerControlViewModel(_memoryNetworkController, pixelsPerRow, pixelsPerRow * 6);

            ClientViewerViewModel[] clientViewModels = Enumerable.Range(0, 3).Select(_ => new ClientViewerViewModel(pixelsPerPi, orientation, pixelsPerColumn,columnsPerClient, rowsPerClient)).ToArray();


            _memoryLedStrips[0].RenderRequested += (sender, colors) => clientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => clientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => clientViewModels[2].DrawFrame(colors);

            ClientsControlViewModel = new ClientsControlViewModel(clientViewModels, orientation);
        }
    }
}
