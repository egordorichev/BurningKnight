using System;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class HeartsComponent : SaveableComponent {
		public const int Cap = 32;
		public const int PerRow = Cap / 2;
		
		private byte shieldHalfs;

		public int ShieldHalfs => shieldHalfs;
		public int Total => shieldHalfs;

		public void ModifyShields(int amount, Entity setter) {
			var component = GetComponent<HealthComponent>();
			amount = (int) (amount < 0 ? Math.Max(ShieldHalfs, amount) : Math.Min(Cap - component.MaxHealth - Total, amount));


			var e = new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false,
				ShieldsTook = true
			};
			
			if (amount != 0 && !Send(e)) {
				if (amount > 0) {
					Entity.GetComponent<HealthComponent>().EmitParticles(true);
				}
				
				shieldHalfs = (byte) Math.Max(0, shieldHalfs + e.Amount);

				if (shieldHalfs > 0) {
					Achievements.Unlock("bk:shielded");
				}

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					From = setter,
					Who = Entity,
					Default = false,
					ShieldsTook = true
				});
			}
		}
		
		public bool Hurt(int amount, Entity setter) {
			var e = new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false,
				ShieldsTook = true
			};
			
			if (!Send(e)) {
				var iron = (byte) Math.Min(e.Amount, shieldHalfs);
				shieldHalfs -= iron;

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					From = setter,
					Who = Entity,
					Default = false,
					ShieldsTook = true
				});
				
				return true;
			}

			return false;
		}
		
		public bool CanHaveMore => Total + GetComponent<HealthComponent>().MaxHealth < Cap;
				
		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte(shieldHalfs);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			shieldHalfs = stream.ReadByte();
		}
	}
}