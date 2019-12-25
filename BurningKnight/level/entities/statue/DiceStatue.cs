using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class DiceStatue : Statue {
		public static Dictionary<string, Action<DiceStatue, Entity>> Effects = new Dictionary<string, Action<DiceStatue, Entity>> {
			// + Hp up
			{ "buffed", (s, e) => e.GetComponent<HealthComponent>().MaxHealth += 2 },
			// - Hp down
			{ "nerfed", (s, e) => e.GetComponent<HealthComponent>().MaxHealth -= 2 },
			// + Heal
			{ "restored", (s, e) => e.GetComponent<HealthComponent>().ModifyHealth(Rnd.Int(1, 3), s) },
			// - Hurt
			{ "damaged", (s, e) => e.GetComponent<HealthComponent>().ModifyHealth(-Rnd.Int(1, 3), s) },
			// + Clear Scourge
			{ "cleansed", (s, e) => Run.RemoveScourge() },
			// - Add Scourge
			{ "scourged", (s, e) => Run.AddScourge() },
			// + Give coins
			{ "gifted", (s, e) => e.GetComponent<ConsumablesComponent>().Coins += Rnd.Int(10, 21) },
			// - Remove coins
			{ "robbed", (s, e) => {
				var c = e.GetComponent<ConsumablesComponent>();
				c.Coins -= (int) Math.Ceiling(c.Coins * Rnd.Float(0.2f, 0.5f));
			} },

			// + Give chest
			{ "lucky", (s, e) => {
				try {
					var chest = (Chest) Activator.CreateInstance(ChestRegistry.Instance.Generate());
					s.Area.Add(chest);
					chest.TopCenter = s.BottomCenter + new Vector2(0, 4);
				} catch (Exception ex) {
					Log.Error(ex);
				}
			} },
			
			// - Remove weapon
			{ "unlucky", (s, e) => {
				var c = e.GetComponent<ActiveWeaponComponent>();
				var item = c.Item;

				c.Drop();
				item.Done = true;

				if (e.GetComponent<WeaponComponent>().Item == null) {
					c.Set(Items.CreateAndAdd("bk:ancient_revolver", s.Area));				
				} else {
					c.RequestSwap();
				}
			} }
		};
		
		protected byte TimesUsed;
		
		public override void AddComponents() {
			base.AddComponents();

			Sprite = "dice_statue";
			Width = 20;
			Height = 27;
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 12, 20, 15);
		}

		protected override bool Interact(Entity e) {
			TimesUsed++;
			
			var keys = Effects.Keys;
			var key = keys.ElementAt(Rnd.Int(keys.Count));
			
			Effects[key](this, e);
			Log.Info(key);

			if (TimesUsed >= 3) {
				Break();
			}

			return true;
		}
	}
}