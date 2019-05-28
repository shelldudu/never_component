using Never.Attributes;
using Never.Commands;
using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class SqliteStartupExtension
    {
        #region sqlite eventprovider commandbus

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="recoveryStorager">批量事件保存接口</param>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "这个方法要提供SqliteFailRecoveryStorager，储存领域事件")]
        public static ApplicationStartup UseSqliteEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, SqliteFailRecoveryStorager recoveryStorager)
            where TCommandContext : ICommandContext
        {
            return UseSqliteEventProviderCommandBus<TCommandContext>(startup, recoveryStorager, EmptyEventStreamStorager.Empty);
        }

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="recoveryStorager">命令，事件初始化出错的保存接口</param>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "这个方法要提供SqliteFailRecoveryStorager，储存领域事件")]
        public static ApplicationStartup UseSqliteEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, SqliteFailRecoveryStorager recoveryStorager, IEventStorager eventStorager)
            where TCommandContext : ICommandContext
        {
            return UseSqliteEventProviderCommandBus<TCommandContext>(startup, recoveryStorager, eventStorager, EmptyCommandStreamStorager.Empty);
        }

        /// <summary>
        /// 启用命令事件发布模式,生命周期通常声明为单例
        /// </summary>
        /// <typeparam name="TCommandContext">命令上下文，如果使用内存模式，请配合MQ使用</typeparam>
        /// <param name="recoveryStorager">命令，事件初始化出错的保存接口</param>
        /// <param name="eventStorager">批量事件保存接口</param>
        /// <param name="commandStorager">命令信息储存</param>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        [Summary(Descn = "这个方法要提供SqliteFailRecoveryStorager，储存领域事件")]
        public static ApplicationStartup UseSqliteEventProviderCommandBus<TCommandContext>(this ApplicationStartup startup, SqliteFailRecoveryStorager recoveryStorager, IEventStorager eventStorager, ICommandStorager commandStorager)
            where TCommandContext : ICommandContext
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseSqliteEventProviderCommandBus"))
                return startup;

            /*注册发布事件*/
            startup.ServiceRegister.RegisterType(typeof(TCommandContext), typeof(ICommandContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterType(typeof(DefaultEventContext), typeof(IEventContext), string.Empty, ComponentLifeStyle.Transient);
            startup.ServiceRegister.RegisterType(typeof(SqliteCommandBus), typeof(ICommandBus), string.Empty, ComponentLifeStyle.Singleton);
            startup.ServiceRegister.RegisterInstance(recoveryStorager, typeof(SqliteFailRecoveryStorager), string.Empty);
            startup.ServiceRegister.RegisterInstance(eventStorager ?? EmptyEventStreamStorager.Empty, typeof(IEventStorager), string.Empty);
            startup.ServiceRegister.RegisterInstance(commandStorager ?? EmptyCommandStreamStorager.Empty, typeof(ICommandStorager), string.Empty);

            //注入handler类型的对象
            startup.UseInjectingCommandHandlerEventHandler(ComponentLifeStyle.Singleton);

            startup.Items["UseSqliteEventProviderCommandBus"] = "t";
            return startup;
        }

        #endregion sqlite eventprovider commandbus
    }
}