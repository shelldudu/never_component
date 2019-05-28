﻿using Memcached.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.MemCached
{
    /// <summary>
    /// Memcached缓存
    /// </summary>
    public sealed class Memcached : Never.Caching.ICaching, IDisposable
    {
        #region field

        /// <summary>
        /// memcached对象
        /// </summary>
        private readonly MemcachedClient cacheClient = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static Memcached()
        {
        }

        /// <summary>
        ///
        /// </summary>
        private Memcached()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="servers">每一个配置如下127.0.0.1:11211</param>
        /// <param name="socketConnectTimeout"></param>
        /// <param name="socketTimeout"></param>
        /// <param name="poolName"></param>
        /// <param name="defaultEncoding"></param>
        public Memcached(string[] servers
            , int socketConnectTimeout = 1000
            , int socketTimeout = 3000
            , string poolName = "BCB.MemCache"
            , string defaultEncoding = "UTF-8")
        {
            var sp = SockIOPool.GetInstance(poolName);
            sp.SetServers(servers);
            sp.SocketConnectTimeout = socketConnectTimeout;
            sp.SocketTimeout = socketTimeout;
            if (!sp.Initialized)
                sp.Initialize();

            cacheClient = new MemcachedClient()
            {
                EnableCompression = true,
                PoolName = poolName,
                DefaultEncoding = defaultEncoding,
            };
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
            return Get(key, itemMissCallBack, TimeSpan.FromMinutes(10));
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

            object obj = cacheClient.Get(key);
            if (obj == null)
            {
                if (itemMissCallBack == null)
                    return default(T);

                obj = itemMissCallBack();
                Set(key, obj, ts);
            }
            return (T)obj;
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

            cacheClient.Delete(key);
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
            return Set<T>(key, obj, TimeSpan.Zero);
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
            success = cacheClient.Set(key, obj, DateTime.Now.Add(ts));
            return success;
        }

        #endregion ICaching 成员

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        public void Dispose()
        {
        }

        #endregion IDisposable成员
    }
}