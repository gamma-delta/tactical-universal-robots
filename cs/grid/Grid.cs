namespace tur.grid;

using Godot;

using System.Collections.Generic;
using System.Linq;


[GlobalClass]
public partial class Grid : Node3D {
  public static readonly int UNITS_PER_SQUARE = 1;

  [Export]
  public Vector2I Size;
  [Export]
  public PackedScene CellPrefab;

  public Cell[,] cells;

  public void ToolCreateGrid() {
    if (this.cells != null) {
      foreach (var c in this.cells) {
        if (c != null && !c.IsQueuedForDeletion()) c.QueueFree();
      }
    }

    this.cells = new Cell[this.Size.X, this.Size.Y];
    for (int x = 0; x < Size.X; x++) {
      for (int y = 0; y < Size.Y; y++) {
        Cell cell = this.CellPrefab.Instantiate<Cell>();
        cell.PostCreateFixup(new(x,y));
        cell.Name = cell.Name + $"({x},{y})";
        this.cells[x,y] = cell;
        this.AddChild(cell);
        cell.Owner = this;
      }
    }
  }

  public override void _Ready() {
    
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

  public static Vector3 GridPosToWorldPos(Vector2I pos) {
    var v2 = pos * UNITS_PER_SQUARE;
    // Remember that Y is up, ugh
    return new Vector3(v2.X, 0, v2.Y);
  }

  public static Vector2I WorldPosToGridPos(Vector3 pos) {
    var v2 = pos / UNITS_PER_SQUARE;
    // Remember that Y is up, ugh
    return new Vector2I((int)v2.X, (int)v2.Z);
  }

  public bool GridPosInBounds(Vector2I pos) {
    // vector comparison operators don't do what i want them to do
    var bounds = this.Bounds();
    return 0 <= pos.X && pos.X < bounds.X
      &&   0 <= pos.Y && pos.Y < bounds.Y;
  }
}
