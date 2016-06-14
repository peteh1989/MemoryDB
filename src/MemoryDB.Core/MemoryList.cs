using MemoryDB.Core.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace MemoryDB.Core
{
    public class MemoryList<T> : ICollection<T> where T : new()
    {

        //The data store
        public IDataStore<T> DataStore { get; }

        /// The in memory list
        private readonly List<T> _list;

        public MemoryList(IDataStore<T> dataStore)
        {
            DataStore = dataStore;
            _list = DataStore.LoadData();
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public virtual void Add(T item)
        {
            this.DataStore.Add(item);
            _list.Add(item);
        }

        public virtual void Clear()
        {
            this.DataStore.Clear();
            _list.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool Remove(T item)
        {
            this.DataStore.Remove(item);
            var removed = _list.Remove(item);
            return removed;
        }
    }
}