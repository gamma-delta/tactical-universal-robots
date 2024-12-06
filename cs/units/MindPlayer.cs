namespace tur.units;

using tur.grid;
using tur.units.actions;

using System.Text;
using System.Collections.Generic;

using Godot;
using GDColl = Godot.Collections;

[GlobalClass]
public partial class MindPlayer : Mind {
  [Export]
  public GDColl.Array<PlayerActionFactory> KnownActions = new();
  
  public override UnitAction Decide(Unit unit, Grid grid) {
    throw new System.Exception("don't call this");
  }

  public override void LongDesc(Unit unit, Grid grid, StringBuilder sb){ }
  
  public override string ShortDesc(Unit unit, Grid grid) => "";
}


