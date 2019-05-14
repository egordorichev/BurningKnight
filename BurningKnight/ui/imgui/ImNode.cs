using System.Collections.Generic;
using BurningKnight.assets;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace BurningKnight.ui.imgui {
	public class ImNode {
		private static Color connectorColor = new Color(0.6f, 0.6f, 0.6f, 1f); 
		private static Color hoveredConnectorColor = new Color(1f, 1f, 1f, 1f);
		private static Color connectionColor = new Color(0.6f, 1f, 0.6f, 0.6f);
		private static Color hoveredConnectionColor = new Color(0.3f, 1f, 0.3f, 1f);

		private const int connectorRadius = 6;
		private const int connectorRadiusSquare = 36;

		private static int lastId;
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

			if (hovered && ImGui.IsMouseClicked(0)) {
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
							active.CurrentActive.ConnectedTo = CurrentActive;
							CurrentActive.ConnectedTo = active.CurrentActive;
						}

						active.CurrentActive.Active = false;
						active.CurrentActive = null;

						if (CurrentActive != null) {
							CurrentActive.Active = false;
							CurrentActive = null;	
						}
					}
				}
			}

			var p2 = ImGui.GetIO().MousePos;
			var draw = CurrentActive == connection;
			
			if (!connection.Input && connection.ConnectedTo != null) {
				var to = connection.ConnectedTo;

				p2 = to.Parent.Position + to.Offset;
				draw = true;
				hovered = hovered || IsConnectorHovered(p2);
			}
			
			var color = hovered ? hoveredConnectionColor : connectionColor;

			if (draw) {
				DrawHermite(ImGui.GetForegroundDrawList(), connector, p2, 12, color);	
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

		public static unsafe void DrawHermite(ImDrawList* drawList, Vector2 p1, Vector2 p2, int steps, Color color) {
			var t1 = new Vector2(+80.0f, 0.0f);
			var t2 = new Vector2(+80.0f, 0.0f);

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