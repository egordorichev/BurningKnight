using System.Collections.Generic;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.events;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level.entities.plant;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.level.entities.decor {
	public class Tree : Prop {
		private byte type;
		private List<Entity> colliding = new List<Entity>();
		public bool High;

		public byte Id {
			get => type;
			set {
				type = value;
			}
		}

		public Tree() {
			type = (byte) Rnd.Int(0, 5);
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ShadowComponent());
		}

		public override void PostInit() {
			base.PostInit();

			if (High) {
				Depth = Layers.Wall + 1;
			}
			
			var s = new PlantGraphicsComponent("props", $"tree_{type}");
			s.RotationModifier = 0.03f;
			s.Flipped = type != 5 && Rnd.Chance();
			
			AddComponent(s);

			Width = s.Sprite.Width;
			Height = s.Sprite.Height;

			if (Width > 14) {
				AddComponent(new SensorBodyComponent(4, 4, Width - 4, Height - 4));
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			type = stream.ReadByte();
			High = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte(type);
			stream.WriteBoolean(High);
		}

		private bool ShouldCollide(Entity e) {
			if (e is BeeHive) {
				return false;
			}
			
			return e is Creature || e is Spikes;
		}

		public override void Update(float dt) {
			base.Update(dt);
			var s = GetComponent<PlantGraphicsComponent>();

			s.Alpha += ((colliding.Count > 0 ? 0.2f : 1) - s.Alpha) * dt * 5;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (ShouldCollide(cse.Entity)) {
					colliding.Add(cse.Entity);
				}
			} else if (e is CollisionEndedEvent cee) {
				colliding.Remove(cee.Entity);
			}

			return base.HandleEvent(e);
		}
	}
}