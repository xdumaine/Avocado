namespace MetroMVVM.Interfaces
{
    using System;

    /// <summary>
    /// Reasons for a Flyout being closed.
    /// </summary>
    public enum FlyoutCloseReason
    {
        BackButton,
        LightDismissal,
        Other
    }

    public delegate void FlyoutClosingDelegate(object sender, FlyoutCloseReason reason);

    public interface IFlyoutWindow
    {
       
        event FlyoutClosingDelegate Closing;
        event FlyoutClosingDelegate Closed;

        object DataContext { get; set; }
        
        void Close();
        void Show();
    }
}