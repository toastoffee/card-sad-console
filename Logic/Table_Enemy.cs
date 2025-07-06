using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleContext;

public enum EnemyActionTag {
  攻击_1,
  攻击_2,
  攻击_3,
  招架,
  蓄力,
}

public delegate void EnemyActionHandler(EnemyContext ctx, Action< ActionDescriptor> enqueue);

public class EnemyAction {
  public EnemyActionTag tag;
  public EnemyActionHandler actionHandler;
}

public class EnmeyActionModel {
  public string modelId;
  public List<EnemyAction> actions = new();
  public Func<EnemyContext, IEnumerator<EnemyActionTag>> actionSelector;
}

[AutoModelTable(typeof(EnmeyActionModel))]
public static class EnemyActionDefine {
  public static EnmeyActionModel Bandit => new EnmeyActionModel {
    modelId = nameof(Bandit),
    actions = BanditAtions(),
    actionSelector = BanditActionSelector,
  };
  public static List<EnemyAction> BanditAtions() {
    var ret = new List<EnemyAction>();
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.攻击_1,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          funcType = ActionFuncType.ATTACK,
          baseDmg = 8,
        });
      }),
    });
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.攻击_2,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          funcType = ActionFuncType.ATTACK,
          baseDmg = 8,
        });
      }),
    });
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.招架,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          funcType = ActionFuncType.GAIN_SHIELD,
          baseDmg = 8,
        });
        enqueue(new ActionDescriptor {
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.防守反击,
          target = ActionTarget.SELF,
          buff_stack_0 = 6,
        });
      }),
    });
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.蓄力,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.攻击力,
          target = ActionTarget.SELF,
          buff_stack_0 = 2,
        });
      }),
    });
    return ret;
  }
  public static IEnumerator<EnemyActionTag> BanditActionSelector(EnemyContext ctx) {
    yield return EnemyActionTag.攻击_1;
    while (true) {
      yield return EnemyActionTag.招架;
      yield return EnemyActionTag.蓄力;
      yield return EnemyActionTag.攻击_2;
    }
  }
}

