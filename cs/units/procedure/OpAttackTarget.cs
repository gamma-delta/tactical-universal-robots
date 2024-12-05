namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;

using System.Linq;

using Godot;

public record OpAttackTarget : Opcode {
  public UnitAction Execute(Unit me, ProcedureMind mind, Grid grid) {
    if (mind.Memory.TryGetValue("target", out var v) && v.As<Unit>() is Unit u) {
      return new ActionAttack(u);
    }
    return new ActionDoNothing();
  }

  public string LongDesc(Unit me, ProcedureMind mind, Grid grid) {
    return "ATTACK $target";
  }

  public string ShortDesc(Unit me, ProcedureMind mind, Grid grid) {
    return "ATTACK $target";
  }
}
