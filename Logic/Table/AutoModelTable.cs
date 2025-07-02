using System.Reflection;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AutoModelTableAttribute : Attribute {
  public Type modelType;
  public bool autoAssignId;
  public AutoModelTableAttribute(Type modelType) {
    this.modelType = modelType;
  }
}

public static class AutoModelTableImpl {
  public static void InitTables() {
    var types = ReflectionUtils.AllTypes();

    var modelTableClasses =
      from type in types
      where type.IsStaticClass() && type.HasAttribute<AutoModelTableAttribute>()
      select type;

    foreach (var type in modelTableClasses) {
      CollectModel(type);
    }
  }

  private static void CollectModel(Type staticDefineType) {
    var attr = staticDefineType.GetCustomAttribute<AutoModelTableAttribute>();
    var modelType = attr.modelType;
    var idFieldInfo = modelType.GetField("modelId");
    if (idFieldInfo == null) {
      Log.Push($"[AutoModelTable] Model类型[{modelType}] 需要定义字段modelId才可被收集");
      return;
    }
    var tableType = BuildTableType(modelType);

    var staticInstType = typeof(StaticInst<>);
    var tableStaticInstType = staticInstType.MakeGenericType(tableType);
    var staticInst = Activator.CreateInstance(tableStaticInstType);

    var staticGetInstMethod = tableStaticInstType.GetProperty(nameof(StaticInst<int>.staticInst));
    var tableAddMethod = tableType.GetMethod(nameof(TemplateTable<int, int>.ReflectOnlyAdd));

    var tableInst = staticGetInstMethod.GetValue(staticInst);
    CollectModelFromType(staticDefineType, modelType, tableInst, tableAddMethod, attr);
  }

  private static Type BuildTableType(Type modelType) {
    var templateTable = typeof(CommonTemplateTable<,>);
    var tableType = templateTable.MakeGenericType(typeof(string), modelType);
    return tableType;
  }

  private static void CollectModelFromType(Type defineType, Type modelType,
    object tableInst, MethodInfo addMethod, AutoModelTableAttribute attr) {
    var defines = from prop in defineType.GetProperties()
                  where prop.IsStaticProp() && prop.GetMethod != null && prop.PropertyType == modelType
                  select prop;
    var modelIdField = modelType.GetField("modelId");

    foreach (var prop in defines) {
      var modelObj = prop.GetGetMethod().Invoke(null, new object[] { });
      string modelId;
      if (!attr.autoAssignId) {
        modelId = modelIdField.GetValue(modelObj) as string;
      } else {
        modelId = prop.Name;
        modelIdField.SetValue(modelObj, modelId);
      }
      addMethod.Invoke(tableInst, new object[] { modelId, modelObj });
    }
  }
}