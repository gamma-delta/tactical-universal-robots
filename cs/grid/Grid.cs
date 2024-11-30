namespace tur.grid;

using tur.items;
using tur.items.inventory;
using tur.factory;

using Godot;

using System.Collections.Generic;
using System.Linq;


public partial class Grid {
  public static readonly int PX_PER_SQUARE = 16;

  private Cell[,] cells;

  // TODO: caching of the graph stuff

  public Grid() {
    int w = 8, h = 8;
    this.cells = new Cell[w,h];
    for (int x = 0; x < w; x++) {
      for (int y = 0; y < h; y++) {
        this.cells[x,y] = new Cell(new(x,y));
      }
    }

    this.cells[1,2].mineable = The.Bulks["coal"];
  }

  public Vector2I Bounds() {
    return new(this.cells.GetLength(0), this.cells.GetLength(1));
  }

  public int Width() => this.Bounds().X;
  public int Height() => this.Bounds().Y;

  public Cell? GetCell(Vector2I pos) {
    return this.GridPosInBounds(pos)
      ? this.cells[pos.X, pos.Y]
      : null;
  }

  public IEnumerable<Cell> GetCells() {
    // nd arrays don't impl the generic version of enumerable for
    // some reason, so explicitly use the Cast method.
    // society if c# had java-like ability to call methods as normal
    // fns with a decurried first argument
    return ((System.Collections.IEnumerable) this.cells).Cast<Cell>();
  }

  public static Vector2I WorldPosToGridPos(Vector2 pos) {
    return (Vector2I) (pos / PX_PER_SQUARE);
  }

  public static Vector2 GridPosToWorldPos(Vector2I pos) {
    return pos * PX_PER_SQUARE;
  }

  public bool GridPosInBounds(Vector2I pos) {
    // vector comparison operators don't do what i want them to do
    var bounds = this.Bounds();
    return 0 <= pos.X && pos.X < bounds.X
      &&   0 <= pos.Y && pos.Y < bounds.Y;
  }

  public Vector2I? MouseOverPos(Vector2 localMousePosition) {
    Vector2I v = WorldPosToGridPos(localMousePosition);
    return this.GridPosInBounds(v) ? v : null;
  }

  public void TickAllFactories() {
    var tarjan = TarjanSccinator.Calculate(this);

    MultiInventory electricGrid = new MultiInventory(
      tarjan.tickOrder.Select(f => f.bufferOut as Inventory).ToList(),
      br => br.ResourceName == "electricity"
    );

    foreach (var factory in tarjan.tickOrder) {
      List<Factory> sources = tarjan.factorySourcesGraph[factory.cell.pos];
      List<Inventory> inventories = new() { electricGrid };
      inventories.AddRange(sources.Select(s => s.bufferOut));
      MultiInventory multiInv = new MultiInventory(inventories);
      factory.Tick(multiInv);
    }
  }


}
