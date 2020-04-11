using System;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.pool;
using BurningKnight.save;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.chest {
	public class ChestRegistry : Pool<Type> {
		public static ChestRegistry Instance = new ChestRegistry();

		static ChestRegistry() {
			Instance.Add(typeof(WoodenChest), 1f);
			Instance.Add(typeof(ScourgedChest), 0.9f);
			Instance.Add(typeof(DoubleChest), 0.1f);
			Instance.Add(typeof(TripleChest), 0.02f);
			Instance.Add(typeof(StoneChest), 1f);
			Instance.Add(typeof(GoldChest), 1f);
			Instance.Add(typeof(RedChest), 0.5f);
			Instance.Add(typeof(GlassChest), 0.5f);
		}

		public static Entity PlaceRandom(Vector2 where, Area area) {
			try {
				var chest = (Chest) Activator.CreateInstance(Instance.Generate());
				
				
				if (!(chest is GlassChest || chest is ProtoChest) && Rnd.Chance(LevelSave.MimicChance)) {
					var mimic = new Mimic {
						Kind = chest.GetSprite(),
						Pool = chest.GetPool()
					};

					area.Add(mimic);
					mimic.BottomCenter = where;
					
					return mimic;
				}
				
				area.Add(chest);
				chest.BottomCenter = where;

				return chest;
			} catch (Exception ex) {
				Log.Error(ex);
			}

			return null;
		}
	}
}