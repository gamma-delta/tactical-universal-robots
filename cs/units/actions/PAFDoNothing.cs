namespace tur.units.actions;

using tur.grid;

using System.Collections.Generic;
using System.Linq;

using Godot;

[GlobalClass]
public partial class PAFDoNothing : PlayerActionFactory {
  public override List<Vector2I>? PlayerPickableCells(Unit unit, Grid grid) {
    return null;
  }

  public override UnitAction MakeAction(Vector2I? choice) {
    return new ActionDoNothing();
  }
}

