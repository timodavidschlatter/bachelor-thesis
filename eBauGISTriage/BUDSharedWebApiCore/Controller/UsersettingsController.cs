using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSwag.Annotations;
using System;
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
    public class UsersettingsController : APIControllerBase<EnvironmentDbContext>
    {
        public UsersettingsController(EnvironmentDbContext ctx, IHttpContextAccessor contextAccessor, ILogger<UsersettingsController> logger) : base(ctx, contextAccessor, logger)
        {
        }

        [HttpGet]
        [OpenApiIgnore]
        [Authorize(Roles = "Admin")]
        public object GetAll(CaseInsensitiveDSLoadOptions loadOptions)
        {
            try
            {
                var data = (from q in _ctx.Usersettings orderby q.Id select q);

                return DataSourceLoader.Load(data, loadOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while querying database");
                throw new Exception("UsersettingsController.GetAll", ex);
            }
        }

        [HttpGet("MyStringSetting")]
        public async Task<string> GetMyStringSetting(string key)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings data = await GetSetting(key, userAlias);
                if(data.Id == 0)
                {
                    return null;
                }

                return data.ValueStr;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Could not GetMySetting({key})");
                throw new Exception($"UsersettingsController.GetMySetting({key})", ex);
            }
        }

        private async Task<Usersettings> GetSetting(string key, string userAlias)
        {
            Usersettings result = await _ctx.Usersettings.Where(o => o.Key == key && o.Userlogin == userAlias).FirstOrDefaultAsync();
            if (result == null)
            {
                result = new Usersettings() { Key = key, Userlogin = userAlias };
            }
            return result;
        }

        [HttpPost("SaveMySetting")]
        public async Task<HttpResponseMessage> SaveMySetting(string key, [FromBody] string value)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings data = await GetSetting(key, userAlias);
                data.ValueStr = value;

                _ctx.Add(data);

                if (data.Id != 0)
                {
                    _ctx.Entry(data).State = EntityState.Modified;
                }
                await _ctx.SaveChangesAsync();
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not save SaveMySetting({key})", ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Delete([FromForm] int key)
        {
            if (_ctx.Usersettings.Where(o => o.Id == key).Any())
            {
                _ctx.Remove(new Usersettings() { Id = key });
                _ctx.SaveChanges();
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Save([FromForm] string values)
        {
            Usersettings data = null;
            try
            {
                data = JsonConvert.DeserializeObject<Usersettings>(values);
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
                _logger.LogError("Could not save Usersettings " + data?.Id, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Update([FromForm] int key, [FromForm] string values)
        {
            try
            {
                var objNew = JsonConvert.DeserializeObject<Usersettings>(values);

                if (objNew != null)
                {
                    Usersettings obj = _ctx.Usersettings.Where(o => o.Id == key).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.Key = objNew.Key != null ? objNew.Key : obj.Key;
                        obj.Userlogin = objNew.Userlogin != null ? objNew.Userlogin : obj.Userlogin;
                        obj.ValueStr = objNew.ValueStr != null ? objNew.ValueStr : obj.ValueStr;
                        _ctx.Entry(obj).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not update Usersettings " + key, ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("GetFile")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        public async Task<FileContentResult> GetFile(int id)
        {
            Usersettings obj = _ctx.Usersettings.Where(o => o.Id == id).FirstOrDefault();
            if (obj != null)
            {
                return new FileContentResult(obj.ValueData, "application/octet-stream")
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

                    Usersettings obj = _ctx.Usersettings.Where(o => o.Id == id).FirstOrDefault();
                    if (obj != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            obj.ValueData = ms.ToArray();
                        }
                        _ctx.Entry(obj).State = EntityState.Modified;
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
    }
}

