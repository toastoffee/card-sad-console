
using static BattleContext;

public static partial class EnemyActionDefine {

  public static EnemyActionModel CreateLongLeggy(CharObject self) {
    return new EnemyActionModel {
      modelId = "LongLeggy",
      actions = LongLeggyActions(self),
      actionSelector = LongLeggyActionSelector,
    };
  }

  public static List<EnemyAction> LongLeggyActions(CharObject self) {
    var ret = new List<EnemyAction>();
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.攻击_1,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ATTACK,
          target = ActionTarget.PLAYER,
          baseDmg = 7,
        });
      }),
    });

    ret.Add(new EnemyAction {
      tag = EnemyActionTag.防御,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.GAIN_SHIELD,
          baseShield = 8,
        });
      }),
    });
    ret.Add(new EnemyAction {
      tag = EnemyActionTag.绊脚,
      actionHandler = new EnemyActionHandler((ctx, enqueue) => {
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ATTACK,
          target = ActionTarget.PLAYER,
          baseDmg = 4,
        });
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.减速,
          target = ActionTarget.PLAYER,
          buff_stack_0 = 5,
          buff_life = 2, //持续1回合 时序有点微妙，现在要设为2才行
        });
      }),
    });

    return ret;
  }
  public static EnemyActionTag LongLeggyActionSelector(EnemyContext ctx) {

    switch (ctx.turnRound % 4) {
      case 0:
        return EnemyActionTag.攻击_1;
      case 1:
        return EnemyActionTag.防御;
      case 2:
        return EnemyActionTag.攻击_1;
      case 3:
        return EnemyActionTag.绊脚;
    }
    

    return EnemyActionTag.攻击_1;
  }

}
