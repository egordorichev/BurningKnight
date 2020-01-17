using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Snek : DungeonShopNpc {
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 18;
			Height = 24;
			Flips = false;
			
			AddComponent(new AnimationComponent("snek"));			
			AddComponent(new RectBodyComponent(1, 14, 16, 10, BodyType.Static));
		}

		protected override void OnItemBought(ItemBoughtEvent ibe) {
			foreach (var s in GetComponent<RoomComponent>().Room.Tagged[Tags.Item]) {
				if (s is SnekStand st && st != ibe.Stand && st.Item != null) {
					return;
				}
			}

			var d = GetComponent<DialogComponent>();
			
			d.StartAndClose("snek_6", 3);

			Timer.Add(() => {
				d.StartAndClose("snek_7", 3);
				
				Timer.Add(() => {
					AnimationUtil.Poof(Center);
					Done = true;

					var stand = new SnekStand();
					Area.Add(stand);
					stand.Center = Center;
					stand.SetItem(Items.CreateAndAdd("bk:snek", Area), this);
				}, 4f);
			}, 4f);
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
				var stand = new SnekStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 4) * i, 4 + stand.Height);

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area, false), null);
			}
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is SnekStand;
		}
		
		protected override string GetDealDialog() {
			return $"snek_{Rnd.Int(3)}";
		}

		protected override string GetHiDialog() {
			return $"snek_{Rnd.Int(3, 6)}";
		}
	}
}