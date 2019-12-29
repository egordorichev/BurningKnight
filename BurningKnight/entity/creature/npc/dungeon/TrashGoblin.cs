using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class TrashGoblin : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 11;
			Height = 21;
			Flips = false;
			
			AddComponent(new AnimationComponent("trash_goblin"));
		}

		public override string GetId() {
			return ShopNpc.TrashGoblin;
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is TrashGoblinStand;
		}

		public static void Place(Vector2 where, Area area) {
			where.Y -= 16;
			
			var snek = new TrashGoblin();
			area.Add(snek);
			snek.BottomCenter = where;
			
			var stand = new TrashGoblinStand();
			area.Add(stand);
			stand.Center = where + new Vector2(0, 4 + stand.Height);

			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.TrashGoblin), area, false), null);
		}
	}
}