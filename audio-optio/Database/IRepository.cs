using System;
using System.Collections.Generic;

namespace audio_optio.Database
{
    public interface IRepository<T> : IDisposable
    {
        IEnumerable<T> Get();
        T Get(int id);
        int Insert(T item);
        void Delete(int id);
        void Update(T item);
        void Save();
    }
}
