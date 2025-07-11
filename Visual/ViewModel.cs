namespace CardConsole.Visual;

public enum GameDisplayStateType {
  Battle,
  Route
}

public enum PopupDisplayType {
  NONE,
  ITEM_DETAIL,
}

public class RouteOptionViewModel {
  public string description;
  public int index;
}


// 添加装备槽位信息的类
public class EquipmentSlotViewModel {
  public GearSlot slotType;
  public string slotName;
  public string equipmentName; // "None" 如果没有装备
}

public class ViewModel {
  public int turn;
  public int eng;
  public int maxEng;
  public int playerHp;
  public int maxPlayerHp;
  public int playerShield; // 添加玩家护盾值
  public int journey;
  public int maxJourney;
  public List<(string buff, int stack)> playerBuffs = new() { };
  public CharProp playerProp; // 添加玩家总属性
  public GameDisplayStateType displayState; // 添加状态类型标识
  public PopupDisplayType popupState;
  public int playerMoney;

  // Route状态相关数据
  public string routeDescription = "";
  public List<RouteOptionViewModel> routeOptions = new List<RouteOptionViewModel>();

  public List<EnemyViewModel> enemies = new List<EnemyViewModel>(); // 添加敌人视图模型列表
  public List<CardViewModel> handCards = new List<CardViewModel>(); // 玩家手牌列表


  // 添加装备信息
  public List<EquipmentSlotViewModel> equipmentSlots = new List<EquipmentSlotViewModel>();

  // 添加卡牌堆数据
  public int deckCount;
  public int discardCount;

  public List<string> gameLogs = new List<string>();    // 游戏相关的日志（如战斗、回合等）
  public List<string> systemLogs = new List<string>();  // 系统相关的日志（如程序运行、错误等）
}

public class EnemyViewModel {
  public string name;
  public int hp;
  public int maxHp;
  public int shield;
  public string intention;
  public List<(string buff, int stack)> buffs = new() { };
}

public class CardViewModel {
  public string name;
  public int cost;
  public string description;
  public string header;
  public List<string> tags = new();
  public Color tagColor = Color.White;
  public Color descColor;
  public Color titleColor;
}
