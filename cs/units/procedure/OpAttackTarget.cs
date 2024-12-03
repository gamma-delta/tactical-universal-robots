namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;

using System.Linq;

using Godot;

public record OpAttackTarget : Opcode {
  public UnitAction Execute(Unit me, ProcedureMind mind, Grid grid) {
    if (mind.Memory.TryGetValue("target", out var v) && v.As<Unit>() is Unit u) {
      return new actions.Attack(u);
    }
    return new actions.DoNothing();
  }

  public string Stringify(Unit me, ProcedureMind mind, Grid grid) {
    return "Attack $target";
  }
}


