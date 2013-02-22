namespace MetroMVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A base class for objects of which the properties must be observable.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Occurs after a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Provides access to the PropertyChanged event handler to derived classes.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }
        /// <summary>
        /// Provides access to the PropertyChanging event handler to derived classes.
        /// </summary>
        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return PropertyChanging;
            }
        }

        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <remarks>If the propertyName parameter does not correspond to an existing
        /// property on the current class, an exception is thrown in DEBUG configuration only.</remarks>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] String propertyName = null)
        {
            VerifyPropertyName(propertyName);

            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                return;
            }

            var handler = PropertyChanged;

            if (handler != null)
            {
                var body = propertyExpression.Body as MemberExpression;
                handler(this, new PropertyChangedEventArgs(body.Member.Name));
            }
        }

        protected virtual void RaisePropertyChanging(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new NotSupportedException("Raising the PropertyChanging event with an empty string or null is not supported in the Windows 8 Developer Preview");
            }
            else
            {
                VerifyPropertyName(propertyName);

                var handler = PropertyChanging;
                if (handler != null)
                {
                    handler(this, new PropertyChangingEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event if needed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changes.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changes.</param>
        protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                var body = propertyExpression.Body as MemberExpression;
                handler(this, new PropertyChangingEventArgs(body.Member.Name));
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        protected void Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            RaisePropertyChanging(propertyExpression);
            field = newValue;
            RaisePropertyChanged(propertyExpression);
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the PropertyChanged event if needed. 
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        protected void Set<T>(string propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            RaisePropertyChanging(propertyName);
            field = newValue;
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// <para>This method is only active in DEBUG mode.</para>
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            var myType = GetType();

            if (!string.IsNullOrEmpty(propertyName) && myType.GetRuntimeProperties().Where(p => p.Name == propertyName) == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
        }
    }
}