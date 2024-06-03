using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using BUDSharedWebApiCore.DTO;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BUDSharedWebApiCore.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiIgnore]
    public class ConfigurationController : APIControllerBase<EnvironmentDbContext>
    {
        public ConfigurationController(EnvironmentDbContext ctx, IHttpContextAccessor contextAccessor, ILogger<CodetableController> logger) : base(ctx, contextAccessor, logger)
        {
        }

        [HttpGet]
        [OpenApiIgnore]
        [Authorize(Roles = "Admin")]
        public object GetAll(CaseInsensitiveDSLoadOptions loadOptions)
        {
            try
            {
                var data = (from q in _ctx.Configuration orderby q.Id select q);

                return DataSourceLoader.Load(data, loadOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("ConfigurationController.GetAll", ex);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Delete([FromForm] int key)
        {
            if (_ctx.Configuration.Where(o => o.Id == key).Any())
            {
                _ctx.Remove(new Configuration() { Id = key });
                _ctx.SaveChanges();
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Save([FromForm] string values)
        {
            Configuration data = null;
            try
            {
                data = JsonConvert.DeserializeObject<Configuration>(values);
                _ctx.Add(data);

                if (data.Id != 0)
                {
                    _ctx.Entry(data).State = EntityState.Modified;
                }
                _ctx.SaveChanges();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not save Configuration " + data?.Id, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Update([FromForm] int key, [FromForm] string values)
        {
            try
            {
                var objNew = JsonConvert.DeserializeObject<Configuration>(values);

                if (objNew != null)
                {
                    Configuration obj = _ctx.Configuration.Where(o => o.Id == key).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.Key = objNew.Key != null ? objNew.Key : obj.Key;
                        obj.ValueStr = objNew.ValueStr != null ? objNew.ValueStr : obj.ValueStr;
                        _ctx.Entry(obj).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not update Configuration " + key, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetFile")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        public async Task<FileContentResult> GetFile(int id)
        {
            Configuration confObj = _ctx.Configuration.Where(o => o.Id == id).FirstOrDefault();
            if (confObj != null)
            {
                return new FileContentResult(confObj.ValueData, "application/octet-stream")
                {
                    FileDownloadName = "file"
                };
            }
            return null;
        }

        [HttpPost("Upload")]
        [Authorize(Roles = "Admin")]
        public IActionResult Post(int id)
        {
            IFormFileCollection files = Request.Form.Files;
            string uniqueFileName = "";

            if (files.Count > 0)
            {
                try
                {
                    var file = files[0];

                    uniqueFileName = file.FileName;

                    Configuration confObj = _ctx.Configuration.Where(o => o.Id == id).FirstOrDefault();
                    if (confObj != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            confObj.ValueData = ms.ToArray();
                        }
                        _ctx.Entry(confObj).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Could not upload file for obj" + id, ex);
                    return BadRequest("Failed to save a file on the server");
                }
            }

            return Ok(uniqueFileName);
        }

        /*[HttpGet("GetGridConfiguration")]
        public List<GridConfig> GetGridConfiguration(string configName)
        {
            try
            {
                Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configName).FirstOrDefault();

                if (config != null)
                {
                    string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();

                    List<GridConfig> configList = JsonConvert.DeserializeObject<List<GridConfig>>(config.ValueStr);

                    if (configList != null)
                    {
                        configList?.ForEach(o => o.Columns?.ForEach(c => {
                            if (c.Filter != null)
                            {
                                for (int i = 0; i < c.Filter.Count; i++)
                                {
                                    if (c.Filter[i] != null && c.Filter[i].ToString().ToLower().Equals("%currentuser%"))
                                    {
                                        c.Filter[i] = userAlias;
                                    }
                                }
                            }
                        }));
                    }

                    return configList;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing GetGridConfiguration", ex);
                return null;
            }
        }*/
    }
}

