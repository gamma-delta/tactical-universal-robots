namespace tur.factory;

using tur.items;
using tur.grid;

using Godot;

using System.Collections.Generic;

public class CraftingFactory : Factory {
  public readonly IReadOnlyList<BulkResourceStack> inputs;
  public readonly BulkResourceStack output;

  public CraftingFactory(Cell cell,
    IReadOnlyList<BulkResourceStack> inputs,
    BulkResourceStack output
  ) : base(cell) {
    this.inputs = inputs;
    this.output = output;
  }

  public override IReadOnlyList<BulkResourceStack> Ingredients() {
    return this.inputs;
  }

  public override BulkResourceStack? Output() {
    return this.output;
  }
}
