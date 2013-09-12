namespace AggregateSource.Testing
{
	/// <summary>
	/// The act of building an event-centric aggregate constructor test specification.
	/// </summary>
	public interface IEventCentricAggregateConstructorTestSpecificationBuilder
	{
		/// <summary>
		/// Builds the test specification thus far.
		/// </summary>
		/// <returns>The test specification.</returns>
		EventCentricAggregateConstructorTestSpecification Build();
	}
}