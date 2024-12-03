namespace tur.grid;

using tur.units;
using tur.units.actions;
using tur.units.procedure;

using Godot;

using System.Collections.Generic;
using System.Linq;


[GlobalClass]
public partial class Grid : Node3D {
  public static readonly int UNITS_PER_SQUARE = 1;

  [Export]
  public PackedScene CellPrefab;

  public Vector2I GridSize { get; private set; }

  private Cell[,] cells;

  public AStarGrid2D AStar = new();
  private TurnSequence turnSequence;
  public TurnOrderTracker TurnOrder = new();

  public override void _Ready() {
    The.Grid = this;
  
    this.GridSize = new(12, 8);

    this.cells = new Cell[this.GridSize.X, this.GridSize.Y];
    for (int x = 0; x < this.GridSize.X; x++) {
      for (int y = 0; y < this.GridSize.Y; y++) {
        Cell cell = this.CellPrefab.Instantiate<Cell>();
        cell.PostCreateFixup(new(x, y));
        this.AddChild(cell);
        this.cells[x,y] = cell;

        if (x == 4 && y == 4) {
          Unit unit = Extensions.LoadPrefab<Unit>("units/Mook");
          this.AddUnit(unit, new(x, y));
        } else if (x == 5 && y == 5) {
          Unit unit = Extensions.LoadPrefab<Unit>("units/Player");
          // No mind! player unit
          this.AddUnit(unit, new(x, y));
        } else if (x > 2 && x < 10 && y == 6) {
          Unit unit = Extensions.LoadPrefab<Unit>("units/Wall");
          this.AddUnit(unit, new(x, y));
        }
      }
    }

    // TODO: make a helper function that sets up astar and forbids locations
    // that have units
    this.AStar.Region = new(0, 0, this.GridSize.X, this.GridSize.Y);
    this.AStar.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Octile;
    this.AStar.DefaultEstimateHeuristic = AStarGrid2D.Heuristic.Octile;
    this.AStar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles;
    this.AStar.Update();
    this.setAStarSolidity();
  }

  public override void _Process(double dt) {
    if (this.TurnOrder.CurrentUnit() is Unit unit) {
      if (this.turnSequence == TurnSequence.JustStarted) {
        if (unit.Mind is Mind mind) {
          UnitAction decision = unit.Mind.Decide(unit, this);
          GD.Print($"Unit {unit} decided to {decision}");
          unit.FinishedWithTurn = false;
          decision.Perform(unit, this);
          this.turnSequence = TurnSequence.FinishingAction;
        } else {
          // Player controlled unit
          GD.Print($"Making the player make a decision for {unit}");
          The.PlayerController.BeginPlayerControlledTurn();
          this.turnSequence = TurnSequence.WaitingToDecide;
        }
      }
      if (this.turnSequence == TurnSequence.WaitingToDecide) {
        if (The.PlayerController.TryConsumePlayerDecision() is UnitAction decision) {
          GD.Print($"Player decided to {decision} for {unit}");
          decision.Perform(unit, this);
          unit.FinishedWithTurn = false;
          this.turnSequence = TurnSequence.FinishingAction;
        }
      }
      if (this.turnSequence == TurnSequence.FinishingAction) {
        if (unit.FinishedWithTurn) {
          GD.Print($"{unit} finished with animation or whatever");

          this.setAStarSolidity();
          this.TurnOrder.NextTurn();
          this.turnSequence = TurnSequence.JustStarted;
        }
      }
    }
  }

  public void AddUnit(Unit unit, Vector2I pos) {
    Cell? c = this.GetCell(pos);
    if (c is Cell cc) {
      cc.AddChild(unit);
      if (!unit.AlwaysSkipTurns)
        this.TurnOrder.AddUnit(unit);
    }
  }
  
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

  public IEnumerable<Unit> GetUnits() {
    foreach (var c in this.cells) {
      if (c.Unit is Unit u)
        yield return u;
    }
  }

  public bool GridPosInBounds(Vector2I pos) {
    // vector comparison operators don't do what i want them to do
    var bounds = this.GridSize;
    return 0 <= pos.X && pos.X < bounds.X
      &&   0 <= pos.Y && pos.Y < bounds.Y;
  }

  private void setAStarSolidity() {
    this.AStar.FillSolidRegion(this.AStar.Region, solid: false);
    foreach (var u in this.GetUnits()) {
      this.AStar.SetPointSolid(u.GridPos, solid: true);
    }
  }

  /// Automatically slices, and returns `true`, if it's too far
  public (List<Vector2I>, bool) GetAStarPath(Unit unit, Vector2I target) {
    var pathfind = this.AStar.GetIdPath(unit.GridPos, target);
    pathfind.RemoveAt(0); // skip the starting pos, we're already there
    bool tooLong = pathfind.Count > unit.MoveDistance;
    if (tooLong) {
      pathfind = pathfind.Slice(0, unit.MoveDistance);
    }
    return (pathfind.ToList(), tooLong);
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
}
