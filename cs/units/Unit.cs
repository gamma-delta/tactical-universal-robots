namespace tur.units;

using tur.grid;
using tur.units.procedure;

using Godot;

using System.Collections.Generic;
using System.Linq;
using System.Text;

[GlobalClass]
public partial class Unit : Node3D {
  [Export]
  public int MoveDistance = 4;
  /// Set this to true for walls and stuff
  [Export]
  public bool Inanimate = false;
  [Export]
  public Mind Mind;
  
  public Vector2I GridPos { get => this.ParentCell.GridPos; }
  public Cell ParentCell { get => this.GetParent<Cell>(); }

  public bool FinishedWithTurn = true;

  public RandomNumberGenerator rng { get; private set; }

  private Label nameLabel { get => this.GetNode<Label>("%Label"); }

  public override void _Ready() {
    this.rng = new RandomNumberGenerator();
  }

  public override void _Process(double dt) {
    if (!this.Inanimate) {
      string s = this.Name;
      s += "\n" + this.Mind.ShortDesc(this, The.Grid);
      this.nameLabel.Text = s;
    }
  }

  public void SetSelected(bool selected) {
    this.GetNode<Node3D>("%SelectionReticle").Visible = selected;
  }

  public Tween QueueMove(List<Vector2I> path) {
    GD.Print(path.ListToString());
    // add back the original so we have a starting pos
    var theCoolerPath = new List<Vector2I>() { this.GridPos };
    theCoolerPath.AddRange(path);

    Vector2I targetPos = theCoolerPath[^1];
    Cell targetCell = The.Grid.GetCell(targetPos)!;
    this.Reparent(targetCell, keepGlobalTransform: true);

    var tween = this.CreateTween();
    for (int i = 0; i < theCoolerPath.Count - 1; i++) {
      Vector2I coord = theCoolerPath[i];
      Vector2I destination = theCoolerPath[i + 1];
      // This is parented to the TARGET CELL, so do the math to un-offset it
      Vector2I offset = destination - targetCell.GridPos;
      Vector3 worldOffset = Grid.GridPosToWorldPos(offset);
      // make diagonal movement not faster than straight
      float distance = coord.DistanceTo(destination);
      tween.TweenProperty(this, "position", worldOffset, 0.1 * distance);
    }
    return tween;
  }

  public void UpdateTerminal(RichTextLabel label) {
    StringBuilder bob = new StringBuilder();
    bob.AppendLine("NAME is " + this.Name);
    bob.AppendLine("DAMAGE is [todo]");

    this.Mind.LongDesc(this, The.Grid, bob);

    // For some reason two newlines displays some ghost character
    bob.Replace("\n\n", "\n \n");

    label.Clear();
    label.AddText(bob.ToString());
  }
}
