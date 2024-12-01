namespace tur.units;

using tur.grid;
using Godot;

[GlobalClass]
public partial class Unit : Node3D {
  public void Move(Cell targetCell) {
    this.Reparent(targetCell);
  }

  public Cell ParentCell { get => this.GetParent<Cell>(); }
}
