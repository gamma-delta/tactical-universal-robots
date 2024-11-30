namespace tur.controls;

using tur.grid;
using tur.factory;
using tur.items;

using Godot;

using System.Text;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class PlayerController : Node2D {
  private Grid grid = null;

  public int ticksLeft { get; private set; } = 100;

  private Vector2I? dragPointStart = null;

  public override void _Ready() {
    this.grid = new();
  }

  public override void _Process(double dt) {
    var maybeMpos = this.grid.MouseOverPos(this.GetLocalMousePosition());
    if (maybeMpos is Vector2I mp && grid.GetCell(mp) is Cell c) {
      this.ProcessMouseOnCell(mp, c);
    }

    if (ExInput.IsActionJustPressed("play")) {
      this.grid.TickAllFactories();
    }

    this.QueueRedraw();
  }

  private void ProcessMouseOnCell(Vector2I mousePos, Cell cell) {
    if (ExInput.IsActionJustPressed("debug")) {
      GD.Print(mousePos);
      if (cell.GetFactory() is Factory f) {
        GD.Print(f);
        GD.Print(f.bufferOut.DumpContents());
      }
    }

    bool rotLeft = ExInput.IsActionJustPressed("rotate_left");
    bool rotRight = ExInput.IsActionJustPressed("rotate_right");
    if (rotLeft ^ rotRight) {
      int rot = rotRight ? 1 : -1;
      if (cell.GetFactory() is Factory f) {
        f.Rotate(rot);
      } else if (cell.GetConduit() is Conduit c) {
        c.isCrossover = !c.isCrossover;
      }
    }

    int? mbBpIndex = null;
    for (int i = 0; i < 5; i++) {
      int shortcutIdx = i + 1;
      if (ExInput.IsActionJustPressed("shortcut_" + shortcutIdx)) {
        mbBpIndex = i;
        break;
      }
    }
    if (mbBpIndex is int idx && idx < FactoryBlueprint.BLUEPRINTS.Count) {
      FactoryBlueprint bp = FactoryBlueprint.BLUEPRINTS[idx];
      cell.TrySetFactory(bp.maker.Invoke(cell));
    }

    if (this.dragPointStart is Vector2I dragStart) {
      if (!ExInput.IsActionPressed("drag_conduit")) {
        this.dragPointStart = null;
      } else {
        Direction4? mbDragDir = (mousePos - dragStart) switch {
          {X: 0, Y: -1} => Direction4.North,
          {X: 1, Y: 0} => Direction4.East,
          {X: 0, Y: 1} => Direction4.South,
          {X: -1, Y: 0} => Direction4.West,
          _ => null,
        };
        if (mbDragDir is Direction4 dragDir
          && this.grid.GetCell(dragStart) is Cell prevCell
        ) {
          // enter this branch even if the next cell doesn't accept
          // a conduit so i can drag "into" factories
          if (prevCell.GetConduit() is Conduit prevCond) {
            prevCond.dirs = prevCond.dirs.SetDir(dragDir);
          } // else something bad happened

          Direction4 inputDir = dragDir.Flip();
          if (cell.GetConduit() is Conduit hereCond) {
            hereCond.dirs = hereCond.dirs.SetDir(inputDir);
          } else {
            cell.TrySetConduit(
              new Conduit(Direction4Set.One(inputDir)));
          }

          this.dragPointStart = mousePos;
        }
      }
    } else {
      if (ExInput.IsActionJustPressed("drag_conduit")) {
        this.dragPointStart = mousePos;
        if (cell.GetConduit() == null) {
          cell.TrySetConduit(new Conduit(Direction4Set.Empty()));
        }
      } else if (ExInput.IsActionPressed("erase_conduit")) {
        cell.ClearConstruction();
      }
    }
  }

  public override void _Draw() {
    for (int y = 0; y < grid.Height(); y++) {
      for (int x = 0; x < grid.Width(); x++) {
        Vector2I gridPos = new(x, y);
        Cell cell = grid.GetCell(gridPos)!;
        Vector2 pos = Grid.GridPosToWorldPos(gridPos);

        DrawTexture(Cell.GROUND, pos);

        if (cell.mineable is BulkResource br) {
          DrawTexture(br.texture, pos);
        }

        switch (cell.construction) {
          case Construction.Conduit { conduit: var cond }: {
            int idx = cond.dirs.Directions();

            Vector2 sz = new Vector2(
              Grid.PX_PER_SQUARE, Grid.PX_PER_SQUARE);
            Vector2 slice = cond.isCrossover
              ? new(0, Grid.PX_PER_SQUARE)
              : new(Grid.PX_PER_SQUARE * cond.dirs.Directions(), 0);
            DrawTextureRectRegion(Conduit.ATLAS,
              new Rect2(pos, sz),
              new Rect2(slice, sz));
            break;
          }

          case Construction.Factory { factory: var f }: {
            DrawTexture(f.Texture(), pos);
            var outTex = f.OutputOverlayTex();
            var w = outTex.GetWidth() / 4;
            var h = outTex.GetHeight();
            var sx = w * (int)f.outDir;
            DrawTextureRectRegion(
              outTex, new Rect2(pos, new(w, h)),
              new Rect2(sx, 0, w, h)
            );
            break;
          }
        }
      }
    }
  }
}
