using System;
using System.Collections.Generic;
using System.Linq;
using CardConsole.Visual;

public partial class BattleContext {
	// 战斗核心数据
	public CharObject playerCharObj = new() {
		name = "Player",
		hp = 70,
		maxHp = 70,
	};

	public List<CharObject> enemies = new();
	public int roundIdx = 0;
	public int mana = 0;

	// 卡牌系统
	public List<CardObjcet> yard = new();
	public List<CardObjcet> deck = new();
	public List<CardObjcet> tokens = new();

	// 卡牌选择状态
	public Action<RogueBattleState.CardSelectInput> selectCardHandler;
	public object selectCardCtx;

	private ViewModel _viewModel;

	// 将 AttackParam 定义迁移到 BattleContext 中
	public struct AttackParam {
		public CharObject attacker;
		public CharObject deffender;
		public int dmg;
	}

	// 将 SWAP_Context 定义迁移到 BattleContext 中
	private class SWAP_Context {
		public CardType type;
	}

	public BattleContext() {
		_viewModel = CardGame.instance.viewModel;
		Initialize();
	}

	private void Initialize() {
		// 重置所有状态
		playerCharObj = new CharObject {
			name = "Player",
		};
		playerCharObj.LoadFromPlayerProp(RoguePlayerData.Instance.totalProp);

		enemies.Clear();
		roundIdx = 0;
		mana = 0;

		yard.Clear();
		deck.Clear();
		tokens.Clear();

		selectCardHandler = null;
		selectCardCtx = null;

		// 设置敌人
		SetupEnemys();

		// 从装备中初始化牌库
		InitializeDeckFromEquipment();

		deck.Shuffle();
		OnRoundStart();
	}

	public bool IsAllEnemiesDead() {
		bool ret = true;
		foreach (var enemy in enemies) {
			ret &= enemy.hp <= 0;
		}
		return ret;
	}

	private void SetupEnemys() {
		var enemy = new CharObject {
			name = "Treeman",
			hp = 27,
			maxHp = 27,
		};
		enemy.enemyAction = new RogueBattleState.TreemanAction(enemy);
		enemies.Add(enemy);
	}

	private void InitializeDeckFromEquipment() {
		var playerData = RoguePlayerData.Instance;

		// 遍历所有装备槽位
		foreach (var slot in playerData.equipmentSlots) {
			if (slot.HasEquipment) {
				var gear = slot.equippedGear;

				// 从装备的卡牌记录中添加卡牌到牌库
				foreach (var cardRecord in gear.cards) {
					AddOriginalToDeck(cardRecord.card.modelId, cardRecord.cnt);
				}
			}
		}

		// 如果牌库为空，添加一些基础卡牌以防游戏无法进行
		if (deck.Count == 0) {
			Log.Push("No cards from equipment, adding basic cards.");
			AddOriginalToDeck("Strike", 2);
			AddOriginalToDeck("Defend", 2);
		}
	}

	#region 卡牌管理方法

	private void AddOriginalToDeck(string id, int cnt) {
		for (int i = 0; i < cnt; i++) {
			var card = new CardObjcet();
			card.LoadFromModel(AutoModelTable<CardModel>.Read(id));
			deck.Add(card);
		}
	}

	public void AddJourney(int cnt) {
		while (cnt > 0) {
			var add = (int)MathF.Min(playerCharObj.maxJourney - playerCharObj.journey, cnt);
			playerCharObj.journey += add;
			cnt -= add;
			if (playerCharObj.journey == playerCharObj.maxJourney) {
				playerCharObj.journey = 0;
				DrawFromDeck(1);
			}
		}
	}

	private void DrawFromDeck(int cnt) {
		for (int i = 0; i < cnt; i++) {
			if (deck.Count <= 0) {
				RefillDeckFromYard();
				i--;
				continue;
			} else {
				var card = deck[0];
				deck.RemoveAt(0);
				tokens.Add(card);
			}
		}
	}

	public void PickFromDeck(CardObjcet card) {
		if (!deck.Contains(card)) {
			Log.Push($"[ERROR] deck not contain {card.name}");
			return;
		}
		Log.Push($"draw from deck [{card.name}]");
		deck.Remove(card);
		tokens.Add(card);
	}

	private void RefillDeckFromYard() {
		Log.Push("refill deck from yard.");
		deck = new List<CardObjcet>(yard);
		yard.Clear();
		deck.Shuffle();
	}

	public void DiscardCard(CardObjcet card) {
		tokens.Remove(card);
		yard.Add(card);
	}

	private void DiscardAllTokens() {
		var cnt = tokens.Count;
		if (cnt == 0) {
			return;
		}
		Log.Push($"discard {cnt} remain hands.");
		while (tokens.Count > 0) {
			DiscardCard(tokens[0]);
		}
	}

	#endregion

	#region 战斗逻辑

	public void DoAttack(AttackParam param) {
		if (param.deffender == playerCharObj) {
			Trigger_OnBeforePlayerBeAttack(param);
		}

		var shieldDmg = (int)MathF.Min(param.dmg, param.deffender.shield);
		param.deffender.shield -= shieldDmg;
		param.deffender.hp -= param.dmg - shieldDmg;
		Log.Push($"[{param.attacker.name}]deal [{param.dmg}] dmg to [{param.deffender.name}]，remain hp {param.deffender.hp} shield {param.deffender.shield}");

		if (param.deffender == playerCharObj) {
			Trigger_OnAfterPlayerBeAttack(param);
		}
	}

	private void GainShield(CharObject cha, int shield) {
		cha.shield += shield;
		Log.Push($"[{cha.name}] gain [{shield}] shield，current: {cha.shield}");
	}

	private void Trigger_OnBeforePlayerBeAttack(AttackParam param) {
		// 预留的触发器方法
	}

	private void Trigger_OnAfterPlayerBeAttack(AttackParam param) {
		// 预留的触发器方法
	}

	public void ExecuteAllEnemyActions() {
		foreach (var enemy in enemies) {
			enemy.enemyAction.ExecuteAction();
		}
	}

	#endregion

	#region 回合管理

	public void EndAndPushRound() {
		Log.Push($"Turn[{roundIdx}]end.");
		DiscardAllTokens();

		ExecuteAllEnemyActions();

		roundIdx++;
		Log.Push($"Turn[{roundIdx}]start.");

		OnRoundStart();
	}

	private void OnRoundStart() {
		playerCharObj.shield = 0;
		mana = 3;
		AddJourney(playerCharObj.speed);
	}

	#endregion

	#region ViewModel 同步

	public void SyncToViewModel() {
		if (_viewModel == null) return;

		_viewModel.currentStateType = GameStateType.Battle;
		_viewModel.turn = roundIdx;
		_viewModel.eng = mana;
		_viewModel.maxEng = 3;
		_viewModel.playerHp = playerCharObj.hp;
		_viewModel.maxPlayerHp = playerCharObj.maxHp;
		_viewModel.playerShield = playerCharObj.shield;

		// 同步卡牌堆数据
		_viewModel.deckCount = deck.Count;
		_viewModel.discardCount = yard.Count;

		_viewModel.enemies.Clear();
		foreach (var enemy in enemies) {
			_viewModel.enemies.Add(new EnemyViewModel {
				name = enemy.name,
				hp = enemy.hp,
				maxHp = enemy.maxHp,
				intention = enemy.enemyAction?.ForeshowAction() ?? "unknown"
			});
		}

		_viewModel.handCards.Clear();
		foreach (var card in tokens) {
			_viewModel.handCards.Add(new CardViewModel {
				name = card.name,
				cost = card.cost,
				description = card.cardModel.desc
			});
		}

		_viewModel.journey = playerCharObj.journey;
		_viewModel.maxJourney = playerCharObj.maxJourney;

		//覆写信息面板，用战斗内的当前属性显示
		_viewModel.playerProp = playerCharObj.playerProp;
	}

	#endregion
}