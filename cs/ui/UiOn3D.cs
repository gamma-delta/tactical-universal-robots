// See: https://github.com/godotengine/godot-demo-projects/blob/master/3d/waypoints/waypoint.gd

namespace tur.ui;

using Godot;

[GlobalClass]
public partial class UiOn3D : Control {
  private Camera3D camera { get => GetViewport().GetCamera3D(); }
  private Node3D parent { get => GetParent<Node3D>(); }

  public override void _Process(double dt) {
    var parentPos = parent.GlobalTransform.Origin;
    var camTf = camera.Transform;
    var camPos = camTf.Origin;

    var distance = camPos.DistanceTo(parentPos);
    var isBehind = camTf.Basis.Z.Dot(parentPos - camPos) > 0;
    // Fade the waypoint if the camera gets too close
    // do we need this in ortho mode?
    this.Modulate = this.Modulate with {
      A = Mathf.Clamp(Mathf.Remap(distance, 0, 2, 0, 1), 0, 1)
    };

    var unprojectedPos = camera.UnprojectPosition(parentPos);
    var vpWindow = this.GetViewport() as Window;
    var viewportBaseSize =
      vpWindow.ContentScaleSize > Vector2I.Zero
        ? vpWindow.ContentScaleSize
        : vpWindow.Size;

    // Never sticky
    this.Position = unprojectedPos;
    this.Visible = !isBehind;
  }
}
