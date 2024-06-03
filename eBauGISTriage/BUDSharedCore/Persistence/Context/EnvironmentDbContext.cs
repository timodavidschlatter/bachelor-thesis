using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BUDSharedCore.Persistence.Context
{
    public class EnvironmentDbContext : CodetableDbContext
    {
        public EnvironmentDbContext(DbContextOptions options,BaseDbContextConfig config) : base(options, config)
        {
        }

        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Usersettings> Usersettings { get; set; }

        public static new DbContextInfo GetDbContextInfo()
        {
            return new DbContextInfo("Environment", typeof(EnvironmentDbContext));
        }
    }
}
