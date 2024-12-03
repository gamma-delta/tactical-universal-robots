namespace tur.units.actions;

using tur.grid;

public record DoNothing : UnitAction {
    public void Perform(Unit unit, Grid grid) {
      unit.FinishedWithTurn = true;
    }
}
