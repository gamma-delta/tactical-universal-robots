namespace tur.tools;

using tur.grid;

using Godot;
using GDColl = Godot.Collections;

[GlobalClass]
[Tool]
public partial class MakeGrid : EditorScript {
  public override void _Run() {
    EditorInterface.Singleton.PopupNodeSelector(
      Callable.From<NodePath>(gridPath => {
        Grid grid = GetScene().GetNode<Grid>(gridPath);
      }), Extensions.GdArray((StringName)"Grid"));
  }
}
