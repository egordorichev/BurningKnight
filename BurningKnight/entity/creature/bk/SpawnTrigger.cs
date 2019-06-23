using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.state;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.timer;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class SpawnTrigger : SaveableEntity {
		public byte[] Tiles;
		public byte[] Liquid;
		public ushort RoomX;
		public ushort RoomY;
		public byte RoomWidth;
		public byte RoomHeight;

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		private bool triggered;
		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			if (triggered) {
				Camera.Instance.Shake(32);
				t += dt;

				if (t >= 2f) {
					Done = true;

					var r = Math.Sqrt(RoomWidth * RoomWidth + RoomHeight * RoomHeight);

					for (var j = 1; j < r; j++) {
						var level = Run.Level;
						var j1 = j;

						Timer.Add(() => {
							for (var y = 0; y < RoomHeight; y++) {
								for (var x = 0; x < RoomWidth; x++) {
									var dx = x - RoomWidth / 2f;
									var dy = y - RoomHeight / 2f;

									if (Math.Sqrt(dx * dx + dy * dy) > j1) {
										continue;	
									}
									
									var i = x + y * RoomWidth;
									var li = level.ToIndex(RoomX + x, RoomY + y);
						
									level.Tiles[li] = Tiles[i];
									level.Liquid[li] = Liquid[i];
								}
							}
				
							level.TileUp();
							level.CreateBody();
						}, j * 0.2f);
					}
					
					/*var bk = new BurningKnight();
					Area.Add(bk);
					bk.Center = Center;*/
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) Width);
			stream.WriteByte((byte) Height);
			
			stream.WriteUInt16(RoomX);
			stream.WriteUInt16(RoomY);
			
			stream.WriteByte(RoomWidth);
			stream.WriteByte(RoomHeight);

			for (var i = 0; i < RoomWidth * RoomHeight; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadByte();
			Height = stream.ReadByte();

			RoomX = stream.ReadUInt16();
			RoomY = stream.ReadUInt16();

			RoomWidth = stream.ReadByte();
			RoomHeight = stream.ReadByte();

			var s = RoomWidth * RoomHeight;

			Tiles = new byte[s];
			Liquid = new byte[s];
			
			for (var i = 0; i < s; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
			}
		}

		public override bool HandleEvent(Event e) {
			if (triggered) {
				return base.HandleEvent(e);
			}
			
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player) {
					triggered = true;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}