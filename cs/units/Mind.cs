namespace tur.units;

using tur.grid;

using Godot;

public abstract class Mind {
  public abstract UnitAction Decide(Unit unit, Grid grid);

  public class MoveRandomly : Mind {
    public override UnitAction Decide(Unit unit, Grid grid) {
      Vector2I pos = unit.GridPos;
      Vector2I moveMaxVec = new Vector2I(unit.MoveDistance, unit.MoveDistance);
      Vector2I min = (pos - moveMaxVec)
        .Clamp(Vector2I.Zero, grid.GridSize - new Vector2I(1, 1));
      Vector2I max = (pos + moveMaxVec)
        .Clamp(Vector2I.Zero, grid.GridSize - new Vector2I(1, 1));
      int targetX = unit.rng.RandiRange(min.X, max.X);
      int targetY = unit.rng.RandiRange(min.Y, max.Y);

      return new UnitAction.Move(new(targetX, targetY));
    }
  }
}
