namespace tur.controls;

using tur.grid;
using tur.units;

using Godot;

using System.Text;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerController : Node3D {
  private Cell? mouseoverCell = null;
  private Unit? selectedUnit = null;

  private Camera3D camera { get => this.GetTypedChild<Camera3D>(); }

  public int QueuedAnimations = 0;
  
  public override void _Ready() {
    The.PlayerController = this;
  }

  public override void _Process(double dt) {
    if (Input.IsActionJustPressed("select")) {
      Unit? prevUnit = this.selectedUnit;
      this.SelectUnit(null);
      if (this.mouseoverCell is Cell c && c.Unit is Unit u) {
        // Allow clicking on the current unit to deselect
        if (u != prevUnit) {
          this.SelectUnit(u);
        }
      }
    }
    if (Input.IsActionJustPressed("command")) {
      if (this.selectedUnit is Unit u
          && this.mouseoverCell is Cell c) {
        Vector2I start = u.ParentCell.GridPos;
        Vector2I end = this.mouseoverCell.GridPos;
        var poses = The.Grid.AStar.GetIdPath(start, end);
        GD.Print(poses);
        if (poses != null) {
          u.QueueMove(c, poses.ToList());
          this.SelectUnit(null);
        }
      }
    }
  }

  public override void _PhysicsProcess(double dt) {
    this.mouseoverCell = this.getMouseoverCell();
  }

  private Cell? getMouseoverCell() {
    Vector2 localMouse = GetViewport().GetMousePosition();
    Vector3 rayOrigin = this.camera.ProjectRayOrigin(localMouse);
    Vector3 rayHeading = this.camera.ProjectRayNormal(localMouse);

    var query = PhysicsRayQueryParameters3D.Create(
      rayOrigin, rayHeading * 1000, 0x1);
    var hit = GetWorld3D().DirectSpaceState.IntersectRay(query);

    if (hit.TryGetValue("collider", out Variant v) && v.AsGodotObject() is Node n) {
      if (n.GetParent() is Cell cell) {
        return cell;
      }
    }
    return null;
  }

  private void SelectUnit(Unit? u) {
    this.selectedUnit?.SetSelected(false);
    this.selectedUnit = u;
    this.selectedUnit?.SetSelected(true);
  }
}
