using BUDSharedCore.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BUDSharedWebApiCore.Controller
{
    public abstract class APIControllerBase<T> : ControllerBase where T : BaseDbContext
    {
        protected T _ctx;
        protected readonly ILogger _logger;

        public APIControllerBase(T ctx, IHttpContextAccessor contextAccessor, ILogger logger)
        {
            this._ctx = ctx;
            this._logger = logger;

            if (this._ctx != null)
            {
                _ctx.ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }
    }
}
