namespace MetroMVVM.Helpers
{
    using System;

    /// <summary>
    /// Stores an <see cref="Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private WeakReference m_Reference;
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction" /> class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            m_Reference = new WeakReference(target);
            Action = action;
        }

        /// <summary>
        /// Gets the Action associated to this instance.
        /// </summary>
        public Action Action { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public bool IsAlive
        {
            get
            {
                if (m_Reference == null)
                {
                    return false;
                }

                return m_Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (m_Reference == null)
                {
                    return null;
                }

                return m_Reference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (Action != null && IsAlive)
            {
                Action();
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            m_Reference = null;
        }
    }
}