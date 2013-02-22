namespace MetroMVVM.Interfaces
{
    using System;
    using Windows.UI.Xaml.Navigation;

    public interface INavigationService
    {
        event NavigatingCancelEventHandler Navigating;

        void GoBack();
        void NavigateTo(Type pageType, object parameter);
    }
}