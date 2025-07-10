public class RogueInitState : RogueRouteState {
  protected override string GetRouteDesc() {
    var ret = "Rogue game init.";
    return ret;
  }

  public override void OnEnter() {
    base.OnEnter();

    // 使用新的装备系统分配初始装备
    RoguePlayerData.Instance.EquipGearToSlot(GearSlot.MAIN_WEAPON, new GearObject(nameof(GearDefine.Wood_Sword)));
    RoguePlayerData.Instance.EquipGearToSlot(GearSlot.ARMOR, new GearObject(nameof(GearDefine.Wood_Armor)));
    RoguePlayerData.Instance.EquipGearToSlot(GearSlot.SHOE, new GearObject(nameof(GearDefine.Wood_Shoe)));
    RoguePlayerData.Instance.EquipGearToSlot(GearSlot.HELMET, new GearObject(nameof(GearDefine.Wood_Helmet)));
    RoguePlayerData.Instance.EquipGearToSlot(GearSlot.MAGIC, new GearObject(nameof(GearDefine.Fire_Magic)));
  }

  protected override List<RouteOption> GetRouteOptions() {
    var ret = new List<RouteOption> {
      new RouteOption {
        desc = "Start a new game",
        onSelect = () => {
          RoguePlayerData.Instance.FinishInitNode();
          CardGame.instance.RouteToProperState();
        }
      },
     };
    return ret;
  }
}
