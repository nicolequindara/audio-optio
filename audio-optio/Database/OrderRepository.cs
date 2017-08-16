using System;
using System.Linq;
using System.Collections.Generic;

using audio_optio.Domain;

namespace audio_optio.Database
{
    public class OrderRepository : IRepository<Order>
    {
        private AoDbContext context;

        public OrderRepository(AoDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            Order Order = context.Orders.Find(id);
            context.Orders.Remove(Order);
        }
        
        public IEnumerable<Order> Get()
        {
            return context.Orders.Include("Contact").ToList();
        }

        public Order Get(int id)
        {
            return context.Orders.Include("Contact").Where(x => x.Id == id).FirstOrDefault();
        }

        public int Insert(Order item)
        {
            context.Orders.Add(item);
            Save();

            return item.Id;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Order item)
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