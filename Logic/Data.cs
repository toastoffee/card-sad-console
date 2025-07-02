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

public class CharObject {
	public string name;
	public int hp;
	public int maxHp;
	public int shield;
	public int def;
	public int atk;
	public int speed;
	public int journey;
	public int maxJourney;
	public RogueBattleState.EnemyAction enemyAction;

	public PlayerProp playerProp => new PlayerProp {
		maxHp = maxHp,
		def = def,
		atk = atk,
		speed = speed,
		maxJourney = maxJourney,
	};

	public void LoadFromPlayerProp(int hp, PlayerProp prop) {
		this.hp = hp;
		maxHp = prop.maxHp;
		def = prop.def;
		atk = prop.atk;
		speed = prop.speed;
		maxJourney = prop.maxJourney;
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
}

public enum CardTag {
	STICKY,
}

public class GearObject {
	public GearModel gearModel;
	public string name => gearModel.modelId.Replace('_', ' ');
	public List<GearModel.CardRecord> cards => gearModel.cards;
	public PlayerProp prop => gearModel.baseProp;

	public GearObject(string modelId) {
		gearModel = AutoModelTable<GearModel>.Read(modelId);
	}
}

public struct PlayerProp {
	public int maxHp;
	public int def;
	public int atk;
	public int speed;
	public int maxJourney;

	public PlayerProp Add(PlayerProp other) {
		var ret = new PlayerProp() {
			maxHp = this.maxHp + other.maxHp,
			def = this.def + other.def,
			atk = this.atk + other.atk,
			speed = this.speed + other.speed,
			maxJourney = this.maxJourney + other.maxJourney
		};
		return ret;
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

	public PlayerProp baseProp;
	public PlayerProp gearProp;
	public PlayerProp totalProp {
		get {
			return baseProp.Add(gearProp);
		}
	}

	public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
	public int hp;
	public int money = 99;

	private RoguePlayerData() {
		// 初始化基础属性
		baseProp = new PlayerProp {
			maxHp = 70,
			def = 0,
			atk = 0,
			speed = 0
		};
		// 初始化所有装备槽位
		InitializeEquipmentSlots();
		UpdateGearProp();

		hp = totalProp.maxHp;
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

	// 更新装备属性
	public void UpdateGearProp() {
		gearProp = new PlayerProp(); // 重置装备属性

		foreach (var slot in equipmentSlots) {
			if (slot.HasEquipment) {
				gearProp = gearProp.Add(slot.equippedGear.prop);
			}
		}
	}

	// 装备管理方法
	public bool EquipGear(GearObject gear) {
		// 找到第一个兼容的空槽位
		var compatibleSlot = equipmentSlots.FirstOrDefault(slot =>
			!slot.HasEquipment && slot.CanEquip(gear));

		if (compatibleSlot != null) {
			compatibleSlot.EquipGear(gear);
			UpdateGearProp();
			return true;
		}
		return false;
	}

	public bool EquipGearToSlot(GearSlot slotType, GearObject gear) {
		var slot = equipmentSlots.FirstOrDefault(s => s.slotType == slotType);
		if (slot != null && slot.CanEquip(gear)) {
			slot.UnequipGear(); // 先卸下旧装备
			slot.EquipGear(gear);
			UpdateGearProp();
			return true;
		}
		return false;
	}

	public GearObject UnequipGear(GearSlot slotType) {
		var slot = equipmentSlots.FirstOrDefault(s => s.slotType == slotType);
		if (slot != null && slot.HasEquipment) {
			var gear = slot.UnequipGear();
			UpdateGearProp();
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
		UpdateGearProp();
	}

	// 静态方法重置单例（主要用于测试或重新开始游戏）
	public static void ResetInstance() {
		_instance = null;
	}
}