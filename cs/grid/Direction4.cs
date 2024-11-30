namespace tur.grid;

using System;
using System.Collections.Generic;

using Godot;

public enum Direction4 {
  North, East, South, West,
}

public struct Direction4Set {
  private byte directions = 0b0;
  
  public Direction4Set(byte mask = 0) {
    this.directions = mask;
  }

  public static Direction4Set Empty() {
    return new Direction4Set();
  }

  public static Direction4Set One(Direction4 dir) {
    return Direction4Set.Empty() | dir;
  }

  public readonly bool IsDirSet(Direction4 d) {
    return (d.Mask() & this.directions) != 0;
  }

  public readonly Direction4Set SetDir(Direction4 d, bool turnOn=true) {
    byte mask = d.Mask();
    // bitwise operators promote to int for some godforsaken reason
    return new((byte)(turnOn
      ? this.directions | mask
      : this.directions & (~mask)));
  }

  public readonly bool IsEmpty() {
    return this.directions == 0;
  }

  public readonly byte Directions() => this.directions;

  public static Direction4[] AllDirections() {
    return Enum.GetValues<Direction4>();
  }

  public IEnumerable<Direction4> IterDirections() {
    foreach (var dir in AllDirections()) {
      if (this & dir) {
        yield return dir;
      }
    }
  }

  public static bool operator &(Direction4Set dirs, Direction4 d) =>
    dirs.IsDirSet(d);

  public static Direction4Set operator |(Direction4Set dirs, Direction4 d) =>
    dirs.SetDir(d);
}

public static class Direction4Ext {
  public static Direction4 Rotated(this Direction4 d, int by) {
    int idx = (int)d + by;
    int idxMod = (idx % 4 + 4) % 4;
    return Direction4.GetValues<Direction4>()[idxMod];
  }

  public static Direction4 Flip(this Direction4 d) => d.Rotated(2);

  public static byte Mask(this Direction4 d) {
    return (byte)(1 << (byte)d);
  }

  public static Vector2I Delta(this Direction4 d) {
    return (d) switch {
      Direction4.North => new(0, -1),
      Direction4.East => new(1, 0),
      Direction4.South => new(0, 1),
      Direction4.West => new(-1, 0),
      _ => throw new Exception("stop that"),
    };
  }
}
