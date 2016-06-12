using System.Collections.Generic;

namespace MemoryDB.Core.Interfaces
{
    public interface IDataStore<T> where T : new()
    {
        T Add(T item);

        void Clear();

        List<T> LoadData();

        void Remove(T item);
    }
}