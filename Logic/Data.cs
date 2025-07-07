using static BattleContext;

public class CardObjcet {
  public CardModel cardModel;
  public string name => cardModel.modelId;
  public int cost => cardModel.cost;
  public CardType type => cardModel.cardType;
  public List<CardTag> tags => cardModel.cardTags;

  public void LoadFromModel(CardModel model) {
    cardModel = model;
  }

  public string GetTagString() {
    return null;
  }
}

public class CharPropSchema : FloatPropSchema<CharProp, CharPropSchema> {
  public class PropField : FloatField<CharProp> {
    public PropField(Func<CharProp, float, CharProp> setter, Func<CharProp, float> getter) : base(setter, getter) { }
  }
  public PropField maxHp = new PropField(
    (src, value) => {
      src.maxHp = (int)value;
      return src;
    },
    src => src.maxHp
  );
  public PropField atk = new PropField(
    (src, value) => {
      src.atk = (int)value;
      return src;
    },
    src => src.atk
  );
  public PropField def = new PropField(
    (src, value) => {
      src.def = (int)value;
      return src;
    },
    src => src.def
  );
  public PropField speed = new PropField(
    (src, value) => {
      src.speed = (int)value;
      return src;
    },
    src => src.speed
  );
  public PropField maxJourney = new PropField(
    (src, value) => {
      src.maxJourney = (int)value;
      return src;
    },
    src => src.maxJourney
  );
}

public class CharObject {
  public class Buff {
    public BuffId buffId;
    public int stack;
    public int life = -1; // Buff的剩余持续回合数,-1表示永久生效
    public int param0;
    public int param1;
    public int param2;
  }

  public string name;
  public int hp;
  public int shield;
  public int journey;
  public int maxHp => finalProp.maxHp;
  public int def => finalProp.def;
  public int atk => finalProp.atk;
  public int speed => finalProp.speed;
  public int maxJourney => finalProp.maxJourney;
  public RogueBattleState.LegacyEnemyAction enemyAction;
  public List<Buff> buffs = new List<Buff>() { };

  public CharProp baseProp;
  public List<CharPropSchema.Modifier> modifiers = new();
  public CharProp finalProp;

  public CharProp playerProp => new CharProp {
    maxHp = maxHp,
    def = def,
    atk = atk,
    speed = speed,
    maxJourney = maxJourney,
  };

  public void LoadFromPlayerProp(int hp, CharProp prop) {
    this.hp = hp;
    baseProp = prop;
  }

  public void UpdateProp() {
    finalProp = CharPropSchema.ApplyModifers(baseProp, modifiers);
  }
}

public enum GameStateEnum {
  IDLE,
  CARD_SELECTING,
  ENEMY_SELECTING,
}

public enum CardType {
  WEAPON,
  ARMOR,
  HELMET,
  SHOE,
  TRINKET,
  MAGIC,
}

public enum CardTag {
  STICKY,//保留
  CONSIST,//固有
  FRAGILE,//消耗
}

public class GearObject {
  public GearModel gearModel;
  public string name => gearModel.modelId.Replace('_', ' ');
  public List<GearModel.CardRecord> cards => gearModel.cards;
  public CharProp prop => gearModel.baseProp;

  public GearObject(string modelId) {
    gearModel = AutoModelTable<GearModel>.Read(modelId);
  }
}

public struct CharProp {
  public int maxHp;
  public int def;
  public int atk;
  public int speed;
  public int maxJourney;

  public CharProp Add(CharProp other) {
    var ret = new CharProp() {
      maxHp = this.maxHp + other.maxHp,
      def = this.def + other.def,
      atk = this.atk + other.atk,
      speed = this.speed + other.speed,
      maxJourney = this.maxJourney + other.maxJourney
    };
    return ret;
  }

  public void ConvertToAddModifiers(List<CharPropSchema.Modifier> modifiers) {
    if (modifiers == null) {
      return;
    }
    if (maxHp != 0) {
      modifiers.Add(new CharPropSchema.Modifier(schema => schema.maxHp) { type = ModifierType.Add, value = maxHp });
    }
    if (def != 0) {
      modifiers.Add(new CharPropSchema.Modifier(schema => schema.def) { type = ModifierType.Add, value = def });
    }
    if (atk != 0) {
      modifiers.Add(new CharPropSchema.Modifier(schema => schema.atk) { type = ModifierType.Add, value = atk });
    }
    if (speed != 0) {
      modifiers.Add(new CharPropSchema.Modifier(schema => schema.speed) { type = ModifierType.Add, value = speed });
    }
    if (maxJourney != 0) {
      modifiers.Add(new CharPropSchema.Modifier(schema => schema.maxJourney) { type = ModifierType.Add, value = maxJourney });
    }
  }
}

public class EquipmentSlot {
  public GearSlot slotType { get; private set; }
  public string displayName { get; private set; }
  public GearObject equippedGear { get; private set; } // 可以为 null

  public EquipmentSlot(GearSlot slotType, string displayName) {
    this.slotType = slotType;
    this.displayName = displayName;
    this.equippedGear = null;
  }

  public bool CanEquip(GearObject gear) {
    return (gear.gearModel.availSlotFlag & slotType) != 0;
  }

  public bool EquipGear(GearObject gear) {
    if (!CanEquip(gear)) {
      return false;
    }
    equippedGear = gear;
    return true;
  }

  public GearObject UnequipGear() {
    var previousGear = equippedGear;
    equippedGear = null;
    return previousGear;
  }

  public bool HasEquipment => equippedGear != null;
}

public class RoguePlayerData {
  private static RoguePlayerData _instance;
  public static RoguePlayerData Instance {
    get {
      if (_instance == null) {
        _instance = new RoguePlayerData();
      }
      return _instance;
    }
  }

  public CharProp baseProp;
  public CharProp finalProp_statsOnly;

  public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
  public int hp;
  public int money = 99;

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
    UpdateStatsProp();

    hp = baseProp.maxHp;
  }

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

  // 静态方法重置单例（主要用于测试或重新开始游戏）
  public static void ResetInstance() {
    _instance = null;
  }
}