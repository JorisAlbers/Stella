using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace StellaVisualizer.Server
{
    /// <summary>
    /// Interaction logic for ServerControlPanelControl.xaml
    /// </summary>
    public partial class ServerControlPanelControl : UserControl
    {
        private SolidColorBrush brush1 = new SolidColorBrush(Colors.Red);
        private SolidColorBrush brush2 = new SolidColorBrush(Colors.Black);

        public ServerControlPanelControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var viewmodel = ((ServerControlPanelViewModel)e.NewValue).BpmViewModel;

                viewmodel.PropertyChanged += (o, args) =>
                {
                    if (args.PropertyName == nameof(BpmViewModel.AnimationToggle))
                    {
                        SolidColorBrush brush;
                        if (viewmodel.AnimationToggle)
                        {
                            brush = brush1;
                        }
                        else
                        {
                            brush = brush2;
                        }

                        Dispatcher.Invoke(() =>
                        {
                            TheBpmGrid.Background = brush;
                        });
                    }
                };

            }
        }


        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ServerControlPanelViewModel viewmodel = DataContext as ServerControlPanelViewModel;

            if (viewmodel.IsPaused)
            {
                PauseButton.Content = "Pause";
                
            }
            else
            {
                PauseButton.Content = "Continue";
            }

            viewmodel.IsPaused = !viewmodel.IsPaused;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ServerControlPanelViewModel viewmodel = DataContext as ServerControlPanelViewModel;
            viewmodel.BpmViewModel.OnNextBeat();

        }

        private bool _bpmTransformationIsRunning;

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            ServerControlPanelViewModel viewmodel = DataContext as ServerControlPanelViewModel;
            if (_bpmTransformationIsRunning)
            {
                viewmodel.BpmViewModel.Stop();
                ToggleBpmTransformationButton.Content = "Start";
                _bpmTransformationIsRunning = false;
                return;
            }

            viewmodel.BpmViewModel.Start();
            ToggleBpmTransformationButton.Content = "Stop";
            _bpmTransformationIsRunning = true;
        }
    }
}
