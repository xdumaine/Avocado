namespace MetroMVVM.Interfaces
{
    using System;

    public interface IFlyoutService
    {
        void ShowFlyout<TViewModel>(IFlyoutWindow view, TViewModel viewModel, Action<TViewModel> onFlyoutClosing = null, Action<TViewModel> onFlyoutClose = null);
    }
}