
using static BattleContext;

public static partial class EnemyActionDefine {

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
          target = ActionTarget.PLAYER,
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
          target = ActionTarget.PLAYER,
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
          baseShield = 8,
        });
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.防守反击,
          target = ActionTarget.SELF,
          buff_stack_0 = 6,
          buff_life = 2, //持续1回合 时序有点微妙，现在要设为2才行
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

    if (ctx.turnRound == 0) {
      return EnemyActionTag.攻击_1;
    } else {
      switch (ctx.turnRound % 3) {
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
