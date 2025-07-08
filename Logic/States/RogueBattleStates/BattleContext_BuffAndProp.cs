using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modifer = CharPropSchema.Modifier;

public partial class BattleContext {
  public void RefreshProps() {
    playerCharObj.modifiers.Clear();
    AddPlayerGearsModifiers(playerCharObj.modifiers);
    CalculateCharBuffsOnProp(playerCharObj);
    playerCharObj.CalculateFinalProp();

    foreach (var enemy in enemies) {
      enemy.modifiers.Clear();
      CalculateCharBuffsOnProp(enemy);
      enemy.CalculateFinalProp();
    }
  }

  public void MarkRoundEndToChars() {
    MarkTurnEndToBuffs(playerCharObj);
    foreach (var enemy in enemies) {
      MarkTurnEndToBuffs(enemy);
    }
  }

  /// <summary>
  /// 计算buff对角色属性的影响，以及buff的寿命
  /// </summary>
  /// <param name="charObject"></param>
  public void CalculateCharBuffsOnProp(CharObject charObject) {
    for (int i = 0; i < charObject.buffs.Count; i++) {
      var buff = charObject.buffs[i];

      if (buff.life == 0) {
        charObject.buffs.RemoveAt(i);
        i--;
        continue;
      }

      switch (buff.buffId) {
        case BuffId.攻击力:
          charObject.modifiers.Add(new(x => x.atk) {
            type = ModifierType.Add,
            value = buff.stack,
          });
          break;
      }
    }
  }

  public void MarkTurnEndToBuffs(CharObject charObject) {
    for (int i = 0; i < charObject.buffs.Count; i++) {
      var buff = charObject.buffs[i];
      if (buff.life > 0) {
        buff.life--;
      }
    }
  }

  public void AddPlayerGearsModifiers(List<Modifer> modifiers) {
    foreach (var slot in RoguePlayerData.Instance.equipmentSlots) {
      slot.equippedGear?.prop.ConvertToAddModifiers(modifiers);
    }
  }

  public void BuffTrigger_AfterBeingActionTarget(CharObject owner, CharObject.Buff buff, ActionDescriptor action) {
    switch (buff.buffId) {
      case BuffId.防守反击:
        if (action.funcType != ActionFuncType.ATTACK) {
          break;
        }
        if (owner.shield <= 0) {
          break;
        }
        var couterTarget = action.invoker;
        var couterAction = new ActionDescriptor {
          invoker = owner,
          funcType = ActionFuncType.NON_ATK_DMG,
          baseDmg = buff.stack,
          target = ActionTarget.PLAYER, //todo: 需要一个根据invoker去决定目标的描述，敌人反player，player反敌人
        };
        ExecuteAction(couterAction);
        break;
    }
  }
}