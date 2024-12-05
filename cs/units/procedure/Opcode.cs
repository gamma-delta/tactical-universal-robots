namespace tur.units.procedure;

using tur.grid;
using tur.units.actions;

public interface Opcode {
  /// Sorry for all the indirection.
  public UnitAction Execute(Unit me, ProcedureMind mind, Grid grid);
  public string LongDesc(Unit me, ProcedureMind mind, Grid grid);
  public string ShortDesc(Unit me, ProcedureMind mind, Grid grid);
}
