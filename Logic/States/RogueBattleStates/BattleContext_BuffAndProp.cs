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
    CalculateCharBuffs(playerCharObj);
    playerCharObj.UpdateProp();

    foreach (var enemy in enemies) {
      enemy.UpdateProp();
    }
  }

  public void MarkRoundEndToChars() {
    MarkTurnEndToBuffs(playerCharObj);
    foreach (var enemy in enemies) {
      MarkTurnEndToBuffs(enemy);
    }
  }

  public void CalculateCharBuffs(CharObject charObject) {
    for (int i = 0; i < charObject.buffs.Count; i++) {
      var buff = charObject.buffs[i];

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
      buff.livedTurn++;
    }
  }

  public void AddPlayerGearsModifiers(List<Modifer> modifiers) {
    foreach (var slot in RoguePlayerData.Instance.equipmentSlots) {
      slot.equippedGear?.prop.ConvertToAddModifiers(modifiers);
    }
  }
}