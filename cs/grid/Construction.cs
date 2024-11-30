namespace tur.grid;

public abstract class Construction {
  // prevent spawning new objects all the time
  public static None NONE { get => None.OhGodWhyCSharp; }
  
  public class None : Construction {
    private None() {}
    public static readonly None OhGodWhyCSharp = new None();
  }

  public class Factory : Construction {
    public readonly factory.Factory factory;

    public Factory(factory.Factory factory) {
      this.factory = factory;
    }
  }

  public class Conduit : Construction {
    public readonly grid.Conduit conduit;

    public Conduit(grid.Conduit conduit) {
      this.conduit = conduit;
    }
  }
}
