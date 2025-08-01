using Avalonia.Controls;

namespace TwitchLeecher.Gui.Views
{
    public partial class SearchResultView : ScrollingView
    {
        #region Constructors

        public SearchResultView() {
            InitializeComponent();
            SetupEventHandlers();
        }

        #endregion Constructors

        protected override ScrollViewer ScrollView => scrollview;
    }
}