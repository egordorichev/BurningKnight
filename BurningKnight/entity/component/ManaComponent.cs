using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.custom;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ManaComponent : SaveableComponent {
		private byte manaMax = 6;
		private byte mana = 6;

		public int ManaMax {
			get => manaMax;
			set {
				manaMax = (byte) MathUtils.Clamp(2, 254, value);
				mana = (byte) MathUtils.Clamp(2, manaMax, mana);
			}
		}

		public int Mana => mana;

		public void ModifyMana(int amount) {
			mana = (byte) MathUtils.Clamp(0, manaMax, mana + amount);
		}

		public void SetMana(int amount) {
			mana = (byte) MathUtils.Clamp(0, manaMax, amount);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			manaMax = stream.ReadByte();
			mana = stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(manaMax);
			stream.WriteByte(mana);
		}
		
		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev && ev.Item.Type == ItemType.Mana) {
				Send(new ItemAddedEvent {
					Item = ev.Item,
					Who = Entity
				});

				Audio.PlaySfx("item_mana");
				
				for (var i = 0; i < 3; i++) {
					Timer.Add(() => {
						var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"star_{Rnd.Int(1, 4)}"))));
						part.Position = Entity.Center;

						if (Entity.TryGetComponent<ZComponent>(out var z)) {
							part.Position -= new Vector2(0, z.Z);
						}
				
						Entity.Area.Add(part);
				
						part.Particle.Velocity = new Vector2(Rnd.Float(8, 16) * (Rnd.Chance() ? -1 : 1), -Rnd.Float(30, 56));
						part.Particle.Angle = 0;
						part.Particle.Alpha = 0.9f;
						part.Depth = Layers.InGameUi;
					}, i * 0.2f);
				}
				
				ev.Item.Use(Entity);

				Engine.Instance.State.Ui.Add(new ConsumableParticle(ev.Item.Animation != null
					? ev.Item.GetComponent<AnimatedItemGraphicsComponent>().Animation.GetFirstCurrent()
					: ev.Item.Region, (Player) Entity));
				
				ev.Item.Done = true;
				return true;
			}
			
			return base.HandleEvent(e);
		}
		
		
		public override void RenderDebug() {
			var v = (int) mana;

			if (ImGui.InputInt("Mana", ref v)) {
				mana = (byte) v;
			}

			v = manaMax;

			if (ImGui.InputInt("Max Mana", ref v)) {
				manaMax = (byte) v;
			}

			if (ImGui.Button("Reset")) {
				ModifyMana(manaMax - mana);
			}
		}

		public bool IsFull() {
			return mana == manaMax;
		}
		
		public bool CanPickup(Item item) {
			return !IsFull();
		}
	}
}