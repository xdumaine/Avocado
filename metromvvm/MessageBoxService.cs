namespace MetroMVVM
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Interfaces;
    using Windows.UI.Popups;

    public class MessageBoxService : IMessageBoxService
    {
        #region Private fields
        private static IMessageBoxService m_DefaultInstance;
        private static readonly object m_CreationLock = new object();
        private GenericMessageBoxResult m_Result;
        private string m_OkString;
        private string m_CancelString;
        #endregion

        #region Constructor (SINGLETON)
        /// <summary>
        /// Gets the MessageBoxService instance
        /// </summary>
        public static IMessageBoxService Default
        {
            get
            {
                if (m_DefaultInstance == null)
                {
                    lock (m_CreationLock)
                    {
                        if (m_DefaultInstance == null)
                        {
                            m_DefaultInstance = new MessageBoxService();
                        }
                    }
                }

                return m_DefaultInstance;
            }
        }
        #endregion

        #region Methods
        public async Task<GenericMessageBoxResult> ShowAsync(string message, string caption, GenericMessageBoxButton buttons)
        {
            MessageDialog msg = new MessageDialog(message, caption);

            Windows.ApplicationModel.Resources.ResourceLoader rl = new Windows.ApplicationModel.Resources.ResourceLoader();
            try
            {
                m_OkString = rl.GetString("OK");
            }
            catch (Exception)
            {
                m_OkString = "OK"; // Use default
            }

            try
            {
                m_CancelString = rl.GetString("Cancel");
            }
            catch (Exception)
            {
                m_CancelString = "Cancel"; // Use default
            }

            // Add buttons and set their command handlers
            if (buttons == GenericMessageBoxButton.Ok)
            {
                msg.Commands.Add(new UICommand(m_OkString, new UICommandInvokedHandler(this.CommandInvokedHandler)));

                // Set the command to be invoked when a user presses 'ESC'
                msg.CancelCommandIndex = 0;
            }

            if (buttons == GenericMessageBoxButton.OkCancel)
            {
                msg.Commands.Add(new UICommand(m_OkString, new UICommandInvokedHandler(this.CommandInvokedHandler)));
                msg.Commands.Add(new UICommand(m_CancelString, new UICommandInvokedHandler(this.CommandInvokedHandler)));

                // Set the command to be invoked when a user presses 'ESC'
                msg.CancelCommandIndex = 1;
            }

            await msg.ShowAsync();

            return m_Result;
        }

        public async void ShowAsync(string message, string caption)
        {
            await ShowAsync(message, caption, GenericMessageBoxButton.Ok);
        }
        #endregion

        #region Private methods
        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == m_OkString)
            {
                m_Result = GenericMessageBoxResult.Ok;
            }
            else if (command.Label == m_CancelString)
            {
                m_Result = GenericMessageBoxResult.Cancel;
            }
        }
        #endregion
    }
}
