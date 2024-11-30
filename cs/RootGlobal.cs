namespace tur;

using grid;

using Godot;

[GlobalClass]
public partial class RootGlobal : Node2D {
  // So it's always accessible, irregardless of runtime nodes,
  // make it the singleton in this group.
  public static readonly StringName GROUP = "root_global";

  public override void _Ready() {
    AddToGroup(GROUP);
  }

  public static RootGlobal Get(SceneTree tree) {
    return (RootGlobal) tree.GetFirstNodeInGroup(GROUP);
  }
}
