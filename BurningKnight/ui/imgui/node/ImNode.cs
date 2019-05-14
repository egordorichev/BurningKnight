using System.Collections.Generic;
using BurningKnight.assets;
using ImGuiNET;
using Lens.lightJson;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace BurningKnight.ui.imgui.node {
	public class ImNode {
		private static Color connectorColor = new Color(0.6f, 0.6f, 0.6f, 1f); 
		private static Color hoveredConnectorColor = new Color(1f, 1f, 1f, 1f);
		private static Color connectionColor = new Color(0.6f, 1f, 0.6f, 0.6f);
		private static Color hoveredConnectionColor = new Color(0.3f, 1f, 0.3f, 1f);
		private static int lastId;

		private const int connectorRadius = 6;
		private const int connectorRadiusSquare = 36;

		public int Id;
		public string Name;
		public Vector2 Position;
		public Vector2 Size;
		public List<ImConnection> Inputs = new List<ImConnection>();
		public List<ImConnection> Outputs = new List<ImConnection>();
		public ImConnection CurrentActive;
		
		public ImNode(string name) {
			Name = name;
			Id = lastId++;
		}

		public ImNode() {
			Id = lastId++;
		}

		private static bool IsConnectorHovered(Vector2 connector) {
			var mouse = ImGui.GetIO().MousePos;

			var dx = mouse.X - connector.X;
			var dy = mouse.Y - connector.Y;

			return (dx * dx + dy * dy) < connectorRadiusSquare;
		}

		public void AddInput() {
			Inputs.Add(new ImConnection {
				Input = true,
				Parent = this
			});
		}

		public void AddOutput() {
			Outputs.Add(new ImConnection {
				Parent = this
			});
		}
		
		private unsafe void RenderConnector(ImDrawListPtr list, ImConnection connection, Vector2 connector) {
			var hovered = IsConnectorHovered(connector);
			list.AddCircleFilled(connector, connectorRadius, (hovered ? hoveredConnectorColor : connectorColor).PackedValue);

			if (hovered) {
				if (ImGui.IsMouseClicked(0)) {
					if (CurrentActive != null && CurrentActive == connection) {
						CurrentActive.Active = false;
						CurrentActive = null;
					} else {
						if (CurrentActive != null) {
							CurrentActive.Active = false;
						}

						CurrentActive = connection;
						CurrentActive.Active = true;

						if (ImGuiHelper.CurrentActive == null) {
							ImGuiHelper.CurrentActive = this;
						} else {
							var active = ImGuiHelper.CurrentActive;
							ImGuiHelper.CurrentActive = null;

							if (active.CurrentActive != CurrentActive && active.CurrentActive.Input != CurrentActive.Input) {
								active.CurrentActive.ConnectedTo.Add(CurrentActive);
								CurrentActive.ConnectedTo.Add(active.CurrentActive);
							}

							active.CurrentActive.Active = false;
							active.CurrentActive = null;

							if (CurrentActive != null) {
								CurrentActive.Active = false;
								CurrentActive = null;
							}
						}
					}
				} else if (ImGui.IsMouseClicked(1) && connection.ConnectedTo.Count > 0) {
					if (ImGuiHelper.CurrentActive == this) {
						ImGuiHelper.CurrentActive = null;
					} else {
						foreach (var to in connection.ConnectedTo) {
							if (ImGuiHelper.CurrentActive == to.Parent) {
								ImGuiHelper.CurrentActive = null;
								break;
							}
						}
					}

					foreach (var to in connection.ConnectedTo) {
						to.Active = false;
						to.ConnectedTo.Clear();
						to.Parent.CurrentActive = null;
					}
					
					connection.ConnectedTo.Clear();
					connection.Active = false;

					CurrentActive = null;
				}
			}

			if (!connection.Input && connection.ConnectedTo.Count > 0) {
				foreach (var to in connection.ConnectedTo) {
					var p = to.Parent.Position + to.Offset;
					DrawHermite(ImGui.GetForegroundDrawList(), connector, p, 12, hovered || IsConnectorHovered(p) ? hoveredConnectionColor : connectionColor);	
				}
			}

			if (CurrentActive == connection) {
				DrawHermite(ImGui.GetForegroundDrawList(), connector, ImGui.GetIO().MousePos, 12, hovered ? hoveredConnectionColor : connectionColor);	
			}
		}
		
		public virtual void Render() {
			ImGui.Begin(Name, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar);
			
			Position = ImGui.GetWindowPos();
			Size = ImGui.GetWindowSize();
			
			var rightSide = Position + new Vector2(Size.X, 0);
			var list = ImGui.GetForegroundDrawList();

			foreach (var input in Inputs) {
				RenderConnector(list, input, Position + input.Offset);
			}
			
			foreach (var output in Outputs) {
				RenderConnector(list, output, rightSide + output.Offset);
			}
			
			RenderElements();
			ImGui.End();
		}

		public virtual void RenderElements() {
			
		}
		
		public virtual void Save(JsonObject root) {
			root["name"] = Name;
			root["id"] = Id;
			root["inputs"] = Inputs.Count;
			root["outputs"] = Outputs.Count;
		}

		public virtual void Load(JsonObject root) {
			Name = root["name"];
			Id = root["id"].AsInteger;

			var inputs = root["inputs"].AsInteger;
			var outputs = root["outputs"].AsInteger;

			while (inputs > Inputs.Count) {
				AddInput();
			}
			
			while (outputs > Outputs.Count) {
				AddOutput();
			}
		}

		public static unsafe void DrawHermite(ImDrawList* drawList, Vector2 p1, Vector2 p2, int steps, Color color) {
			var t1 = new Vector2(80.0f, 0.0f);
			var t2 = new Vector2(80.0f, 0.0f);

			for (var step = 0; step <= steps; step++) {
				var t = (float) step / steps;
				var h1 = +2 * t * t * t - 3 * t * t + 1.0f;
				var h2 = -2 * t * t * t + 3 * t * t;
				var h3 = t * t * t - 2 * t * t + t;
				var h4 = t * t * t - t * t;

				ImGuiNative.ImDrawList_PathLineTo(drawList, new Vector2(h1 * p1.X + h2 * p2.X + h3 * t1.X + h4 * t2.X, h1 * p1.Y + h2 * p2.Y + h3 * t1.Y + h4 * t2.Y));
			}

			ImGuiNative.ImDrawList_PathStroke(drawList, color.PackedValue, 0, 3.0f);
		}
	}
}