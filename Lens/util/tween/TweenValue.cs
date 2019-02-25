using System;
using System.Linq;
using System.Reflection;

namespace Lens.util.tween {
	public class TweenValue {
		static TweenValue() {
			NumericTypes = new[] {
				typeof(Int16),
				typeof(Int32),
				typeof(Int64),
				typeof(UInt16),
				typeof(UInt32),
				typeof(UInt64),
				typeof(Single),
				typeof(Double),
				typeof(Byte)
			};
		}

		private static readonly Type[] NumericTypes;
		private readonly FieldInfo field;
		private readonly PropertyInfo prop;
		private readonly bool isNumeric;
		private readonly object target;

		public string Name { get; private set; }

		public object Value {
			get { return field != null ? field.GetValue(target) : prop.GetValue(target, null); }
			set {
				if (isNumeric) {
					Type type = null;
					
					if (field != null) {
						type = field.FieldType;
					}

					if (prop != null) {
						type = prop.PropertyType;
					}

					if (AnyEquals(type, NumericTypes)) {
						value = Convert.ChangeType(value, type);
					}
				}

				if (field != null) {
					field.SetValue(target, value);
				} else {
					prop?.SetValue(target, value, null);
				}
			}
		}

		public TweenValue(object target, string property, bool writeRequired = true) {
			this.target = target;
			Name = property;

			Type targetType;

			if (IsType(target)) {
				targetType = (Type) target;
			}	else {
				targetType = target.GetType();
			}

			field = targetType.GetTypeInfo().DeclaredFields.FirstOrDefault(f =>
				string.Equals(property, f.Name) && !f.IsStatic);

			prop = writeRequired
				? targetType.GetTypeInfo().DeclaredProperties.FirstOrDefault(p =>
					string.Equals(property, p.Name) && !p.GetMethod.IsStatic && p.CanRead && p.CanWrite)
				: targetType.GetTypeInfo().DeclaredProperties.FirstOrDefault(p =>
					string.Equals(property, p.Name) && !p.GetMethod.IsStatic && p.CanRead);

			if (field == null) {
				if (prop == null) {
					//	Couldn't find either
					throw new Exception(string.Format("Field or '{0}' property '{1}' not found on object of type {2}.",
						writeRequired ? "read/write" : "readable",
						property, targetType.FullName));
				}
			}

			var valueType = Value.GetType();
			isNumeric = AnyEquals(valueType, NumericTypes);
			CheckPropertyType(valueType, property, targetType.Name);
		}

		bool IsType(object target) {
			var type = target.GetType();
			var baseType = typeof(Type);

			if (type == baseType) {
				return true;
			}

			var rootType = typeof(object);

			while (type != null && type != rootType) {
				var info = type.GetTypeInfo();
				var current = info.IsGenericType && info.IsGenericTypeDefinition ? info.GetGenericTypeDefinition() : type;

				if (baseType == current) {
					return true;
				}

				type = info.BaseType;
			}

			return false;
		}

		private void CheckPropertyType(Type type, string prop, string targetTypeName) {
			if (!ValidatePropertyType(type)) {
				throw new InvalidCastException(string.Format("Property is invalid: ({0} on {1}).", prop, targetTypeName));
			}
		}

		protected virtual bool ValidatePropertyType(Type type) {
			return isNumeric;
		}

		static bool AnyEquals<T>(T value, params T[] options) {
			foreach (var option in options) {
				if (value.Equals(option)) {
					return true;
				}
			}

			return false;
		}
	}
}