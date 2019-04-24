using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaServer.Animation;


namespace StellaVisualizer.ViewModels
{
    /// <summary>
    /// Window for a new animation
    /// </summary>
    public class NewAnimationWindowViewModel
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

        public ObservableCollection<PatternViewModel> PatternViewModels { get; } = new ObservableCollection<PatternViewModel>
        {
            new PatternViewModel(100,100,100)
        };


        public NewAnimationWindowViewModel()
        {
            
        }


    }
}
