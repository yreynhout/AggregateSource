using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
	/// <summary>
	/// Extension methods for running AggregateSource.Testing specifications using NUnit.
	/// </summary>
	public static class NUnitExtensions
	{
		/// <summary>
		/// Runs an event-centric aggregate factory specification using NUnit.
		/// </summary>
		/// <param name="builder">The specification builder.</param>
		public static void Assert(this IAggregateFactoryThenStateBuilder builder)
		{
			var specification = builder.Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			var result = specification.When(sut);
			NUnit.Framework.Assert.That(
				result.GetChanges(),
				Is.EquivalentTo(specification.Thens));
		}

		/// <summary>
		/// Runs an exception-centric aggregate factory specification using NUnit.
		/// </summary>
		/// <typeparam name="TException">Type of the expected exception.</typeparam>
		/// <param name="builder">The specification builder.</param>
		/// <param name="exception">An instance equivalent to the expected exception.</param>
		public static void AssertThrows<TException>(this IAggregateFactoryWhenStateBuilder builder,
													TException exception) where TException : Exception
		{
			var specification = builder.Throws(exception).Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			NUnit.Framework.Assert.That(
				NUnit.Framework.Assert.Throws<TException>(() => specification.When(sut)).Message,
				Is.EqualTo(exception.Message));
		}

		/// <summary>
		/// Runs an event-centric aggregate constructor specification using NUnit.
		/// </summary>
		/// <param name="builder">The specification builder.</param>
		public static void Assert(this IAggregateConstructorThenStateBuilder builder)
		{
			var specification = builder.Build();
			var sut = specification.SutFactory();
			NUnit.Framework.Assert.That(
				sut.GetChanges(),
				Is.EquivalentTo(specification.Thens));
		}

		/// <summary>
		/// Runs an exception-centric aggregate constructor specification using NUnit.
		/// </summary>
		/// <typeparam name="TException">Type of the expected exception.</typeparam>
		/// <param name="builder">The specification builder.</param>
		/// <param name="exception">An instance equivalent to the expected exception.</param>
		public static void AssertThrows<TException>(this IAggregateConstructorWhenStateBuilder builder,
													TException exception) where TException : Exception
		{
			var specification = builder.Throws(exception).Build();
			NUnit.Framework.Assert.That(
				NUnit.Framework.Assert.Throws<TException>(() => specification.SutFactory()).Message,
				Is.EqualTo(exception.Message));
		}

		/// <summary>
		/// Runs an event-centric aggregate command specification using NUnit.
		/// </summary>
		/// <param name="builder">The specification builder.</param>
		public static void Assert(this IAggregateCommandThenStateBuilder builder)
		{
			var specification = builder.Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			specification.When(sut);
			NUnit.Framework.Assert.That(
				sut.GetChanges(),
				Is.EquivalentTo(specification.Thens));
		}

		/// <summary>
		/// Runs an exception-centric aggregate command specification using NUnit.
		/// </summary>
		/// <typeparam name="TException">Type of the expected exception.</typeparam>
		/// <param name="builder">The specification builder.</param>
		/// <param name="exception">An instance equivalent to the expected exception.</param>
		public static void AssertThrows<TException>(this IAggregateCommandWhenStateBuilder builder,
													TException exception) where TException : Exception
		{
			var specification = builder.Throws(exception).Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			NUnit.Framework.Assert.That(
				NUnit.Framework.Assert.Throws<TException>(() => specification.When(sut)).Message,
				Is.EqualTo(exception.Message));
		}

		/// <summary>
		/// Runs an event-centric aggregate query specification using NUnit.
		/// </summary>
		/// <param name="builder"></param>
		public static void Assert(this IAggregateQueryThenStateBuilder builder)
		{
			var specification = builder.Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			var result = specification.When(sut);
			NUnit.Framework.Assert.That(
				result,
				Is.EqualTo(specification.Then));
		}

		/// <summary>
		/// Runs an exception-centric aggregate query specification using NUnit.
		/// </summary>
		/// <typeparam name="TException">Type of the expected exception.</typeparam>
		/// <typeparam name="TResult">Type of the query result.</typeparam>
		/// <param name="builder">The specification builder.</param>
		/// <param name="exception">An instance equivalent to the expected exception.</param>
		public static void AssertThrows<TResult, TException>(this IAggregateQueryWhenStateBuilder<TResult> builder,
															 TException exception) where TException : Exception
		{
			var specification = builder.Throws(exception).Build();
			var sut = specification.SutFactory();
			sut.Initialize(specification.Givens);
			NUnit.Framework.Assert.That(
				NUnit.Framework.Assert.Throws<TException>(() => specification.When(sut)).Message,
				Is.EqualTo(exception.Message));
		}
	}
}