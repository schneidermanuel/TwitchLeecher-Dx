﻿using Avalonia.Controls;
using Avalonia.Input;
using TwitchLeecher.Gui.ViewModels;

namespace TwitchLeecher.Gui.Views
{
    public partial class SearchResultView : UserControl
    {
        #region Constructors

        public SearchResultView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var viewModel = (SearchResultViewModel)DataContext;
            viewModel!.ViewCommand.Execute(null);
        }
    }
}