namespace AggregateSource.EventStorage {
  public interface IHandle<in TMessage> {
    void Handle(TMessage message);
  }
}