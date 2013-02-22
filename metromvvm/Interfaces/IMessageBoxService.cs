namespace MetroMVVM.Interfaces
{
    using System.Threading.Tasks;

    public interface IMessageBoxService
    {
        Task<GenericMessageBoxResult> ShowAsync(string message, string caption, GenericMessageBoxButton buttons);
        void ShowAsync(string message, string caption);
    }
}