namespace tur.units.actions;

using tur.grid;

using System.Linq;

using Godot;

public record ActionMoveTo(Vector2I target) : UnitAction {
  public void Perform(Unit unit, Grid grid) {
    // Pathfind
    var (pathfind, _tooLong) = grid.GetAStarPath(unit, target);

    var tween = unit.QueueMove(pathfind.ToList());
    tween.TweenCallback(Callable.From(() => {
      unit.FinishedWithTurn = true;
    }));
  }
}
