using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace rabbithole
{
    public class EventSinkContext : DbContext
    {
        public EventSinkContext(DbContextOptions options) : base(options) { }
        
        public DbSet<Event> Events { get; set; }
    }
}