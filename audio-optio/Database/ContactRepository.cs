using System;
using System.Linq;
using System.Collections.Generic;

using audio_optio.Domain;
using audio_optio.Models;

namespace audio_optio.Database
{
    public class ContactRepository : IRepository<Contact>
    {
        private AoDbContext context;

        public ContactRepository(AoDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            Contact contact = context.Contacts.Find(id);
            context.Contacts.Remove(contact);
        }
        
        public IEnumerable<Contact> Get()
        {
            return context.Contacts.Include("Orders").Include("Comments").ToList();
        }
        
        public Contact Get(string FirstName, string LastName)
        {
            Contact contact = context.Contacts.Include("Orders").Include("Comments").Where(x => x.FirstName == FirstName && x.LastName == LastName).FirstOrDefault();

            return contact;
        }
        public Contact Get(int id)
        {
            return context.Contacts.Include("Orders").Include("Comments").Where(x => x.Id == id).FirstOrDefault();
        }

        public int Insert(Contact item)
        {
            context.Contacts.Add(item);
            Save();

            return item.Id;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Contact item)
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