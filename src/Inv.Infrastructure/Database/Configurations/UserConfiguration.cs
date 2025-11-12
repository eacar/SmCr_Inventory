using Inv.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inv.Infrastructure.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Columns

            builder.ToTable("Users");
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            builder.Property(p => p.Email).HasColumnType("NVARCHAR(512)");
            builder.Property(p => p.PasswordHash).IsRequired().HasColumnType("NVARCHAR(512)");
            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);
            builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

            #endregion

            #region Indexes

            builder.HasIndex(u => u.Email).IsUnique();

            #endregion
        }
    }
}