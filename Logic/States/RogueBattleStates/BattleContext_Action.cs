using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class BattleContext {
  public enum ActionFuncType {
    ATTACK,
    NON_ATK_DMG,
    GAIN_SHIELD,
    ADD_BUFF,
    SP_SWAP,
  }
  public enum ActionTarget {
    SELF,
    PLAYER,
    ALL_ENEMY,
  }
  public enum BuffId {
    攻击力,
    防守反击,
  }

  public class ActionDescriptor {
    public CharObject invoker;
    public ActionFuncType funcType;
    public ActionTarget target;
    public BuffId addBuffId;
    public int baseDmg;
    public float ratioDmg;
    public int baseShield;
    public float ratioShield;
    public int buff_stack_0;
  }

  #region 执行怪味行为逻辑

  public void ExecuteAction(ActionDescriptor action) {
    var targets = new List<CharObject>();
    targets.AddRange(FindTarget(action));
    switch (action.funcType) {
      case ActionFuncType.ATTACK:
        _ExecuteAttackAction(action, targets);
        break;
      case ActionFuncType.NON_ATK_DMG:
        _ExecuteAttackAction(action, targets); //暂时使用攻击逻辑处理非攻击伤害，区分是为了避免反击buff的嵌套
        break;
      case ActionFuncType.GAIN_SHIELD:
        _ExcuteGainShieldAction(action, targets);
        break;
      case ActionFuncType.ADD_BUFF:
        _ExecuteAddBuffAction(action, targets);
        break;
    }
    foreach (var target in targets) {
      foreach (var buff in target.buffs) {
        BuffTrigger_AfterBeingActionTarget(target, buff, action);
      }
    }
  }

  private void _ExecuteAttackAction(ActionDescriptor action, IEnumerable<CharObject> targets) {
    foreach (var target in targets) {
      var attacker = action.invoker;

      var dmg = action.baseDmg + (int)(attacker.atk * action.ratioDmg);

      var shieldDmg = (int)MathF.Min(dmg, target.shield);
      target.shield -= shieldDmg;
      target.hp -= dmg - shieldDmg;

      Log.Push($"[{attacker.name}] => [{target.name}]  -[{dmg}]");
    }
  }

  private void _ExcuteGainShieldAction(ActionDescriptor action, IEnumerable<CharObject> targets) {
    foreach (var target in targets) {
      var deltaShield = action.baseShield + (int)(target.def * action.ratioShield);
      target.shield += deltaShield;
      Log.Push($"[{target.name}] [+{deltaShield}] shield => [{target.shield}]");
    }
  }

  private void _ExecuteAddBuffAction(ActionDescriptor action, IEnumerable<CharObject> targets) {
    var buff = new CharObject.Buff {
      buffId = action.addBuffId,
      stack = action.buff_stack_0,
    };
    foreach (var target in targets) {
      target.buffs.Add(buff);
    }
  }

  private IEnumerable<CharObject> FindTarget(ActionDescriptor action) {
    if (action.target == ActionTarget.SELF) {
      yield return action.invoker;
    } else if (action.target == ActionTarget.PLAYER) {
      yield return playerCharObj;
    } else if (action.target == ActionTarget.ALL_ENEMY) {
      foreach (var enemy in enemies) {
        yield return enemy;
      }
    }
  }

  #endregion

  #region 展示怪物行为逻辑

  public void UpdateForeShowAction(ActionDescriptor action) {

    switch (action.funcType) {
      case ActionFuncType.ATTACK:
        _ForeShowAttackAction(action);
        break;
      case ActionFuncType.GAIN_SHIELD:
        _ForeShowGainShieldAction(action);
        break;
      case ActionFuncType.ADD_BUFF:
        _ForeShowAddBuffAction(action);
        break;
    }
  }

  private void _ForeShowAttackAction(ActionDescriptor action) {

    var attacker = action.invoker;
    var dmg = action.baseDmg + (int)(attacker.atk * action.ratioDmg);

    string targets = "";
    foreach (var target in FindTarget(action)) {
      targets += target.name;
      targets += ",";
    }

    if (targets.Length > 0) {
      targets = targets.Substring(0, targets.Length - 1);
    }

    string ret = $"cause {dmg} to {targets},";

    action.invoker.enemyIntention += ret;
  }

  private void _ForeShowGainShieldAction(ActionDescriptor action) {

    string ret = "";
    foreach (var target in FindTarget(action)) {
      var deltaShield = action.baseShield + (int)(target.def * action.ratioShield);
      target.shield += deltaShield;
      ret += $"make {target.name} gain {deltaShield} shield,";
    }

    action.invoker.enemyIntention += ret;
  }

  private void _ForeShowAddBuffAction(ActionDescriptor action) {
    var buff = new CharObject.Buff {
      buffId = action.addBuffId,
      stack = action.buff_stack_0,
    };

    string ret = "";
    foreach (var target in FindTarget(action)) {

      switch (buff.buffId) {
        case BuffId.防守反击:
          ret += $"{target.name} acquire {buff.stack} fightback,"; 
          break;
        case BuffId.攻击力:
          ret += $"{target.name} acquire {buff.stack} strength,";
          break;
      }
    }

    action.invoker.enemyIntention += ret;
  }

  #endregion

  public static class CommonAction {
    public static ActionDescriptor player_atack(float ratioDmg = 1.0f, int baseDmg = 0) {
      return new ActionDescriptor {
        funcType = ActionFuncType.ATTACK,
        target = ActionTarget.ALL_ENEMY,
        ratioDmg = ratioDmg,
      };
    }
    public static ActionDescriptor player_gainShield(int baseShield = 0, float ratioShield = 0.0f) {
      return new ActionDescriptor {
        funcType = ActionFuncType.GAIN_SHIELD,
        target = ActionTarget.SELF,
        baseShield = baseShield,
        ratioShield = ratioShield,
      };
    }

    public static ActionDescriptor enemy_attack(CharObject attacker, float ratioDmg = 1.0f, int baseDmg = 0) {
      return new ActionDescriptor {
        invoker = attacker,
        funcType = ActionFuncType.ATTACK,
        target = ActionTarget.PLAYER,
        ratioDmg = ratioDmg,
        baseDmg = baseDmg,
      };
    }
  }
}