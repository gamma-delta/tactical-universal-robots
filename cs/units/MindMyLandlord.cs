namespace tur.units;

using tur.grid;
using tur.units.actions;

using System.Text;

using Godot;

[GlobalClass]
public partial class MindMyLandlord : Mind {
  public override UnitAction Decide(Unit unit, Grid grid) {
    return new ActionDoNothing();
  }

  public override void LongDesc(Unit unit, Grid grid, StringBuilder sb){ }
  
  public override string ShortDesc(Unit unit, Grid grid) => "";
}

