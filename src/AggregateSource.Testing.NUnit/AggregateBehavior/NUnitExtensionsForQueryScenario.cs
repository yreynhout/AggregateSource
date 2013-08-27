using System;
using System.IO;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    public static class NUnitExtensionsForQueryScenario
    {
        public static void Assert(this IResultCentricAggregateQueryTestSpecificationBuilder builder,
            IResultComparer comparer)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (comparer == null) throw new ArgumentNullException("comparer");
            var specification = builder.Build();
            var runner = new ResultCentricAggregateQueryTestRunner(comparer);
            var result = runner.Run(specification);
            if (result.Failed)
            {
                if (result.ButException.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Then);
                        writer.WriteLine("  But was:  {0}", result.ButException.Value);

                        throw new NUnit.Framework.AssertionException(writer.ToString());
                    }
                }

                if (result.ButResult.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Then);
                        writer.WriteLine("  But was:  {0}", result.ButResult.Value);

                        throw new NUnit.Framework.AssertionException(writer.ToString());
                    }
                }

                if (result.ButEvents.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Then);
                        writer.WriteLine("  But was:  {0} events ({1})",
                            result.ButEvents.Value.Length,
                            String.Join(",", result.ButEvents.Value.Select(_ => _.GetType().Name).ToArray()));

                        throw new NUnit.Framework.AssertionException(writer.ToString());
                    }
                }
            }
        }

        public static void Assert(this IExceptionCentricAggregateQueryTestSpecificationBuilder builder,
            IExceptionComparer comparer)
        {
            if (builder == null) throw new ArgumentNullException("builder");
            if (comparer == null) throw new ArgumentNullException("comparer");
            var specification = builder.Build();
            var runner = new ExceptionCentricAggregateQueryTestRunner(comparer);
            var result = runner.Run(specification);
            if (result.Failed)
            {
                if (result.ButException.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                        writer.WriteLine("  But was:  {0}", result.ButException.Value);

                        throw new NUnit.Framework.AssertionException(writer.ToString());
                    }
                }

                if (result.ButEvents.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                        writer.WriteLine("  But was:  {0} events ({1})",
                            result.ButEvents.Value.Length,
                            String.Join(",", result.ButEvents.Value.Select(_ => _.GetType().Name).ToArray()));

                        throw new NUnit.Framework.AssertionException(writer.ToString());
                    }
                }
            }
        }
    }
}
