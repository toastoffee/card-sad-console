using static GearModel;
using static BattleContext;

public class CardModel {
  public string modelId;
  public int cost;
  public CardType cardType;
  public List<CardTag> cardTags = new();

  private Action<BattleContext, Action<ActionDescriptor>> mAction;
  public Action<BattleContext, Action<ActionDescriptor>> action => mAction;

  public List<ActionDescriptor> cardActions = new();

  public CardModel() {
    mAction = (ctx, enqueue) => {
      foreach (var action in cardActions) {
        enqueue(action);
      }
    };
  }
}

[AutoModelTable(typeof(CardModel))]
public static class CardDefine {
  public static CardModel Strike => new CardModel {
    modelId = nameof(Strike),
    cost = 1,
    cardType = CardType.WEAPON,
    cardActions = new List<ActionDescriptor> {
      CommonAction.player_attack(1.0f)
    }
  };
  public static CardModel Swap => new CardModel {
    modelId = nameof(Swap),
    cost = 1,
    cardType = CardType.TRINKET,
  };
  public static CardModel Defend => new CardModel {
    modelId = nameof(Defend),
    cost = 1,
    cardType = CardType.ARMOR,
    cardActions = new List<ActionDescriptor> {
      CommonAction.player_gainShield(ratioShield: 1.0f)
    }
  };
  public static CardModel Hit => new CardModel {
    modelId = nameof(Hit),
    cost = 2,
    cardType = CardType.WEAPON,
    cardActions = new List<ActionDescriptor> {
      CommonAction.player_attack(2.5f)
    }
  };
  public static CardModel Fire => new CardModel {
    modelId = nameof(Fire),
    cost = 0,
    cardType = CardType.MAGIC,
    cardTags = new List<CardTag> {
      CardTag.STICKY,
      CardTag.CONSIST,
      CardTag.FRAGILE,
    },
  };

  public static CardModel CutOff => new CardModel {
    modelId = nameof(CutOff),
    cost = 0,
    cardType = CardType.WEAPON,
    cardActions = new List<ActionDescriptor> {
      CommonAction.player_attack(0.8f)
    }
  };

  public static CardModel SkilledAttack => new CardModel {
    modelId = nameof(SkilledAttack),
    cost = 1,
    cardType = CardType.WEAPON,
    cardActions = new List<ActionDescriptor> {
      CommonAction.player_attack(1.0f)
    },
    cardTags = new List<CardTag> {
      CardTag.RELOAD
    },
  };
}

public static class EnumTrans {
  static EnumTrans() {
    Impl<CardType>.Set(new List<KeyValuePair<CardType, string>> {
      new KeyValuePair<CardType, string>(CardType.WEAPON, "Weapon"),
      new KeyValuePair<CardType, string>(CardType.ARMOR, "Armor"),
      new KeyValuePair<CardType, string>(CardType.HELMET, "Helmet"),
      new KeyValuePair<CardType, string>(CardType.SHOE, "Shoe"),
      new KeyValuePair<CardType, string>(CardType.TRINKET, "Trinket"),
      new KeyValuePair<CardType, string>(CardType.MAGIC, "Magic"),
    });
    Impl<GameStateEnum>.Set(new List<KeyValuePair<GameStateEnum, string>> {
      new KeyValuePair<GameStateEnum, string>(GameStateEnum.IDLE, "等待出牌"),
      new KeyValuePair<GameStateEnum, string>(GameStateEnum.CARD_SELECTING, "选择一张手牌，按'e'取消，按'c'确认"),
      new KeyValuePair<GameStateEnum, string>(GameStateEnum.ENEMY_SELECTING, "选择一个敌人"),
    });
    Impl<CardTag>.Set(new List<KeyValuePair<CardTag, string>> {
      new KeyValuePair<CardTag, string>(CardTag.STICKY, "Sticky"),
    });
    Impl<BuffId>.SetFunc((BuffId id) => {
      switch (id) {
        case BuffId.攻击力:
          return "Strength";
        case BuffId.防守反击:
          return "Fightback";
        case BuffId.减速:
          return "SlowDown";
        case BuffId.熔炉护盾:
          return "FurnaceShield";
        default:
          return id.ToString();
      }
    });
  }

  public static class Impl<T> where T : notnull {
    public static Dictionary<T, string> dict = new Dictionary<T, string>();
    public static Func<T, string> toStringFunc = null;
    public static void Set(List<KeyValuePair<T, string>> list) {
      foreach (var pair in list) {
        dict[pair.Key] = pair.Value;
      }
    }

    public static void SetFunc(Func<T, string> toStringFunc) {
      Impl<T>.toStringFunc = toStringFunc;
    }
  }
  public static string Get<T>(T value) where T : notnull {
    if (Impl<T>.toStringFunc != null) {
      return Impl<T>.toStringFunc(value);
    } else if (Impl<T>.dict != null && Impl<T>.dict.TryGetValue(value, out var ret)) {
      return ret;
    }
    Log.PushSys($"[EnumTrans] no trans for [{value}]");
    return value.ToString();
  }
}

[Flags]
public enum GearSlot {
  NONE = 0,
  MAIN_WEAPON = 1 << 0,
  SIDE_WEAPON = 1 << 1,
  ARMOR = 1 << 2,
  HELMET = 1 << 3,
  SHOE = 1 << 4,
  MAGIC = 1 << 5,
}

public class GearModel {
  public class CardRecord {
    public CardModel card;
    public int cnt;
    public CardRecord(string id, int cnt) {
      this.card = AutoModelTable<CardModel>.Read(id);
      this.cnt = cnt;
    }
  }

  public string modelId;
  public GearSlot availSlotFlag;
  public CharProp baseProp;
  public List<CardRecord> cards;
}

[AutoModelTable(typeof(GearModel))]
public static class GearDefine {
  public static GearModel Wood_Sword {
    get {
      var ret = new GearModel() {
        modelId = nameof(Wood_Sword),
        availSlotFlag = GearSlot.MAIN_WEAPON,
        cards = new List<CardRecord>() {
          new CardRecord(nameof(CardDefine.Strike), 5),
        },
        baseProp = new CharProp() {
          atk = 3,
        },
      };
      return ret;
    }
  }

  public static GearModel Wood_Armor {
    get {
      var ret = new GearModel() {
        modelId = nameof(Wood_Armor),
        availSlotFlag = GearSlot.ARMOR,
        cards = new List<CardRecord>() {
          new CardRecord(nameof(CardDefine.Defend), 5),
        },
        baseProp = new CharProp() {
          def = 3,
        },
      };
      return ret;
    }
  }

  public static GearModel Wood_Shoe {
    get {
      var ret = new GearModel() {
        modelId = nameof(Wood_Shoe),
        availSlotFlag = GearSlot.SHOE,
        cards = new List<CardRecord>() {
        }
      };
      return ret;
    }
  }

  public static GearModel Wood_Helmet {
    get {
      var ret = new GearModel() {
        modelId = nameof(Wood_Helmet),
        availSlotFlag = GearSlot.HELMET,
        cards = new List<CardRecord>() {
        }
      };
      return ret;
    }
  }

  public static GearModel Fire_Magic {
    get {
      var ret = new GearModel() {
        modelId = nameof(Fire_Magic),
        availSlotFlag = GearSlot.MAGIC,
        cards = new List<CardRecord>() {
          new CardRecord(nameof(CardDefine.Fire), 1),
        }
      };
      return ret;
    }
  }
}