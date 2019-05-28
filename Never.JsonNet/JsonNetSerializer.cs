using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.JsonNet
{

    public struct JsonNetSerializer : Never.Serialization.IJsonSerializer
    {
        #region 序列化

        /// <summary>
        ///
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public string SerializeObject(object @object)
        {
            if (@object == null)
                return string.Empty;

            return JsonConvert.SerializeObject(@object, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public string Serialize<T>(T @object)
        {
            if (@object == null)
                return string.Empty;

            return JsonConvert.SerializeObject(@object, new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }

        #endregion 序列化

        #region 反序列化

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public T Deserialize<T>(string input)
        {
            if (string.IsNullOrEmpty(input))
                return default(T);

            return JsonConvert.DeserializeObject<T>(input);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object DeserializeObject(string input, Type targetType)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            return JsonConvert.DeserializeObject(input, targetType);
        }

        #endregion 反序列化
    }
}