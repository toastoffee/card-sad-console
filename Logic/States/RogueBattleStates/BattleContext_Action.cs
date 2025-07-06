using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class BattleContext {
  public enum ActionFuncType {
    ATTACK,
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

  public void ExecuteAction(ActionDescriptor action) {
    switch (action.funcType) {
      case ActionFuncType.ATTACK:
        _ExecuteAttackAction(action);
        break;
      case ActionFuncType.GAIN_SHIELD:
        _ExcuteGainShieldAction(action);
        break;
      case ActionFuncType.ADD_BUFF:
        _ExecuteAddBuffAction(action);
        break;
    }
  }

  private void _ExecuteAttackAction(ActionDescriptor action) {
    foreach (var target in FindTarget(action)) {
      var attacker = action.invoker;

      var dmg = action.baseDmg + (int)(attacker.atk * action.ratioDmg);

      var shieldDmg = (int)MathF.Min(dmg, target.shield);
      target.shield -= shieldDmg;
      target.hp -= dmg - shieldDmg;

      Log.Push($"[{attacker.name}] => [{target.name}]  -[{dmg}]");
    }
  }

  private void _ExcuteGainShieldAction(ActionDescriptor action) {
    foreach (var target in FindTarget(action)) {
      var deltaShield = action.baseShield + (int)(target.def * action.ratioShield);
      target.shield += deltaShield;
      Log.Push($"[{target.name}] [+{deltaShield}] shield => [{target.shield}]");
    }
  }

  private void _ExecuteAddBuffAction(ActionDescriptor action) {
    var buff = new CharObject.Buff {
      buffId = action.addBuffId,
      stack = action.buff_stack_0,
    };
    foreach (var target in FindTarget(action)) {
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