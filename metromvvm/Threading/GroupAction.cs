namespace MetroMVVM.Threading
{
    public delegate void ActionCompletion();

    public class GroupAction
    {
        private readonly ActionCompletion m_Callback;
        private readonly object m_LockObject = new object();
        private uint m_Count;
        public GroupAction(uint count, ActionCompletion callback)
        {
            m_Count = count;
            m_Callback = callback;
        }

        public delegate void ActionCompletion();
        public void SingleActionComplete()
        {
            lock (m_LockObject)
            {
                if (m_Count > 0)
                {
                    m_Count--;
                    if (m_Count == 0)
                    {
                        m_Callback();
                    }
                }
            }
        }
    }
}