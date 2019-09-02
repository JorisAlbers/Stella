using System.Windows.Controls;
using StellaVisualizer.Client;

namespace StellaVisualizer
{
    public class ClientsControlViewModel
    {
        public ClientViewerViewModel[] ClientViewModels { get; set; }

        public int GridColumns { get; set; }
        public int GridRows { get; set; }

        public ClientsControlViewModel(ClientViewerViewModel[] clientViewerViewModels, Orientation orientation)
        {
            ClientViewModels = clientViewerViewModels;
            if (orientation == Orientation.Horizontal)
            {
                GridColumns = 1;
                GridRows = 3;
            }
            else
            {
                GridColumns = 3;
                GridRows = 1;
            }
        }

    }
}