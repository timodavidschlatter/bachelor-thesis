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
    public class FavoriteController : APIControllerBase<EnvironmentDbContext>
    {
        public FavoriteController(EnvironmentDbContext ctx, IHttpContextAccessor contextAccessor, ILogger<FavoriteController> logger) : base(ctx, contextAccessor, logger)
        {
        }


        [HttpPost("ModifyFavorite")]
        public HttpResponseMessage ModifyFavorite([FromBody] Favorite fav)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "Favorites" && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (!fav.Id.HasValue)
                {
                    fav.Id = Guid.NewGuid();
                }

                if (configUser != null)
                {
                    List<Favorite> list = JsonConvert.DeserializeObject<List<Favorite>>(Encoding.UTF8.GetString(configUser.ValueData));
                    if (list != null)
                    {
                        if (list.Any(o => o.Path == fav.Path))
                        {
                            list.Remove(list.Find(o => o.Path == fav.Path));
                        }
                        else
                        {
                            list.Add(fav);
                        }
                        configUser.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                        _ctx.Entry(configUser).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                }
                else
                {
                    configUser = new Usersettings();
                    configUser.Userlogin = userAlias;
                    configUser.Key = "Favorites";

                    List<Favorite> favs = new List<Favorite>();
                    favs.Add(fav);
                    configUser.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(favs));
                    _ctx.Entry(configUser).State = EntityState.Added;
                    _ctx.SaveChanges();
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing ModifyFavorite", ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("ModifyFavoriteTitle")]
        public HttpResponseMessage ModifyFavoriteTitle([FromBody] Favorite fav)
        {
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "Favorites" && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (configUser != null)
                {
                    List<Favorite> list = JsonConvert.DeserializeObject<List<Favorite>>(Encoding.UTF8.GetString(configUser.ValueData));
                    if (list != null)
                    {
                        if (list.Any(o => o.Id == fav.Id))
                        {
                            list.Where(o => o.Id == fav.Id).First().Title = fav.Title;
                        }
                        
                        configUser.ValueData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(list));
                        _ctx.Entry(configUser).State = EntityState.Modified;
                        _ctx.SaveChanges();
                    }
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing ModifyFavoriteTitle", ex);
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        [HttpPost("IsFavorite")]
        public bool IsFavorite([FromBody] Favorite fav)
        {
            bool result = false;
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "Favorites" && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (configUser != null)
                {
                    List<Favorite> list = JsonConvert.DeserializeObject<List<Favorite>>(Encoding.UTF8.GetString(configUser.ValueData));
                    if (list != null)
                    {
                        if (list.Any(o => o.Path == fav.Path))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing IsFavorite", ex);
            }
            return result;
        }

        [HttpGet("GetFavorites")]
        public List<Favorite> GetFavorites()
        {
            List<Favorite> result = new List<Favorite>();
            try
            {
                string userAlias = User.FindFirstValue(ClaimTypes.NameIdentifier).ToLower();
                Usersettings configUser = _ctx.Usersettings.Where(o => o.Key == "Favorites" && o.Userlogin.ToLower() == userAlias).FirstOrDefault();

                if (configUser != null)
                {
                    result = JsonConvert.DeserializeObject<List<Favorite>>(Encoding.UTF8.GetString(configUser.ValueData));
                    if (result != null)
                    {
                        result = result.OrderBy(o => o.Title).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured executing GetFavorites", ex);
            }
            return result;
        }
    }
}

