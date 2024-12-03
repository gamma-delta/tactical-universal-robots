namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;
using tur.units.procedure;

using System.Collections.Generic;

using Godot;
using GDColl = Godot.Collections;

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
    GD.Print($"Executing opcode {opc.Stringify(unit, this, grid)}");
    UnitAction action = opc.Execute(unit, this, grid);
    return action;
  }
}
