using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace rabbithole
{
    public class EventSinkContext : DbContext
    {

        private readonly string connStr;

        public EventSinkContext(string connectionString) {
            connStr = connectionString;
        }

        public DbSet<Event> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connStr);
        }
    }
}