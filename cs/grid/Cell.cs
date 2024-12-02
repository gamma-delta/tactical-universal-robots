namespace tur.grid;

using tur.draw;
using tur.units;

using Godot;

[GlobalClass]
public partial class Cell : Node3D {
  public Vector2I GridPos { get; private set; }

  public void PostCreateFixup(Vector2I gridPos) {
    this.GridPos = gridPos;
    this.Position = Grid.GridPosToWorldPos(gridPos);
  }

  public Unit? Unit { get => this.GetTypedChild<Unit>(); }
}
