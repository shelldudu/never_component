using Never.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// sqlite数据执行者
    /// </summary>
    public class SqliteExecuter : Never.SqlClient.SqliteExecuter, ISqlExecuter, ITransactionExecuter
    {
        #region ctor

#if !NET461

        /// <summary>
        /// 
        /// </summary>
        static SqliteExecuter()
        {
            SQLitePCL.Batteries.Init();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(string connectionString)
            : base(Microsoft.Data.Sqlite.SqliteFactory.Instance, connectionString)
        {
           
        }
#else

        /// <summary>
        /// 
        /// </summary>
        static SqliteExecuter()
        {
            SQLitePCL.Batteries.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(string connectionString)
            : base(System.Data.SQLite.SQLiteFactory.Instance, connectionString)
        {
        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteExecuter"/> class.
        /// </summary>
        /// <param name="provider">驱动</param>
        /// <param name="connectionString">连接字符串.</param>
        public SqliteExecuter(DbProviderFactory provider, string connectionString)
            : base(provider, connectionString)
        {
        }

        #endregion ctor
    }
}