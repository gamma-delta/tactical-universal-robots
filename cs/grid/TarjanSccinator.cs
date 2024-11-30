namespace tur.grid;

using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

using tur.factory;

using Godot;

// wow, graphs are so cool ,,,, i wish mr tarjan was real,,,
public static class TarjanSccinator {

  public struct Output {
    public Dictionary<Vector2I, List<Factory>> factorySourcesGraph;
    public List<Factory> tickOrder;
  }

  public static Output Calculate(Grid grid) {
    var graph = MakeGraph(grid);
    var sccs = TarjanScc(graph);

    Dictionary<Vector2I, List<Factory>> outDict = new();
    foreach (var (pos, vert) in graph) {
      outDict.Add(pos, vert.sources);
    }

    // foreach (var (pos, sources) in outDict) {
    //   GD.Print($"- {pos} <- {String.Join(" ", sources.Select(f => f.cell.pos))}");
    // }

    // Each blob is the minimum "loopy" area.
    // Sort that in reading order.
    List<Factory> tickOrder = new();
    foreach (var blob in sccs) {
      blob.Sort((a, b) => {
        if (a.pos.Y != b.pos.Y) {
          return (a.pos.Y < b.pos.Y) ? -1 : 1;
        } else if (a.pos.X != b.pos.X) {
          return (a.pos.X < b.pos.X) ? -1 : 1;
        } else {
          return 0;
        }
      });
      tickOrder.AddRange(blob.Select(v => 
        grid.GetCell(v.pos)!.GetFactory()!));
    }

    return new Output {
      factorySourcesGraph = outDict,
      tickOrder = tickOrder,
    };
  }
  
  private class GraphTy : Dictionary<Vector2I, Vert> {}
  class Vert {
    public Vector2I pos;
    public List<Factory> sources = new();
    public int? index = null;
    public int lowlink = 0;

    public Vert(Vector2I pos) {
      this.pos = pos;
    }
  }
  // thanks rosetta code
  private static List<List<Vert>> TarjanScc(GraphTy graph) {
    // capsed to remind me it's closed over
    int Index = 0;
    Stack<Vert> WorkStack = new();

    List<List<Vert>> sccs = new();

    Action<Vert> strongConnect = (_) => {};
    strongConnect = (v) => {
      v.index = Index;
      v.lowlink = Index;

      Index += 1;
      WorkStack.Push(v);

      foreach (var factory in v.sources) {
        Vert w = graph[factory.cell.pos];
        if (w.index == null) {
          strongConnect.Invoke(w);
          v.lowlink = Math.Min(v.lowlink, w.lowlink);
        } else if (WorkStack.Contains(w)) {
          v.lowlink = Math.Min(v.lowlink, (int)w.index);
        }
      }

      if (v.lowlink == v.index) {
        List<Vert> scc = new();
        Vert w;
        do {
          w = WorkStack.Pop();
          scc.Add(w);
        } while (w != v);
        sccs.Add(scc);
      }
    };

    foreach (Vert v in graph.Values) {
      if (v.index == null) {
        strongConnect.Invoke(v);
      }
    }

    return sccs;
  }

  // each node => the factories it takes from
  // conveniently, the SCCs are the same no matter which way the
  // arrows go if i flip them all, so i'll calculate the graph first.
  private static GraphTy MakeGraph(
    Grid grid
  ) {
    GraphTy outputGraph = new();

    HashSet<Vector2I> knownFactoryPoses = new();
    foreach (var cell in grid.GetCells()) {
      if (cell.GetFactory() is Factory f && 
        knownFactoryPoses.Add(cell.pos)) {
        // Make sure even factories with no sources
        // get put in the graph
        if (!outputGraph.ContainsKey(cell.pos)) {
          outputGraph.Add(cell.pos, new Vert(cell.pos));
        }
        
        var (sinks, _) = TraceFactoryOutput(grid, f);
        foreach (var sink in sinks) {
          // we have get_or_insert_with at home
          if (!outputGraph.ContainsKey(sink.cell.pos)) {
            outputGraph.Add(sink.cell.pos, new Vert(sink.cell.pos));
          }
          List<Factory> sources = outputGraph[sink.cell.pos].sources;
          sources.Add(f);
        }
      }
    }

    return outputGraph;
  }

  // return the destination factories, and any other factories also
  // outputting to this conduit. returns itself in the second
  // list for convenience
  private static (List<Factory>, List<Factory>) TraceFactoryOutput(
    Grid grid, Factory startFactory) {
    // contains the cell and the direction entered from.
    Stack<(Vector2I, Direction4)> work; {
      work = new();
      Direction4 startDir = startFactory.outDir;
      work.Push((startFactory.cell.pos + startDir.Delta(), 
        startDir.Flip()));
    };

    // This only tracks non-crossover conduits, because xover
    // probably needs to be visited twice.
    // Fortunately there's no way to make a loop using only xovers
    // so it's ok to not put them in here.
    // This saves me having to track like, "entered direction"
    // on conduits or some shit
    HashSet<Vector2I> visitedConduitPoses = new();
    List<Factory> sinks = new();
    List<Factory> siblings = new() { startFactory };

    while (work.Count != 0) {
      var (pos, enterDir) = work.Pop();
      if (!grid.GridPosInBounds(pos)) {
        continue;
      }
      Cell cell = grid.GetCell(pos)!;
      if (cell.GetFactory() is Factory f) {
        // eyy we found something
        if (f.outDir == enterDir) {
          // then another factory is outputting onto this line.
          // remember the enter dir is "from this direction,"
          // and this means we're outputting *to* that direction,
          // so it's the same direction
          siblings.Add(f);
        } else {
          // we found a destination
          sinks.Add(f);
        }
        continue;
      }

      if (cell.GetConduit() is Conduit c) {
        if (c.isCrossover || visitedConduitPoses.Add(pos)) {
          Direction4Set nextDirs = c.OutputsTo(enterDir);
          // GD.Print($"{pos.ToString()} {enterDir} -> " +
          //   String.Join(' ', nextDirs.IterDirections()));
          foreach (var nextDir in nextDirs.IterDirections()) {
            work.Push((pos + nextDir.Delta(), nextDir.Flip()));
          }
        }
      }
    }

    return (sinks, siblings);
  }
}
