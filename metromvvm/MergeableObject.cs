namespace MetroMVVM
{
    using System;

    public abstract class MergeableObject : ObservableObject
    {
        #region Events
        public event EventHandler Merged;

        #endregion

        #region Properties

        public abstract string MergeID { get; }

        #endregion

        #region Constructors

        public MergeableObject()
        {
        }

        #endregion

        #region Methods

        public abstract void Merge(MergeableObject obj);

        public virtual bool Equals(MergeableObject obj)
        {
            if (MergeID.ToLower() == obj.MergeID.ToLower())
            {
                return true;
            }
            else
            {
                return false;
            }
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