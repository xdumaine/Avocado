namespace MetroMVVM.Extensions
{
    using System;

    /// <summary>
    /// Represents a collection of commands
    /// </summary>
    public class CommandCollection : AttachableCollection<EventToCommand>
    {
        /// <summary>
        /// Implemented by derived class notifies an item was added
        /// </summary>
        /// <param name="item">Item added</param>
        internal override void ItemAdded(EventToCommand item)
        {
            if (base.AssociatedObject != null)
            {
                item.Attach(AssociatedObject);
            }
        }

        /// <summary>
        /// Implemented by derived class notifies an item was removed
        /// </summary>
        /// <param name="item">Item removed</param>
        internal override void ItemRemoved(EventToCommand item)
        {
            if (base.AssociatedObject != null)
            {
                item.Detach();
            }
        }

        /// <summary>
        /// Notifies a DependencyObject has been attached
        /// </summary>
        protected override void OnAttached()
        {
            foreach (EventToCommand binding in this)
            {
                binding.Attach(AssociatedObject);
            }
        }

        /// <summary>
        /// Notifies a DependencyObject is detaching
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (EventToCommand binding in this)
            {
                binding.Detach();
            }
        }
    }
}