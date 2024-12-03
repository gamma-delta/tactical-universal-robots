namespace tur;

using tur.grid;
using System.Linq;
using System.Collections.Generic;

using Godot;
using System.Text;

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

  public static T? GetTypedChild<T>(this Node node) where T: Node {
    foreach (Node n in node.GetChildren()) {
      if (n is T nT) return nT;
    }
    return null;
  }

  public static Godot.Collections.Array<T> GdArray<[MustBeVariant]T>(params T[] vals) {
    return new Godot.Collections.Array<T>(vals);
  }

  public static string ListToString<T>(this List<T> list) {
    var bob = new StringBuilder();
    bob.Append('[');
    bob.AppendJoin(", ", list);
    bob.Append(']');
    return bob.ToString();
  }
}

/// exact input
public static class ExInput {
  public static bool IsActionJustPressed(StringName key) =>
    Input.IsActionJustPressed(key, true);
  public static bool IsActionPressed(StringName key) =>
    Input.IsActionPressed(key, true);
}
