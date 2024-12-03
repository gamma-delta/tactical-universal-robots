namespace tur.units.actions;

using Godot;

using tur.grid;

public record Attack(Unit target) : UnitAction {
    public void Perform(Unit me, Grid grid) {
      GD.Print($"Bang, shooting unit {target}");
      var tw = me.CreateTween();
      tw.TweenInterval(1);
      tw.TweenCallback(Callable.From(() => {
        me.FinishedWithTurn = true;
      }));
    }
}

