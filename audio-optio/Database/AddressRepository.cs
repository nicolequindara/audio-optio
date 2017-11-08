using System;
using System.Linq;
using System.Collections.Generic;

using audio_optio.Domain;

namespace audio_optio.Database
{
    public class AddressRepository : IRepository<Address>
    {
        private AoDbContext context;

        public AddressRepository(AoDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            Address Address = context.Addresses.Find(id);
            context.Addresses.Remove(Address);
        }
        
        public IEnumerable<Address> Get()
        {
            return context.Addresses.Include("Order").ToList();
        }

        public Address Get(int id)
        {
            return context.Addresses.Include("Order").Where(x => x.Id == id).FirstOrDefault();
        }

        public int Insert(Address item)
        {
            context.Addresses.Add(item);
            Save();

            return item.Id;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Address item)
        {
            context.Entry(item).State = System.Data.Entity.EntityState.Modified;
            Save();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}