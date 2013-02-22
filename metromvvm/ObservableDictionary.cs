namespace MetroMVVM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Windows.Foundation.Collections;

    /// <summary>
    /// Implementation of IObservableMap that supports reentrancy.
    /// </summary>
    public class ObservableDictionary<K, V> : IObservableMap<K, V>
    {
        private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<K>
        {
            public ObservableDictionaryChangedEventArgs(CollectionChange change, K key)
            {
                this.CollectionChange = change;
                this.Key = key;
            }

            public CollectionChange CollectionChange { get; private set; }
            public K Key { get; private set; }
        }

        private readonly Dictionary<K, V> m_Dictionary = new Dictionary<K, V>();
        public event MapChangedEventHandler<K, V> MapChanged;

        private void InvokeMapChanged(CollectionChange change, K key)
        {
            var eventHandler = MapChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new ObservableDictionaryChangedEventArgs(CollectionChange.ItemInserted, key));
            }
        }

        public void Add(K key, V value)
        {
            this.m_Dictionary.Add(key, value);
            this.InvokeMapChanged(CollectionChange.ItemInserted, key);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool Remove(K key)
        {
            if (this.m_Dictionary.Remove(key))
            {
                this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            V currentValue;
            if (this.m_Dictionary.TryGetValue(item.Key, out currentValue) &&
                Object.Equals(item.Value, currentValue) && this.m_Dictionary.Remove(item.Key))
            {
                this.InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
                return true;
            }
            return false;
        }

        public V this[K key]
        {
            get
            {
                return this.m_Dictionary[key];
            }
            set
            {
                this.m_Dictionary[key] = value;
                this.InvokeMapChanged(CollectionChange.ItemChanged, key);
            }
        }

        public void Clear()
        {
            var priorKeys = this.m_Dictionary.Keys.ToArray();
            this.m_Dictionary.Clear();
            foreach (var key in priorKeys)
            {
                this.InvokeMapChanged(CollectionChange.ItemRemoved, key);
            }
        }

        public ICollection<K> Keys
        {
            get { return this.m_Dictionary.Keys; }
        }

        public bool ContainsKey(K key)
        {
            return this.m_Dictionary.ContainsKey(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return this.m_Dictionary.TryGetValue(key, out value);
        }

        public ICollection<V> Values
        {
            get { return this.m_Dictionary.Values; }
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return this.m_Dictionary.Contains(item);
        }

        public int Count
        {
            get { return this.m_Dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return this.m_Dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.m_Dictionary.GetEnumerator();
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            int arraySize = array.Length;
            foreach (var pair in this.m_Dictionary)
            {
                if (arrayIndex >= arraySize) break;
                array[arrayIndex++] = pair;
            }
        }
    }
}