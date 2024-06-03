using System;
using System.Text.Json;

namespace BUDSharedCore.Helpers
{
    public static class JsonHelper
    {
        public static void PopulateObject<T>(T target, string jsonSource) where T : class =>
            PopulateObject(target, jsonSource, typeof(T));

        public static void OverwriteProperty<T>(T target, JsonProperty updatedProperty) where T : class =>
            OverwriteProperty(target, updatedProperty, typeof(T));

        public static void PopulateObject(object target, string jsonSource, Type type)
        {
            var json = JsonDocument.Parse(jsonSource).RootElement;

            foreach (var property in json.EnumerateObject())
            {
                OverwriteProperty(target, property, type);
            }
        }

        public static void OverwriteProperty(object target, JsonProperty updatedProperty, Type type)
        {
            var propertyInfo = type.GetProperty(updatedProperty.Name);

            if (propertyInfo == null)
            {
                return;
            }

            var propertyType = propertyInfo.PropertyType;
            object? parsedValue;

            if (propertyType.IsValueType || propertyType == typeof(string))
            {
                parsedValue = JsonSerializer.Deserialize(
                    updatedProperty.Value.GetRawText(),
                    propertyType);
            }
            else
            {
                parsedValue = propertyInfo.GetValue(target);
                if (parsedValue != null)
                {
                    PopulateObject(
                        parsedValue,
                        updatedProperty.Value.GetRawText(),
                        propertyType);
                }
            }

            propertyInfo.SetValue(target, parsedValue);
        }
    }
}
