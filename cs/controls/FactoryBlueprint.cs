namespace tur.controls;

using tur.factory;
using tur.grid;
using tur.items;

using Godot;

using System;
using System.Collections.Generic;

public class FactoryBlueprint {
  public Func<Cell, Factory> maker;
  public Texture2D texture;

  public FactoryBlueprint(Func<Cell, Factory> maker, Texture2D texture) {
    this.maker = maker;
    this.texture = texture;
  }

  public static readonly FactoryBlueprint MINER = new(
    (c) => new Miner(c),
    ResourceLoader.Load<Texture2D>("res://textures/miner.png")
  );
  public static readonly FactoryBlueprint TEST = new(
    (c) => new CraftingFactory(c, 
      new[] {new BulkResourceStack(The.Bulks["coal"], 10)},
      new BulkResourceStack(The.Bulks["electricity"], 20)),
    ResourceLoader.Load<Texture2D>("res://textures/factory.png")
  );
  public static readonly FactoryBlueprint COPPER_MATERIALIZER = new(
    (c) => new CraftingFactory(c, 
      new[] {new BulkResourceStack(The.Bulks["electricity"], 10)},
      new BulkResourceStack(The.Bulks["copper"], 5)),
    ResourceLoader.Load<Texture2D>("res://textures/factory.png")
  );

  public static readonly List<FactoryBlueprint> BLUEPRINTS = new() {
    FactoryBlueprint.MINER,
    FactoryBlueprint.TEST,
    FactoryBlueprint.COPPER_MATERIALIZER,
  };
}
