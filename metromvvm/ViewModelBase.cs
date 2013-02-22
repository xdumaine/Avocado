namespace MetroMVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using MetroMVVM.Messaging;
    using MetroMVVM.Interfaces;
    using Windows.ApplicationModel.Resources;

    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, ICleanup, INavigable
    {
        private static bool? m_IsInDesignMode;
        private IMessenger m_MessengerInstance;
        private INavigationService m_NavigationService;
        private IFlyoutService m_FlyoutService;
        private IMessageBoxService m_MessageBoxService;

        private static readonly object LockObj = new object();
        private static ResourceLoader m_ResourceLoader;

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="messenger">An instance of a <see cref="Messenger" />
        /// used to broadcast messages to other objects. If null, this class
        /// will attempt to broadcast using the Messenger's default
        /// instance.</param>
        /// <param name="navigationService">An instance of a <see cref="NavigationService" />
        /// used to navigate between views. If null, this class
        /// will attempt to navigate using the NavigatorService's default
        /// instance.</param>
        public ViewModelBase(IMessenger messenger = null, INavigationService navigationService = null, IFlyoutService flyoutService = null, IMessageBoxService msgBoxService = null)
        {
            if (messenger == null)
            {
                m_MessengerInstance = Messenger.Default;
            }
            else
            {
                m_MessengerInstance = messenger;
            }

            if (navigationService == null)
            {
                m_NavigationService = NavigationService.Default;
            }
            else
            {
                m_NavigationService = navigationService;	
            }

            if (flyoutService == null)
            {
                m_FlyoutService = FlyoutService.Default;
            }
            else
            {
                m_FlyoutService = flyoutService;
            }

            if (msgBoxService == null)
            {
                m_MessageBoxService = MessageBoxService.Default;
            }
            else
            {
                m_MessageBoxService = msgBoxService;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
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
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }

        /// <summary>
        /// Gets or sets an instance of a <see cref="IMessenger" /> used to broadcast messages to other objects.
        /// </summary>
        protected IMessenger MessengerInstance
        {
            get
            {
                return m_MessengerInstance;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (m_MessengerInstance != null)
                {
                    throw new InvalidOperationException("Messenger Instance can be set only once");
                }

                m_MessengerInstance = value;
            }
        }


        /// <summary>
        /// Gets or sets an instance of a <see cref="INavigationService" />
        /// </summary>
        protected INavigationService NavigationServiceInstance
        {
            get
            {
                return m_NavigationService;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (m_NavigationService != null)
                {
                    throw new InvalidOperationException("Navigation Service Instance can be set only once");
                }

                m_NavigationService = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of a <see cref="IFlyoutService" /> used to show Flyouts.
        /// </summary>
        protected IFlyoutService FlyoutInstance
        {
            get
            {
                return m_FlyoutService;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (m_FlyoutService != null)
                {
                    throw new InvalidOperationException("Flyout Service Instance can be set only once");
                }

                m_FlyoutService = value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of a <see cref="IMessageBoxService" /> used to show MessageBoxes.
        /// </summary>
        protected IMessageBoxService MessageBoxInstance
        {
            get
            {
                return m_MessageBoxService;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (m_MessageBoxService != null)
                {
                    throw new InvalidOperationException("MessageBox Service Instance can be set only once");
                }

                m_MessageBoxService = value;
            }
        }

        public ResourceLoader ResourceLoader
        {
            get
            {
                if (m_ResourceLoader == null)
                {
                    lock (LockObj)
                    {
                        if (m_ResourceLoader == null)
                            m_ResourceLoader = new ResourceLoader();
                    }
                }
                return m_ResourceLoader;
            }
        }
        
        /// <summary>
        /// Unregisters this instance from the Messenger class.
        /// <para>To cleanup additional resources, override this method, clean
        /// up and then call base.Cleanup().</para>
        /// </summary>
        public virtual void Cleanup()
        {
            m_MessengerInstance.Unregister(this);
        }

        /// <summary>
        /// Broadcasts a PropertyChangedMessage using either the instance of
        /// the Messenger that was passed to this class (if available) 
        /// or the Messenger's default instance.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The value of the property before it changed.</param>
        /// <param name="newValue">The value of the property after it changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            m_MessengerInstance.Send(new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName));
        }

        protected virtual void NavigateTo(Type destinationViewModelType, object parameter)
        {
            if (destinationViewModelType == null)
            {
                throw new ArgumentNullException("destinationViewModelType");
            }

            m_MessengerInstance.Send(new NavigationMessage(destinationViewModelType.FullName, parameter));
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        protected virtual void RaisePropertyChanged<T>(string propertyName, T oldValue, T newValue, bool broadcast)
        {
            RaisePropertyChanged(propertyName);

            if (broadcast)
            {
                Broadcast(oldValue, newValue, propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        /// <param name="oldValue">The property's value before the change occurred.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            if (propertyExpression == null)
            {
                return;
            }

            var handler = PropertyChangedHandler;

            if (handler != null || broadcast)
            {
                var body = propertyExpression.Body as MemberExpression;

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(body.Member.Name));
                }

                if (broadcast)
                {
                    Broadcast(oldValue, newValue, body.Member.Name);
                }
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        protected void Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue,
            bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyExpression, oldValue, field, broadcast);
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        protected void Set<T>(
            string propertyName,
            ref T field,
            T newValue,
            bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyName, oldValue, field, broadcast);
        }

        public virtual void OnNavigatedTo(object parameter)
        {

        }

        public virtual void OnNavigatedFrom()
        {
        	
        }

        public virtual void OnNavigatingFrom()
        {
        	
        }
    }
}