using Never.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.RedisCache
{
    /// <summary>
    ///
    /// </summary>
    public sealed class DatabaseRedis : Never.Caching.ICaching
    {
        #region field

        /// <summary>
        /// memcached对象
        /// </summary>
        private readonly ConnectionMultiplexer redis = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IJsonSerializer jsonSerializer = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static DatabaseRedis()
        {
        }

        /// <summary>
        ///
        /// </summary>
        private DatabaseRedis()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        /// <param name="jsonSerializer"></param>
        public DatabaseRedis(string config, IJsonSerializer jsonSerializer)
            : this(ConfigurationOptions.Parse(config), jsonSerializer)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="configOptions"></param>
        /// <param name="jsonSerializer"></param>
        public DatabaseRedis(ConfigurationOptions configOptions, IJsonSerializer jsonSerializer)
        {
            this.redis = ConnectionMultiplexer.Connect(configOptions);
            this.jsonSerializer = jsonSerializer;
        }

        #endregion ctor

        #region ICaching 成员

        /// <summary>
        /// 从缓存中获取某一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Get<T>(key, null);
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> itemMissCallBack)
        {
            return Get<T>(key, itemMissCallBack, TimeSpan.FromMinutes(10));
        }

        /// <summary>
        /// 从缓存中获取某一项，如果没有命中，即调用CachingMissItemCallBack中获得值并将其加入缓存中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键值</param>
        /// <param name="itemMissCallBack">没有命中后的回调方法</param>
        /// <param name="ts">成功回调后插入缓存中过期时间</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> itemMissCallBack, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            var db = redis.GetDatabase();

            var json = (string)db.StringGet(key);
            if (json == null)
            {
                if (itemMissCallBack == null)
                    return default(T);

                T item = itemMissCallBack();
                Set(key, item, ts);
            }
            return jsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 从缓存中删除某一项
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            var db = redis.GetDatabase();
            db.KeyDelete(key);
        }

        /// <summary>
        /// 向缓存中插入某一项，默认为10分钟过期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键值</param>
        /// <param name="obj">要插入的值</param>
        /// <returns></returns>
        public bool Set<T>(string key, T obj)
        {
            return Set(key, obj, TimeSpan.Zero);
        }

        /// <summary>
        /// 向缓存中插入某一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键值</param>
        /// <param name="obj">要插入的值</param>
        /// <param name="ts">缓存中过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T obj, TimeSpan ts)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("缓存的key不能为空");

            if (obj == null)
                return false;

            bool success = false;
            if (ts <= TimeSpan.Zero)
                ts = TimeSpan.FromMinutes(10);

            var db = redis.GetDatabase();
            var json = jsonSerializer.Serialize(obj);
            success = db.StringSet(key, json);
            return success;
        }

        #endregion ICaching 成员

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Dispose()
        {
            if (redis != null)
                redis.Dispose();
        }

        #endregion IDisposable成员
    }
}