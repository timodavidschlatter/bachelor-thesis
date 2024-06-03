using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Tasks;

namespace BUDSharedWebApiCore.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiIgnore]
    public class CodetableController : APIControllerBase<CodetableDbContext>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly DbContextInfoRegistry _dbContextInfoRegistry;

        public CodetableController(IServiceScopeFactory serviceScopeFactory, DbContextInfoRegistry dbContextInfoRegistry, IHttpContextAccessor contextAccessor, ILogger<CodetableController> logger)
            : base(null, contextAccessor, logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _dbContextInfoRegistry = dbContextInfoRegistry;
        }

        [HttpGet]
        [OpenApiIgnore]
        [Authorize(Roles = "Admin")]
        public object GetAll(string source, CaseInsensitiveDSLoadOptions loadOptions)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;
                    var data = (from q in ctx.Codetable orderby q.Id select q).AsNoTracking().ToList();

                    return DataSourceLoader.Load(data, loadOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("CodetableController.GetAll", ex);
            }
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public object GetAllTest(string source)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;
                    var data = (from q in ctx.Codetable orderby q.Id select q).AsNoTracking().ToList();

                    return DataSourceLoader.Load(data, new DataSourceLoadOptionsBase());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("CodetableController.GetAllTest", ex);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Delete([FromForm] int key, [FromForm] string source)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;
                if (ctx.Codetable.Where(o => o.Id == key).Any())
                {
                    ctx.Remove(new Codetable() { Id = key });
                    ctx.SaveChanges();
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Save([FromForm] string values, [FromForm] string source)
        {
            Codetable data = null;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;
                    data = JsonConvert.DeserializeObject<Codetable>(values);
                    ctx.Add(data);

                    if (data.Id != 0)
                    {
                        ctx.Entry(data).State = EntityState.Modified;
                    }
                    ctx.SaveChanges();
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not save Codetable " + data?.Id, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Update([FromForm] int key, [FromForm] string values, [FromForm] string source)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;

                    var objNew = JsonConvert.DeserializeObject<Codetable>(values);

                    if (objNew != null)
                    {
                        Codetable obj = ctx.Codetable.Where(o => o.Id == key).FirstOrDefault();
                        if (obj != null)
                        {
                            obj.Abbreviation = objNew.Abbreviation != null ? objNew.Abbreviation : obj.Abbreviation;
                            obj.CodeType = objNew.CodeType != null ? objNew.CodeType : obj.CodeType;
                            obj.Internal = objNew.Internal != null ? objNew.Internal : obj.Internal;
                            obj.Meaning = objNew.Meaning != null ? objNew.Meaning : obj.Meaning;
                            obj.SortOrder = objNew.SortOrder;
                            obj.State = objNew.State != null ? objNew.State : obj.State;
                            ctx.Entry(obj).State = EntityState.Modified;
                            ctx.SaveChanges();
                        }
                    }
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not update Codetable " + key, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetCodetableSources")]
        public object GetCodetableSources(CaseInsensitiveDSLoadOptions loadOptions)
        {
            try
            {
                return DataSourceLoader.Load(from x in _dbContextInfoRegistry select new { x.Name, x.Type.FullName }, loadOptions); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("CodetableController.GetAllCodeTypes", ex);
            }
        }

        [HttpGet("GetAllCodeTypes")]
        public List<string> GetAllCodeTypes(string source)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;
                    return (from q in ctx.Codetable group q by q.CodeType into g orderby g.Key select g.Key).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("CodetableController.GetAllCodeTypes", ex);
            }
        }

        [HttpGet("GetCodes")]
        public List<Codetable> GetCodes(string source, string codeType)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService(_dbContextInfoRegistry.Single(o => o.Name.Equals(source)).Type) as CodetableDbContext;

                    return ctx.Codetable.Where(o => o.CodeType == codeType).OrderBy(o => o.Abbreviation).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("CodetableController.GetCodes", ex);
            }
        }

    }
}

