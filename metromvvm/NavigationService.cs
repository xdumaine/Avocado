namespace MetroMVVM
{
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using MetroMVVM.Interfaces;

    public class NavigationService : INavigationService
    {
        #region Events
        public event NavigatingCancelEventHandler Navigating;
        #endregion

        #region Private fields
        private static INavigationService m_DefaultInstance;
        private static readonly object m_CreationLock = new object();
        private Frame m_MainFrame;
        #endregion
                
        /// <summary>
        /// Gets the NavigationService instance
        /// </summary>
        public static INavigationService Default
        {
            get
            {
                if (m_DefaultInstance == null)
                {
                    lock (m_CreationLock)
                    {
                        if (m_DefaultInstance == null)
                        {
                            m_DefaultInstance = new NavigationService();
                        }
                    }
                }

                return m_DefaultInstance;
            }
        }

        #region Methods
        public void GoBack()
        {
            if (EnsureMainFrame() && m_MainFrame.CanGoBack)
            {
                m_MainFrame.GoBack();
            }
        }

        public void NavigateTo(Type pageType, object parameter)
        {
            if (EnsureMainFrame())
            {
                m_MainFrame.Navigate(pageType, parameter);
            }
        }

        #endregion

        #region Private methods
        private bool EnsureMainFrame()
        {
            if (m_MainFrame != null)
            {
                return true;
            }

            m_MainFrame = Window.Current.Content as Frame;
            //m_MainFrame = (Window.Current.Content as Page).Frame;

            if (m_MainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                m_MainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                return true;
            }

            return false;
        }
        #endregion
    }
}