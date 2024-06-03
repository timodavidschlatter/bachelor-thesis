using System;
using System.Text.Json.Serialization;

namespace BUDSharedCore.Persistence.Context
{
    public class DbContextInfo
    {
        private readonly string _name;
        private readonly Type _type;

        public DbContextInfo(string name, Type type)
        {
            _name = name;
            _type = type;
        }
        public string Name { get => _name; }
        [JsonIgnore]
        public Type Type { get => _type; }
    }
}
