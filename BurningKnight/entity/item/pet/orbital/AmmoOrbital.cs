using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.pet.impl;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.pet.orbital {
	public class AmmoOrbital : Pet {
		public AmmoOrbital() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("ammo_orbital");
				Description = Locale.Get("ammo_orbital_desc");
				Sprite = "item-ammo_orbital";
			}
		}

		public override PetEntity Create() {
			return new Impl();
		}

		public static class Impl : Orbital {
			private float Last;

			public Impl() {
				_Init();
			}

			protected void _Init() {
				{
					Sprite = "item-ammo_orbital";
				}
			}

			public override void Init() {
				base.Init();
				Last = Random.NewFloat(3);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Last += Dt;

				if (Last > 3f) {
					Last = 0;

					if (Player.Instance.Room != null)
						foreach (Mob Mob in Mob.Every)
							if (Mob.OnScreen && !Mob.Friendly && Mob.Room == Owner.Room && !Mob.GetState().Equals("unactive") && !Mob.GetState().Equals("defeated")) {
								BulletProjectile Ball = new SimpleBullet();
								PlaySfx("gun_machinegun");
								var A = GetAngleTo(Mob.X + Mob.W / 2, Mob.Y + Mob.H / 2);
								Ball.Velocity = new Point((float) Math.Cos(A) / 2f, (float) Math.Sin(A) / 2f).Mul(60f * 5f);
								Ball.X = this.X + W / 2 + Math.Cos(A) * 8;
								Ball.Damage = 4;
								Ball.Y = (float) (this.Y + Math.Sin(A) * 8 + 6);
								Dungeon.Area.Add(Ball);

								break;
							}
				}
			}
		}
	}
}