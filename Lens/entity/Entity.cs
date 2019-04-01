using System;
using System.Collections.Generic;
using Lens.entity.component;
using Lens.entity.component.graphics;
using Microsoft.Xna.Framework;

namespace Lens.entity {
	public delegate void PositionChanged();
	
	public class Entity : Subscriber {
		public Area Area;
		
		public bool Active = true;
		public bool Visible = true;
		public bool AlwaysActive;
		public bool AlwaysVisible;
		public bool OnScreen;
		public bool Done;
		
		public int Depth;
		
		#region Bounds and position

		private Vector2 position;
		public Vector2 Position {
			get => position;
			set {
				if (value != position) {
					position = value;
					PositionChanged?.Invoke();
				}
			}
		}

		public Vector2 PositionWithoutTrigger {
			set => position = value;
		}
		
		public event PositionChanged PositionChanged;
		public float Width = 16;
		public float Height = 16;
		public bool Centered;
		
		public float X {
			get => Centered ? Position.X - Width / 2 : Position.X;
			set {
				position.X = Centered ? value + Width / 2 : value;
				PositionChanged?.Invoke();
			}
		}

		public float Y {
			get => Centered ? Position.Y - Height / 2 : Position.Y;
			set {
				position.Y = Centered ? value + Height / 2 : value;
				PositionChanged?.Invoke();
			}
		}

		public Vector2 Center {
			get => new Vector2(CenterX, CenterY);
			set {
				X = value.X - Width / 2;
				Y = value.Y - Height / 2;
				PositionChanged?.Invoke();
			}
		}
		
		public float CenterX {
			get => X + Width / 2;
			set {
				X = value - Width / 2;
				PositionChanged?.Invoke();
			}
		}

		public float CenterY {
			get => Y + Height / 2;
			set {
				Y = value - Height / 2;
				PositionChanged?.Invoke();
			}
		}

		public float Right => X + Width;
		public float Bottom => Y + Height;

		#endregion
		
		#region Entity tags

		private int tag;
		
		public int Tag {
			get => tag;

			set {
				if (tag != value && Area != null) {
					for (int i = 0; i < BitTag.Total; i++) {
						int check = 1 << i;
						bool add = (value & check) != 0;
						bool has = (Tag & check) != 0;

						if (has != add) {
							if (add) {
								Area.Tags[i].Add(this);
							} else {
								Area.Tags[i].Remove(this);
							}
						}
					}	

					tag = value;
				}	
			}
		}

		public bool HasTag(int tag) {
			return (this.tag & 1 << tag) != 0;
		}

		public void AddTag(int tag) {
			Tag |= 1 << tag;
		}

		public void RemoveTag(int tag) {
			Tag &= ~(1 << tag);
		}
		
		#endregion
		
		#region Entity logic

		public GraphicsComponent GraphicsComponent;
		protected Dictionary<Type, Component> components;
		
		public virtual void Init() {
			if (components == null) {
				AddComponents();
			}
		}

		public virtual void AddComponents() {
			components = new Dictionary<Type, Component>();
		}

		public virtual void PostInit() {
			
		}
		
		public virtual void Destroy() {
			foreach (var component in components.Values) {
				component.Destroy();
			}
		}
		
		public virtual void Update(float dt) {
			foreach (var component in components.Values) {
				component.Update(dt);
			}
		}
		
		public virtual bool HandleEvent(Event e) {
			foreach (var component in components.Values) {
				if (component.HandleEvent(e)) {
					return true;
				}
			}
			
			return Area.EventListener.Handle(e);
		}

		public virtual void Render() {
			GraphicsComponent?.Render();
		}

		public virtual void RenderDebug() {
			
		}

		public void RemoveSelf() {
			if (Area != null) {
				Area.Remove(this);
				Area = null;
			}
		}

		public void AddComponent(Component component) {
			if (component is GraphicsComponent g) {
				GraphicsComponent = g;
			}
			
			components[component.GetType()] = component;

			component.Entity = this;
			component.Init();
		}
		
		public void RemoveComponent<T>() {
			var type = typeof(T);
			
			if (components.TryGetValue(type, out var component)) {
				component.Destroy();
				components.Remove(type);
			}
		}

		public void Subscribe<T>(Area area = null) where T : Event {
			(area ?? Area).EventListener.Subscribe<T>(this);
		}

		public void Unsubscribe<T>(Area area = null) where T : Event {
			(area ?? Area).EventListener.Unsubscribe<T>(this);
		}
				
		public T GetComponent<T>() where T : Component {
			return (T) components[typeof(T)];
		}
		
		public T GetAnyComponent<T>() where T : Component {
			var type = typeof(T);

			foreach (var component in components.Values) {
				var t = component.GetType();
				
				if (t == type || t.IsSubclassOf(type)) {
					return component as T;
				}
			}

			return null;
		}

		public bool HasComponent(Type type) {
			return components.ContainsKey(type);
		}

		public bool HasComponent<T>() {
			return components.ContainsKey(typeof(T));
		}
		public bool TryGetComponent<T>(out T t) where T : Component {
			if (components.TryGetValue(typeof(T), out var tmp)) {
				t = (T) tmp;
				return true;
			}


			t = default(T);
			return false;
		}

		#endregion
		
		#region Distance and angle

		public float DxTo(Entity entity) {
			return entity.CenterX - CenterX;
		}
		
		public float DyTo(Entity entity) {
			return entity.CenterY - CenterY;
		}

		public float DistanceTo(Entity entity) {
			float dx = DxTo(entity);
			float dy = DyTo(entity);
			
			return dx * dx + dy * dy;
		}

		public float DxTo(Vector2 entity) {
			return entity.X - CenterX;
		}
		
		public float DyTo(Vector2 entity) {
			return entity.Y - CenterY;
		}

		public float DistanceTo(Vector2 entity) {
			float dx = DxTo(entity);
			float dy = DyTo(entity);
			
			return dx * dx + dy * dy;
		}

		public float DistanceSquaredTo(Vector2 entity) {
			return (float) Math.Sqrt(DistanceTo(entity));
		}

		public float DistanceSquaredTo(Entity entity) {
			return (float) Math.Sqrt(DistanceTo(entity));
		}
		
		public float AngleTo(Entity entity) {
			return (float) Math.Atan2(DyTo(entity), DxTo(entity));
		}
		
		public float AngleTo(Vector2 vec) {
			return (float) Math.Atan2(DyTo(vec), DxTo(vec));
		}
		
		#endregion
		
		#region Simple collision

		public virtual bool Overlaps(Entity entity) {
			return !(entity.X > Right ||
			         entity.Right < X ||
			         entity.Y > Bottom ||
			         entity.Bottom < Y);
		}

		public virtual bool Contains(Entity entity) {
			return entity.X >= X && entity.Right <= Right
			                     && entity.Y >= Y && entity.Bottom <= Bottom;
		}

		public virtual bool Contains(Vector2 point) {
			return point.X >= X && point.X <= Right
													&& point.Y >= Y && point.Y <= Bottom;
		}
		
		#endregion
	}
}