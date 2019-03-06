using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class HeartsComponent : SaveableComponent {
		public static int Cap = 40;
		
		private byte ironHalfs;
		private byte goldenHalfs;

		public int IronHalfs => ironHalfs;
		public int GoldenHalfs => goldenHalfs;
		public int Total => IronHalfs + GoldenHalfs;

		public void Hurt(int amount, Entity setter) {
			if (!Send(new HealthModifiedEvent {
				Amount = amount,
				From = setter
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
		
		public bool CanHaveMore {
			get {
				var count = ironHalfs + goldenHalfs;

				if (Entity.TryGetCompoenent<HealthComponent>(out var health)) {
					count += health.Health;
				}

				return count < Cap;
			}
		}
				
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