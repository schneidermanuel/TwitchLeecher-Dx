using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class PreferencesView : ScrollingView
    {
        public PreferencesView()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        protected override ScrollViewer ScrollView => scrollview;
    }
}