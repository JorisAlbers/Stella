using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaServer.Animation;


namespace StellaVisualizer.ViewModels
{
    /// <summary>
    /// Window for a new animation
    /// </summary>
    public class NewAnimationWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The draw methods available
        /// </summary>
        public string[] DrawMethods { get; } = Enum.GetNames(typeof(DrawMethod));

        /// <summary>
        /// The selected draw method
        /// </summary>
        public string SelectedDrawMethod { get; set; } = Enum.GetName(typeof(DrawMethod), DrawMethod.Unknown);

        /// <summary>
        /// The number of milliseconds each frame will be visible for.
        /// </summary>
        public int WaitMS { get; set; } = 100;

        private PatternViewModel _previouslySelectedPatternViewModel;

        private PatternViewModel _selectedPatternViewModel;
        public PatternViewModel SelectedPatternViewModel
        {
            get => _selectedPatternViewModel;
            set
            {
                if (_previouslySelectedPatternViewModel != null)
                {
                    _previouslySelectedPatternViewModel.IsSelected = false;
                }

                _selectedPatternViewModel = value;
                _previouslySelectedPatternViewModel = value;
            }
        }

        public ObservableCollection<PatternViewModel> PatternViewModels { get; } = new ObservableCollection<PatternViewModel>
        {
            new PatternViewModel(100,100,100)
        };


        public NewAnimationWindowViewModel()
        {
            
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
