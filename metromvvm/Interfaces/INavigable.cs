namespace MetroMVVM.Interfaces
{
    public interface INavigable
    {
        void OnNavigatedFrom();

        void OnNavigatedTo(object parameter);

        void OnNavigatingFrom();
    }
}