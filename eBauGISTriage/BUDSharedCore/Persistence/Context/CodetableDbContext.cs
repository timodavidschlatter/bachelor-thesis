using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BUDSharedCore.Persistence.Context
{
    public class CodetableDbContext : BaseDbContext
    {
        public CodetableDbContext(DbContextOptions options, BaseDbContextConfig config) : base(options, config)
        {
        }

        public virtual DbSet<Codetable> Codetable { get; set; }

        public static DbContextInfo GetDbContextInfo()
        {
            return new DbContextInfo("Codetable", typeof(CodetableDbContext));
        }
    }
}
