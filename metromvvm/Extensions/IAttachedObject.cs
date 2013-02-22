namespace MetroMVVM.Extensions
{
    using System;
    using System.Linq;
    using Windows.UI.Xaml;

    /// <summary>
    /// Describes an object that can be associated to another DependencyObject
    /// </summary>
    public interface IAttachedObject
    {
        /// <summary>
        /// Exposes the instance of the associated DependencyObject
        /// </summary>
        DependencyObject AssociatedObject { get; }
        /// <summary>
        /// Attaches a DependencyObject to this instance
        /// </summary>
        /// <param name="dependencyObject">The dependencyObject to attach</param>
        void Attach(DependencyObject dependencyObject);

        /// <summary>
        /// Detaches the currently associated DependencyObject
        /// </summary>
        void Detach();
    }
}