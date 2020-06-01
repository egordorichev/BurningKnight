using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class Boxy : DungeonShopNpc {
		private bool open;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Width = 15;
			Height = 18;
			Flips = false;
			
			AddComponent(new AnimationComponent("boxy"));
			GetComponent<AnimationComponent>().Animation.Tag = "idle";

			GetComponent<DropsComponent>().Add("bk:boxy");
			
			AddComponent(new RectBodyComponent(0, 9, 15, 9, BodyType.Static));
		}

		public override void PostInit() {
			base.PostInit();

			if (open) {
				GetComponent<AnimationComponent>().Animation.Tag = "open";
			}
		}

		protected override void OnItemBought(ItemBoughtEvent ibe) {
			if (open) {
				return;
			}
			
			foreach (var s in GetComponent<RoomComponent>().Room.Tagged[Tags.Item]) {
				if (s is BoxyStand st && st != ibe.Stand && st.Item != null) {
					return;
				}
			}

			var d = GetComponent<DialogComponent>();
			
			d.StartAndClose("boxy_9", 3);

			Timer.Add(() => {
				AnimationUtil.Poof(Center);
				
				GetComponent<DropsComponent>().SpawnDrops();

				open = true;
				var a = GetComponent<AnimationComponent>();
				a.Animation.Tag = "open";
				a.Animate();
				
				Achievements.Unlock("bk:open_up");
			}, 4f);
		}

		public override string GetId() {
			return ShopNpc.Boxy;
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is BoxyStand;
		}

		public static void Place(Vector2 where, Area area) {
			where.Y -= 16;
			
			var snek = new Boxy();
			area.Add(snek);
			snek.BottomCenter = where;
			
			var pool = Items.GeneratePool(Items.GetPool(ItemPool.Boxy));

			for (var i = -1; i < 2; i++) {
				var stand = new BoxyStand();
				area.Add(stand);
				stand.Center = where + new Vector2((stand.Width + 4) * i, 4 + stand.Height);

				var id = Items.GenerateAndRemove(pool, null, true);
				stand.SetItem(Items.CreateAndAdd(id, area, false), null);
			}
		}
		
		protected override string GetDealDialog() {
			return $"boxy_{Rnd.Int(3)}";
		}

		protected override string GetHiDialog() {
			return $"boxy_{Rnd.Int(3, 6)}";
		}
		
		public override string GetFailDialog() {
			return $"boxy_{Rnd.Int(7, 9)}";
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			open = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(open);
		}
		
		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}
	}
}