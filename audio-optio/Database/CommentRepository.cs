using System;
using System.Linq;
using System.Collections.Generic;

using audio_optio.Domain;

namespace audio_optio.Database
{
    public class CommentRepository : IRepository<Comment>
    {
        private AoDbContext context;

        public CommentRepository(AoDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            Comment Comment = context.Comments.Find(id);
            context.Comments.Remove(Comment);
        }
        
        public IEnumerable<Comment> Get()
        {
            return context.Comments.Include("Contact").ToList();
        }

        public Comment Get(int id)
        {
            return context.Comments.Include("Contact").Where(x => x.Id == id).FirstOrDefault();
        }

        public int Insert(Comment item)
        {
            context.Comments.Add(item);
            Save();

            return item.Id;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(Comment item)
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