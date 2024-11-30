namespace tur.items;

using Godot;

/// Represents the resource type.
[GlobalClass]
public partial class BulkResource : Resource {
  [Export]
  public Texture2D texture;
  
  public override string ToString() {
    return $"BulkResource({this.ResourceName})";
  }
}
