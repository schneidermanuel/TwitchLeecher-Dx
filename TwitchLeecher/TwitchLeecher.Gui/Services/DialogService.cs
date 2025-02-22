using Ninject;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using TwitchLeecher.Core.Models;
using TwitchLeecher.Gui.Interfaces;
using TwitchLeecher.Gui.Types;
using TwitchLeecher.Gui.ViewModels;
using TwitchLeecher.Gui.Views;
using TwitchLeecher.Services.Interfaces;

namespace TwitchLeecher.Gui.Services
{
    internal class DialogService : IDialogService
    {
        #region Fields

        private readonly IKernel _kernel;
        private readonly ILogService _logService;

        private bool _busy;

        #endregion Fields

        #region Constructor

        public DialogService(IKernel kernel, ILogService logService)
        {
            _kernel = kernel;
            _logService = logService;
        }

        #endregion Constructor

        #region Methods

        public async Task<MessageBoxResult> ShowMessageBox(string message, string caption, MessageBoxButton buttons,
            MessageBoxImage icon)
        {
            MessageBoxWindow msg = new MessageBoxWindow();
            var vm = new MessageBoxViewModel(res => msg.Close(res))
            {
                Caption = caption,
                Message = message
            };
            vm.SetIcon(icon);
            vm.SetButtons(buttons);
            msg.DataContext = vm;
            var result =
                await Dispatcher.UIThread.InvokeAsync(() =>
                    msg.ShowDialog<MessageBoxResult>(_kernel.Get<MainWindow>()));

            return result;
        }

        public void ShowAndLogException(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            string logFile = _logService.LogException(ex);
        }

        public async void ShowFolderBrowserDialog(string folder, Action<bool, string> dialogCompleteCallback)
        {
            var window = _kernel.Get<MainWindow>();
            var picker = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                AllowMultiple = false
            });
            if (picker.Any())
            {
                var uri = picker.First().Path;
                var rawPath = uri.OriginalString;
                var decodedPath = Uri.UnescapeDataString(rawPath);

                if (decodedPath.StartsWith("file://"))
                {
                    decodedPath = decodedPath.Substring(7); // Besser als Replace
                }

                if (OperatingSystem.IsWindows())
                {
                    decodedPath = decodedPath.Replace("/", "\\");

                    if (decodedPath.Length >= 2 && decodedPath[1] == ':')
                    {
                        if (decodedPath[2] != '\\')
                        {
                            decodedPath = decodedPath.Substring(0, 2) + '\\' + decodedPath.Substring(3);
                        }
                    }
                }

                dialogCompleteCallback(false, decodedPath);
                return;
            }

            dialogCompleteCallback(true, null);
        }

        private static string GetFullPath(string rawPath)
        {
            string decodedPath;
            decodedPath = DecodedPath(rawPath);

            var fullPath = Path.GetFullPath(decodedPath);
            return fullPath;
        }

        private static string DecodedPath(string rawPath)
        {
            if (Uri.IsWellFormedUriString(rawPath, UriKind.Absolute))
            {
                var uri = new Uri(rawPath);
                if (uri.IsFile)
                {
                    return uri.LocalPath;
                }

                throw new InvalidOperationException("Unsupported URI scheme: " + uri.Scheme);
            }

            return rawPath;
        }

        public async void ShowFileBrowserDialog(CommonFileDialogFilter filter, string folder,
            Action<bool, string> dialogCompleteCallback)
        {
            var window = _kernel.Get<MainWindow>();
            var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation =
                    await window.StorageProvider.TryGetFolderFromPathAsync(folder),
                FileTypeFilter = new[]
                    { new FilePickerFileType(filter.Name) { Patterns = new[] { $"*.{filter.Extension}" } } }
            });
            if (result.Any())
            {
                dialogCompleteCallback(true, result.First().Path.AbsolutePath);
                return;
            }

            dialogCompleteCallback(false, null);
        }

        public void ShowUpdateInfoDialog(UpdateInfo updateInfo)
        {
            UpdateInfoViewModel model = _kernel.Get<UpdateInfoViewModel>();
            var mainWindow = _kernel.Get<MainWindow>();
            model.UpdateInfo = updateInfo;

            UpdateInfoView view = new UpdateInfoView
            {
                DataContext = model
            };

            view.ShowDialog(mainWindow);
        }

        public void SetBusy()
        {
            SetBusy(true);
        }

        private void SetBusy(bool busy)
        {
            if (_busy != busy)
            {
                _busy = busy;

                var window = _kernel.Get<MainWindow>();
                window.Cursor = busy ? new Cursor(StandardCursorType.Wait) : Cursor.Default;
                if (_busy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle,
                        DispatcherTimer_Tick);
                }
            }
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            if (sender is DispatcherTimer dispatcherTimer)
            {
                SetBusy(false);
                dispatcherTimer.Stop();
            }
        }

        #endregion Methods
    }
}