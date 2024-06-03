using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BUDSharedCore.ConfigurationBase
{
    public class ParameterDefBase
    {
        protected readonly ICentralConfig _config;

        public ParameterDefBase(ICentralConfig config)
        {
            _config = config;
        }

        public List<IParameterType> AllParameterTypes
        {
            get
            {
                Type type = GetType();
                FieldInfo[] fields = type.GetFields();

                var staticFields = from field in fields
                                   where field.IsStatic && typeof(IParameterType).IsAssignableFrom(field.FieldType)
                                   select (IParameterType)field.GetValue(this);

                return staticFields.ToList();
            }
        }
    }
}
