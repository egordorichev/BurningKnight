using BurningKnight.core.assets;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class BrokenOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("broken_orbital");
				Description = Locale.Get("broken_orbital_desc");
				Sprite = "item-broken_orbital";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-broken_orbital";
				}
			}

			protected override Void OnHit(Entity Entity) {
				base.OnHit(Entity);

				if ((Entity is Projectile && ((Projectile) Entity).Bad && Random.Chance(20))) {
					this.Done = true;

					for (int I = 0; I < 10; I++) {
						PoofFx Fx = new PoofFx();
						Fx.X = this.X + this.W / 2;
						Fx.Y = this.Y + this.H / 2;
						Dungeon.Area.Add(Fx);
					}

					PlayerSave.Remove(this);
				} 
			}

			public Impl() {
				_Init();
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public BrokenOrbital() {
			_Init();
		}
	}
}
