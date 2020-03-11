using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.chest {
	public class GlassChest : Chest {
		private Item item;
		private float t;
		
		protected override string GetSprite() {
			return "glass_chest";
		}

		protected override void DefineDrops() {
			
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var c) || c.Keys < 1) {
				return false;
			}

			c.Keys -= 1;
			return true;
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		protected override void SpawnDrops() {
			item.AddDroppedComponents();
			item.TopCenter = BottomCenter;
			item = null;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			var id = stream.ReadString();

			if (id != null) {
				item = Items.CreateAndAdd(id, Area);
				item.RemoveDroppedComponents();
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(item?.Id);
		}

		public override void Render() {
			if (item != null) {
				var region = item.Region;
				var s = Math.Min(13f / region.Width, 13f / region.Height);
				
				Graphics.Render(region, Center + new Vector2(0, ItemGraphicsComponent.CalculateMove(t) / 5.5f), (float) Math.Cos(t * 1.8f) * 0.2f, region.Center, new Vector2(s, s));
			}

			base.Render();
		}

		public override void PostInit() {
			base.PostInit();

			if (!open && item == null) {
				item = Items.CreateAndAdd(Items.Generate(ItemPool.GlassChest), Area);
				item.RemoveDroppedComponents();
			}
		}
	}
}