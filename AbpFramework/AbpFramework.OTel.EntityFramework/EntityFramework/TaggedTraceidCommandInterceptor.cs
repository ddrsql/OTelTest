using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpFramework.OTel.EntityFramework
{
    public class TaggedTraceidCommandInterceptor : DbCommandInterceptor
    {
        private static readonly ActivitySource _activitySource = new ActivitySource(OTelModule.AspNetSourceName);
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            ManipulateCommand(command, nameof(ReaderExecuting));
            base.ReaderExecuting(command, interceptionContext);
        }

        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            ManipulateCommand(command, nameof(NonQueryExecuting));
            base.NonQueryExecuting(command, interceptionContext);
        }

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            ManipulateCommand(command, nameof(ScalarExecuting));
            base.ScalarExecuting(command, interceptionContext);
        }

        private void ManipulateCommand(DbCommand command, string methodName)
        {
            var activity = Activity.Current;
            if (activity != null && activity.IsAllDataRequested == true)
            {
                var traceId = activity.TraceId.ToString();
                command.CommandText = string.Format("/* TraceId:{0} */ \n {1}", traceId, command.CommandText);
                activity?.SetTag("Sql", command.CommandText);
            }
            else
            {
                Activity sqlActivity = null;
                try
                {
                    sqlActivity = _activitySource.StartActivity(methodName);
                    if (sqlActivity != null && sqlActivity.IsAllDataRequested == true)
                    {
                        var traceId = sqlActivity.TraceId.ToString();
                        command.CommandText = string.Format("/* TraceId:{0} */ \n {1}", traceId, command.CommandText);
                        sqlActivity?.SetTag("Sql", command.CommandText);
                    }
                }
                finally
                {
                    sqlActivity?.Stop();
                    sqlActivity?.Dispose();
                }
            }
        }
    }
}
