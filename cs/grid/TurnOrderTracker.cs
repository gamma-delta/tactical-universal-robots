namespace tur.grid;

using tur.units;

using Godot;

using System.Collections.Generic;
using System;

public class TurnOrderTracker {
  private List<Unit> units = new();
  // The "head" of the list, to prevent tons of reshuffling
  private int cursor = 0;

  public void AddUnit(Unit unit) {
    if (this.units.Contains(unit)) {
      GD.PrintErr($"{unit} was already in the turn order tracker");
      return;
    }

    // Place it right before the "start"
    if (this.units.Count == 0) {
      this.units.Add(unit);
    } else {
      this.units.Insert(Mathf.PosMod(this.cursor - 1, this.units.Count), unit);
    }
  }

  public void NextTurn() {
    this.cursor = Mathf.PosMod(this.cursor + 1, this.units.Count);
  }

  public Unit? CurrentUnit() {
    return this.units.Count == 0 ? null : this.units[this.cursor];
  }

  public void RemoveUnit(Unit unit) {
    int presentIdx = this.units.IndexOf(unit);
    if (presentIdx == -1) {
      GD.PrintErr($"Tried to remove unit {unit} from the turn order tracker but it was not there");
      return;
    }

    if (presentIdx < this.cursor) {
      this.cursor -= 1;
    }
    this.units.RemoveAt(presentIdx);
  }

  public IEnumerable<Unit> Units() {
    for (int i = 0; i < this.units.Count; i++) {
      int realIdx = Mathf.PosMod(this.cursor + i, this.units.Count);
      yield return this.units[realIdx];
    }
  }
}
