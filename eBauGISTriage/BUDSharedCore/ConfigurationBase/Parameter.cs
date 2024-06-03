using System;


namespace BUDSharedCore.ConfigurationBase
{
    public class Parameter<T> : IParameter where T : IConvertible
    {
        private ParameterType<T> _type;
        private ICentralConfig _config;

        public Parameter(ParameterType<T> type, ICentralConfig config)
        {
            _type = type;
            _config = config;
        }

        public string Key
        {
            get
            {
                return _type.Key;
            }
        }

        public T Value
        {
            get
            {
                return (T)Convert.ChangeType(_config.GetValue(Key, _type.DefaultValue.ToString()), typeof(T));
            }
        }
    }
}
