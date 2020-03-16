using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Teleporter : SaveableEntity {
		public string Id = "a";
		private bool ignoreCollision;

		public override void AddComponents() {
			base.AddComponents();

			Depth = Layers.Entrance;
			
			AddTag(Tags.Teleport);
			AddComponent(new SensorBodyComponent(2, 2, 12, 12, BodyType.Static));
		}

		public override void PostInit() {
			base.PostInit();
			AddComponent(new SliceComponent("props", $"teleport_{Id}"));
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Id = stream.ReadString();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Player) {
					ignoreCollision = false;
				}
			} else if (e is CollisionStartedEvent cse && cse.Entity is Creature c) {
				if (ignoreCollision) {
					return base.HandleEvent(e);
				}
				
				foreach (var t in Area.Tagged[Tags.Teleport]) {
					var tr = (Teleporter) t;
					
					if (tr != this && tr.Id == Id) {
						AnimationUtil.TeleportAway(c, () => {
							tr.ignoreCollision = true;
							c.BottomCenter = tr.Center;
							Camera.Instance.Jump();
							AnimationUtil.TeleportIn(c);
						});
						
						return base.HandleEvent(e);
					}
				}

				Log.Error($"Failed to teleport to {Id}");
			}
			
			return base.HandleEvent(e);
		}
	}
}