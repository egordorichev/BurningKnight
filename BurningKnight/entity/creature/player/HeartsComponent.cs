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
		
		private byte ironHalfs;
		private byte goldenHalfs;

		public int IronHalfs => ironHalfs;
		public int GoldenHalfs => goldenHalfs;
		public int Total => ironHalfs + goldenHalfs;

		public void ModifyIronHearts(int amount, Entity setter) {
			var component = GetComponent<HealthComponent>();
			amount = amount < 0 ? Math.Max(IronHalfs, amount) : Math.Min(Cap - component.Health - Total, amount);

			if (amount != 0 && !Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Default = false
			})) {
				ironHalfs = (byte) Math.Max(0, ironHalfs + amount);
			}
		}
		
		public void ModifyGoldHearts(int amount, Entity setter) {
			var component = GetComponent<HealthComponent>();
			amount = amount < 0 ? Math.Max(GoldenHalfs, amount) : Math.Min(Cap - component.Health - Total, amount);

			if (amount != 0 && !Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Default = false
			})) {
				goldenHalfs = (byte) Math.Max(0, goldenHalfs + amount);
			}
		}
		
		public void Hurt(int amount, Entity setter) {
			if (!Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Default = false
			})) {
				byte golden = (byte) Math.Min(amount, goldenHalfs);
				goldenHalfs -= golden;

				if (amount > golden) {
					byte iron = (byte) Math.Min(amount - golden, ironHalfs);
					ironHalfs -= iron;
					
					// note: maybe pass the remaining amount of hp to remove to health component?
				}
			}
		}
		
		public bool CanHaveMore => Total + GetComponent<HealthComponent>().Health < Cap;
				
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(ironHalfs);
			stream.WriteByte(goldenHalfs);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			ironHalfs = stream.ReadByte();
			goldenHalfs = stream.ReadByte();
		}
	}
}