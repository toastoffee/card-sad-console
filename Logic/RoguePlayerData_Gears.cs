using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class RoguePlayerData {
  private void InitializeEquipmentSlots() {
    equipmentSlots.Clear();
    equipmentSlots.Add(new EquipmentSlot(GearSlot.MAIN_WEAPON, "Main Weapon"));
    equipmentSlots.Add(new EquipmentSlot(GearSlot.SIDE_WEAPON, "Side Weapon"));
    equipmentSlots.Add(new EquipmentSlot(GearSlot.ARMOR, "Armor"));
    equipmentSlots.Add(new EquipmentSlot(GearSlot.HELMET, "Helmet"));
    equipmentSlots.Add(new EquipmentSlot(GearSlot.SHOE, "Shoe"));
    equipmentSlots.Add(new EquipmentSlot(GearSlot.MAGIC, "Magic"));
  }

  public void UpdateStatsProp() {
    var modifierList = CharPropSchema.NewModifierList();
    foreach (var slot in equipmentSlots) {
      slot.equippedGear?.prop.ConvertToAddModifiers(modifierList);
    }
    finalProp_statsOnly = CharPropSchema.ApplyModifers(baseProp, modifierList);
  }

  // 装备管理方法
  public bool EquipGear(GearObject gear) {
    // 找到第一个兼容的空槽位
    var compatibleSlot = equipmentSlots.FirstOrDefault(slot =>
      !slot.HasEquipment && slot.CanEquip(gear));

    if (compatibleSlot != null) {
      compatibleSlot.EquipGear(gear);
      UpdateStatsProp();
      return true;
    }
    return false;
  }

  public bool EquipGearToSlot(GearSlot slotType, GearObject gear) {
    var slot = equipmentSlots.FirstOrDefault(s => s.slotType == slotType);
    if (slot != null && slot.CanEquip(gear)) {
      slot.UnequipGear(); // 先卸下旧装备
      slot.EquipGear(gear);
      UpdateStatsProp();
      return true;
    }
    return false;
  }

  public GearObject UnequipGear(GearSlot slotType) {
    var slot = equipmentSlots.FirstOrDefault(s => s.slotType == slotType);
    if (slot != null && slot.HasEquipment) {
      var gear = slot.UnequipGear();
      UpdateStatsProp();
      return gear;
    }
    return null;
  }

  public EquipmentSlot GetSlot(GearSlot slotType) {
    return equipmentSlots.FirstOrDefault(s => s.slotType == slotType);
  }

  // 清空所有装备
  public void ClearAllGears() {
    foreach (var slot in equipmentSlots) {
      slot.UnequipGear();
    }
    UpdateStatsProp();
  }
}
