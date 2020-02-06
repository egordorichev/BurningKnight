using System.Collections.Generic;
using BurningKnight.assets.mod;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.item;
using Lens.util;

namespace BurningKnight.assets.loot {
	public static class Drops {
		public static Dictionary<string, Drop> Defined = new Dictionary<string, Drop>();

		static Drops() {
			Define("wooden_chest", new OneOfDrop(
				new AnyDrop(
					new SimpleDrop(0.7f, 1, 2, "bk:key"),
					new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
					new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
					new SimpleDrop(0.6f, 1, 4, "bk:coin")
				),
				
				new PoolDrop(ItemPool.WoodenChest)
			));
			
			Define("gold_chest", new PoolDrop(ItemPool.GoldChest));
			Define("double_chest", new PoolDrop(ItemPool.DoubleChest));
			Define("triple_chest", new PoolDrop(ItemPool.TripleChest));
			Define("red_chest", new PoolDrop(ItemPool.RedChest));
			
			Define("stone_chest", new AnyDrop(
				new PoolDrop(ItemPool.StoneChest),
				
				new AnyDrop(
					new SimpleDrop(0.7f, 1, 2, "bk:key"),
					new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
					new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
					new SimpleDrop(0.6f, 1, 4, "bk:coin")
				) { Chance = 0.5f }
			));
			
			Define("scourged_chest", new PoolDrop(ItemPool.ScourgedChest));
			Define("duck_chest", new PoolDrop(ItemPool.DuckChest));

			Define("pouch", new OneOfDrop(
				new SimpleDrop(0.7f, 1, 2, "bk:key"),
				new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
				new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
				new SimpleDrop(0.6f, 1, 4, "bk:coin")
			));
			
			Define("rock", new AnyDrop(
				// new SingleDrop("bk:pickaxe", 0.01f)
			));
			
			Define("tinted_rock", new OneOfDrop(
				new SimpleDrop(0.2f, 1, 2, "bk:key"),
				new SimpleDrop(0.2f, 1, 2, "bk:bomb"),
				new SimpleDrop(0.2f, 1, 2, "bk:troll_bomb"),
				new SimpleDrop(0.4f, 1, 2, "bk:heart")
			));
			
			Define("safe", new AnyDrop(
				new SimpleDrop {
					Items = new [] {
						"bk:coin"
					},
				
					Chance = 0.8f,
					Min = 3,
					Max = 10
				},
				
				new SimpleDrop {
					Items = new[] {
						"bk:key"
					},

					Chance = 0.5f,
					Min = 1,
					Max = 4
				},
				
				new SimpleDrop {
					Items = new [] {
						"bk:bomb"
					},
				
					Chance = 0.3f,
					Min = 1,
					Max = 2
				},
				
				new PoolDrop(ItemPool.Safe)
			));

			Define("charger", new AnyDrop(
				new SimpleDrop {
					Items = new [] {
						"bk:battery"
					},
				
					Chance = 0.5f,
					Min = 1,
					Max = 2
				},
				
				new SimpleDrop {
					Items = new [] {
						"bk:coin"
					},
				
					Chance = 0.5f,
					Min = 1,
					Max = 3
				},
				
				new PoolDrop(ItemPool.Charger, 0.5f)
			));
			
			Define("vending_machine", new AnyDrop(
				new SimpleDrop {
					Chance = 1f,
					Items = new[] {
						"bk:coin"
					},
				
					Min = 2,
					Max = 7
				}, 
				
				new SimpleDrop {
					Chance = 0.3f,
					Items = new[] {
						"bk:key"
					}
				}
			));

			Define("boxy", new OneOfDrop(
				new OneOfDrop(
					new SimpleDrop(0.7f, 1, 1, "bk:coin_pouch"),
					new SimpleDrop(0.5f, 1, 1, "bk:bloody_chest"),
					new SimpleDrop(0.5f, 1, 1, "bk:backpack"),
					new SimpleDrop(0.6f, 1, 1, "bk:star")
				) {
					Chance = 0.9f
				},
				
				new SimpleDrop {
					Items = new [] {
						"bk:coin"
					},
				
					Chance = 0.8f,
					Min = 5,
					Max = 8
				}
			));
		}

		public static Drop Get(string drop) {
			if (!Defined.TryGetValue(drop, out var d)) {
				Log.Error($"Unknown drop {drop}");
				return null;
			}

			return d;
		}
		
		public static void Define(string id, Drop drop, Mod mod = null) {
			Defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = drop;
		}
	}
}