namespace tur.units.actions;

using tur.grid;

using System.Collections.Generic;
using System.Linq;

using Godot;

public interface UnitAction {
  public abstract void Perform(Unit unit, Grid grid);
}
