using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace StellaServer.Setup
{
    /// <summary>
    /// Interaction logic for SetupPanel.xaml
    /// </summary>
    public partial class SetupPanel : ReactiveUserControl<SetupPanelViewModel>
    {
        public SetupPanel()
        {
            InitializeComponent();
        }
    }

    public class SetupPanelViewModel : ReactiveObject
    {
    }
}
