using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item.stand {
	public class ShopStand : ItemStand {
		public bool Sells = true;
		public bool Free;
		private int price;
		private string priceString;
		private float priceX;
		private bool onSale;
		private bool hasSale;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new AudioEmitterComponent());
		}

		public override void Init() {
			base.Init();
			onSale = Random.Chance(10 + Run.Luck * 2);
		}

		protected override string GetSprite() {
			return "shop_stand";
		}

		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (!Sells || Free) {
				return true;
			}

			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Coins < price) {
				AnimationUtil.ActionFailed();

				foreach (var n in GetComponent<RoomComponent>().Room.Tagged[Tags.Npc]) {
					if (n is ShopNpc || n is ShopKeeper) {
						n.GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Random.Int(15, 18)}", 3);
						break;
					}
				}
				
				return false;
			}

			component.Coins -= price;
			Sells = false;

			GetComponent<AudioEmitterComponent>().Emit("item_purchase");
			
			return true;
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);

			if (!Free) {
				who.HandleEvent(new ItemBoughtEvent {
					Item = item,
					Who = who,
					Stand = this
				});
			}
		}

		public override void Render() {
			base.Render();

			if (Item != null && Sells && !Free) {
				if (hasSale) {
					Graphics.Color = Palette.Default[35];
				}
				
				var r = GetComponent<RoomComponent>().Room;

				foreach (var p in r.Tagged[Tags.Player]) {
					if (p.GetComponent<ConsumablesComponent>().Coins < price) {
						Graphics.Color *= 0.6f;
						break;
					}					
				}
				
				Graphics.Print(priceString, Font.Small, Position + new Vector2(priceX, 14));
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}

		public void Recalculate() {
			if (Free) {
				price = 0;
				return;
			}
			
			if (Item == null) {
				price = 0;
				Sells = false;
				return;
			}
			
			Sells = true;
			price = PriceCalculator.Calculate(Item);

			if (onSale) {
				price = (int) Math.Floor(price * 0.5f);
				hasSale = true;
			}
			
			var r = GetComponent<RoomComponent>().Room;

			if (r != null) {
				var e = new ItemPriceCalculationEvent {
					Stand = this,
					Item = Item
				};

				foreach (var p in r.Tagged[Tags.Player]) {
					p.HandleEvent(e);
				}

				if (e.Percent > 0.001f) {
					price = (int) Math.Floor(price * Math.Max(0f, (1f - e.Percent * 0.01f)));
					hasSale = true;
				}

				if (r.Tagged[Tags.ShopKeeper].Count > 0) {
					var min = sbyte.MaxValue;
					
					foreach (var s in r.Tagged[Tags.ShopKeeper]) {
						min = Math.Min(min, ((ShopKeeper) s).Mood);
					}

					price = Math.Max(1, price - (min - 3));
				}
			}

			CalculatePriceSize();
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemPlacedEvent || e is RoomChangedEvent) {
				Recalculate();
			}
			
			return base.HandleEvent(e);
		}

		private void CalculatePriceSize() {
			priceString = $"{price}";
			priceX = (Width - Font.Small.MeasureString(priceString).Width) / 2f;
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputInt("Price", ref price)) {
				CalculatePriceSize();
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			onSale = stream.ReadBoolean();
			Free = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(onSale);
			stream.WriteBoolean(Free);
		}
	}
}