namespace tur.units;

using tur.grid;

using Godot;

using System.Collections.Generic;

[GlobalClass]
public partial class Unit : Node3D {
  public Vector2I GridPos { get => this.ParentCell.GridPos; }
  public Cell ParentCell { get => this.GetParent<Cell>(); }

  public void SetSelected(bool selected) {
    this.GetNode<Node3D>("%SelectionReticle").Visible = selected;
  }

  public void QueueMove(Cell targetCell, List<Vector2I> path) {
    this.Reparent(targetCell, keepGlobalTransform: true);
    // The.PlayerController.QueuedAnimations += 1;
    var tween = this.CreateTween();
    for (int i = 0; i < path.Count - 1; i++) {
      Vector2I coord = path[i];
      Vector2I destination = path[i + 1];
      // This is parented to the TARGET CELL, so do the math to un-offset it
      Vector2I offset = destination - targetCell.GridPos;
      Vector3 worldOffset = Grid.GridPosToWorldPos(offset);
      // make diagonal movement not faster than straight
      float distance = coord.DistanceTo(destination);
      tween.TweenProperty(this, "position", worldOffset, 0.1 * distance);
    }
  }
}
