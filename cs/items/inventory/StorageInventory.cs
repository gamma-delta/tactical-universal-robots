namespace tur.items.inventory;

using System;
using System.Text;
using System.Collections.Generic;

public class StorageInventory : Inventory {
  protected Dictionary<BulkResource, ulong> items;

  public StorageInventory() {
    this.items = new();
  }

  public ulong CountOf(BulkResource ty) {
    return this.items.GetValueOrDefault<BulkResource, ulong>(ty);
  }

  public ulong Extract(BulkResource ty, ulong count) {
    ulong countHere = this.CountOf(ty);
    ulong countToExtract = (count <= countHere)
      ? count
      : countHere;
    this.items[ty] = checked(countHere - countToExtract);
    return countToExtract;
  }

  /// <summary>
  /// Returns the number of items left in the source stack
  /// </summary>
  public ulong Insert(BulkResource ty, ulong count) {
    // TODO: item count stacks mb
    ulong countHere = this.CountOf(ty);
    this.items[ty] = countHere + count;
    return 0;
  }

  public string DumpContents() {
    StringBuilder bob = new("StorageInventory:\n");
    foreach (var (k, v) in this.items) {
      bob.Append("- ")
        .Append(v)
        .Append("x ")
        .Append(k.ResourceName)
        .AppendLine();
    }
    return bob.ToString();
  }
}

