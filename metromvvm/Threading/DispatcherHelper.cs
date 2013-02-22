namespace MetroMVVM.Threading
{
    using System;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    /// <summary>
    /// Helper class for dispatcher operations on the UI thread.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Gets a reference to the UI thread's dispatcher, after the
        /// <see cref="Initialize" /> method has been called on the UI thread.
        /// </summary>
        public static CoreDispatcher UIDispatcher { get; private set; }

        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immendiately. If the
        /// method is called from another thread, the action will be enqueued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// <para>For additional operations on the UI thread, you can get a
        /// reference to the UI thread's dispatcher thanks to the property
        /// <see cref="UIDispatcher" /></para>.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (UIDispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
        }

        /// <summary>
        /// This method should be called once on the UI thread to ensure that
        /// the <see cref="UIDispatcher" /> property is initialized.
        /// <para>In a Silverlight application, call this method in the
        /// Application_Startup event handler, after the MainPage is constructed.</para>
        /// <para>In WPF, call this method on the static App() constructor.</para>
        /// </summary>
        public static void Initialize()
        {
            if (UIDispatcher != null)
            {
                return;
            }
            UIDispatcher = Window.Current.Dispatcher;
        }

        public static void InvokeAsync(object sender, Action action)
        {
            UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}