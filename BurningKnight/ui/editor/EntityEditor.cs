using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.fx;
using BurningKnight.level.entities;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Num = System.Numerics;

namespace BurningKnight.ui.editor {
	public static class EntityEditor {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static Num.Vector2 size = new Num.Vector2(200, 400);

		private static bool move;
		private static Type currentType;
		private static Vector2 offset;
		private static Type copy;
		private static Entity entity;	
		private static int selected;
		private static List<TypeInfo> types = new List<TypeInfo>();

		public static bool SnapToGrid = true;
		public static bool Center;
		public static Editor Editor;
		public static Entity CurrentEntity => entity;
		public static Entity HoveredEntity;
		
		static EntityEditor() {
			var blocked = new List<Type> {
				typeof(Slime),
				typeof(Prop),
				typeof(SlicedProp),
				typeof(SolidProp),
				typeof(BreakableProp),
				typeof(PlaceableEntity),
				typeof(Mob),
				typeof(Npc)
			};
			
			var pe = typeof(PlaceableEntity);
			
			foreach (var t in Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(type => pe.IsAssignableFrom(type) && type != pe && !blocked.Contains(type))) {

				types.Add(new TypeInfo {
					Type = t,
					Name = t.Name
				});
			}
			
			types.Sort((a, b) => a.GetType().FullName.CompareTo(b.GetType().FullName));
		}

		private static bool open;
		
		public static void Render() {
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);
			
			if (!ImGui.Begin("Entity placer")) {
				ImGui.End();
				open = false;
				return;
			}

			open = true;
			
			var down = !ImGui.GetIO().WantCaptureMouse && Input.Mouse.CheckLeftButton;
			var clicked = !ImGui.GetIO().WantCaptureMouse && MouseData.HadClick;
			
			if (Input.Keyboard.IsDown(Keys.LeftControl, true)) {
				if (entity != null) {
					if (Input.Keyboard.WasPressed(Keys.C, true)) {
						copy = entity.GetType();
					}
					
					if (copy != null && Input.Keyboard.WasPressed(Keys.V, true)) {
						var od = currentType;
						currentType = copy;
						CreateEntity(false);
						currentType = od;
					}
					
					if (Input.Keyboard.WasPressed(Keys.D, true)) {
						entity.Done = true;
						entity = null;
					}
				}
			}
			
			ImGui.Checkbox("Move", ref move);/*) {
				if (entityMode != 0) {
					RemoveEntity();
				}
			}*/

			if (!move && entity != null) {
				var mouse = Input.Mouse.GamePosition;

				if (SnapToGrid) {
					mouse.X = (float) Math.Floor(mouse.X / 16) * 16;
					mouse.Y = (float) Math.Floor(mouse.Y / 16) * 16;
				}
				
				mouse += new Vector2(8 - entity.Width / 2f, 8 - entity.Height / 2f);
				
				if (Center) {
					entity.Center = mouse;
				} else {
					entity.Position = mouse;
				}

				if (clicked) {
					CreateEntity(false);
				}
			} else if (move) {
				var mouse = Input.Mouse.GamePosition;
				Entity selected = null;
					
				foreach (var e in Editor.Area.Entities.Entities) {
					if (e.OnScreen && AreaDebug.PassFilter(e) && !(e is Firefly || e is WindFx)) {
						if (e.Contains(mouse)) {
							selected = e;
						}
					}
				}

				HoveredEntity = selected;
				
				if (clicked) {
					entity = selected;

					if (selected != null) {
						AreaDebug.ToFocus = entity;
						offset = entity.Position - mouse;
					}
				} else if (entity != null && (down && entity.Contains(mouse) || Input.Keyboard.IsDown(Keys.LeftAlt, true))) {
					mouse += offset;
					
					if (SnapToGrid) {
						mouse.X = (float) Math.Round(mouse.X / 16) * 16;
						mouse.Y = (float) Math.Round(mouse.Y / 16) * 16;
					}
					
					mouse += new Vector2(8 - entity.Width / 2f, 8 - entity.Height / 2f);
				
					if (Center) {
						entity.Center = mouse;
					} else {
						entity.Position = mouse;
					}
				}
			}
			
			ImGui.Checkbox("Snap to grid", ref SnapToGrid);
			ImGui.SameLine();
			ImGui.Checkbox("Center", ref Center);
			
			if (entity != null) {
				ImGui.Separator();
				ImGui.Text(entity.GetType().Name);

				if (ImGui.Button("Open debug")) {
					AreaDebug.ToFocus = entity;
				}
				
				ImGui.Separator();
			}

			filter.Draw("");
			var i = 0;
			
			ImGui.Separator();
			var h = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionConsole", new System.Numerics.Vector2(0, -h), 
				false, ImGuiWindowFlags.HorizontalScrollbar);
		
			foreach (var t in types) {
				if (filter.PassFilter(t.Name)) {
					if (ImGui.Selectable(t.Name, selected == i)) {
						selected = i;

						currentType = t.Type;
						CreateEntity();
					}
				}

				i++;
			}

			ImGui.EndChild();
			ImGui.End();
		}

		private static void CreateEntity(bool remove = true) {
			if (remove) {
				RemoveEntity();
			}

			try {
				entity = (Entity) Activator.CreateInstance(currentType);
				Editor.Area.Add(entity);
				entity.Position = Input.Mouse.GamePosition;
				// somethig wrong here
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public static void RemoveEntity() {
			if (entity != null) {
				entity.Done = true;
				entity = null;
			}
		}

		public static void RenderInGame() {
			if (!open) {
				return;
			}
			
			if (HoveredEntity != null) {
				Graphics.Batch.DrawRectangle(HoveredEntity.Position - new Vector2(1), new Vector2(HoveredEntity.Width + 2, HoveredEntity.Height + 2), new Color(0.7f, 0.7f, 1f, 1f));
			}
				
			if (CurrentEntity != null) {
				var e = CurrentEntity;
				Graphics.Batch.DrawRectangle(e.Position - new Vector2(1), new Vector2(e.Width + 2, e.Height + 2), new Color(0.7f, 1f, 0.7f, 1f));
			}
		}
	}
}