using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.items;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities.chest;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class DiceStatue : Statue {
		// todo: if effect does nothing alter to another effect
		// fixme: hp up/down is not saved
		public static Dictionary<string, Action<DiceStatue, Entity>> Effects = new Dictionary<string, Action<DiceStatue, Entity>> {
			// + Hp up
			
			{ "buffed", (s, e) => {
					e.GetComponent<HealthComponent>().MaxHealth += 2;
					TextParticle.Add(e, Locale.Get("max_hp"), 2, true);
				}
			},
			// - Hp down
			{ "nerfed", (s, e) => {
					e.GetComponent<HealthComponent>().MaxHealth -= 2;
					TextParticle.Add(e, Locale.Get("max_hp"), 2, true, true);
				}
			},
			// + Heal
			{ "restored", (s, e) => {
					var c = Rnd.Int(1, 3);
					e.GetComponent<HealthComponent>().ModifyHealth(c, s);
					TextParticle.Add(e, "HP", c, true);
				}
			},
			// - Hurt
			{ "damaged", (s, e) => {
					var c = Rnd.Int(1, 3);
					e.GetComponent<HealthComponent>().ModifyHealth(-c, s);
					TextParticle.Add(e, "HP", c, true, true);
				}
			},
			// + Clear Scourge
			{ "cleansed", (s, e) => {
					Run.RemoveScourge();
				}
			},
			// - Add Scourge
			{ "scourged", (s, e) => {
					Run.AddScourge();
				}
			},
			// + Give coins
			{ "gifted", (s, e) => {
					var c = Rnd.Int(10, 21);
					e.GetComponent<ConsumablesComponent>().Coins += c;
					TextParticle.Add(e, Locale.Get("coins"), c, true);
				}
			},
			// - Remove coins
			{ "robbed", (s, e) => {
				var c = e.GetComponent<ConsumablesComponent>();
				var cn = (int) Math.Ceiling(c.Coins * Rnd.Float(0.2f, 0.5f));
				e.GetComponent<ConsumablesComponent>().Coins -= cn;
				TextParticle.Add(e, Locale.Get("coins"), cn, true, true);
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
				TextParticle.Add(e, item.Name, 1, true, true);

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
			
			TextParticle.Add(this, Locale.Get(key));

			Timer.Add(() => {
				Effects[key](this, e);
			}, 1f);

			if (TimesUsed >= 3) {
				Break();
			}

			return true;
		}
	}
}