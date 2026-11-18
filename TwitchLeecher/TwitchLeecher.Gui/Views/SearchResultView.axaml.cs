using System;
using Avalonia.Controls;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Gui.Views
{
    public partial class SearchResultView : UserControl
    {
        #region Fields

        private bool _restored;

        #endregion Fields

        #region Constructors

        public SearchResultView() {
            InitializeComponent();

            AttachedToVisualTree += (s, e) =>
            {
                _restored = false;
                Scroller.LayoutUpdated += RestoreScrollPosition;
            };

            Scroller.ScrollChanged += (s, e) =>
            {
                if (_restored && DataContext is INavigationState state)
                {
                    state.ScrollPosition = Scroller.Offset;
                }
            };
        }

        #endregion Constructors

        #region Methods

        // Runs once the results are laid out, so the saved offset is not clamped to an empty extent.
        private void RestoreScrollPosition(object sender, EventArgs e)
        {
            if (Scroller.Extent.Height <= 0)
            {
                return;
            }

            Scroller.LayoutUpdated -= RestoreScrollPosition;

            if (DataContext is INavigationState state)
            {
                Scroller.Offset = state.ScrollPosition;
            }

            _restored = true;
        }

        #endregion Methods
    }
}
