namespace tur.controls;

using tur.grid;
using tur.units;
using tur.units.actions;

using Godot;

using System.Text;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerController : Node3D {
  private Cell? mouseoverCell = null;
  private Unit? selectedUnit = null;
  private Unit? lastHoveredUnit = null;

  public bool PlayersTurn { get; private set; } = false;

  private Camera3D camera { get => this.GetNode<Camera3D>("%Camera3D"); }

  private UnitAction? playerDecision = null;

  public override void _Ready() {
    The.PlayerController = this;

    this.resetCameraToGridCenter();
  }

  public override void _Process(double dt) {
    if (!GodotObject.IsInstanceValid(this.lastHoveredUnit))
      this.lastHoveredUnit = null;
    if (!GodotObject.IsInstanceValid(this.selectedUnit))
      this.selectedUnit = null;

    if (this.getMouseoverCell()?.Unit is Unit u && !u.Inanimate) {
      this.lastHoveredUnit = u;
    }
  
    if (this.PlayersTurn && Input.IsActionJustPressed("command")) {
      if (this.selectedUnit is Unit
          && this.mouseoverCell is Cell c) {
        Vector2I end = this.mouseoverCell.GridPos;
        this.playerDecision = new ActionMoveTo(end);
      }
    }

    this.updateSidebars();
  }

  public override void _PhysicsProcess(double dt) {
    this.mouseoverCell = this.getMouseoverCell();
  }

  public void BeginPlayerControlledTurn() {
    this.PlayersTurn = true;
    Unit u = The.Grid.TurnOrder.CurrentUnit();
    this.selectUnit(u);
  }

  public UnitAction? TryConsumePlayerDecision() {
    var decision = this.playerDecision;
    if (decision != null) {
      this.playerDecision = null;
      this.PlayersTurn = false;
    }
    return decision;
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

  private void selectUnit(Unit? u) {
    this.selectedUnit?.SetSelected(false);
    this.selectedUnit = u;
    this.selectedUnit?.SetSelected(true);
  }

  private void resetCameraToGridCenter() {
    Vector3 gridCenterReal = Grid.GridPosToWorldPos(The.Grid.GridSize) / 2f;
    var camPoint = this.camera.Transform.Basis.GetRotationQuaternion()
      * Vector3.Forward;
    this.camera.Position = gridCenterReal - camPoint * 10;
  }

  private void updateSidebars() {
    var rmt = this.GetNode<RichTextLabel>("%ReadMemoryText");
    if (this.lastHoveredUnit is Unit u2) {
      u2.UpdateTerminal(rmt);
    }

    var ttt = this.GetNode<RichTextLabel>("%TurnTrackerText");
    StringBuilder bob = new();
    var units = The.Grid.TurnOrder.Units();
    foreach (var (i, unit) in The.Grid.TurnOrder.Units().Indexed()) {
      char sigil = i == 0 ? '>' : ' ';
      bob.Append('[')
        .Append(sigil)
        .Append("] ")
        .Append(unit.Name)
        .AppendLine();
    }
    ttt.Clear();
    ttt.AddText(bob.ToString());
  }
}
