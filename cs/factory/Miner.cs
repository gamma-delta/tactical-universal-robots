namespace tur.factory;

using tur.items;
using tur.grid;

using Godot;

using System.Collections.Generic;

public class Miner : Factory {
  public readonly ulong extractPerTick = 10;
  public readonly ulong electricityUsage = 1000;
  
  public Miner(Cell cell) : base(cell) {
  }

  public override Texture2D Texture() {
    return ResourceLoader.Load<Texture2D>("res://textures/miner.png");
  }

  public override IReadOnlyList<BulkResourceStack> Ingredients() {
    // TODO: electricity
    return new List<BulkResourceStack>();
  }

  public override BulkResourceStack? Output() {
    if (this.cell.mineable is BulkResource br) {
      return new BulkResourceStack(br, this.extractPerTick);
    } else {
      return null;
    }
  }
}
