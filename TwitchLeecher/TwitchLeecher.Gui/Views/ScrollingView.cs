using Avalonia;
using Avalonia.Controls;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Gui.Views;

public abstract class ScrollingView : UserControl {
    private INavigationState _state;

    /// Call this in the constructor of derived classes, after InitializeComponent()
    protected void SetupEventHandlers() {
        AttachedToVisualTree += ViewAttached;
        ScrollView.ScrollChanged += ScrollChanged;
    }

    protected abstract ScrollViewer ScrollView {
        get;
    }

    private void ViewAttached(object sender, EventArgs eventArgs) {
        _state = DataContext as INavigationState;
        
        if (_state != null) {
            ScrollView.Offset = new Vector(0, _state.ScrollPosition);
        }
    }

    private void ScrollChanged(object sender, ScrollChangedEventArgs eventArgs) {
        if (_state != null) {
            _state.ScrollPosition = ScrollView.Offset.Y;
        }
    }
}