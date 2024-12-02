namespace tur.grid;

using tur.units;

using Godot;

using System.Collections.Generic;
using System.Linq;


[GlobalClass]
public partial class Grid : Node3D {
  public static readonly int UNITS_PER_SQUARE = 1;

  [Export]
  public PackedScene CellPrefab;

  private Cell[,] cells;

  public AStarGrid2D AStar = new();
  private List<Unit> turnOrder = new();

  public override void _Ready() {
    The.Grid = this;
  
    int sizeX = 12, sizeY = 8;

    this.cells = new Cell[sizeX, sizeY];
    for (int x = 0; x < sizeX; x++) {
      for (int y = 0; y < sizeY; y++) {
        Cell cell = this.CellPrefab.Instantiate<Cell>();
        cell.PostCreateFixup(new(x, y));
        this.AddChild(cell);
        this.cells[x,y] = cell;

        if (x == 4 && y == 7) {
          Unit unit = Extensions.LoadPrefab<Unit>("BoringUnit");
          this.AddUnit(unit, new(x, y));
        }
      }
    }

    this.AStar.Region = new(0, 0, sizeX, sizeY);
    this.AStar.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Octile;
    this.AStar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Octile;
    this.AStar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles;
    this.AStar.Update();
  }

  public void AddUnit(Unit unit, Vector2I pos) {
    Cell? c = this.GetCell(pos);
    if (c is Cell cc) {
      cc.AddChild(unit);
      this.turnOrder.Append(unit);
    }
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
    return new Vector2I(Mathf.RoundToInt(v2.X), Mathf.RoundToInt(v2.Z));
  }

  public bool GridPosInBounds(Vector2I pos) {
    // vector comparison operators don't do what i want them to do
    var bounds = this.Bounds();
    return 0 <= pos.X && pos.X < bounds.X
      &&   0 <= pos.Y && pos.Y < bounds.Y;
  }
}
