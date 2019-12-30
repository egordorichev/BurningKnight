using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using BurningKnight.level.entities;
using BurningKnight.level.entities.chest;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc.dungeon {
	public class TrashGoblin : DungeonShopNpc {
		private bool freed;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AlwaysActive = true;
			Height = 26;
			Flips = false;
			
			AddComponent(new AnimationComponent("trash_goblin"));
			AddComponent(new RectBodyComponent(0, 16, 14, 10, BodyType.Static));
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(freed);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			freed = stream.ReadBoolean();
		}

		public override void PostInit() {
			base.PostInit();

			if (freed) {
				GetComponent<AnimationComponent>().Animation.Tag = "free";
			}
		}

		public override string GetId() {
			return ShopNpc.TrashGoblin;
		}

		public void Free() {
			if (freed) {
				return;
			}

			freed = true;
			
			GetComponent<AnimationComponent>().Animation.Tag = "free";
			GetComponent<DialogComponent>().StartAndClose("trash_goblin_1", 5);

			Timer.Add(() => {
				try {
					var chest = (Chest) Activator.CreateInstance(ChestRegistry.Instance.Generate());
					Area.Add(chest);
					chest.TopCenter = BottomCenter + new Vector2(0, 4);
				} catch (Exception ex) {
					Log.Error(ex);
				}
			}, 3f);
		}

		protected override string GetHiDialog() {
			return freed ? "trash_goblin_2" : "trash_goblin_0";
		}

		protected override bool OwnsStand(ItemStand stand) {
			return stand is TrashGoblinStand;
		}

		public static void Place(Vector2 where, Area area) {
			where.Y -= 16;
			
			var goblin = new TrashGoblin();
			area.Add(goblin);
			goblin.BottomCenter = where;
			goblin.X += 1;

			var stand = new TrashGoblinStand();
			area.Add(stand);
			stand.Center = where + new Vector2(0, 4 + stand.Height);

			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.TrashGoblin), area, false), null);
		}

		public override bool ShouldCollide(Entity entity) {
			return entity is Creature || base.ShouldCollide(entity);
		}
	}
}