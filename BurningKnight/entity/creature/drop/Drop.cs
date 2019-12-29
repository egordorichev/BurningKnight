using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.loot;
using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.drop {
	public abstract class Drop {
		// From 0 to 1
		public float Chance = 1f;
		
		public virtual List<string> GetItems() { 
			return new List<string>();
		}

		public abstract string GetId();

		public virtual void Load(JsonValue root) {
			Chance = root["chance"].Number(1);
		}

		public virtual void Save(JsonValue root) {
			root["chance"] = Chance;
		}

		public static void Create(string id, Entity entity, Area area = null, Dot where = null) {
			var drop = Drops.Get(id);

			if (drop == null) {
				return;
			}
			
			Create(new List<Drop> { drop }, entity, area, where);
		}

		public static void Create(List<Drop> dr, Entity entity, Area area = null, Dot where = null) {
			var drops = new List<Item>();
			var ar = entity?.Area ?? area;
			var wh = entity?.BottomCenter ?? where;
			
			foreach (var drop in dr) {
				if (Rnd.Float() > drop.Chance) {
					continue;
				}
				
				var ids = drop.GetItems();

				foreach (var id in ids) {
					if (id != null) {
						if (id == "bk:troll_bomb") {
							var bomb = new Bomb(entity);
							ar.Add(bomb);
							bomb.Center = wh;
							
							continue;
						}
						
						var item = Items.Create(id);

						if (item != null) {
							drops.Add(item);
						}
					}
				}
			}

			if (entity is DropModifier d) {
				d.ModifyDrops(drops);
			}

			foreach (var item in drops) {
				item.CenterX = wh.X;
				item.CenterY = wh.Y + 4;
				ar.Add(item);
				item.AddDroppedComponents();
				item.RandomizeVelocity(1f);
			}
		}
	}
}