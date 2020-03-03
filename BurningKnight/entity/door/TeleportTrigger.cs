using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class TeleportTrigger : SaveableEntity, PlaceableEntity {
		private sbyte depth;
		private string id;
		private string toId;
		private bool ignoreCollision;
		private bool toTop;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.TeleportTrigger);
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Static));
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionEndedEvent cee) {
				if (cee.Entity is Player) {
					ignoreCollision = false;
				}
			} else if (e is CollisionStartedEvent cse && cse.Entity is Player p) {
				if (ignoreCollision) {
					return base.HandleEvent(e);
				}
				
				if (depth != 0) {
					Run.Depth = depth;
				} else {
					foreach (var t in Area.Tagged[Tags.TeleportTrigger]) {
						var tr = (TeleportTrigger) t;
						
						if (tr.id == toId) {
							if (tr.toTop) {
								p.TopCenter = tr.TopCenter - new Vector2(0, 1);
							} else {
								tr.ignoreCollision = true;
								p.BottomCenter = tr.BottomCenter + new Vector2(0, 1);
							}
							
							return base.HandleEvent(e);
						}
					}

					Log.Error($"Failed to teleport to {toId}");
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void RenderImDebug() {
			var v = (int) depth;

			if (id == null) {
				id = "";
			}
			
			ImGui.InputText("Id", ref id, 128);
			
			if (ImGui.InputInt("To depth", ref v)) {
				depth = (sbyte) v;
			}
			
			if (v == 0) {
				if (toId == null) {
					toId = "";
				}
				
				ImGui.InputText("To Id", ref toId, 128);
			}

			ImGui.Separator();

			var x = (int) X;
			var y = (int) Y;

			if (ImGui.InputInt("X", ref x)) {
				X = x;
			}

			if (ImGui.InputInt("Y", ref y)) {
				Y = y;
			}
			
			ImGui.InputFloat("Width", ref Width);
			ImGui.InputFloat("Height", ref Height);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			Width = stream.ReadFloat();
			Height = stream.ReadFloat();
			depth = stream.ReadSbyte();
			id = stream.ReadString();

			if (depth == 0) {
				toId = stream.ReadString();
			}

			toTop = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteFloat(Width);
			stream.WriteFloat(Height);
			stream.WriteSbyte(depth);
			stream.WriteString(id);

			if (depth == 0) {
				stream.WriteString(toId);
			}

			stream.WriteBoolean(toTop);
		}

		public override void Render() {
			if (Engine.EditingLevel) {
				Graphics.Batch.FillRectangle(X, Y, Width, Height, ColorUtils.WhiteColor);
			}
		}
	}
}