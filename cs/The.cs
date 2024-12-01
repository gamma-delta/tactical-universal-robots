namespace tur;

using Godot;

public static class The {
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
}
