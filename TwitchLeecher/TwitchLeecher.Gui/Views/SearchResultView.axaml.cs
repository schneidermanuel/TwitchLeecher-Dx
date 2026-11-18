using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class SearchResultView : UserControl
    {
        #region Constructors

        public SearchResultView() {
            InitializeComponent();
            ScrollPositionRestorer.Attach(this, Scroller);
        }

        #endregion Constructors
    }
}