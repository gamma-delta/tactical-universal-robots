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

  /// A null mind means that the player controls it.
  [Export(hintString: "A null mind means the player controls it")]
  public Mind? Mind = null;
  
  public Vector2I GridPos { get => this.ParentCell.GridPos; }
  public Cell ParentCell { get => this.GetParent<Cell>(); }

  public bool FinishedWithTurn = true;

  public RandomNumberGenerator rng { get; private set; }

  public override void _Ready() {
    this.rng = new RandomNumberGenerator();
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

  public void UpdateTextLabel(RichTextLabel label) {
    StringBuilder bob = new StringBuilder();
    bob.AppendLine("NAME is " + this.Name);
    bob.AppendLine("DAMAGE is [todo]");

    if (this.Mind is ProcedureMind proc) {
      bob.AppendLine("\nPROCEDURE is");
      for (int i = 0; i < proc.Opcodes.Count; i++) {
        Opcode opc = proc.Opcodes[i];
        string desc = opc.Stringify(this, proc, The.Grid);
        char sigil = i == proc.Ip ? '>' : ' ';

        bob.Append('[').Append(sigil).Append("] ").Append(opc);
        bob.AppendLine();
      }

      bob.AppendLine("\nMEMORY is");
      var sortedMem = proc.Memory.OrderBy(kv => kv.Key);
      foreach (var kv in sortedMem) {
        if (kv.Value.VariantType != Variant.Type.Nil) {
          bob.Append("- $")
            .Append(kv.Key)
            .Append(": ")
            .Append(kv.Value.ToString());
          bob.AppendLine();
        }
      }
    }

    // For some reason two newlines displays some ghost character
    bob.Replace("\n\n", "\n \n");

    label.Clear();
    label.AddText(bob.ToString());
  }
}
