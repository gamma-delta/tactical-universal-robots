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

  public bool PlayersTurn { get; private set; } = false;

  private Camera3D camera { get => this.GetTypedChild<Camera3D>(); }

  private UnitAction? playerDecision = null;

  public override void _Ready() {
    The.PlayerController = this;

    this.resetCameraToGridCenter();
  }

  public override void _Process(double dt) {
    if (this.PlayersTurn && Input.IsActionJustPressed("command")) {
      if (this.selectedUnit is Unit u
          && this.mouseoverCell is Cell c) {
        Vector2I end = this.mouseoverCell.GridPos;
        this.playerDecision = new ActionMoveTo(end);
      }
    }
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
    Vector2 gridCenter = (Vector2)The.Grid.GridSize / 2f;
    this.Position = new(3 + gridCenter.X, 5, 3 + gridCenter.Y);
  }
}
