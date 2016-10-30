using System;
using System.Threading.Tasks;

namespace AggregateSource.EventStore.Framework
{
    public static class Catch
    {
        public static void ExceptionOf(Action action)
        {
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                action();
            }
            catch {}
            // ReSharper restore EmptyGeneralCatchClause
        }

        public static async Task ExceptionOf(Func<Task> action)
        {
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                await action();
            }
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
        }
    }
}