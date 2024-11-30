namespace tur.items.inventory;

using System;
using System.Collections.Generic;
using System.Linq;

public class MultiInventory : Inventory {
  protected List<Inventory> children;
  protected Predicate<BulkResource>? filter;

  public MultiInventory(
    List<Inventory> children,
    Predicate<BulkResource>? filter = null) {
    this.children = children;
    this.filter = filter;
  }

  private bool CheckFilter(BulkResource ty) =>
    this.filter == null || this.filter.Invoke(ty);

  public ulong CountOf(BulkResource ty) {
    if (!CheckFilter(ty)) {
      return 0;
    }

    return this.children
      .Select(i => i.CountOf(ty))
      .Aggregate((acc, x) => acc + x);
  }

  public ulong Extract(BulkResource ty, ulong count) {
    if (!CheckFilter(ty)) {
      return 0;
    }

    ulong countLeft = count;
    foreach (Inventory kid in this.children) {
      ulong extracted = kid.Extract(ty, countLeft);
      countLeft -= extracted;
      if (countLeft == 0) {
        break;
      }
    }
    return count - countLeft;
  }
}
