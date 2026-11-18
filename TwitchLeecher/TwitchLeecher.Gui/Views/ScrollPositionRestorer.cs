using System;
using Avalonia.Controls;
using TwitchLeecher.Gui.Interfaces;

namespace TwitchLeecher.Gui.Views
{
    /// <summary>
    /// Keeps a ScrollViewer's offset in the view model's <see cref="INavigationState"/>.
    /// A two-way Offset binding fires before the content is laid out and snaps to the top,
    /// so the saved offset is restored after the first layout instead.
    /// </summary>
    internal static class ScrollPositionRestorer
    {
        public static void Attach(Control view, ScrollViewer scroller)
        {
            bool restored = false;

            void Restore(object sender, EventArgs e)
            {
                // shortcut: content that never gets a height is never restored, fine for these pages
                if (scroller.Extent.Height <= 0)
                {
                    return;
                }

                scroller.LayoutUpdated -= Restore;

                if (view.DataContext is INavigationState state)
                {
                    scroller.Offset = state.ScrollPosition;
                }

                restored = true;
            }

            view.AttachedToVisualTree += (s, e) =>
            {
                restored = false;
                scroller.LayoutUpdated += Restore;
            };

            scroller.ScrollChanged += (s, e) =>
            {
                if (restored && view.DataContext is INavigationState state)
                {
                    state.ScrollPosition = scroller.Offset;
                }
            };
        }
    }
}
