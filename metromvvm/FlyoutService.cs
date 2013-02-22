namespace MetroMVVM
{
    using System;
    using MetroMVVM.Interfaces;

    public class FlyoutService : IFlyoutService
    {
        #region Private fields
        private static IFlyoutService m_DefaultInstance;
        private static readonly object m_CreationLock = new object();
        #endregion

        #region Methods
        public void ShowFlyout<TViewModel>(IFlyoutWindow view, TViewModel viewModel, Action<TViewModel> onFlyoutClosing = null, Action<TViewModel> onFlyoutClose = null)
        {
            if (viewModel != null)
            {
                view.DataContext = viewModel;
            }

            if (onFlyoutClosing != null)
            {
                view.Closing += (sender, e) => onFlyoutClosing(viewModel);
            }

            if (onFlyoutClose != null)
            {
                view.Closed += (sender, e) => onFlyoutClose(viewModel);
            }

            view.Show();
        }
        #endregion

        /// <summary>
        /// Gets the FlyoutService instance
        /// </summary>
        public static IFlyoutService Default
        {
            get
            {
                if (m_DefaultInstance == null)
                {
                    lock (m_CreationLock)
                    {
                        if (m_DefaultInstance == null)
                        {
                            m_DefaultInstance = new FlyoutService();
                        }
                    }
                }

                return m_DefaultInstance;
            }
        }
    }
}