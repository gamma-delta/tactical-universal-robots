namespace tur.units;

using tur.grid;
using tur.units.actions;

using System.Text;

using Godot;

public abstract partial class Mind : Resource {
  public abstract UnitAction Decide(Unit unit, Grid grid);

  public abstract void LongDesc(Unit unit, Grid grid, StringBuilder sb);
  public abstract string ShortDesc(Unit unit, Grid grid);
}
