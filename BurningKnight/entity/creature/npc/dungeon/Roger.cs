using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Roger : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 18;
			Height = 24;
			Flips = false;
			
			AddComponent(new AnimationComponent("roger"));
		}

		public override string GetId() {
			return ShopNpc.Roger;
		}
		
		protected override string GetDealDialog() {
			return $"roger_{Rnd.Int(4)}";
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is RogerStand;
		}

		public static void Place(Vector2 where, Area area) {
			where.Y -= 16;
			
			var roger = new Roger();
			area.Add(roger);
			roger.BottomCenter = where;
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Roger));

			for (var i = -1; i < 2; i++) {
				var stand = new RogerStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 8) * i, 4 + stand.Height);

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area, false), null);
			}
		}
	}
}