using System;
using System.Linq;
using System.Reflection;

namespace BUDSharedCore.ConfigurationBase
{
    public class ParameterType<T> : IParameterType where T : IConvertible
    {
        private static Type _type;

        private string _key;
        private T _defaultValue;
        private string _description;
        private T[] _validValueItems;

        public ParameterType(string key, T defaultValue, string description, params T[] validValueItems)
        {
            Type type = typeof(T);

            if (type != typeof(string) && type != typeof(int) && type != typeof(decimal) && type != typeof(bool) && type != typeof(byte[]))
            {
                throw new ArgumentException("only string , int, decimal, bool, byte[] are supported as parameter value");
            }
            _type = type;

            _key = key;
            _defaultValue = defaultValue;
            _description = description;
            _validValueItems = validValueItems;
        }

        public static implicit operator string(ParameterType<T> value)
        {
            return _type.FullName;
        }

        public override string ToString()
        {
            return this;
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public object[] ValidValueItems
        {
            get
            {
                return _validValueItems.Cast<object>().ToArray();
            }
        }

        public bool TryParse(string value, out T result)
        {
            MethodInfo methode = _type.GetMethod("TryParse", new Type[] { typeof(string), typeof(T).MakeByRefType() });
            bool success = true;

            if (methode != null)
            {
                object parseResult = DefaultValue;
                object[] arguments = new object[] { value, parseResult };

                success = (bool)methode.Invoke(null, arguments);

                result = (T)arguments[1];
            }
            else
            {
                throw new ArgumentException("type can not be parsed!");
            }

            return success;
        }

        public bool IsDataParam
        {
            get { return _type == typeof(byte[]); }
        }

        public bool IsStringParam
        {
            get { return _type == typeof(string); }
        }
    }
}
