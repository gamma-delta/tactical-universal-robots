namespace tur.grid;

using Godot;

public class Conduit {
  // The display of the input direction is calculated only
  // at draw time.

  public static Texture2D ATLAS = ResourceLoader.Load<Texture2D>(
    "res://textures/conduit-map.png");

  public Direction4Set dirs;
  public bool isCrossover = false;

  public Conduit(Direction4Set dirs) {
    this.dirs = dirs;
  }

  public Direction4Set OutputsTo(Direction4 input) {
    if (this.isCrossover) {
      // Only output to the opposite direction.
      return Direction4Set.One(input.Flip());
    } else {
      // Normal case: output but not to the input.
      return this.dirs.SetDir(input, false);
    }
  }
}
