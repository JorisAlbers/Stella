using StellaVisualizer.Client;
using StellaVisualizer.Model;
using StellaVisualizer.Model.Client;
using StellaVisualizer.Server;

namespace StellaVisualizer
{
    public class MainWindowViewModel
    {
        public ServerControlViewModel ServerViewModel { get; set; }

        public ClientViewerViewModel[] ClientViewModels { get; set; }

        private MemoryNetworkController _memoryNetworkController;

        private MemoryLedStrip[] _memoryLedStrips;

        public MainWindowViewModel()
        {
            _memoryLedStrips = new MemoryLedStrip[3];
            _memoryLedStrips[0] = new MemoryLedStrip(1200);
            _memoryLedStrips[1] = new MemoryLedStrip(1200);
            _memoryLedStrips[2] = new MemoryLedStrip(1200);
            _memoryNetworkController = new MemoryNetworkController(_memoryLedStrips, 1200, 20, 255);
            ServerViewModel = new ServerControlViewModel(_memoryNetworkController);

            ClientViewModels = new ClientViewerViewModel[3];
            ClientViewModels[0] = new ClientViewerViewModel(1200);
            ClientViewModels[1] = new ClientViewerViewModel(1200);
            ClientViewModels[2] = new ClientViewerViewModel(1200);

            _memoryLedStrips[0].RenderRequested += (sender, colors) => ClientViewModels[0].DrawFrame(colors);
            _memoryLedStrips[1].RenderRequested += (sender, colors) => ClientViewModels[1].DrawFrame(colors);
            _memoryLedStrips[2].RenderRequested += (sender, colors) => ClientViewModels[2].DrawFrame(colors);
        }
    }
}
