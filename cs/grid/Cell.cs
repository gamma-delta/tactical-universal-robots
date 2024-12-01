namespace tur.grid;

using tur.draw;
using tur.units;

using Godot;

[GlobalClass]
public partial class Cell : Node3D {
  public Vector2I GridPos { get; private set; }

  public override void _Ready() {
    base._Ready();

    this.GridPos = Grid.WorldPosToGridPos(this.Position);
  }

  public Unit? Unit { get => this.GetTypedChild<Unit>(); }
}
