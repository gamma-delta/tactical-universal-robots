namespace tur.units.actions;

using tur.grid;

using System.Collections.Generic;
using System.Linq;

using Godot;

[GlobalClass]
public abstract partial class PlayerActionFactory : Resource {
  /// Return Null to say that there is nothing to pick.
  public virtual List<Vector2I>? PlayerPickableCells(Unit unit, Grid grid) {
    return null;
  }

  public abstract UnitAction MakeAction(Vector2I? choice);
}

