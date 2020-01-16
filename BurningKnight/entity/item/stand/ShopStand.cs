using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using ImGuiNET;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.stand {
	public class ShopStand : ItemStand {
		public bool Sells = true;
		public bool Free;

		protected int Price;
		protected string PriceString;
		protected float PriceX;
		protected bool OnSale;
		protected bool HasSale;

		public override void Init() {
			base.Init();
			OnSale = Rnd.Chance(10 + Run.Luck * 2);
		}

		public override ItemPool GetPool() {
			return ItemPool.Shop;
		}

		protected override string GetSprite() {
			return "shop_stand";
		}

		protected override bool CanInteract(Entity e) {
			return Item != null && base.CanInteract(e);
		}

		protected virtual bool TryPay(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Coins < Price) {
				return false;
			}

			component.Coins -= Price;
			// fixme: add it
			Achievements.Unlock("bk:shopper");
			
			return true;
		}

		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (!Sells || Free) {
				return true;
			}
			
			if (!TryPay(entity)) {
				AnimationUtil.ActionFailed();

				foreach (var n in GetComponent<RoomComponent>().Room.Tagged[Tags.Npc]) {
					if (n is ShopNpc s) {
						n.GetComponent<DialogComponent>().StartAndClose(s.GetFailDialog(), 3);
						break;
					} else if (n is ShopKeeper) {
						n.GetComponent<DialogComponent>().StartAndClose($"shopkeeper_{Rnd.Int(15, 18)}", 3);
						break;
					}
				}

				return false;
			}
			
			Sells = false;

			Audio.PlaySfx("item_purchase");
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
				RenderPrice();
			}
		}

		protected virtual void RenderPrice() {
			if (HasSale) {
				Graphics.Color = Palette.Default[35];

				if (Price == 0) {
					HasSale = false;
					OnSale = false;
					Price = 1;
				}
			}
				
			var r = GetComponent<RoomComponent>().Room;

			foreach (var p in r.Tagged[Tags.Player]) {
				if (p.GetComponent<ConsumablesComponent>().Coins < Price) {
					Graphics.Color *= 0.6f;
					break;
				}					
			}
				
			Graphics.Print(PriceString, Font.Small, Position + new Vector2(PriceX, 14));
			Graphics.Color = ColorUtils.WhiteColor;
		}

		protected virtual int CalculatePrice() {
			return PriceCalculator.Calculate(Item);
		}
		
		public void Recalculate() {
			if (Free) {
				Price = 0;
				return;
			}
			
			if (Item == null) {
				Price = 0;
				Sells = false;
				return;
			}
			
			Sells = true;
			Price = CalculatePrice();

			if (OnSale) {
				Price = (int) Math.Floor(Price * 0.5f);
				HasSale = true;
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
					Price = (int) Math.Floor(Price * Math.Max(0f, (1f - e.Percent * 0.01f)));
					HasSale = true;
				}

				if (r.Tagged[Tags.ShopKeeper].Count > 0) {
					var min = sbyte.MaxValue;
					
					foreach (var s in r.Tagged[Tags.ShopKeeper]) {
						min = Math.Min(min, ((ShopKeeper) s).Mood);
					}

					Price = Math.Max(1, Price - (min - 3));
				}
			}

			CalculatePriceSize();
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemPlacedEvent || (e is RoomChangedEvent rce && rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room)) {
				Recalculate();
			}
			
			return base.HandleEvent(e);
		}

		protected virtual void CalculatePriceSize() {
			PriceString = $"{Price}";
			PriceX = (Width - Font.Small.MeasureString(PriceString).Width) / 2f;
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputInt("Price", ref Price)) {
				CalculatePriceSize();
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			OnSale = stream.ReadBoolean();
			Free = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(OnSale);
			stream.WriteBoolean(Free);
		}
	}
}