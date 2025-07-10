using CardConsole.Visual;

class CardGame {
  public static CardGame instance;
  public LiteStateEngine stateEngine;
  public ViewModel viewModel = new();

  public CardGame() {
    instance = this;
  }

  public void Setup() {
    var playerData = RoguePlayerData.Instance;
    playerData.baseProp = new CharProp {
      maxHp = 70,
      def = 2,
      atk = 2,
      speed = 33,
      maxJourney = 10,
    };

    var battleState = new RogueBattleState();
    RogueBattleState.instance = battleState;
    RogueInitState initState = new RogueInitState();
    stateEngine = new LiteStateEngine(new List<LiteState> {
      initState,
      battleState,
      new RogueBattleFinishState(),
      new RogueInterludeState(),
    });

    CharProp prop = new CharProp() {
      atk = 10,
      maxHp = 100,
    };
  }

  public void Tick() {
    var state = stateEngine.frontState as GameState;

    // 同步信息面板数据
    SyncInfoPanelToViewModel();

    // 添加日志同步
    SyncLogsToViewModel();

    //tick在最后，子状态可以覆盖ViewModel中的数据
    state.OnTick();
  }

  public void SyncLogsToViewModel() {
    if (viewModel == null) return;

    // 分别同步游戏日志和系统日志
    viewModel.gameLogs.Clear();
    viewModel.systemLogs.Clear();

    for (int i = 0; i < Log.gameLogs.Count; i++) {
      var log = Log.gameLogs[i];
      viewModel.gameLogs.Add(log);
      if (i < Log.gameLogs.Count - 1) {
        viewModel.gameLogs.Add("");
      }
    }
    viewModel.systemLogs.AddRange(Log.systemLogs);
  }

  public void SyncInfoPanelToViewModel() {
    if (viewModel == null) return;

    var playerData = RoguePlayerData.Instance;

    // 同步玩家属性
    viewModel.playerProp = playerData.finalProp_statsOnly;
    viewModel.playerMoney = playerData.money;

    // 同步装备信息
    viewModel.equipmentSlots.Clear();
    foreach (var slot in playerData.equipmentSlots) {
      viewModel.equipmentSlots.Add(new EquipmentSlotViewModel {
        slotType = slot.slotType,
        slotName = slot.displayName,
        equipmentName = slot.HasEquipment ? slot.equippedGear.name : "None"
      });
    }
  }

  public void RouteToProperState() {
    switch (RoguePlayerData.Instance.nodeType) {
      case RogueNodeType.Init:
        stateEngine.ReplaceTop<RogueInitState>();
        break;
      case RogueNodeType.Enemy:
        if (!RoguePlayerData.Instance.enemyNodeDescriptor.isKilled) {
          stateEngine.ReplaceTop<RogueBattleState>();
        } else {
          stateEngine.ReplaceTop<RogueBattleFinishState>();
        }
        break;
      case RogueNodeType.Interlude:
        stateEngine.ReplaceTop<RogueInterludeState>();
        break;
      default:
        break;
    }
  }
}

public struct InputValue {
  public int intValue;
}

public static class GameInput {
  public enum Type {
    CARD = 1,
    END_TURN = 2,
    ROUTE_STATE_SELECT = 3,
    SKIP_BATTLE = 4,
    ENUM_COUNT,
  }
  public struct InputCache {
    public bool flag;
    public InputValue value;
  }
  public static bool isDirty;
  public static int cardIdx;
  public static InputCache[] inputFlags = new InputCache[(int)Type.ENUM_COUNT];

  public static void Reset() {
    isDirty = false;
    for (int i = 0; i < (int)Type.ENUM_COUNT; i++) {
      inputFlags[i].flag = false;
      inputFlags[i].value.intValue = 0;
    }
  }

  public static void Set(Type type) {
    inputFlags[(int)type].flag = true;
    isDirty = true;
  }

  public static void Set(Type type, InputValue value) {
    inputFlags[(int)type].flag = true;
    inputFlags[(int)type].value = value;
    isDirty = true;
  }

  public static bool Read(Type type) {
    return inputFlags[(int)type].flag;
  }

  public static bool Read(Type type, out InputValue value) {
    value = inputFlags[(int)type].value;
    return inputFlags[(int)type].flag;
  }
}