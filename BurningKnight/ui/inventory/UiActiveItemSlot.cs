using System;
using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.inventory {
	public class UiActiveItemSlot : UiEntity {
		private UiInventory inventory;

		private TextureRegion activeSide;
		private TextureRegion activeBorder;
		private TextureRegion activeEmpty;
		private TextureRegion activeFull;

		private Vector2 activeScale = new Vector2(1);

		public float ActivePosition = -1f;
		private bool tweened;

		private UiItem uiItem;
		
		public UiActiveItemSlot(UiInventory inv) {
			inventory = inv;
		}

		public override void Init() {
			base.Init();

			uiItem = new UiItem {
				OnTop = true
			};
			
			Area.Add(uiItem);
			
			var area = inventory.Player.Area;
			
			Subscribe<ItemUsedEvent>(area);
			Subscribe<ItemAddedEvent>(area);
			
			var anim = Animations.Get("ui");

			activeSide = anim.GetSlice("active_side");
			activeBorder = anim.GetSlice("active_border");
			activeEmpty = anim.GetSlice("active_empty");
			activeFull = anim.GetSlice("active_full");
			
			if (inventory.Player?.GetComponent<ActiveItemComponent>().Item != null) {
				ActivePosition = 0;
			}
		}

		public override void Render() {
			if (ActivePosition <= -0.99f) {
				return;
			}
			
			var component = inventory.Player.GetComponent<ActiveItemComponent>();
			var item = component.Item;
			
			if (item != null && item.Id != uiItem.Id) {
				uiItem.Id = item.Id;
				uiItem.Scourged = item.Scourged;
			}

			var v = ActivePosition * (inventory.ItemSlot.Width + 10);
			var pos = new Vector2(inventory.UseSlot.Center.X + 8 + v, inventory.UseSlot.Center.Y + 8);
			
			Graphics.Render(inventory.ItemSlot, pos, 0, inventory.ItemSlot.Center, activeScale);

			uiItem.Center = pos;
			
			if (item == null || (item.Done && !tweened)) {
				tweened = true;
					
				Tween.To(-1, 0, x => ActivePosition = x, 0.3f).OnEnd = () => {
					inventory.Player.GetComponent<ActiveItemComponent>().Clear();
					tweened = false;
				};
			}
			
			if (item != null) {
				// var region = item.Region;
				var timed = item.UseTime < 0;
				var chargeMax = Math.Abs(item.UseTime);
				var charge = timed ? chargeMax - item.Delay : (float) Math.Floor(chargeMax - item.Delay);

				uiItem.DrawBorder = Math.Abs(charge - chargeMax) < 0.01f;

				// Render the use time
				if (Math.Abs(item.UseTime) <= 0.01f) {
					return;
				}
				
				pos.X += inventory.ItemSlot.Width / 2f;
				pos.Y -= inventory.ItemSlot.Height / 2f;

				var h = inventory.ItemSlot.Height;
				var cellH = timed ? (h - 2) : (float) Math.Floor((h - 2) / chargeMax);
				var barH = (timed ? h : (float) Math.Floor((double) ((h - 2) / (int) chargeMax)) * chargeMax);
				var n = timed ? 0 : (h - barH - 1);
			
				Graphics.Render(activeSide, pos + new Vector2(0, n));
				Graphics.Render(activeSide, pos + new Vector2(0, inventory.ItemSlot.Height - 1));
			
				Graphics.Render(activeBorder, pos + new Vector2(3, 1 + n), 0, Vector2.Zero, new Vector2(1, barH - 1));

				if (charge < chargeMax) {
					Graphics.Render(activeEmpty, pos + new Vector2(0, 1 + n), 0, Vector2.Zero, new Vector2(1, barH - (timed ? 2 : 1)));
				}

				if (charge > 0) {
					if (timed) {
						var hh = charge / chargeMax * (cellH);
						Graphics.Render(activeFull, pos + new Vector2(0, barH - hh - 1), 0, Vector2.Zero,
							new Vector2(1, hh));
					} else {
						for (var i = 0; i < charge; i++) {
							Graphics.Render(activeFull, pos + new Vector2(0, h - (i + 1) * cellH), 0, Vector2.Zero,
								new Vector2(1, cellH - 1));
						}
					}
				}

				if (!timed) {
					for (var i = 0; i < chargeMax - 1; i++) {
						Graphics.Render(activeSide, pos + new Vector2(0, barH - (i + 1) * cellH + n));
					}
				}
			}
		}
		
		public void Animate() {
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
			if (e is ItemUsedEvent item) {
				if (inventory.Player.GetComponent<ActiveItemComponent>().Item == item.Item) {
					Animate();
				}
			} else if (e is ItemAddedEvent iae) {
				if (iae.Who == inventory.Player) {
					if (iae.Item.Type == ItemType.Active) {
						if (ActivePosition <= 0f || tweened) {
							Tween.To(0, -1, x => ActivePosition = x, 0.6f, Ease.BackOut);
						} else {
							Animate();
						}
					}
				}
			}

			return base.HandleEvent(e);
		}
	}
}