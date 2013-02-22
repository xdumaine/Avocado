namespace MetroMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public class MergeableCollection<T> : ObservableCollection<T>
        where T : MergeableObject, new()
    {
        #region Events
        public event EventHandler Merged;

        #endregion

        #region Constructors

        public MergeableCollection()
        {
        }

        #endregion

        #region Methods

        protected override void SetItem(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }

        protected override void InsertItem(int index, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        public virtual void Merge(IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            Type itemType = typeof(T);
            Type comparableType = typeof(IComparable<T>);
            bool comparable = false;

            // Determine if the type this collection contains implements IComparable
            var interfaces = itemType.GetTypeInfo().ImplementedInterfaces;
            foreach (Type @interface in interfaces)
            {
                if (@interface == comparableType)
                {
                    comparable = true;
                    break;
                }
            }

            // Remove any existing and invalid items
            for (int i = Count - 1; i >= 0; i--)
            {
                T item = this[i];
                if (collection.Contains(item) == false)
                {
                    RemoveItem(i);
                }
            }

            // Attempt to merge/update any existing items
            foreach (T existingItem in this)
            {
                foreach (T newItem in collection)
                {
                    if (existingItem.Equals(newItem))
                    {
                        existingItem.Merge(newItem);
                        break;
                    }
                }
            }

            // Add any missing items
            foreach (T newItem in collection)
            {
                if (Contains(newItem) == false)
                {
                    if (comparable && Count > 0)
                    {
                        for (int i = 0; i <= Count; i++)
                        {
                            // If we've reached the end of the original collection, just add the new item to the end
                            if (i == Count)
                            {
                                Add(newItem);
                                break;
                            }
                            else
                            {
                                // Look for the right spot to add the new item to the collection
                                IComparable<T> existingItem = (IComparable<T>)this[i];
                                if (existingItem.CompareTo(newItem) > 0)
                                {
                                    InsertItem(i, newItem);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Add(newItem);
                    }
                }
            }

            OnMerged();
        }

        protected virtual void OnMerged()
        {
            EventHandler mergedHandler = Merged;

            if (mergedHandler != null)
            {
                mergedHandler(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}