using BUDSharedCore.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace eBauGISTriageApi.Persistence
{
    /// <summary>
    /// Represents the database context for the Gdwh database.
    /// </summary>
    public class GdwhCtx : BaseDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GdwhCtx"/> class.
        /// </summary>
        /// <param name="options">The options for configuring the database context.</param>
        /// <param name="config">The configuration for the database context.</param>
        public GdwhCtx(DbContextOptions<GdwhCtx> options, DbContextConfig<GdwhCtx> config)
            : base(options, config)
        {
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }
    }
}
