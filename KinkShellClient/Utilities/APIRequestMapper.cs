using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CatboyEngineering.KinkShellClient.Utilities
{
    public static class APIRequestMapper
    {
        public static T? MapRequestToModel<T>(JObject request) where T : struct
        {
            try
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (isNullable(prop))
                    {
                        continue;
                    }

                    if (request.GetValue(prop.Name, StringComparison.OrdinalIgnoreCase) == null)
                    {
                        Plugin.Logger.Error($"Expected {prop.Name}, but it was not found.");
                        return null;
                    }
                }

                return JsonConvert.DeserializeObject<T>(request.ToString());
            }
            catch (Exception e)
            {
                Plugin.Logger.Error(e.Message);

                return null;
            }
        }

        private static bool isNullable(PropertyInfo prop)
        {
            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
            {
                return true;
            }

            if (prop.CustomAttributes.Count() > 0)
            {
                return prop.CustomAttributes.First().AttributeType == typeof(System.Runtime.CompilerServices.NullableAttribute);
            }

            return false;
        }
    }
}