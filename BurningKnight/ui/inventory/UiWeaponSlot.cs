using System;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.inventory {
	public class UiWeaponSlot : UiEntity {
		private UiInventory inventory;
		private Vector2 activeScale = new Vector2(1);
		private UiItem uiItem;

		public bool Active;
		
		public UiWeaponSlot(UiInventory inv) {
			inventory = inv;
		}

		public override void Init() {
			base.Init();

			uiItem = new UiItem {
				OnTop = true
			};
			
			Area.Add(uiItem);
			
			var area = inventory.Player.Area;
			
			Subscribe<WeaponSwappedEvent>(area);
			Subscribe<ItemAddedEvent>(area);
		}

		public override void Render() {
			var component = Active ? inventory.Player.GetComponent<ActiveWeaponComponent>() : inventory.Player.GetComponent<WeaponComponent>();

			if (component.Disabled) {
				return;
			}
			
			var item = component.Item;

			if (item == null) {
				uiItem.Id = null;
			} else if (item.Id != uiItem.Id) {
				uiItem.Id = item.Id;
				uiItem.Scourged = item.Scourged;
			}

			// Graphics.Render(inventory.ItemSlot, pos, 0, inventory.ItemSlot.Center, activeScale);

			if (uiItem.Region != null) {
				uiItem.Center = new Vector2(uiItem.Region.Width / 2f + (Active ? 8 : 24), Display.UiHeight - uiItem.Region.Height / 2f - 8);
			}
		}
		
		public void Animate() {
			var v = Active ? 2 : 1;
			
			Tween.To(0.6f, activeScale.X, x => activeScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1.5f, activeScale.X, x => activeScale.X = x, 0.1f).OnEnd = () =>
							Tween.To(1f, activeScale.X, x => activeScale.X = x, 0.2f);
						
			Tween.To(1.5f, activeScale.Y, x => activeScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(0.6f, activeScale.Y, x => activeScale.Y = x, 0.1f).OnEnd = () =>
							Tween.To(1f, activeScale.Y, x => activeScale.Y = x, 0.2f);

			
			Tween.To(0.3f, uiItem.IconScale.X, x => uiItem.IconScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1f, uiItem.IconScale.X, x => uiItem.IconScale.X = x, 0.2f);
						
			Tween.To(2f, uiItem.IconScale.Y, x => uiItem.IconScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, uiItem.IconScale.Y, x => uiItem.IconScale.Y = x, 0.2f);
		}

		public override bool HandleEvent(Event e) {
			if (e is WeaponSwappedEvent) {
				Animate();
			} else if (e is ItemAddedEvent iae) {
				if (iae.Who == inventory.Player) {
					if (iae.Item.Type == ItemType.Weapon) {
						Animate();
					}
				}
			}

			return base.HandleEvent(e);
		}
	}
}