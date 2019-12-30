using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Snek : DungeonShopNpc {
		/*
		 * todo:
		 * at least 10 items in item pool
		 *  - frog
		 *
		 * 
		 * remove his items from other pools
		 * the slavery pet end
		 * make sure resupply works only with shop
		 * his custom dialog
		 * saving him before he first appears
		 */
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 18;
			Height = 24;
			Flips = false;
			
			AddComponent(new AnimationComponent("snek"));			
			AddComponent(new RectBodyComponent(1, 14, 16, 10, BodyType.Static));
		}

		public override string GetId() {
			return ShopNpc.Snek;
		}

		public static void Place(Vector2 where, Area area) {
			var snek = new Snek();
			area.Add(snek);
			snek.BottomCenter = where;
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Snek));

			for (var i = -1; i < 2; i++) {
				var stand = new ShopStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 4) * i, 4 + stand.Height);

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area, false), null);
			}
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}
	}
}