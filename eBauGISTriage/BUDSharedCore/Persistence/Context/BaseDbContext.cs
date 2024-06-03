using Microsoft.EntityFrameworkCore;


namespace BUDSharedCore.Persistence.Context
{
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContextConfig _config;

        public BaseDbContext(DbContextOptions options, BaseDbContextConfig config) : base(options)
        {
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_config.Schema))
            {
                modelBuilder.HasDefaultSchema(_config.Schema);
            }
        }
    }
}
