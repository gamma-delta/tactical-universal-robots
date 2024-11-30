namespace tur.draw;

using tur.grid;

using Godot;

// the easiest way to rotate a flipping
[GlobalClass]
public partial class ConduitDrawer : Sprite2D {
  protected readonly Cell parent;

  public ConduitDrawer(Cell parent) {
    this.parent = parent;
  }

  public override void _Ready() {
  }
}
