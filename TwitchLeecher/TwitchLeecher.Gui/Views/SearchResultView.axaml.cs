using Avalonia;
using Avalonia.Controls;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Gui.Views
{
    public partial class SearchResultView : UserControl
    {
        private INavigationState _state;
        
        #region Constructors

        public SearchResultView() {
            InitializeComponent();

            // old TL used the Loaded event, but now it results in a visible jump 
            AttachedToVisualTree += ViewAttached;
            scrollview.ScrollChanged += ScrollChanged;
        }

        #endregion Constructors

        #region EventHandlers

        private void ScrollChanged(object sender, ScrollChangedEventArgs eventargs) {
            if (_state != null) {
                _state.ScrollPosition = scrollview.Offset.Y;
            }
        }

        private void ViewAttached(object sender, EventArgs eventArgs) {
            _state = DataContext as INavigationState;
            
            if (_state != null) {
                scrollview.Offset = new Vector(0, _state.ScrollPosition);
            }
        }

        #endregion EventHandlers
    }
}