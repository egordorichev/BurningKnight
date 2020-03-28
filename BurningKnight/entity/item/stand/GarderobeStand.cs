using System.Collections.Generic;
using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class GarderobeStand : ItemStand {
		public static List<string> AlreadyOnStand = new List<string>();

		public GarderobeStand() {
			dontSaveItem = true;
		}

		public override void Destroy() {
			base.Destroy();

			if (Item != null) {
				AlreadyOnStand.Remove(Item.Id);
			}
		}
		
		protected override string GetSprite() {
			return "gobetta_stand";
		}

		public override void Init() {
			base.Init();
			var item = PickItem();

			if (item != null) {
				SetItem(item, null);
			}
		}

		public void UpdateItem() {
			if (Item != null) {
				return;
			}
			
			var item = PickItem();

			if (item != null) {
				SetItem(item, null);
			}
		}

		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}
		
			var ht = entity.GetComponent<HatComponent>();
			var old = ht.Item;
			
			ht.Set(item, true);
			item = null;
			
			SetItem(old, entity);
		
			return false;
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
			Achievements.Unlock("bk:fancy_hat");
		}

		private Item PickItem() {
			var items = new List<ItemData>();

			foreach (var i in Items.Datas.Values) {
				if (i.Type == ItemType.Hat && GlobalSave.GetString("hat") != i.Id && (i.Id == "bk:no_hat" || GlobalSave.IsTrue(i.Id)) && !AlreadyOnStand.Contains(i.Id)) {
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
	}
}