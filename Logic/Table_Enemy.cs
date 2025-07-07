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
public static class EnemyActionDefine {
  //public static EnemyActionModel Bandit => new EnemyActionModel {
  //  modelId = nameof(Bandit),
  //  actions = BanditActions(),
  //  actionSelector = BanditActionSelector,
  //};
  

  public static EnemyActionModel CreateBandit(CharObject self) {
    return new EnemyActionModel {
      modelId = "Bandit",
      actions = BanditActions(self),
      actionSelector = BanditActionSelector,
    };
  }

  public static List<EnemyAction> BanditActions(CharObject self) {
    var ret = new List<EnemyAction>();
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.攻击_1,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ATTACK,
          baseDmg = 8,
        });
      }),
    });
    
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.攻击_2,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ATTACK,
          baseDmg = 8,
        });
      }),
    });
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.招架,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.GAIN_SHIELD,
          baseDmg = 8,
        });
        enqueue(new ActionDescriptor {
          invoker = self,
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
          invoker = self,
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.攻击力,
          target = ActionTarget.SELF,
          buff_stack_0 = 2,
        });
      }),
    });
    return ret;
  }
  public static EnemyActionTag BanditActionSelector(EnemyContext ctx) {

    if(ctx.turnRound == 0) {
      return EnemyActionTag.攻击_1;
    }
    else {
      switch(ctx.turnRound % 3) {
        case 1:
          return EnemyActionTag.招架;
        case 2:
          return EnemyActionTag.蓄力;
        case 0:
          return EnemyActionTag.攻击_2;
      }
    }

    return EnemyActionTag.攻击_1;
  }
}

