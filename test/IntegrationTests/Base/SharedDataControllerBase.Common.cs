using Inv.Infrastructure.Database.Context;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Base
{
    public partial class SharedDataControllerBase<TFactory>
    {        
        protected T GetService<T>() where T : notnull
            => Factory.Services.CreateScope().ServiceProvider.GetRequiredService<T>();
        protected static Task SaveAsync(AppDbContext db, CancellationToken ct = default)
            => db.SaveChangesAsync(ct);
    }
}