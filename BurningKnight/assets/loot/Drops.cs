using System.Collections.Generic;
using BurningKnight.assets.mod;
using BurningKnight.entity.creature.drop;
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
				new SingleDrop("bk:potatoo")	
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
			
			Define("pouch", new AnyDrop(
				new SimpleDrop(0.7f, 1, 2, "bk:key"),
				new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
				new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
				new SimpleDrop(0.6f, 1, 4, "bk:coin")
			));
			
			
			Define("rock", new AnyDrop(
				new SingleDrop("bk:pickaxe", 0.01f)
			));
			
			Define("tinted_rock", new AnyDrop(
				new SimpleDrop(0.3f, 1, 2, "bk:key"),
				new SimpleDrop(0.3f, 1, 2, "bk:bomb"),
				new SimpleDrop(0.3f, 1, 2, "bk:troll_bomb"),
				new SimpleDrop(0.6f, 1, 2, "bk:heart")
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