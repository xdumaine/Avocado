namespace System.ComponentModel
{
    /// <summary>
    /// Provides data for the System.ComponentModel.INotifyPropertyChanging.PropertyChanging 
    /// event.
    /// </summary>
    public class PropertyChangingEventArgs
    {
        /// <summary>
        /// Gets the name of the property that is changing.
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the System.ComponentModel.PropertyChangingEventArgs class.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        public PropertyChangingEventArgs(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}