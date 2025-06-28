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
	public RogueBattleState.EnemyAction enemyAction;
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
	public int hp;
	public int maxHp;
	public int def;
	public int atk;
	public int speed;

	public PlayerProp Add(PlayerProp other) {
		var ret = new PlayerProp() {
			hp = this.hp + other.hp,
			maxHp = this.maxHp + other.maxHp,
			def = this.def + other.def,
			atk = this.atk + other.atk,
			speed = this.speed + other.speed,
		};
		return ret;
	}
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

	public List<GearObject> gears = new List<GearObject>();

	// 私有构造函数，防止外部直接创建实例
	private RoguePlayerData() {
		// 初始化基础属性
		baseProp = new PlayerProp {
			hp = 70,
			maxHp = 70,
			def = 0,
			atk = 0,
			speed = 0
		};
		UpdateGearProp();
	}

	// 更新装备属性
	public void UpdateGearProp() {
		gearProp = new PlayerProp(); // 重置装备属性

		foreach (var gear in gears) {
			gearProp = gearProp.Add(gear.prop);
		}
	}

	// 添加装备
	public void AddGear(GearObject gear) {
		gears.Add(gear);
		UpdateGearProp();
	}

	// 移除装备
	public void RemoveGear(GearObject gear) {
		if (gears.Remove(gear)) {
			UpdateGearProp();
		}
	}

	// 清空所有装备
	public void ClearGears() {
		gears.Clear();
		UpdateGearProp();
	}

	// 静态方法重置单例（主要用于测试或重新开始游戏）
	public static void ResetInstance() {
		_instance = null;
	}
}