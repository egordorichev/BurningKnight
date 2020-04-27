using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.stand {
	public class LampStand : ItemStand {
		public static List<string> AlreadyOnStand = new List<string>();
		
		public LampStand() {
			ShowUnlocked = true;
			dontSaveItem = true;
		}

		public override void AddComponents() {
			base.AddComponents();

			t = Rnd.Float(6);
			AddComponent(new LightComponent(this, 32f, new Color(1f, 0.8f, 0.3f, 1f)));
		}

		public override void PostInit() {
			base.PostInit();
			Check();
			Subscribe<Item.UnlockedEvent>();
		}

		private void Check() {
			if (Item != null) {
				return;
			}
			
			var item = PickItem();

			if (item != null) {
				SetItem(item, null);
				Hidden = false;
			} else if (!Engine.EditingLevel) {
				Hidden = true;
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Item != null) {
				AlreadyOnStand.Remove(Item.Id);
			}
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
			AlreadyOnStand.Remove(item.Id);
		}

		protected override string GetSprite() {
			return "lamp_stand";
		}
		
		protected virtual bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Lamp && item.Id != "bk:no_lamp";
		}

		protected bool ShowUnlocked = true;

		private Item PickItem() {
			var items = new List<ItemData>();

			foreach (var i in Items.Datas.Values) {
				if (ApproveItem(i) && !AlreadyOnStand.Contains(i.Id) && GlobalSave.IsTrue(i.Id)) {
					items.Add(i);
				}
			}

			if (items.Count == 0) {
				var a = AlreadyOnStand;
				return null;
			}
			
			var id = items[0].Id;
			AlreadyOnStand.Add(id);

			return Items.CreateAndAdd(id, Area);
		}

		protected override bool CanInteract(Entity e) {
			return Item != null;
		}

		private float t;
		
		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
			GetComponent<LightComponent>().Light.Radius = 32f + (float) Math.Cos(t) * 6;
		}

		public override bool HandleEvent(Event e) {
			if (e is Item.UnlockedEvent) {
				Check();
			}
			
			return base.HandleEvent(e);
		}
	}
}