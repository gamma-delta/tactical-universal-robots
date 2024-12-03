namespace tur.units;

using tur.grid;

using Godot;

[GlobalClass]
public partial class Team : Resource {
  [Export(hintString: "Set if enemies should never try to target this")]
  public bool NonAggressive;

  [Export]
  public Color HpColor;
  [Export]
  public Color HpMissingColor;
}
