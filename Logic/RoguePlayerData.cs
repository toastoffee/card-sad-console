public enum RogueNodeType {
  Init,
  Enemy,
  Interlude,
}

public class RogueInitNodeDescriptor {

}

public class RogueEnemyNodeDescriptor {
  public bool isKilled = false;
}

public class RogueInterludeNodeDescriptor {

}

public partial class RoguePlayerData {
  private static RoguePlayerData _instance;
  public static RoguePlayerData Instance {
    get {
      if (_instance == null) {
        _instance = new RoguePlayerData();
      }
      return _instance;
    }
  }

  //player
  public CharProp baseProp;
  public CharProp finalProp_statsOnly;

  public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

  public int hp;
  public int money = 99;

  //nodes
  public RogueNodeType nodeType;
  public RogueInitNodeDescriptor initNodeDescriptor;
  public RogueEnemyNodeDescriptor enemyNodeDescriptor;
  public RogueInterludeNodeDescriptor interludeNodeDescriptor;

  private RoguePlayerData() {
    // 初始化基础属性
    baseProp = new CharProp {
      maxHp = 70,
      def = 0,
      atk = 0,
      speed = 30,
    };
    // 初始化所有装备槽位
    InitializeEquipmentSlots();
    InitNodes();
    UpdateStatsProp();

    hp = baseProp.maxHp;
  }


  public static void ResetInstance() {
    _instance = null;
  }
}