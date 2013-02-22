namespace System.ComponentModel
{
    /// <summary>
    /// Defines an event for notifying clients that a property value is changing.
    /// </summary>
    public interface INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        event PropertyChangingEventHandler PropertyChanging;
    }
}
