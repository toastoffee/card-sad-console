
using static BattleContext;

public static partial class EnemyActionDefine {

  public static EnemyActionModel CreateFurnaceWorker(CharObject self) {
    return new EnemyActionModel {
      modelId = "FurnaceWorker",
      actions = FurnaceWorkerActions(self),
      actionSelector = FurnaceWorkerActionSelector,
    };
  }

  public static List<EnemyAction> FurnaceWorkerActions(CharObject self) {
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
          baseShield = 6,
        });
        enqueue(new ActionDescriptor {
          invoker = self,
          funcType = ActionFuncType.ADD_BUFF,
          addBuffId = BuffId.熔炉护盾,
          target = ActionTarget.SELF,
          buff_stack_0 = 4,
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
  public static EnemyActionTag FurnaceWorkerActionSelector(EnemyContext ctx) {

    switch (ctx.turnRound % 4) {
      case 0:
        return EnemyActionTag.攻击_1;
      case 1:
        return EnemyActionTag.防御;
      case 2:
        return EnemyActionTag.攻击_1;
      case 3:
        return EnemyActionTag.蓄力;
    }

    return EnemyActionTag.攻击_1;
  }

}
