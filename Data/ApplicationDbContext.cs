using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryDB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ensure all Identity tables use varchar(255) for key/index columns to work with MySQL
            builder.Entity<IdentityUser>(b => b.Property(u => u.Id).HasColumnType("varchar(255)"));
            builder.Entity<IdentityRole>(b => b.Property(r => r.Id).HasColumnType("varchar(255)"));

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.Property(r => r.UserId).HasColumnType("varchar(255)");
                b.Property(r => r.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(l => l.LoginProvider).HasColumnType("varchar(255)");
                b.Property(l => l.ProviderKey).HasColumnType("varchar(255)");
                b.Property(l => l.UserId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(t => t.UserId).HasColumnType("varchar(255)");
                b.Property(t => t.LoginProvider).HasColumnType("varchar(255)");
                b.Property(t => t.Name).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property(c => c.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.Property(c => c.UserId).HasColumnType("varchar(255)");
            });
        }
    }
}
