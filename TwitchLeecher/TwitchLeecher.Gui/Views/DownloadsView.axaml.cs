using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class DownloadsView : UserControl
    {
        #region Constructors

        public DownloadsView() {
            InitializeComponent();
            ScrollPositionRestorer.Attach(this, Scroller);
        }

        #endregion Constructors
    }
}