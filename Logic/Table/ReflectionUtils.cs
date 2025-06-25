using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class ReflectionUtils {
	public static bool HasAttribute<T>(this Type type) {
		if (type == null) {
			return false;
		}
		return type.IsDefined(typeof(T), false);
	}

	public static bool IsStaticClass(this Type type) {
		return type.IsClass && type.IsAbstract && type.IsSealed;
	}

	public static bool IsStaticProp(this PropertyInfo prop) {
		return (prop.GetMethod != null && prop.GetMethod.IsStatic) || (prop.SetMethod != null && prop.SetMethod.IsStatic);
	}

	public static IEnumerable<Type> AllTypes() {
		return Assembly.GetExecutingAssembly().GetTypes();
	}
}