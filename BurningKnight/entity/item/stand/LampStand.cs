using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.graphics;

namespace BurningKnight.entity.item.stand {
	public class LampStand : ItemStand {
		public static List<string> AlreadyOnStand = new List<string>();
		
		public LampStand() {
			dontSaveItem = true;
		}

		public override void Init() {
			base.Init();
			var item = PickItem();

			if (item != null) {
				SetItem(item, null);
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
				if (ApproveItem(i) && !AlreadyOnStand.Contains(i.Id) && (ShowUnlocked || GlobalSave.IsFalse(i.Id))) {
					items.Add(i);
				}
			}

			if (items.Count == 0) {
				return null;
			}
			
			items.Sort((a, b) => a.UnlockPrice.CompareTo(b.UnlockPrice));

			var id = items[0].Id;
			AlreadyOnStand.Add(id);

			return Items.CreateAndAdd(id, Area);
		}

		protected override bool CanInteract(Entity e) {
			return Item != null;
		}
	}
}