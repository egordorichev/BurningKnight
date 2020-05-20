using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss.rooms;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.entities.decor;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class DM : Boss {
		private const int Hp = 10;

		protected override void AddPhases() {
			base.AddPhases();
			
			for (var i = 1; i < Hp; i++) {
				HealthBar.AddPhase(i / (float) Hp);
			}
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 14;
			Height = 17;
			
			AddComponent(new SensorBodyComponent(1, 1, 12, 16));

			var body = new RectBodyComponent(1, 16, 12, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.5f;
			body.Body.LinearDamping = 3;

			AddComponent(new ZComponent());
			AddComponent(new ZAnimationComponent("dark_mage"));
			
			SetMaxHp(Hp);

			Flying = true;
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target != null) {
				SelectAttack();
			}
		}

		public override void SelectAttack() {
			Become<IdleState>();
			
			Timer.Add(() => {
				ChangeupRoom();
			}, 1f);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme && hme.Amount < 0) {
				hme.Amount = Math.Max(-1, hme.Amount);
				ChangeupRoom();
			}
			
			return base.HandleEvent(e);
		}

		private void ChangeupRoom() {
			foreach (var r in Area.Tagged[Tags.Room]) {
				var room = (Room) r;

				if (room.Type != RoomType.Boss) {
					room.Done = true;
				}
			}

			foreach (var d in Area.Tagged[Tags.Door]) {
				d.Done = true;
			}

			foreach (var e in Area.Entities.Entities) {
				if (e is WallTorch || e is Torch || e is Prop || e is Entrance || (e is Creature && !(e is Player || e is Boss)) || e is Turret || e is SpawnPoint || e is Projectile) {
					e.Done = true;
				}
			}
			
			var rm = GetComponent<RoomComponent>().Room;
			var level = Run.Level;
			var rmdef = (DmRoom) Activator.CreateInstance(DmRoomRegistry.Rooms[Rnd.Int(DmRoomRegistry.Rooms.Length)]);

			rm.MapW = Math.Min(Rnd.Int(rmdef.GetMinWidth(), rmdef.GetMaxWidth()), level.Width - 2);
			rm.MapH = Math.Min(Rnd.Int(rmdef.GetMinHeight(), rmdef.GetMaxHeight()), level.Height - 2);
			rm.MapX = (int) Math.Ceiling((level.Width - rm.MapW) / 2f);
			rm.MapY = (int) Math.Ceiling((level.Height - rm.MapH) / 2f);
			rm.UpdateSize();

			rmdef.Set(rm.MapX, rm.MapY, rm.MapX + rm.MapW - 1, rm.MapY + rm.MapH - 1);
			
			Painter.Fill(level, new Rect(0, 0, level.Width, level.Height), Tile.WallA);
			Painter.Fill(level, rmdef, 1, Tile.FloorA);
			rmdef.PaintFloor(level);
			rmdef.Paint(level, rm);

			level.TileUp();
			level.RecreateBody();

			for (var i = 0; i < level.Size; i++) {
				level.Light[i] = 1f;
			}
			
			rmdef.PlaceMage(rm, this);

			foreach (var p in Area.Tagged[Tags.Player]) {
				var s = new SpawnPoint();
				Area.Add(s);

				rmdef.PlacePlayer(rm, (Player) p);
				s.Center = p.Center;
			}
		}

		private int counter;
		
		#region DM States
		public class IdleState : SmartState<DM> {
			public override void Init() {
				base.Init();
				Self.TouchDamage = 2;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob].Count > 1) {
					Become<FlyingState>();
				}
			}
		}

		public class FlyingState : SmartState<DM> {
			public override void Init() {
				base.Init();
				
				Self.TouchDamage = 0;
				Self.GetComponent<HealthComponent>().Unhittable = true;

				Self.GetComponent<ZAnimationComponent>().Tint.A = 140;
				Self.GetComponent<ZComponent>().Float = true;
			}

			public override void Destroy() {
				base.Destroy();
				
				Self.TouchDamage = 2;
				Self.GetComponent<HealthComponent>().Unhittable = false;
				
				Self.GetComponent<ZAnimationComponent>().Tint.A = 255;
				Self.GetComponent<ZComponent>().Float = false;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob].Count <= 1) {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}