namespace AggregateSource.Testing {
  /// <summary>
  /// The throw state within the test specification building process.
  /// </summary>
  public interface IThrowStateBuilder {
    /// <summary>
    /// Builds the test specification thus far.
    /// </summary>
    /// <returns>The test specification.</returns>
    ExceptionCentricTestSpecification Build();
  }
}