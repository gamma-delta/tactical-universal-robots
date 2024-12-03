namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;

public record OpNop : Opcode {
  public UnitAction Execute(Unit me, ProcedureMind mind, Grid grid) {
    return new ActionDoNothing();
  }

  public string Stringify(Unit me, ProcedureMind mind, Grid grid) {
    return "NOOP";
  }
}
