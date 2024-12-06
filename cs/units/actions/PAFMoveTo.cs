namespace tur.units.actions;

using tur.grid;

using System.Collections.Generic;
using System.Linq;

using Godot;

[GlobalClass]
public partial class PAFMoveTo : PlayerActionFactory {
  /// Return Null to say that there is nothing to pick.
  public override List<Vector2I>? PlayerPickableCells(Unit unit, Grid grid) {
    return grid.CellsWithinRange(unit.GridPos, unit.MoveDistance);
  }

  public override UnitAction MakeAction(Vector2I? choice) {
    return new ActionMoveTo((Vector2I)choice);
  }
}
