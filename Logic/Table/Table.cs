using System.Collections.Generic;
using System;

public class CardModel {
	public string modelId;
	public int cost;
	public string desc;
	public CardType cardType;
	public List<CardTag> cardTags;
}
[AutoModelTable(typeof(CardModel))]
public static class CardDefine {
	public static CardModel 打击 => new CardModel {
		modelId = nameof(打击),
		cost = 1,
		desc = "cause 6 damage",
		cardType = CardType.WEAPON,
	};
	public static CardModel SWAP => new CardModel {
		modelId = nameof(SWAP),
		cost = 1,
		desc = "discard all weapons/armors, and draw other kind as same counts",
		cardType = CardType.TRINKET,
	};
	public static CardModel 防御 => new CardModel {
		modelId = nameof(防御),
		cost = 1,
		desc = "gain 5 defend",
		cardType = CardType.ARMOR,
	};
	public static CardModel 重击 => new CardModel {
		modelId = nameof(重击),
		cost = 2,
		desc = "cause 15 damage",
		cardType = CardType.WEAPON,
	};
}

public static class EnumTrans {
	static EnumTrans() {
		Impl<CardType>.Set(new List<KeyValuePair<CardType, string>> {
			new KeyValuePair<CardType, string>(CardType.WEAPON, "武器"),
			new KeyValuePair<CardType, string>(CardType.ARMOR, "防具"),
			new KeyValuePair<CardType, string>(CardType.HELMET, "头盔"),
			new KeyValuePair<CardType, string>(CardType.SHOE, "鞋子"),
			new KeyValuePair<CardType, string>(CardType.TRINKET, "饰品"),
		});
		Impl<GameStateEnum>.Set(new List<KeyValuePair<GameStateEnum, string>> {
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.IDLE, "等待出牌"),
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.CARD_SELECTING, "选择一张手牌，按'e'取消，按'c'确认"),
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.ENEMY_SELECTING, "选择一个敌人"),
		});
		Impl<CardTag>.Set(new List<KeyValuePair<CardTag, string>> {
			new KeyValuePair<CardTag, string>(CardTag.STICKY, "粘性"),
		});
	}

	public static class Impl<T> {
		public static Dictionary<T, string> dict = new Dictionary<T, string>();
		public static void Set(List<KeyValuePair<T, string>> list) {
			foreach (var pair in list) {
				dict[pair.Key] = pair.Value;
			}
		}
	}
	public static string Get<T>(T value) {
		return Impl<T>.dict?[value] ?? value.ToString();
	}
}

[Flags]
public enum WeaponSlot {
	NONE = 0,
	MAIN_WEAPON = 1 >> 0,
	SIDE_WEAPON = 1 >> 1,
	ARMOR = 1 >> 2,
	HELMET = 1 >> 3,
	SHOE = 1 >> 4,
}

public class WeaponModel {
	public string modelId;
	public WeaponSlot availSlotFlag;
	public List<CardModel> cards;
}

[AutoModelTable(typeof(WeaponModel))]
public static class WeaponDefine {
	public static WeaponModel 木剑 => new WeaponModel() {
		modelId = nameof(木剑),
		availSlotFlag = WeaponSlot.MAIN_WEAPON,
		cards = new List<CardModel>()
	};
}