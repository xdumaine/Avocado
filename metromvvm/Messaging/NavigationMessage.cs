namespace MetroMVVM.Messaging
{
    using System;
    using System.Linq;

    public class NavigationMessage
    {
        #region Properties
        public string DestinationPage { get; private set; }
        public object Parameter { get; private set; }
        #endregion

        #region Constructor
        public NavigationMessage(string destinationViewModelTypeName, object parameter)
        {
            DestinationPage = destinationViewModelTypeName;
            Parameter = parameter;
        }
        #endregion
    }
}