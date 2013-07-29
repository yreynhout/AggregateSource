using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing
{
    /// <summary>
    /// Represents an aggregate command test specification runner.
    /// </summary>
    public class EventCentricAggregateCommandTestRunner : IEventCentricAggregateCommandTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateCommandTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateCommandTestRunner(IEqualityComparer<object> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>
        /// The result of running the test specification.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        public EventCentricAggregateCommandTestResult Run(EventCentricAggregateCommandTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            var result = Catch.Exception(() => specification.When(sut));
            if (result.HasValue)
            {
                return new EventCentricAggregateCommandTestResult(specification, TestResultState.Failed, actualException: result.Value);
            }
            var actualEvents = sut.GetChanges().ToArray();
            if (!actualEvents.SequenceEqual(specification.Thens, _comparer))
            {
                return new EventCentricAggregateCommandTestResult(specification, TestResultState.Failed, actualEvents);
            }
            return new EventCentricAggregateCommandTestResult(specification, TestResultState.Passed);
        }
    }

    /// <summary>
    /// Represents an aggregate constructor test specification runner.
    /// </summary>
    public class EventCentricAggregateConstructorTestRunner : IEventCentricAggregateConstructorTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateConstructorTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateConstructorTestRunner(IEqualityComparer<object> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>
        /// The result of running the test specification.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        public EventCentricAggregateConstructorTestResult Run(EventCentricAggregateConstructorTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            IAggregateRootEntity sut = null;
            var result = Catch.Exception(() => sut = specification.SutFactory());
            if (result.HasValue)
            {
                return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Failed, actualException: result.Value);
            }
            var actualEvents = sut.GetChanges().ToArray();
            if (!actualEvents.SequenceEqual(specification.Thens, _comparer))
            {
                return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Failed, actualEvents);
            }
            return new EventCentricAggregateConstructorTestResult(specification, TestResultState.Passed);
        }
    }

    /// <summary>
    /// Represents an aggregate factory test specification runner.
    /// </summary>
    public class EventCentricAggregateFactoryTestRunner : IEventCentricAggregateFactoryTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventCentricAggregateFactoryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public EventCentricAggregateFactoryTestRunner(IEqualityComparer<object> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>
        /// The result of running the test specification.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        public EventCentricAggregateFactoryTestResult Run(EventCentricAggregateFactoryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            IAggregateRootEntity factoryResult = null;
            var result = Catch.Exception(() => factoryResult = specification.When(sut));
            if (result.HasValue)
            {
                return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualException: result.Value);
            }
            var actualEvents = factoryResult.GetChanges().ToArray();
            if (!actualEvents.SequenceEqual(specification.Thens, _comparer))
            {
                return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Failed, actualEvents);
            }
            return new EventCentricAggregateFactoryTestResult(specification, TestResultState.Passed);
        }
    }

    /// <summary>
    /// Represents an aggregate command test specification runner.
    /// </summary>
    public class ResultCentricAggregateQueryTestRunner : IResultCentricAggregateQueryTestRunner
    {
        readonly IEqualityComparer<object> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultCentricAggregateQueryTestRunner"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing events.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="comparer"/> is <c>null</c>.</exception>
        public ResultCentricAggregateQueryTestRunner(IEqualityComparer<object> comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
            _comparer = comparer;
        }

        /// <summary>
        /// Runs the specified test specification.
        /// </summary>
        /// <param name="specification">The test specification to run.</param>
        /// <returns>
        /// The result of running the test specification.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is <c>null</c>.</exception>
        public ResultCentricAggregateQueryTestResult Run(ResultCentricAggregateQueryTestSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");
            var sut = specification.SutFactory();
            sut.Initialize(specification.Givens);
            object queryResult = null;
            var result = Catch.Exception(() => queryResult = specification.When(sut));
            if (result.HasValue)
            {
                return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Failed, Optional<object>.Empty, result.Value);
            }
            if (!_comparer.Equals(queryResult, specification.Then))
            {
                return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Failed, new Optional<object>(queryResult));
            }
            return new ResultCentricAggregateQueryTestResult(specification, TestResultState.Passed, Optional<object>.Empty);
        }
    }

}