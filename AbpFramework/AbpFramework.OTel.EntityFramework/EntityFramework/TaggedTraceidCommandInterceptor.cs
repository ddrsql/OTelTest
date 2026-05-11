using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AbpFramework.OTel.EntityFramework
{
    /// <summary>
    /// EF6 SQL 命令拦截器：为每条 SQL 操作创建独立的 OpenTelemetry 子 Span。
    /// 通过 DbCommandInterceptionContext.UserState 在 Executing/Executed 之间传递 Activity 引用，
    /// 实现准确的 SQL 执行耗时记录和异常状态标记。
    /// </summary>
    public class TaggedTraceidCommandInterceptor : DbCommandInterceptor
    {
        private static readonly ActivitySource _activitySource = new ActivitySource(OTelModule.AspNetSourceName);

        // ---- Reader（查询类，返回结果集）----

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            StartSpan(command, nameof(ReaderExecuting), interceptionContext);
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            StopSpan(interceptionContext);
            base.ReaderExecuted(command, interceptionContext);
        }

        // ---- NonQuery（非查询类，返回受影响行数）----

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            StartSpan(command, nameof(NonQueryExecuting), interceptionContext);
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            StopSpan(interceptionContext);
            base.NonQueryExecuted(command, interceptionContext);
        }

        // ---- Scalar（标量查询，返回单个值）----

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            StartSpan(command, nameof(ScalarExecuting), interceptionContext);
            base.ScalarExecuting(command, interceptionContext);
        }

        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            StopSpan(interceptionContext);
            base.ScalarExecuted(command, interceptionContext);
        }

        /// <summary>
        /// SQL 执行前：创建子 Activity，解析 SQL 设置 DisplayName，注入 TraceId 注释到 SQL 语句。
        /// 将 Activity 存入 interceptionContext.UserState，供 Executed 中取出。
        /// </summary>
        private static void StartSpan<TResult>(DbCommand command, string methodName, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            var activity = _activitySource.StartActivity(methodName);
            if (activity != null && activity.IsAllDataRequested)
            {
                // 从 SQL 语句中提取操作类型（SELECT/INSERT/UPDATE/DELETE）和驱动主表名
                var result = GetDrivingTableName(command.CommandText);
                var action = result.Item1;
                var table = result.Item2;
                var traceId = activity.TraceId.ToString();
                var label = action != null && table != null ? string.Format("{0} {1}", action, table) : "";

                // 在 SQL 前注入 /* TraceId:xxx SELECT AppUsers */ 注释，便于数据库端日志关联
                command.CommandText = string.Format("/* TraceId:{0} {1} */ \n {2}", traceId, label, command.CommandText);

                activity.SetTag("db.statement", command.CommandText);
                var oTelSqlParameters = ConfigurationManager.AppSettings["OTel_RecordSqlParam"] ?? "false";
                bool.TryParse(oTelSqlParameters, out bool oTelSqlParametersBool);
                if (oTelSqlParametersBool && command.Parameters.Count > 0)
                {
                    activity.SetTag("db.parameters", FormatParameters(command.Parameters));
                }
                if (!string.IsNullOrEmpty(label))
                {
                    // Span Name 显示为 "SELECT AppUsers" 而非默认方法名
                    activity.DisplayName = label;
                }

                interceptionContext.SetUserState("Activity", activity);
            }
        }

        /// <summary>
        /// SQL 执行后：从 UserState 取出 Activity，标记异常状态，关闭 Span。
        /// </summary>
        private static void StopSpan<TResult>(DbCommandInterceptionContext<TResult> interceptionContext)
        {
            var activity = interceptionContext.FindUserState("Activity") as Activity;
            if (activity != null)
            {
                if (interceptionContext.Exception != null)
                {
                    activity.SetTag("otel.status_code", "ERROR");
                    activity.SetTag("otel.status_description", interceptionContext.Exception.Message);
                }
                activity.Stop();
            }
        }

        /// <summary>
        /// 从 SQL 语句中提取操作类型和驱动主表名。
        /// 例如 "SELECT * FROM AppUsers" → ("SELECT", "AppUsers")
        /// </summary>
        public static Tuple<string, string> GetDrivingTableName(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return Tuple.Create((string)null, (string)null);

            // 截取到第一个分号，只解析第一条语句
            var firstStatement = sql;
            var semiColonIndex = sql.IndexOf(';');
            if (semiColonIndex > 0)
                firstStatement = sql.Substring(0, semiColonIndex);

            // 识别 SQL 动作关键字
            var actionMatch = Regex.Match(firstStatement, @"^\s*(SELECT|INSERT|UPDATE|DELETE)\b", RegexOptions.IgnoreCase);
            if (!actionMatch.Success)
                return Tuple.Create((string)null, (string)null);

            var action = actionMatch.Groups[1].Value.ToUpperInvariant();

            // 按动作类型提取驱动主表名，支持反引号、方括号、无引用，支持 schema.table 格式
            const string tablePattern = @"(?:[`[]?\w+[`\]]?\.)?[`[]?(\w+)[`\]]?";

            string table = null;
            switch (action)
            {
                case "SELECT":
                    table = ExtractMatch(firstStatement, string.Format("(?i)FROM\\s+{0}", tablePattern));
                    break;
                case "INSERT":
                    table = ExtractMatch(firstStatement, string.Format("(?i)INSERT\\s+(?:INTO\\s+)?{0}", tablePattern));
                    break;
                case "UPDATE":
                    table = ExtractMatch(firstStatement, string.Format("(?i)UPDATE\\s+{0}", tablePattern));
                    break;
                case "DELETE":
                    table = ExtractMatch(firstStatement, string.Format("(?i)FROM\\s+{0}", tablePattern));
                    break;
            }

            return Tuple.Create(action, table);
        }

        private static string ExtractMatch(string input, string pattern)
        {
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        /// <summary>
        /// 将 DbParameterCollection 格式化为 JSON 对象字符串。
        /// </summary>
        private static string FormatParameters(DbParameterCollection parameters)
        {
            var dict = new Dictionary<string, object>();
            foreach (DbParameter p in parameters)
            {
                dict[p.ParameterName] = p.Value ?? "NULL";
            }
            return JsonConvert.SerializeObject(dict);
        }
    }
}
