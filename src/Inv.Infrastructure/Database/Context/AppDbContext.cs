using System.Reflection;
using Inv.Domain.Users;
using Inv.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;

namespace Inv.Infrastructure.Database.Context
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }

        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        //There could be some auditing for trailing the actions and Audit.EntityFramework.Core is really helpful
    }
}