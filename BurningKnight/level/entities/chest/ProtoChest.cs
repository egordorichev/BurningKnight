using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.chest {
	public class ProtoChest : AnimatedChest {
		private List<Player> colling = new List<Player>();
		private InteractFx fx;
		private TextureRegion itemRegion;

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ItemComponent {
				DontSave = true
			});
			
			var i = GetComponent<InteractableComponent>();
			
			i.CanInteract = e => e.GetComponent<ActiveWeaponComponent>().Item != null;
			i.OnStart = e => AddFx();
			
			try {
				var id = GlobalSave.GetString("proto_chest", "bk:ancient_revolver");

				if (id != null) {
					var item = Items.CreateAndAdd(id, Area);
					GetComponent<ItemComponent>().Set(item, false);
					itemRegion = item.Region;
				}
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public override void Destroy() {
			GlobalSave.Put("proto_chest", GetComponent<ItemComponent>().Item?.Id);
			base.Destroy();
		}

		private void AddFx() {
			if (fx != null && !fx.Done) {
				fx.Close();
			}
			
			var i = GetComponent<ItemComponent>().Item;
			Engine.Instance.State.Ui.Add(fx = new InteractFx(this, i == null ? Locale.Get("place_an_item") : i.Name));
		}

		protected override float GetWidth() {
			return 19;
		}

		protected override float GetHeight() {
			return 14;
		}

		protected override string GetSprite() {
			return "proto_chest";
		}

		protected override void SpawnDrops() {
			
		}

		protected override bool ShouldOpen() {
			return true;
		}

		protected override bool Interact(Entity entity) {
			var w = entity.GetComponent<ActiveWeaponComponent>();
			GetComponent<ItemComponent>().Exchange(w);

			if (w.Item != null) {
				Audio.PlaySfx(w.Item.Data.WeaponType.GetSwapSfx());
				entity.GetComponent<PlayerGraphicsComponent>().AnimateSwap();
			} else if (entity.GetComponent<WeaponComponent>().Item != null) {
				w.RequestSwap();
			}
			
			itemRegion = GetComponent<ItemComponent>().Item.Region;
			AddFx();
			return false;
		}

		public override void Save(FileWriter stream) {
			open = false;
			base.Save(stream);
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player p) {
					colling.Add(p);
					Open();
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Player p) {
					colling.Remove(p);
					if (colling.Count == 0) {
						Close();
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Render() {
			base.Render();

			if (open && itemRegion != null) {
				Graphics.Render(itemRegion, Position + new Vector2(10, 7), 0, itemRegion.Center, GetComponent<AnimationComponent>().Scale);
			}
		}
	}
}