using Newtonsoft.Json;

namespace DiscordJob
{
    public static class StringExtensions
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string TrySerialize<T>(this T @this)
        {
            return @this.TrySerialize(false);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public static string TrySerialize<T>(this T @this, bool ignoreNull)
        {
            var value = string.Empty;
            try
            {
                if (@this == null)
                    return value;

                var setting = new JsonSerializerSettings()
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    DateFormatString = "yyyy-MM-dd HH:mm:ss.fffff"
                };
                if (ignoreNull)
                    setting.NullValueHandling = NullValueHandling.Ignore;

                value = JsonConvert.SerializeObject(@this, setting);
            }
            catch (Exception ex)
            {

            }
            return value;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TrySerialize<T>(this T @this, out string value)
        {
            value = string.Empty;

            try
            {
                value = JsonConvert.SerializeObject(@this);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static T TryDeserialize<T>(this string @this, params JsonConverter[] converters)
        {
            if (string.IsNullOrWhiteSpace(@this))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(@this, converters);
            }
            catch (Exception ex)
            {

            }
            return default;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool TryDeserialize<T>(this string @this, out T t)
        {
            t = default;
            if (string.IsNullOrWhiteSpace(@this))
                return false;

            try
            {
                t = JsonConvert.DeserializeObject<T>(@this);
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T TryDeserialize<T>(this JsonReader @this)
        {
            try
            {
                var t = JsonSerializer.CreateDefault().Deserialize<T>(@this);
                return t;
            }
            catch { }
            return default;
        }
    }
}
