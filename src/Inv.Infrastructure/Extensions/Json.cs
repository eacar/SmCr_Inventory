using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Inv.Infrastructure.Extensions
{
    public static class JsonExtensions
    {
        public static T FromJson<T>(this string jsonString)
        {
            try
            {
                return (T)JsonConvert.DeserializeObject(jsonString, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"JsonString: {jsonString}", ex);
            }
        }
        public static string ToJson(this object[] objs)
        {
            if (objs != null && objs.Length > 0)
                return JsonConvert.SerializeObject(objs);

            return string.Empty;
        }
        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            settings = settings ?? new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}