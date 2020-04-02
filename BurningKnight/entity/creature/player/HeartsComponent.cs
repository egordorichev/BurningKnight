using System;
using BurningKnight.assets.achievements;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using ImGuiNET;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class HeartsComponent : SaveableComponent {
		public const int Cap = 32;
		public const int PerRow = Cap / 2;
		
		private byte shieldHalfs;
		private byte bombs;
		private byte bombsMax;

		public int ShieldHalfs => shieldHalfs;
		public int Bombs => bombs;
		public int BombsMax => bombsMax;
		public int Total => ((int) bombs) * 2 + shieldHalfs;

		public void ModifyShields(int amount, Entity setter) {
			var component = GetComponent<HealthComponent>();
			amount = (int) (amount < 0 ? Math.Max(ShieldHalfs, amount) : Math.Min(Cap - component.MaxHealth - Total, amount));

			var e = new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false,
				HealthType = HealthType.Shield
			};
			
			if (amount != 0 && !Send(e)) {
				if (amount > 0) {
					Entity.GetComponent<HealthComponent>().EmitParticles(HealthType.Shield);
				}
				
				shieldHalfs = (byte) Math.Max(0, (float) shieldHalfs + e.Amount);

				if (shieldHalfs > 0) {
					Achievements.Unlock("bk:shielded");
				}

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					From = setter,
					Who = Entity,
					Default = false,
					HealthType = HealthType.Shield
				});
			}
		}
		
		public void ModifyBombs(int amount, Entity setter) {
			var component = GetComponent<HealthComponent>();
			amount = (int) (amount < 0 ? Math.Max(Bombs, amount) : Math.Min(Cap - component.MaxHealth - Total, amount));

			var e = new HealthModifiedEvent {
				Amount = amount,
				From = setter,
				Who = Entity,
				Default = false,
				HealthType = HealthType.Bomb
			};
			
			if (amount != 0 && !Send(e)) {
				if (amount > 0) {
					Entity.GetComponent<HealthComponent>().EmitParticles(HealthType.Bomb);
				}
				
				bombs = (byte) Math.Max(0, (float) bombs + e.Amount);

				Send(new PostHealthModifiedEvent {
					Amount = e.Amount,
					From = setter,
					Who = Entity,
					Default = false,
					HealthType = HealthType.Bomb
				});
			}
		}
		
		public bool Hurt(int amount, Entity setter, DamageType type = DamageType.Regular) {
			if (amount > 0) {
				amount *= -1;
			}
			
			var e = new HealthModifiedEvent {
				Amount = amount,
				Type = type,
				From = setter,
				Who = Entity,
				Default = false,
				HealthType = shieldHalfs > 0 ? HealthType.Shield : HealthType.Bomb
			};
			
			if (!Send(e)) {
				if (shieldHalfs > 0) {
					var iron = Math.Min(e.Amount, shieldHalfs);
					shieldHalfs = (byte) Math.Max(0, shieldHalfs + iron);

					Send(new PostHealthModifiedEvent {
						Amount = e.Amount,
						From = setter,
						Type = type,
						Who = Entity,
						Default = false,
						HealthType = HealthType.Shield
					});
					
					return true;
				} else if (bombs > 0) {
					var iron = Math.Min(e.Amount, bombs);
					bombs = (byte) Math.Max(0, bombs + iron);

					Send(new PostHealthModifiedEvent {
						Amount = e.Amount,
						From = setter,
						Type = type,
						Who = Entity,
						Default = false,
						HealthType = HealthType.Bomb
					});
					
					return true;
				}
			}

			return false;
		}
		
		public bool CanHaveMore => Total + GetComponent<HealthComponent>().MaxHealth < Cap;
				
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(shieldHalfs);
			stream.WriteByte(bombs);
			stream.WriteByte(bombsMax);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			shieldHalfs = stream.ReadByte();
			bombs = stream.ReadByte();
			bombsMax = stream.ReadByte();
		}

		public override void RenderDebug() {
			base.RenderDebug();
			
			ImGui.Text($"Iron halfs: {shieldHalfs}");
			var v = (int) bombsMax;

			if (ImGui.InputInt("Bombs Max", ref v)) {
				bombsMax = (byte) v;
			}
			
			v = (int) bombs;

			if (ImGui.InputInt("Bombs", ref v)) {
				bombs = (byte) v;
			}
		}
	}
}