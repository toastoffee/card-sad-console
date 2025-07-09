
using static BattleContext;

public enum EnemyActionTag {
  攻击_1,
  攻击_2,
  攻击_3,
  
  防御,

  招架,
  蓄力,
  绊脚,

}

public delegate void EnemyActionHandler(EnemyContext ctx, Action< ActionDescriptor> enqueue);

public class EnemyAction {
  public EnemyActionTag tag;
  public EnemyActionHandler actionHandler;
}

public class EnemyActionModel {
  public string modelId;
  public List<EnemyAction> actions = new();
  public Func<EnemyContext, EnemyActionTag> actionSelector;

  public EnemyAction this[EnemyActionTag tag] {
    get {
      var action = actions.FirstOrDefault(action => action.tag == tag);

      return action;
    }
    set {
      var idx = actions.FindIndex(action => action.tag == tag);
      if (idx >= 0 && idx < actions.Count) {
        actions[idx] = value;
      }else {
        throw new Exception($"妹有找到：{tag.GetType().Name}");
      }
    }
  }
}

[AutoModelTable(typeof(EnemyActionModel))]
public static partial class EnemyActionDefine {
  
}

