namespace System.ComponentModel
{
    /// <summary>
    /// Represents a method that will handle the System.ComponentModel.INotifyPropertyChanging.PropertyChanging event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs e);
}