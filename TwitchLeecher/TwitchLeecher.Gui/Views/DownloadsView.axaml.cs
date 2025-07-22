using Avalonia;
using Avalonia.Controls;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Gui.Views
{
    public partial class DownloadsView : UserControl
    {
        #region Fields

        private INavigationState _state;

        #endregion Fields

        #region Constructors

        public DownloadsView() {
            InitializeComponent();
            
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
                // scrollview.Offset.X = _state.ScrollPosition;
                scrollview.Offset = new Vector(0, _state.ScrollPosition);
            }
        }

        #endregion EventHandlers
    }
}