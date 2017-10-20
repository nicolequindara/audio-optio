using System.Data.Entity;

using audio_optio.Domain;
using audio_optio.Models;

namespace audio_optio.Database
{
    public class AoDbContext : DbContext
    {
        public AoDbContext() : base("AoDbEntities")
        {

        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<User> Users { get;  set; }
    }
}