using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace TwitchLeecher.Gui.Views
{
    /// <summary>
    /// Set <c>views:WindowDrag.IsHandle="True"</c> on a control to let the user move its window by dragging it.
    /// Needed where the window's content covers the system title bar.
    /// </summary>
    public class WindowDrag
    {
        public static readonly AttachedProperty<bool> IsHandleProperty =
            AvaloniaProperty.RegisterAttached<WindowDrag, Control, bool>("IsHandle");

        static WindowDrag()
        {
            IsHandleProperty.Changed.AddClassHandler<Control>((handle, e) =>
            {
                handle.PointerPressed -= OnPointerPressed;

                if (e.NewValue is true)
                {
                    handle.PointerPressed += OnPointerPressed;
                }
            });
        }

        public static bool GetIsHandle(Control control) => control.GetValue(IsHandleProperty);

        public static void SetIsHandle(Control control, bool value) => control.SetValue(IsHandleProperty, value);

        private static void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is not Control handle || !e.GetCurrentPoint(handle).Properties.IsLeftButtonPressed)
            {
                return;
            }

            // Presses on controls inside the handle keep their normal behaviour.
            for (Visual v = e.Source as Visual; v != null && v != handle; v = v.GetVisualParent())
            {
                if (v is Button or TextBox or ScrollBar)
                {
                    return;
                }
            }

            if (TopLevel.GetTopLevel(handle) is Window window)
            {
                window.BeginMoveDrag(e);
                e.Handled = true;
            }
        }
    }
}
