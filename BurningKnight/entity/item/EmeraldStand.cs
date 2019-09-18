using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	/*
	 * todo: show price
	 */
	public class EmeraldStand : ItemStand {
		public static List<string> AlreadyOnStand = new List<string>();
		
		private int price;
		private string priceString;
		private float priceX;

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

		protected override string GetSprite() {
			return "shop_stand";
		}
		
		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (GlobalSave.Emeralds < price) {
				AnimationUtil.ActionFailed();
				return false;
			}

			GlobalSave.Emeralds -= price;
			Items.Unlock(Item.Id);
			AlreadyOnStand.Remove(Item.Id);
			Item.Done = true;
			
			var item = PickItem();
			SetItem(item, entity, false);

			AnimationUtil.Poof(Center, 1);

			return false;
		}

		private Item PickItem() {
			var items = new List<ItemData>();

			foreach (var i in Items.Datas.Values) {
				if (i.Lockable && i.UnlockPrice > 0 && !AlreadyOnStand.Contains(i.Id) && GlobalSave.IsFalse(i.Id)) {
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

		public override void Render() {
			base.Render();

			if (Item != null) {
				Graphics.Print(priceString, Font.Small, Position + new Vector2(priceX, 14));
			}
		}

		public override void SetItem(Item i, Entity entity, bool remove = true) {
			if (i != null) {
				price = i.Data.UnlockPrice;
				priceString = $"{price}";
				priceX = (Width - Font.Small.MeasureString(priceString).Width) / 2f;
			}
			
			base.SetItem(i, entity, remove);
		}
	}
}