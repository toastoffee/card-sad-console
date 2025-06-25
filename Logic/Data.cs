public class CardObjcet {
	public CardModel cardModel;
	public string name => cardModel.modelId;
	public int cost => cardModel.cost;
	public CardType type => cardModel.cardType;
	public List<CardTag> tags => cardModel.cardTags;

	public void LoadFromModel(CardModel model) {
		cardModel = model;
	}

	public void Render(int optionIdx) {
		//Console.WriteLine($"[{optionIdx}] 【{cardModel.cost}】费 【{cardModel.modelId}】 [{EnumTrans.Get(cardModel.cardType)}]  {cardModel.desc}");
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

	public void Render(int idx = -1) {
		//var prefix = idx >= 0 ? $"[{idx}]  " : "";
		//Console.WriteLine($"{prefix}角色: {name}, 生命值: {hp}/{maxHp} {RemainShieldStr()}");
		//if (enemyAction != null) {
		//	Console.WriteLine($"  - {enemyAction.ForeshowAction()}");
		//}
	}

	public string RemainShieldStr() {
		return shield > 0 ? $"护盾: {shield}" : "";
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