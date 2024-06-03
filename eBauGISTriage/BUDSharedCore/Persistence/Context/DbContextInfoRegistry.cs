using System.Collections.Generic;

namespace BUDSharedCore.Persistence.Context
{
    public class DbContextInfoRegistry : List<DbContextInfo>
    {
        public DbContextInfoRegistry(List<DbContextInfo> entries)
        {
            this.AddRange(entries);
        }
    }
}
