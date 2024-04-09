using System;
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
                    if (request.GetValue(prop.Name, StringComparison.OrdinalIgnoreCase) == null)
                    {
                        return null;
                    }
                }

                return JsonConvert.DeserializeObject<T>(request.ToString());
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
