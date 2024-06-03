using System;
using System.Collections.Generic;
using System.Text;

namespace BUDSharedCore.Persistence.Context
{
    public abstract class BaseDbContextConfig
    {
        public readonly string Schema;

        public BaseDbContextConfig(string schema)
        {
            Schema = schema;
        }
    }

    /// <summary>
    /// we need this only so that we can have individual configuration in the dependency injection service for each db context
    /// </summary>
    /// <typeparam name="T">for which db context type is this configuration used</typeparam>
    public class DbContextConfig<T> : BaseDbContextConfig
    {
        public DbContextConfig(string schema) : base (schema)
        {
        }
    }
}
