namespace tur.factory;

using tur.items;
using tur.items.inventory;
using tur.grid;

using Godot;

using System.Linq;
using System.Collections.Generic;

public abstract class Factory {
  public readonly Cell cell;
  public readonly StorageInventory bufferOut;

  public Direction4 outDir;

  /// <summary>
  /// How many crafts this is allowed to store in its buffer.
  /// </summary>
  public readonly ulong craftsBufferSize = 2;

  protected Factory(Cell cell) {
    this.cell = cell;
    this.bufferOut = new();
    this.outDir = Direction4.North;
  }

  public abstract IReadOnlyList<BulkResourceStack> Ingredients();
  /// Potential output.
  public abstract BulkResourceStack? Output();

  public virtual Texture2D Texture() {
    return ResourceLoader.Load<Texture2D>("res://textures/factory.png");
  }
  public virtual Texture2D OutputOverlayTex() {
    return ResourceLoader.Load<Texture2D>("res://textures/outputs-map.png");
  }
  
  public virtual void Tick(Inventory inputs) {
    var ingredients = this.Ingredients();
    var output = this.Output();
    
    double worstRatio;
    if (ingredients.Count == 0) {
      worstRatio = 1.0;
    } else {
      worstRatio = ingredients
        .Select(i => {
          ulong available = inputs.CountOf(i.type);
          return (double)available / (double)i.count;
        }).Min();
    }

    // can kind of consider "space left in the buffer"
    // to also be a resource (?)
    double bufferRatio;
    // i love scoping rules i want to explode
    { if (this.Output() is BulkResourceStack o) {
      ulong bufSize = o.count * this.craftsBufferSize;
      ulong currentBufCount = this.bufferOut.CountOf(o.type);
      ulong bufSpaceLeft = (currentBufCount <= bufSize)
        ? bufSize - currentBufCount
        : 0;
      bufferRatio = (double)bufSpaceLeft / (double)o.count;
    } else {
      bufferRatio = 1.0;
    } }
    
    worstRatio = Mathf.Min(worstRatio, bufferRatio);
    worstRatio = Mathf.Min(worstRatio, 1.0);

    foreach (var i in ingredients) {
      ulong count = (ulong)Mathf.Floor(i.count * worstRatio);
      bool ok = inputs.ExtractOk(i.type, count);
      if (!ok) {
        throw new System.Exception($"was promised i could extract {count} of {i.type} but could not");
      }
    }

    { if (output is BulkResourceStack o) {
      ulong targetCount = (ulong)Mathf.Floor(output.count * worstRatio);
      this.bufferOut.Insert(output.type, targetCount);
    } }
  }

  public virtual void Rotate(int by) {
    this.outDir = this.outDir.Rotated(by);
  }
}
