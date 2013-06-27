using System.Threading.Tasks;

namespace AggregateSource.GEventStore.Snapshots {
  public interface IAsyncSnapshotReader {
    Task<Optional<Snapshot>> ReadOptionalAsync(string identifier);
  }
}