using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.item.pet.orbital {
	public class BrokenOrbital : Pet {
		public BrokenOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("broken_orbital");
				Description = Locale.Get("broken_orbital_desc");
				Sprite = "item-broken_orbital";
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public static class Impl : Orbital {
			public Impl() {
				_Init();
			}

			protected void _Init() {
				{
					Sprite = "item-broken_orbital";
				}
			}

			protected override void OnHit(Entity Entity) {
				base.OnHit(Entity);

				if (Entity is Projectile && ((Projectile) Entity).Bad && Random.Chance(20)) {
					Done = true;

					for (var I = 0; I < 10; I++) {
						var Fx = new PoofFx();
						Fx.X = this.X + W / 2;
						Fx.Y = this.Y + H / 2;
						Dungeon.Area.Add(Fx);
					}

					PlayerSave.Remove(this);
				}
			}
		}
	}
}