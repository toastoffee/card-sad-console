using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FloatField<Src> where Src : struct {
  public int reflection_idx;
  public Func<Src, float, Src> setter;
  public Func<Src, float> getter;
  public FloatField(Func<Src, float, Src> setter, Func<Src, float> getter) {
    this.setter = setter;
    this.getter = getter;
  }
}
public enum ModifierType {
  Add,
  RatioAdd,
}

public class FloatPropSchema<TSrc, TSchema> where TSrc : struct where TSchema : new() {
  public delegate FloatField<TSrc> FieldSelector(TSchema schema);

  public class Modifier {
    public FieldSelector fieldSelector;
    public ModifierType type;
    public float value;

    public Modifier(FieldSelector fieldSelector) {
      this.fieldSelector = fieldSelector;
    }
  }

  public class ModifyContext {
    public FloatField<TSrc> fieldInfo;
    public float addVal;
    public float ratioAddVal;

    public void Clear() {
      addVal = 0;
      ratioAddVal = 0;
    }

    public void AddMod(Modifier mod) {
      switch (mod.type) {
        case ModifierType.Add:
          addVal += mod.value;
          break;
        case ModifierType.RatioAdd:
          ratioAddVal += mod.value;
          break;
      }
    }

    public void Execute(ref TSrc input) {
      if (fieldInfo == null) return;
      var value = fieldInfo.getter(input);
      if (ratioAddVal != 0) {
        value *= (1 + ratioAddVal);
      }
      if (addVal != 0) {
        value += addVal;
      }
      input = fieldInfo.setter(input, value);
    }
  }

  public static List<Modifier> NewModifierList() {
    return new List<Modifier>();
  }
  private static TSchema schemaInst = new();

  private static Dictionary<FloatField<TSrc>, ModifyContext> s_fieldToModCtx = new Dictionary<FloatField<TSrc>, ModifyContext>();
  public static TSrc ApplyModifers(TSrc input, List<Modifier> mods) {
    if (mods == null || mods.Count == 0) {
      return input;
    }
    var output = input;
    foreach (var pair in s_fieldToModCtx) {
      pair.Value.Clear();
    }
    foreach (var mod in mods) {
      var field = mod.fieldSelector(schemaInst);
      if (!s_fieldToModCtx.ContainsKey(field)) {
        s_fieldToModCtx[field] = new ModifyContext() {
          fieldInfo = field,
        };
      }
      s_fieldToModCtx[field].AddMod(mod);
    }
    foreach (var ctx in s_fieldToModCtx.Values) {
      ctx.Execute(ref output);
    }
    return output;
  }
}
