using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class PreferencesView : UserControl
    {
        public PreferencesView()
        {
            InitializeComponent();
            ScrollPositionRestorer.Attach(this, Scroller);
        }
    }
}