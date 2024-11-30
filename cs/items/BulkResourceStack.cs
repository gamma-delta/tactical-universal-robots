namespace tur.items;

public class BulkResourceStack {
  public BulkResource type { get; init; }
  /// Zero is allowed because this represents like a "slot"
  public ulong count { get; init; }
  
  public BulkResourceStack(BulkResource type, ulong count) {
    this.type = type;
    this.count = count;
  }

  public BulkResourceStack CloneWithCount(ulong newCount) {
    return new(this.type, newCount);
  }
}
