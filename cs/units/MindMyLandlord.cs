namespace tur.units;

using tur.grid;
using tur.units.actions;

using Godot;

[GlobalClass]
public partial class MindMyLandlord : Mind {
  public override UnitAction Decide(Unit unit, Grid grid) {
    return new ActionDoNothing();
  }
}

