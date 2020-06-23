using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.items;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities.chest;
using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.statue {
	public class DiceStatue : Statue {
		// todo: if effect does nothing alter to another effect
		// fixme: hp up/down is not saved
		public static Dictionary<string, Func<Statue, Entity, bool>> Effects =
			new Dictionary<string, Func<Statue, Entity, bool>> {
				// + Hp up

				{
					"buffed", (s, e) => {
						e.GetComponent<HealthComponent>().MaxHealth += 2;
						TextParticle.Add(e, Locale.Get("max_hp"), 2, true);
						return true;
					}
				},

				// - Hp down
				{
					"nerfed", (s, e) => {
						if (e.GetComponent<HeartsComponent>().Total > 2) {
							e.GetComponent<HealthComponent>().ModifyHealth(-2, s, DamageType.Custom);
						} else {
							e.GetComponent<HealthComponent>().MaxHealth -= 2;
						}

						TextParticle.Add(e, Locale.Get("max_hp"), 2, true, true);
						return false;
					}
				},

				// + Heal
				{
					"restored", (s, e) => {
						var c = Rnd.Int(1, 3);
						e.GetComponent<HealthComponent>().ModifyHealth(c, s);
						TextParticle.Add(e, "HP", c, true);
						return true;
					}
				},

				// - Hurt
				{
					"damaged", (s, e) => {
						var c = Rnd.Int(1, 3);
						e.GetComponent<HealthComponent>().ModifyHealth(-c, s);
						TextParticle.Add(e, "HP", c, true, true);
						return false;
					}
				},

				// + Clear Scourge
				{
					"cleansed", (s, e) => {
						Run.RemoveScourge();
						return true;
					}
				},

				// - Add Scourge
				{
					"scourged", (s, e) => {
						Run.AddScourge(true);
						return false;
					}
				},

				// + Give coins
				{
					"gifted", (s, e) => {
						var c = Rnd.Int(10, 21);
						e.GetComponent<ConsumablesComponent>().Coins += c;
						TextParticle.Add(e, Locale.Get("coins"), c, true);
						return true;
					}
				},

				// - Remove coins
				{
					"robbed", (s, e) => {
						var c = e.GetComponent<ConsumablesComponent>();
						var cn = (int) Math.Ceiling(c.Coins * Rnd.Float(0.2f, 0.5f));
						e.GetComponent<ConsumablesComponent>().Coins -= cn;
						TextParticle.Add(e, Locale.Get("coins"), cn, true, true);
						return false;
					}
				},

				// + Give chest
				{
					"lucky", (s, e) => {
						try {
							ChestRegistry.PlaceRandom(s.BottomCenter + new Vector2(0, 12), s.Area);
						} catch (Exception ex) {
							Log.Error(ex);
						}
						return true;
					}
				},

				// - Remove weapon
				{
					"unlucky", (s, e) => {
						var c = e.GetComponent<ActiveWeaponComponent>();
						var item = c.Item;
						TextParticle.Add(e, item.Name, 1, true, true);

						c.Drop();
						item.Done = true;

						if (e.GetComponent<WeaponComponent>().Item == null) {
							c.Set(Items.CreateAndAdd(LevelSave.MeleeOnly ? "bk:ancient_sword" : "bk:ancient_revolver", s.Area));
						} else {
							c.RequestSwap();
						}
						return false;
					}
				}
			};

		protected byte TimesUsed;

		public override void AddComponents() {
			base.AddComponents();

			Sprite = "dice_statue";
			Width = 20;
			Height = 27;
			
			AddComponent(new AudioEmitterComponent());
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 12, 20, 15);
		}

		protected override bool Interact(Entity e) {
			TimesUsed++;

			var keys = Effects.Keys;
			var key = keys.ElementAt(Rnd.Int(keys.Count));
			var bad = false;

			TextParticle.Add(this, Locale.Get(key));

			Timer.Add(() => {
				bad = Effects[key](this, e);

				if (!Broken) {
					GetComponent<AudioEmitterComponent>().Emit(bad ? "item_dice_good" : "item_dice_bad");
				}
			}, 1f);

			if (TimesUsed >= 3) {
				GetComponent<AudioEmitterComponent>().Emit("item_dice_break");
				
				Timer.Add(() => {
					GetComponent<AudioEmitterComponent>().Emit(bad ? "item_dice_good" : "item_dice_bad");
				}, 0.6f);
				
				Break();
			}

			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			TimesUsed = stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte(TimesUsed);
		}
	}
}