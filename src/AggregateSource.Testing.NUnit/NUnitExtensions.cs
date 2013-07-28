using System;
using NUnit.Framework;

namespace AggregateSource.Testing
{
	public static class NUnitExtensions
	{
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

		public static void Assert(this IAggregateConstructorThenStateBuilder builder)
		{
			var specification = builder.Build();
			var sut = specification.SutFactory();
			NUnit.Framework.Assert.That(
				sut.GetChanges(),
				Is.EquivalentTo(specification.Thens));
		}

		public static void AssertThrows<TException>(this IAggregateConstructorWhenStateBuilder builder,
													TException exception) where TException : Exception
		{
			var specification = builder.Throws(exception).Build();
			NUnit.Framework.Assert.That(
				NUnit.Framework.Assert.Throws<TException>(() => specification.SutFactory()).Message,
				Is.EqualTo(exception.Message));
		}

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