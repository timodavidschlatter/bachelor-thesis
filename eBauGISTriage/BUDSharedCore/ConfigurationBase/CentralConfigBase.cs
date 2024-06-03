
using BUDSharedCore.Persistence.Context;
using BUDSharedCore.Persistence.Model.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace BUDSharedCore.ConfigurationBase
{
    public class CentralConfigBase : ICentralConfig
    {
        private readonly ILogger<CentralConfigBase> _logger;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CentralConfigBase(IConfiguration config, ILogger<CentralConfigBase> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _config = config;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public string CurrentUser { get { return _config.GetValue<string>("USERNAME"); } }

        public IConfiguration Config { get { return _config; } }

        protected ILogger Logger { get { return _logger; } }
        protected IServiceScopeFactory ServiceScopeFactory { get { return _serviceScopeFactory; } }

        public virtual string GetValue(string key, string defaultValue)
        {
            return GetFromConfigOrDB(key, defaultValue);
        }

        private string GetFromConfigOrDB(string key, string defaultValue = "")
        {
            _logger.LogTrace($"CentralConfig getting parameter {key}");
            string result = ReadConfig(@"Parameter", key);

            if (result == null)
            {
                result = ReadConfigFromDB(key, defaultValue);
            }
            return result;
        }

        public string ReadConfig(params string[] sectionNames)
        {
            string result = null;
            try
            {
                int idx = 0;
                int lastIdx = sectionNames.Count() - 1;
                IConfigurationSection section = _config.GetSection(sectionNames[idx]);

                while (idx < lastIdx)
                {
                    idx++;
                    section = section.GetSection(sectionNames[idx]);
                }

                if (section.Value != null)
                {
                    result = section.Value.Trim();
                    _logger.LogTrace($"found {string.Join('.', sectionNames)} = {result} in local configuration");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading {string.Join('.', sectionNames)} from local configuration");
            }
            return result;
        }

        private string ReadConfigFromDB(string key, string defaultValue)
        {
            string result = defaultValue;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService<EnvironmentDbContext>();
                    var entry = ctx.Configuration.SingleOrDefault(x => x.Key.ToUpper().Equals(key.Trim().ToUpper()));
                    if (entry != null)
                    {
                        if (entry.ValueStr != null)
                        {
                            result = entry.ValueStr.Trim();
                            _logger.LogTrace($"found {key} = {result} in database");
                        }

                        if (entry.ValueData != null)
                        {
                            result = Encoding.UTF8.GetString(entry.ValueData);
                            _logger.LogTrace($"found {key} = {result} in database");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(defaultValue))
                    {
                        defaultValue = defaultValue.Trim(); 
                        Configuration config = defaultValue.Length < 2000 ? new Configuration() { Key = key, ValueStr = defaultValue } : new Configuration() { Key = key, ValueData = Encoding.UTF8.GetBytes(defaultValue) };
                        ctx.Configuration.Add(config);
                        ctx.SaveChanges();
                        _logger.LogTrace($"added default value {result} for {key} to database");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while reading parameter {key} from database - using default value");
            }
            return result;
        }
    }

}
