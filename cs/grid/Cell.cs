namespace tur.grid;

using tur.items;
using tur.factory;
using tur.draw;

using Godot;

[GlobalClass]
public partial class Cell : Node2D {
  public static readonly Texture2D GROUND = 
    ResourceLoader.Load<Texture2D>("res://textures/ground.png");

  public readonly Vector2I pos;
  
  public BulkResource? mineable;
  public Construction construction { get; private set; }

  public Cell(Vector2I pos) {
    this.pos = pos;
    this.construction = Construction.NONE;
    this.Position = Grid.GridPosToWorldPos(this.pos);
  }

  public override void _EnterTree() {
    // this.AddChild(new ConduitDrawer(this));
  }

  public Factory? GetFactory() =>
    this.construction switch {
      Construction.Factory it => it.factory,
      _ => null
    };

  public Conduit? GetConduit() =>
    this.construction switch {
      Construction.Conduit it => it.conduit,
      _ => null
    };

  public bool TrySetConduit(Conduit? c) {
    if (this.construction is Construction.Factory) {
      return false;
    } else {
      this.construction = c is Conduit realC 
        ? new Construction.Conduit(realC)
        : Construction.NONE;
      return true;
    }
  }

  public bool TrySetFactory(Factory f) {
    this.construction = new Construction.Factory(f);
    return true;
  }

  public void ClearConstruction() {
    this.construction = Construction.NONE;
  }
}
