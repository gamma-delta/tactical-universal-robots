namespace tur.units;

using tur.grid;
using tur.units.actions;

using Godot;

public abstract partial class Mind : Resource {
  public abstract UnitAction Decide(Unit unit, Grid grid);
}
