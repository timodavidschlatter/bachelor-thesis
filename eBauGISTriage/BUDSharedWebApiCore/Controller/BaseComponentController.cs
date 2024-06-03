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
using System.Text;
using System.Threading.Tasks;

namespace BUDSharedWebApiCore.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiIgnore]
    public class BaseComponentController : APIControllerBase<EnvironmentDbContext>
    {
        public BaseComponentController(EnvironmentDbContext ctx, IHttpContextAccessor contextAccessor, ILogger<BaseComponentController> logger) : base(ctx, contextAccessor, logger)
        {
        }

        [HttpDelete("DeleteGridConfiguration")]
        public HttpResponseMessage DeleteGridConfiguration(string id, string configName)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();

                Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configName).FirstOrDefault();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (config != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                    if (list != null)
                    {
                        GridConfig c = list.Where(o => o.Id == Guid.Parse(id)).FirstOrDefault();
                        if (c != null)
                        {
                            list.Remove(c);
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                            _ctx.Entry(config).State = EntityState.Modified;
                            _ctx.SaveChanges();
                        }
                    }
                }
                if (configUser != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(configUser.ValueData));
                    if (list != null)
                    {
                        GridConfig c = list.Where(o => o.Id == Guid.Parse(id)).FirstOrDefault();
                        if (c != null)
                        {
                            list.Remove(c);
                            configUser.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                            _ctx.Entry(configUser).State = EntityState.Modified;
                            _ctx.SaveChanges();
                        }
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing DeleteGridConfiguration", ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("SaveGridConfiguration")]
        public HttpResponseMessage SaveGridConfiguration([FromBody] GridConfigParams configParams)
        {
            try
            {
                configParams.Config.Protected = configParams.PrivateConfig;
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                if (configParams.Config.Id == null)
                {
                    configParams.Config.Id = Guid.NewGuid();
                    if (!configParams.PrivateConfig)
                    {
                        Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configParams.ConfigName).FirstOrDefault();
                        if (config == null)
                        {
                            config = new Configuration();
                            config.Key = "GridConfig." + configParams.ConfigName;
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new List<GridConfig> { configParams.Config }));
                            _ctx.Add(config);
                        }
                        else
                        {
                            List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                            list.Add(configParams.Config);
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                            _ctx.Entry(config).State = EntityState.Modified;
                        }
                    }
                    else
                    {
                        Usersettings config = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configParams.ConfigName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();
                        if (config == null)
                        {
                            config = new Usersettings();
                            config.Key = "GridConfig." + configParams.ConfigName;
                            config.Userlogin = userAlias;
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new List<GridConfig> { configParams.Config }));
                            _ctx.Add(config);
                        }
                        else
                        {
                            List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                            list.Add(configParams.Config);
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                            _ctx.Entry(config).State = EntityState.Modified;
                        }
                    }
                    _ctx.SaveChanges();
                }
                else
                {
                    if (!configParams.PrivateConfig)
                    {
                        Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configParams.ConfigName).FirstOrDefault();
                        if (config != null)
                        {
                            List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));

                            GridConfig oldConfig = list.Where(o => o.Id == configParams.Config.Id).FirstOrDefault();
                            if (oldConfig != null)
                            {
                                oldConfig.GridState = configParams.Config.GridState;
                                config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                                _ctx.Entry(config).State = EntityState.Modified;
                            }
                        }
                    }
                    else
                    {
                        Usersettings config = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configParams.ConfigName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();
                        if (config != null)
                        {
                            List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));

                            GridConfig oldConfig = list.Where(o => o.Id == configParams.Config.Id).FirstOrDefault();
                            if (oldConfig != null)
                            {
                                oldConfig.GridState = configParams.Config.GridState;
                                config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                                _ctx.Entry(config).State = EntityState.Modified;
                            }
                        }
                    }
                    _ctx.SaveChanges();
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing SaveGridConfiguration", ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet("GetGridConfiguration")]
        public List<GridConfig> GetGridConfiguration(string configName)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();

                List<GridConfig> configList = new List<GridConfig>();
                Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configName).FirstOrDefault();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (config != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                    configList.AddRange(list);
                }
                if (configUser != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(configUser.ValueData));
                    configList.AddRange(list);
                }

                foreach (GridConfig gridConfig in configList)
                {
                    if (gridConfig.Protected)
                    {
                        gridConfig.DisplayText = gridConfig.DisplayText + " (Privat)";
                    }
                    else
                    {
                        gridConfig.DisplayText = gridConfig.DisplayText + " (Öffentlich)";
                    }
                    if (!string.IsNullOrEmpty(gridConfig.GridState))
                    {
                        gridConfig.GridState = gridConfig.GridState.Replace("%currentuser%", userAlias);
                    }
                }

                return configList;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing GetGridConfiguration", ex);
                return null;
            }
        }

        [HttpGet("GetStandardGridConfiguration")]
        public GridConfig GetStandardGridConfiguration(string configName)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();

                List<GridConfig> configList = new List<GridConfig>();
                Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configName).FirstOrDefault();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (configUser != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(configUser.ValueData));
                    configList.AddRange(list);
                }
                if (config != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                    configList.AddRange(list);
                }

                GridConfig result = configList.Where(o => o.Standard).FirstOrDefault();
                if (result == null)
                {
                    result = configList.FirstOrDefault();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing GetStandardGridConfiguration", ex);
                return null;
            }
        }

        [HttpPost("SetStandardGridConfiguration")]
        public bool SetStandardGridConfiguration([FromBody] GridConfigParamsSimple configParams)
        {
            bool result = false;
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();

                Configuration config = _ctx.Configuration.Where(o => o.Key == "GridConfig." + configParams.ConfigName).FirstOrDefault();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "GridConfig." + configParams.ConfigName && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (configUser != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(configUser.ValueData));
                    // clear all private entries
                    list.ForEach(o =>
                    {
                        o.Standard = false;
                    });

                    // set the new entry as default if applicable
                    GridConfig gridConfig = list.Where(o => o.Id == Guid.Parse(configParams.ConfigId)).FirstOrDefault();
                    if (gridConfig != null)
                    {
                        gridConfig.Standard = true;
                        result = true;
                    }

                    configUser.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                    _ctx.Entry(configUser).State = EntityState.Modified;
                    _ctx.SaveChanges();
                }
                if (config != null)
                {
                    List<GridConfig> list = JsonConvert.DeserializeObject<List<GridConfig>>(Encoding.UTF8.GetString(config.ValueData));
                    GridConfig gridConfig = list.Where(o => o.Id == Guid.Parse(configParams.ConfigId)).FirstOrDefault();
                    if (gridConfig != null)
                    {
                        if (User.IsInRole("Power"))
                        {
                            list.ForEach(o =>
                            {
                                o.Standard = false;
                            });
                            gridConfig.Standard = true;
                            config.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                            _ctx.Entry(config).State = EntityState.Modified;
                            _ctx.SaveChanges();
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing GetStandardGridConfiguration", ex);
                return false;
            }
        }
    }
}

