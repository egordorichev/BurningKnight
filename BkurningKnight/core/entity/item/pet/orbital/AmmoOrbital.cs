using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.pet.impl;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.pet.orbital {
	public class AmmoOrbital : Pet {
		protected void _Init() {
			{
				Name = Locale.Get("ammo_orbital");
				Description = Locale.Get("ammo_orbital_desc");
				Sprite = "item-ammo_orbital";
			}
		}

		public static class Impl : Orbital {
			protected void _Init() {
				{
					Sprite = "item-ammo_orbital";
				}
			}

			private float Last;

			public override Void Init() {
				base.Init();
				Last = Random.NewFloat(3);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.Last += Dt;

				if (this.Last > 3f) {
					this.Last = 0;

					if (Player.Instance.Room != null) {
						foreach (Mob Mob in Mob.Every) {
							if (Mob.OnScreen && !Mob.Friendly && Mob.Room == this.Owner.Room && !Mob.GetState().Equals("unactive") && !Mob.GetState().Equals("defeated")) {
								BulletProjectile Ball = new SimpleBullet();
								PlaySfx("gun_machinegun");
								float A = this.GetAngleTo(Mob.X + Mob.W / 2, Mob.Y + Mob.H / 2);
								Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(60f * 5f);
								Ball.X = (float) (this.X + this.W / 2 + Math.Cos(A) * 8);
								Ball.Damage = 4;
								Ball.Y = (float) (this.Y + Math.Sin(A) * 8 + 6);
								Dungeon.Area.Add(Ball);

								break;
							} 
						}
					} 
				} 
			}

			public Impl() {
				_Init();
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public AmmoOrbital() {
			_Init();
		}
	}
}
