using Inv.Domain.Base;
using Inv.Domain.Users;
using Inv.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries<IModified>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        //There could be some auditing for trailing the actions and Audit.EntityFramework.Core is really helpful
    }
}