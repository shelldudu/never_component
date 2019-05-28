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
    /// ProtoBuf序列化接口,目前只对消息作处理
    /// </summary>
    public struct MessageSerializer : Never.Serialization.IBinarySerializer
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

            if (graph is Never.Messages.MessagePacket)
            {
                var ntmsg = graph as Never.Messages.MessagePacket;
                return this.Serialize(new MessagePacket()
                {
                    ContentType = ntmsg.ContentType,
                    Body = ntmsg.Body
                });
            }

            if (graph is MessagePacket)
            {
                return this.Serialize((MessagePacket)graph);
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
            if (graph is Never.Messages.MessagePacket)
            {
                var ntmsg = graph as Never.Messages.MessagePacket;
                return this.Serialize(new MessagePacket()
                {
                    ContentType = ntmsg.ContentType,
                    Body = ntmsg.Body
                });
            }

            if (graph is MessagePacket)
            {
                return this.Serialize(graph as MessagePacket);
            }

            using (var st = new MemoryStream())
            {
                Serializer.Serialize(st, graph);
                st.Position = 0;
                return st.ToArray();
            }
        }

        private byte[] Serialize(MessagePacket message)
        {
            using (var st = new MemoryStream())
            {
                Serializer.Serialize(st, message);
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
            if (buffer == null || buffer.Length == 0)
            {
                return default(T);
            }

            if (typeof(T) == typeof(Never.Messages.MessagePacket))
            {
                var msg = this.DeserializeMessagePacket(buffer);
                var org = new Never.Messages.MessagePacket()
                {
                    Body = msg.Body,
                    ContentType = msg.ContentType,
                };

                return (T)(object)org;
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

            if (targetType == typeof(Never.Messages.MessagePacket))
            {
                var msg = this.DeserializeMessagePacket(buffer);
                var org = new Never.Messages.MessagePacket()
                {
                    Body = msg.Body,
                    ContentType = msg.ContentType,
                };

                return org;
            }

            using (var st = new MemoryStream(buffer))
            {
                return NonGeneric.Deserialize(targetType, st);
            }
        }

        private MessagePacket DeserializeMessagePacket(byte[] buffer)
        {
            using (var st = new MemoryStream(buffer))
            {
                return Serializer.Deserialize<MessagePacket>(st);
            }
        }

        #endregion IBinarySerializer

        #region nested

        /// <summary>
        /// 消息
        /// </summary>
        [ProtoContract]
        public class MessagePacket
        {
            #region prop

            /// <summary>
            /// 消息对象的具体类型
            /// </summary>
            [ProtoMember(1)]
            public string ContentType { get; set; }

            /// <summary>
            /// 消息对象的主体,通常是json对象
            /// </summary>
            [ProtoMember(2)]
            public string Body { get; set; }

            #endregion prop
        }

        #endregion nested
    }
}