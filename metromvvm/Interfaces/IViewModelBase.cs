namespace MetroMVVM.Interfaces
{
    using System;
    using System.ComponentModel;
    using Windows.ApplicationModel.Resources;

    public interface IViewModelBase : ICleanup, INavigable, INotifyPropertyChanged
    {
        ResourceLoader ResourceLoader { get; }
    }
}