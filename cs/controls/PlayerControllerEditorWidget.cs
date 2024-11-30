namespace tur.controls;

using tur.grid;

using Godot;

using System.Text;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
[Tool]
public partial class PlayerControllerEditorWidget : Node2D {
  public override void _Draw() {
    if (!Engine.IsEditorHint()) {
      return;
    }

    int sz = Grid.PX_PER_SQUARE * 8;
    DrawRect(new(0, 0, sz, sz), Color.Color8(100, 255, 100), false);
    DrawRect(new(0, 0, sz, sz), Color.Color8(100, 255, 100, 100), true);
  }
}
