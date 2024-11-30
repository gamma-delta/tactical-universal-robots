namespace tur.items.inventory;

public interface Inventory {
  public ulong CountOf(BulkResource ty);

  /// <summary>
  /// Returns the amount of resources they managed to extract.
  /// Must never return more than the count.
  /// </summary>
  public ulong Extract(BulkResource ty, ulong count);

  public bool ExtractOk(BulkResource ty, ulong count) {
    ulong extracted = this.Extract(ty, count);
    return extracted == count;
  }

  private class DummyInventory : Inventory {
    public ulong CountOf(BulkResource ty) => 0;
    public ulong Extract(BulkResource ty, ulong count) => 0;
  }
  public static readonly Inventory DUMMY = new DummyInventory();

  private class CreativeInventory : Inventory {
    public ulong CountOf(BulkResource ty) => ulong.MaxValue;
    public ulong Extract(BulkResource ty, ulong count) => count;
  }
  public static readonly Inventory CREATIVE = new CreativeInventory();
}
