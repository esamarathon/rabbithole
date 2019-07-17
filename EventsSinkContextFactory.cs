using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace rabbithole {

    public class EventsSinkDBContextFactory : IDesignTimeDbContextFactory<EventSinkContext>
    {
        public EventSinkContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.default.json")
            .AddJsonFile($"appsettings.json", true)
            .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("events");
            return Create(connstr);
        }
        
        private EventSinkContext Create(string connectionString)
        {
        if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentException(
            $"{nameof(connectionString)} is null or empty.",
            nameof(connectionString));

        var optionsBuilder = new DbContextOptionsBuilder<EventSinkContext>();

        optionsBuilder.UseNpgsql(connectionString);

        return new EventSinkContext(optionsBuilder.Options);
        }
    }
}