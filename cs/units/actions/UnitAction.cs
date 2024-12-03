namespace tur.units.actions;

using tur.grid;

using System.Collections.Generic;
using System.Linq;

using Godot;

public interface UnitAction {
  public abstract void Perform(Unit unit, Grid grid);

  public record Move(Vector2I target) : UnitAction {
    public void Perform(Unit unit, Grid grid) {
      // Pathfind
      var pathfind = grid.AStar.GetIdPath(unit.GridPos, target);
      if (pathfind.Count > unit.MoveDistance) {
        pathfind = pathfind.Slice(0, unit.MoveDistance);
      }

      var tween = unit.QueueMove(pathfind.ToList());
      tween.TweenCallback(Callable.From(() => {
        unit.FinishedWithTurn = true;
      }));
    }
  }
}
