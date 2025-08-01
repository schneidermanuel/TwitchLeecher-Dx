using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class DownloadsView : ScrollingView
    {
        #region Constructors

        public DownloadsView() {
            InitializeComponent();
            SetupEventHandlers();
        }

        #endregion Constructors

        protected override ScrollViewer ScrollView => scrollview;
    }
}