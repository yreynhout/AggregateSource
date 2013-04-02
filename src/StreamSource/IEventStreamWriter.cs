namespace StreamSource {
  public interface IEventStreamWriter {
    void Write(EventStreamChangeset changeset);
  }
}