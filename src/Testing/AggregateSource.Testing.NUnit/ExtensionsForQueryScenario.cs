using System;
using System.IO;
using System.Linq;

namespace AggregateSource.Testing
{
#if NUNIT
    /// <summary>
    /// NUnit specific extension methods for asserting aggregate query behavior.
    /// </summary>
    public static class NUnitExtensionsForQueryScenario
#elif XUNIT
    /// <summary>
    /// Xunit specific extension methods for asserting aggregate query behavior.
    /// </summary>
    public static class XunitExtensionsForQueryScenario
#endif
    {
        /// <summary>
        /// Asserts that the specification is met.
        /// </summary>
        /// <param name="builder">The specification builder.</param>
        /// <param name="comparer">The result comparer.</param>
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

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }

                if (result.ButResult.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Then);
                        writer.WriteLine("  But was:  {0}", result.ButResult.Value);

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }

                if (result.ButEvents.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Then);
                        writer.WriteLine("  But was:  {0} event(s) ({1})",
                            result.ButEvents.Value.Length,
                            String.Join(",", result.ButEvents.Value.Select(_ => _.GetType().Name).ToArray()));

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Asserts that the specification is met.
        /// </summary>
        /// <param name="builder">The specification builder.</param>
        /// <param name="comparer">The exception comparer.</param>
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
                    if (result.ButException.Value.GetType() != result.Specification.Throws.GetType())
                    {
                        using (var writer = new StringWriter())
                        {
                            writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                            writer.WriteLine("  But was:  {0}", result.ButException.Value);

#if NUNIT
                            throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                            throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                        }
                    }
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                        writer.WriteLine("  But found the following differences:");
                        foreach (var difference in comparer.Compare(result.Specification.Throws, result.ButException.Value))
                        {
                            writer.WriteLine("    {0}", difference.Message);
                        }

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }

                if (result.ButEvents.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                        writer.WriteLine("  But was:  {0} event(s) ({1})",
                            result.ButEvents.Value.Length,
                            String.Join(",", result.ButEvents.Value.Select(_ => _.GetType().Name).ToArray()));

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }

                if (result.ButResult.HasValue)
                {
                    using (var writer = new StringWriter())
                    {
                        writer.WriteLine("  Expected: {0},", result.Specification.Throws);
                        writer.WriteLine("  But was:  {0}", result.ButResult.Value);

#if NUNIT
                        throw new NUnit.Framework.AssertionException(writer.ToString());
#elif XUNIT
                        throw new Xunit.Sdk.AssertException(writer.ToString());
#endif
                    }
                }
            }
        }
    }
}
