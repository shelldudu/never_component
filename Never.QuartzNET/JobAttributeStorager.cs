using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.QuartzNET
{
    /// <summary>
    /// 当前Job特性存储
    /// </summary>
    public static class JobAttributeStorager
    {
        #region field

        /// <summary>
        /// 只有1个type的命令
        /// </summary>
        private static readonly Dictionary<Type, IEnumerable<Attribute>> one = new Dictionary<Type, IEnumerable<Attribute>>();

        #endregion field

        #region add

        /// <summary>
        /// Adds the specified jobTypeType type.
        /// </summary>
        /// <paramm name="jobTypeType">Type of the jobTypeType.</paramm>
        /// <paramm name="attributes">The attributes.</paramm>
        internal static void Add(Type jobType, IEnumerable<Attribute> attributes)
        {
            if (jobType == null)
                return;

            if (!one.ContainsKey(jobType))
                one.Add(jobType, attributes);
        }

        #endregion add

        #region value

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <paramm name="jobTypeType">Type of the jobTypeType.</paramm>
        /// <returns></returns>
        public static IEnumerable<Attribute> GetAttributes(Type jobType)
        {
            if (jobType == null)
                return new Attribute[0];

            IEnumerable<Attribute> result = null;
            if (one.TryGetValue(jobType, out result))
                return result;

            return new Attribute[0];
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="jobTypeType">Type of the jobTypeType.</paramm>
        /// <returns></returns>
        public static T GetAttribute<T>(Type jobType) where T : Attribute
        {
            var attributes = GetAttributes(jobType);
            for (var i = 0; i < attributes.Count(); i++)
            {
                var attribute = attributes.ElementAt(i) as T;
                if (attribute != null)
                    return attribute;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparamm name="T"></typeparamm>
        /// <paramm name="jobTypeType">Type of the jobTypeType.</paramm>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(Type jobType) where T : Attribute
        {
            var attributes = GetAttributes(jobType);
            for (var i = 0; i < attributes.Count(); i++)
            {
                var attribute = attributes.ElementAt(i) as T;
                if (attribute != null)
                    yield return attribute;
            }
        }

        #endregion value
    }
}