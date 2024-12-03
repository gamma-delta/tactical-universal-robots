namespace tur.units;

using tur.grid;

using Godot;

using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Unit : Node3D {
  [Export]
  public int MoveDistance = 4;
  /// Set this to true for walls and stuff
  [Export]
  public bool AlwaysSkipTurns = false;

  /// A null mind means that the player controls it.
  [Export(hintString: "A null mind means the player controls it")]
  public Mind? Mind = null;
  
  public Vector2I GridPos { get => this.ParentCell.GridPos; }
  public Cell ParentCell { get => this.GetParent<Cell>(); }

  public bool FinishedWithTurn = true;

  public RandomNumberGenerator rng { get; private set; }

  public override void _Ready() {
    this.rng = new RandomNumberGenerator();
  }

  public void SetSelected(bool selected) {
    this.GetNode<Node3D>("%SelectionReticle").Visible = selected;
  }

  public Tween QueueMove(List<Vector2I> path) {
    GD.Print(path.ListToString());
    // add back the original so we have a starting pos
    var theCoolerPath = new List<Vector2I>() { this.GridPos };
    theCoolerPath.AddRange(path);

    Vector2I targetPos = theCoolerPath[^1];
    Cell targetCell = The.Grid.GetCell(targetPos)!;
    this.Reparent(targetCell, keepGlobalTransform: true);

    var tween = this.CreateTween();
    for (int i = 0; i < theCoolerPath.Count - 1; i++) {
      Vector2I coord = theCoolerPath[i];
      Vector2I destination = theCoolerPath[i + 1];
      // This is parented to the TARGET CELL, so do the math to un-offset it
      Vector2I offset = destination - targetCell.GridPos;
      Vector3 worldOffset = Grid.GridPosToWorldPos(offset);
      // make diagonal movement not faster than straight
      float distance = coord.DistanceTo(destination);
      tween.TweenProperty(this, "position", worldOffset, 0.1 * distance);
    }
    return tween;
  }
}
