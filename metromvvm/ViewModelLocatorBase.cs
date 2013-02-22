namespace MetroMVVM
{
    using System;

    public abstract class ViewModelLocatorBase<TViewModel> : ObservableObject 
        where TViewModel : class
    {
        private static bool? m_IsInDesignMode;
        private TViewModel m_DesignTimeViewModel;
        private TViewModel m_RuntimeViewModel;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!m_IsInDesignMode.HasValue)
                {
                    m_IsInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
                }

                return m_IsInDesignMode.Value;
            }
        }

        /// <summary>
        /// Holds the intance of the designtime version of the ViewModel that is instantiated
        /// only when application is opened in IDE designer (VisualStudio, Blend etc).
        /// </summary>
        public TViewModel DesignTimeViewModel
        {
            get
            {
                return m_DesignTimeViewModel;
            }

            set
            {
                m_DesignTimeViewModel = value;
                RaisePropertyChanged("ViewModel");
            }
        }

        /// <summary>
        /// Gets current ViewModel instance so if we are in designer its <see cref="DesignTimeViewModel"/>
        /// and if its runtime then its <see cref="RuntimeViewModel"/>.
        /// </summary>
        public TViewModel ViewModel
        {
            get
            {
                return IsInDesignMode ? DesignTimeViewModel : RuntimeViewModel;
            }
        }

        /// <summary>
        /// Holds the intance of the runtime version of the ViewModel that is instantiated only when
        /// application is really running by retrieving the instance from IOC container
        /// </summary>
        protected TViewModel RuntimeViewModel
        {
            get
            {
                if (m_RuntimeViewModel == null)
                {
                    RuntimeViewModel = BasicServiceLocator.Instance.Get<TViewModel>();
                }
                return m_RuntimeViewModel;
            }

            set
            {
                m_RuntimeViewModel = value;
                RaisePropertyChanged("ViewModel");
            }
        }
    }
}