using System;
using System.Collections.Generic;
using static GearModel;

public class CardModel {
	public string modelId;
	public int cost;
	public string desc;
	public CardType cardType;
	public List<CardTag> cardTags;
}
[AutoModelTable(typeof(CardModel))]
public static class CardDefine {
	public static CardModel Strike => new CardModel {
		modelId = nameof(Strike),
		cost = 1,
		desc = "cause 6 damage",
		cardType = CardType.WEAPON,
	};
	public static CardModel Swap => new CardModel {
		modelId = nameof(Swap),
		cost = 1,
		desc = "discard all weapons/armors, and draw other kind as same counts",
		cardType = CardType.TRINKET,
	};
	public static CardModel Defend => new CardModel {
		modelId = nameof(Defend),
		cost = 1,
		desc = "gain 5 defend",
		cardType = CardType.ARMOR,
	};
	public static CardModel Hit => new CardModel {
		modelId = nameof(Hit),
		cost = 2,
		desc = "cause 15 damage",
		cardType = CardType.WEAPON,
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
		});
		Impl<GameStateEnum>.Set(new List<KeyValuePair<GameStateEnum, string>> {
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.IDLE, "等待出牌"),
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.CARD_SELECTING, "选择一张手牌，按'e'取消，按'c'确认"),
			new KeyValuePair<GameStateEnum, string>(GameStateEnum.ENEMY_SELECTING, "选择一个敌人"),
		});
		Impl<CardTag>.Set(new List<KeyValuePair<CardTag, string>> {
			new KeyValuePair<CardTag, string>(CardTag.STICKY, "Sticky"),
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
	public PlayerProp baseProp;
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
				}
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
				}
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
					new CardRecord(nameof(CardDefine.Strike), 5),
				}
			};
			return ret;
		}
	}
}