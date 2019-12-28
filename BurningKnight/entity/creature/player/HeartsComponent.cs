using System;
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

			if (amount != 0 && !Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false
			})) {
				shieldHalfs = (byte) Math.Max(0, shieldHalfs + amount);
			}
		}
		
		public bool Hurt(int amount, Entity setter) {
			if (!Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false
			})) {
				var iron = (byte) Math.Min(amount, shieldHalfs);
				shieldHalfs -= iron;

				return true;
			}

			return false;
		}
		
		public bool CanHaveMore => Total + GetComponent<HealthComponent>().Health < Cap;
				
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