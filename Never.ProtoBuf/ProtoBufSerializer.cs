using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NonGeneric = ProtoBuf.Serializer.NonGeneric;

namespace Never.ProtoBuf
{
    /// <summary>
    /// ProtoBuf序列化接口
    /// </summary>
    public struct ProtoBufSerializer : Never.Serialization.IBinarySerializer
    {
        #region IBinarySerializer

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="graph">要序列化的对象或对象图形的根。将自动序列化此根对象的所有子对象。</param>
        /// <returns></returns>
        public byte[] SerializeObject(object graph)
        {
            if (graph == null)
            {
                return null;
            }

            using (var st = new MemoryStream())
            {
                Serializer.Serialize(st, graph);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <typeparam name="T">目标对象</typeparam>
        /// <param name="graph">要序列化的对象或对象图形的根。将自动序列化此根对象的所有子对象。</param>
        /// <returns></returns>
        public byte[] Serialize<T>(T graph)
        {
            using (var st = new MemoryStream())
            {
                Serializer.Serialize<T>(st, graph);
                st.Position = 0;
                return st.ToArray();
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="buffer">源字符串</param>
        /// <returns></returns>
        public T Deserialize<T>(byte[] buffer)
        {
            if (buffer == null)
            {
                return default(T);
            }

            using (var st = new MemoryStream(buffer))
            {
                return Serializer.Deserialize<T>(st);
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="buffer">源字符串</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public object DeserializeObject(byte[] buffer, Type targetType)
        {
            if (buffer == null)
            {
                return null;
            }

            using (var st = new MemoryStream(buffer))
            {
                return NonGeneric.Deserialize(targetType, st);
            }
        }

        #endregion IBinarySerializer
    }
}