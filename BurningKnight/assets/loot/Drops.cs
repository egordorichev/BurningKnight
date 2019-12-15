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
				
				new AnyDrop(
					new SingleDrop("bk:halo", 1f)
				)
			));
			
			Define("gold_chest", new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo"),
				new SingleDrop("bk:spike")
			));

			Define("double_chest", new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo")
			));
			
			Define("triple_chest", new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo")
			));
			
			Define("red_chest", new OneOfDrop(
				new SingleDrop("bk:broken_heart"),
				new SimpleDrop(1f, 3, 8, "bk:coin"),
				
				new EmptyDrop(0.5f)
			));
			
			Define("stone_chest", new AnyDrop(
				new OneOfDrop(
					new SingleDrop("bk:tnt"),
					new SingleDrop("bk:ninjia_bomb"),
					new SingleDrop("bk:crying_bomb")
				),
				
				new AnyDrop(
					new SimpleDrop(0.7f, 1, 2, "bk:key"),
					new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
					new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
					new SimpleDrop(0.6f, 1, 4, "bk:coin")
				) { Chance = 0.5f }
			));
			
			Define("cursed_chest", new OneOfDrop(
				new AnyDrop(
					new SingleDrop("bk:halo", 1f)
				),
				
				new EmptyDrop(0.5f)
			));
			
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