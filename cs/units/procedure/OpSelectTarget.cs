namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;

using System.Linq;

using Godot;

public record OpSelectTarget : Opcode {
  public UnitAction Execute(Unit me, ProcedureMind mind, Grid grid) {
    Unit? aPlayer = grid.GetUnits()
      .FirstOrDefault(u => u.Mind is MindPlayer, null);
    mind.Memory["target"] = aPlayer;
    return new ActionDoNothing();
  }

  public string LongDesc(Unit me, ProcedureMind mind, Grid grid) {
    return "SELECT $target";
  }

  public string ShortDesc(Unit me, ProcedureMind mind, Grid grid) {
    return "SELECT $target";
  }
}

