namespace tur;

using tur.grid;
using tur.controls;

using Godot;

public class The {
  public class ResourceGetter<T> where T: Resource {
    internal string path;

    internal ResourceGetter(string path) {
      this.path = path;
    }
  
    public T this[string key] { get {
      string fullPath = "res://resources/" 
        + this.path
        + "/" + key + ".tres";
      return ResourceLoader.Load<T>(fullPath);
    } }
  }

  public static Grid Grid;
  public static PlayerController PlayerController;
}
