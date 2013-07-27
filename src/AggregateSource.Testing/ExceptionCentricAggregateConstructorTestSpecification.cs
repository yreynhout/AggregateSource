using System;

namespace AggregateSource.Testing
{
	/// <summary>
	/// Represents an exception centric constructor test specification, meaning that the outcome revolves whether the constructor
	/// of the subject under test throws the specified exception when called.
	/// </summary>
	public class ExceptionCentricAggregateConstructorTestSpecification
	{
		readonly Func<IAggregateRootEntity> _sutFactory;
		readonly Exception _throws;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionCentricAggregateConstructorTestSpecification"/> class.
		/// </summary>
		/// <param name="sutFactory">The sut factory.</param>
		/// <param name="throws">The expected exception to assert.</param>
		public ExceptionCentricAggregateConstructorTestSpecification(Func<IAggregateRootEntity> sutFactory, Exception throws)
		{
			_sutFactory = sutFactory;
			_throws = throws;
		}

		/// <summary>
		/// Gets the sut factory.
		/// </summary>
		/// <value>
		/// The sut factory.
		/// </value>
		public Func<IAggregateRootEntity> SutFactory
		{
			get { return _sutFactory; }
		}

		/// <summary>
		/// The expected exception to assert.
		/// </summary>
		public Exception Throws
		{
			get { return _throws; }
		}
	}
}