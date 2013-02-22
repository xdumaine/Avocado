namespace MetroMVVM.Extensions
{
    using System;
    using System.Linq;
    using Windows.UI.Xaml;

    public static class CommandExtensions
    {
        /// <summary>
        /// Exposes the Attached Property related to the Commands property
        /// </summary>
        public static readonly DependencyProperty CommandsProperty =
            DependencyProperty.RegisterAttached(
                "Commands",
                typeof(CommandCollection),
                typeof(CommandExtensions),
                new PropertyMetadata(DependencyProperty.UnsetValue, CommandExtensions.OnCommandsChanged));

        /// <summary>
        /// Gets the value of the Commands attached property
        /// </summary>
        /// <param name="dependencyObject">dependency object to get the value</param>
        /// <returns>Instance of the command collection</returns>
        public static CommandCollection GetCommands(DependencyObject dependencyObject)
        {
            CommandCollection collection = (CommandCollection)dependencyObject.GetValue(CommandsProperty);

            if (collection == null)
            {
                collection = new CommandCollection();
                dependencyObject.SetValue(CommandsProperty, collection);
            }

            return collection;
        }

        /// <summary>
        /// Sets the value of the Commands attached property
        /// </summary>
        /// <param name="dependencyObject">dependency object to set the value</param>
        /// <param name="value">Instance of the command collection</param>
        public static void SetCommands(DependencyObject dependencyObject, CommandCollection value)
        {
            dependencyObject.SetValue(CommandsProperty, value);
        }

        /// <summary>
        /// handles the change of the Commands property
        /// </summary>
        /// <param name="dependencyObject">source of the event</param>
        /// <param name="e">Parameters of the event</param>
        private static void OnCommandsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CommandCollection oldValue = e.OldValue as CommandCollection;
            CommandCollection newValue = e.NewValue as CommandCollection;

            if (oldValue != newValue)
            {
                if ((oldValue != null) && (((IAttachedObject)oldValue).AssociatedObject != null))
                {
                    oldValue.Detach();
                }
                if ((newValue != null) && (dependencyObject != null))
                {
                    if (((IAttachedObject)newValue).AssociatedObject != null)
                    {
                        throw new InvalidOperationException("Too many");
                    }

                    newValue.Attach(dependencyObject);
                }
            }
        }
    }
}
