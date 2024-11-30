namespace tur;

using tur.grid;
using Godot;

public static class Extensions {
  public static RootGlobal GetRootGlobal(this Node node) {
    return RootGlobal.Get(node.GetTree());
  }

  public static T LoadScene<T>(string path)
    where T: Node
  {
    PackedScene scene = ResourceLoader.Load<PackedScene>(path);
    return scene.Instantiate<T>();
  }

  public static T LoadPrefab<T>(string path)
    where T: Node
  {
    return LoadScene<T>("res://scenes/prefabs/" + path + ".tscn");
  }

  public static void DrawTextureRegionRotated(
    this Node2D node, Texture2D tex, Vector2 pos, float rot,
    Vector2 size, Vector2 sliceOrigin
  ) {
    node.DrawSetTransformMatrix(Transform2D.Identity
      .Translated(-size / 2)
      .Rotated(rot)
      .Translated(size / 2 + pos));
    node.DrawTextureRectRegion(tex, 
      new(Vector2.Zero, size), new(sliceOrigin, size));
    node.DrawSetTransformMatrix(Transform2D.Identity);
  }
}

/// exact input
public static class ExInput {
  public static bool IsActionJustPressed(StringName key) =>
    Input.IsActionJustPressed(key, true);
  public static bool IsActionPressed(StringName key) =>
    Input.IsActionPressed(key, true);
}
