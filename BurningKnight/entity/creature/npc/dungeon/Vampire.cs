using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Vampire : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			Width = 22;
			Height = 30;
			Flips = false;
			
			AddComponent(new AnimationComponent("vampire"));
			AddComponent(new RectBodyComponent(4, 19, 14, 11, BodyType.Static, false));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2));
			AddComponent(new InteractableComponent(Interact));
		}

		private bool Interact(Entity entity) {
			if (entity.GetComponent<HealthComponent>().ModifyHealth(-1, this)) {
				for (var i = 0; i < 3; i++) {
					var coin = Items.CreateAndAdd("bk:copper_coin", Area);
					coin.TopCenter = BottomCenter;
				}	
			}
			
			return false;
		}

		public override string GetId() {
			return ShopNpc.Vampire;
		}

		public static void Place(Vector2 where, Area area) {
			var sells = true || Rnd.Chance(60); // todo: random

			if (sells) {
				where.Y -= 16;
			}

			var vampire = new Vampire();
			area.Add(vampire);
			vampire.BottomCenter = where;

			if (!sells) {
				return;
			}
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Vampire));
			var c = Rnd.Int(1, 4);
			var s = (int) Math.Floor(c / 2f) * 18;
			
			for (var i = 0; i < c; i++) {
				var stand = new VampireStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 4) * i - s, 4 + stand.Height);

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area), null);
			}
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}
	}
}