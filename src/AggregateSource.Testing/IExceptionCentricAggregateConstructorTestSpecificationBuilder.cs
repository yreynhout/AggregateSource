namespace AggregateSource.Testing
{
	/// <summary>
	/// The act of building an exception-centric aggregate constructor test specification.
	/// </summary>
	public interface IExceptionCentricAggregateConstructorTestSpecificationBuilder
	{
		/// <summary>
		/// Builds the test specification thus far.
		/// </summary>
		/// <returns>The test specification.</returns>
		ExceptionCentricAggregateConstructorTestSpecification Build();
	}
}