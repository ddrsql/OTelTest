using Abp.Dependency;
using Abp.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AbpCore.OTel.EntityFrameworkCore;

public class TaggedTraceidCommandInterceptor : DbCommandInterceptor, ISingletonDependency
{
    private readonly OTelOptions _oTelOptions;

    public TaggedTraceidCommandInterceptor(IOptions<OTelOptions> oTelOptions)
    {
        _oTelOptions = oTelOptions.Value;
    }

    /// <summary>
    /// 拦截 查询类命令（返回结果集）
    /// </summary>
    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        ManipulateCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData,
        InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);
        return new ValueTask<InterceptionResult<DbDataReader>>(result);
    }

    /// <summary>
    /// 拦截 非查询类命令（不返回结果集，只返回受影响行数）
    /// </summary>
    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData,
        InterceptionResult<int> result)
    {
        ManipulateCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);
        return new ValueTask<InterceptionResult<int>>(result);
    }

    /// <summary>
    /// 拦截 标量查询（返回单个值）
    /// </summary>
    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData,
        InterceptionResult<object> result)
    {
        ManipulateCommand(command);
        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData,
        InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        ManipulateCommand(command);
        return new ValueTask<InterceptionResult<object>>(result);
    }

    private void ManipulateCommand(DbCommand command)
    {
        var activity = Activity.Current;
        if (activity != null && activity.IsAllDataRequested == true)
        {
            var (action, table) = GetDrivingTableName(command.CommandText);
            var traceId = activity.TraceId.ToString();
            var label = action != null && table != null ? $"{action} {table}" : "";
            command.CommandText = string.Format("/* TraceId:{0} {1} */ \n {2}", traceId, label, command.CommandText);
            activity.SetTag("db.statement", command.CommandText);
            if (_oTelOptions.RecordSqlParameter && command.Parameters.Count > 0)
            {
                activity.SetTag("db.parameters", FormatParameters(command.Parameters));
            }
            if (!label.IsNullOrEmpty())
                activity.DisplayName = label;
        }
    }

    public (string? Action, string? Table) GetDrivingTableName(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return (null, null);

        // 截取到第一个分号，处理多语句场景
        var firstStatement = sql;
        var semiColonIndex = sql.IndexOf(';');
        if (semiColonIndex > 0)
            firstStatement = sql.Substring(0, semiColonIndex);

        // Step 1: 识别 SQL 动作关键字
        var actionMatch = Regex.Match(firstStatement, @"^\s*(SELECT|INSERT|UPDATE|DELETE)\b", RegexOptions.IgnoreCase);
        if (!actionMatch.Success)
            return (null, null);

        var action = actionMatch.Groups[1].Value.ToUpperInvariant();

        // Step 2: 按动作类型 提取驱动主表名
        // 表名模式：支持反引号、方括号、无引用，支持 schema.table 格式
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

        return (action, table);
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
        return JsonSerializer.Serialize(dict);
    }
}
