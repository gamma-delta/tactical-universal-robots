namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;
using tur.units.procedure;

using System.Collections.Generic;

using Godot;
using GDColl = Godot.Collections;
using System.Text;
using System.Linq;

[GlobalClass]
public partial class ProcedureMind : Mind {
  public GDColl.Dictionary<string, Variant> Memory = new();
  
  public List<Opcode> Opcodes = new() {
    new OpSelectTarget(),
    new OpAttackTarget(),
  };
  public int Ip = 0;
  
  public override UnitAction Decide(Unit unit, Grid grid) {
    if (this.Opcodes.Count == 0) return new ActionDoNothing();

    Opcode opc = this.Opcodes[this.Ip];
    this.Ip = Mathf.PosMod(this.Ip + 1, this.Opcodes.Count);
    GD.Print($"Executing opcode {opc.LongDesc(unit, this, grid)}");
    UnitAction action = opc.Execute(unit, this, grid);
    return action;
  }

  public override void LongDesc(Unit unit, Grid grid, StringBuilder bob) {
    bob.AppendLine("\nPROCEDURE is");
    for (int i = 0; i < this.Opcodes.Count; i++) {
      Opcode opc = this.Opcodes[i];
      string desc = opc.LongDesc(unit, this, The.Grid);
      char sigil = i == this.Ip ? '>' : ' ';

      bob.Append('[').Append(sigil).Append("] ").Append(desc);
      bob.AppendLine();
    }

    if (this.Memory.Count == 0) {
      bob.AppendLine("\nMEMORY is empty");
    } else {
      bob.AppendLine("\nMEMORY is");
      var sortedMem = this.Memory.OrderBy(kv => kv.Key);
      foreach (var kv in sortedMem) {
        if (kv.Value.VariantType != Variant.Type.Nil) {
          string s;
          if (kv.Value.As<Node>() is Node n) {
            s = n.Name;
          } else {
            s = kv.Value.ToString();
          }
          bob.Append("- $")
            .Append(kv.Key)
            .Append(": ")
            .Append(s);
          bob.AppendLine();
        }
      }
    }
  }

  public override string ShortDesc(Unit unit, Grid grid) {
    Opcode opc = this.Opcodes[this.Ip];
    return opc.ShortDesc(unit, this, grid);
  }
}
