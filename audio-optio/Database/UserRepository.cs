using System;
using System.Collections.Generic;
using System.Linq;

using audio_optio.Domain;

namespace audio_optio.Database
{
    public class UserRepository : IRepository<User>
    {
        private AoDbContext context;

        public UserRepository(AoDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            User user = context.Users.Find(id);
            context.Users.Remove(user);
        }

        public IEnumerable<User> Get()
        {
            return context.Users.ToList();
        }

        public User Get(int id)
        {
            return context.Users.Include("Contact").Where(x => x.Id == id).FirstOrDefault();
        }

        public int Insert(User user)
        {
            context.Users.Add(user);
            Save();

            return user.Id;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Entry(user).State = System.Data.Entity.EntityState.Modified;
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