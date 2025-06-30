using System;
using System.Collections.Generic;
using System.Linq;
using CardConsole.Visual;

public partial class BattleContext {
	// ս����������
	public CharObject playerCharObj = new() {
		name = "Player",
		hp = 70,
		maxHp = 70,
	};

	public List<CharObject> enemies = new();
	public int roundIdx = 0;
	public int mana = 0;

	// ����ϵͳ
	public List<CardObjcet> yard = new();
	public List<CardObjcet> deck = new();
	public List<CardObjcet> tokens = new();

	// ����ѡ��״̬
	public Action<RogueBattleState.CardSelectInput> selectCardHandler;
	public object selectCardCtx;

	private ViewModel _viewModel;

	// �� AttackParam ����Ǩ�Ƶ� BattleContext ��
	public struct AttackParam {
		public CharObject attacker;
		public CharObject deffender;
		public int dmg;
	}

	// �� SWAP_Context ����Ǩ�Ƶ� BattleContext ��
	private class SWAP_Context {
		public CardType type;
	}

	public BattleContext() {
		_viewModel = CardGame.instance.viewModel;
		Initialize();
	}

	private void Initialize() {
		// ��������״̬
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

		// ���õ���
		SetupEnemys();

		// ��װ���г�ʼ���ƿ�
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

		// ��������װ����λ
		foreach (var slot in playerData.equipmentSlots) {
			if (slot.HasEquipment) {
				var gear = slot.equippedGear;

				// ��װ���Ŀ��Ƽ�¼����ӿ��Ƶ��ƿ�
				foreach (var cardRecord in gear.cards) {
					AddOriginalToDeck(cardRecord.card.modelId, cardRecord.cnt);
				}
			}
		}

		// ����ƿ�Ϊ�գ����һЩ���������Է���Ϸ�޷�����
		if (deck.Count == 0) {
			Log.Push("No cards from equipment, adding basic cards.");
			AddOriginalToDeck("Strike", 2);
			AddOriginalToDeck("Defend", 2);
		}
	}

	#region ���ƹ�����

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

	#region ս���߼�

	public void DoAttack(AttackParam param) {
		if (param.deffender == playerCharObj) {
			Trigger_OnBeforePlayerBeAttack(param);
		}

		var shieldDmg = (int)MathF.Min(param.dmg, param.deffender.shield);
		param.deffender.shield -= shieldDmg;
		param.deffender.hp -= param.dmg - shieldDmg;
		Log.Push($"[{param.attacker.name}]deal [{param.dmg}] dmg to [{param.deffender.name}]��remain hp {param.deffender.hp} shield {param.deffender.shield}");

		if (param.deffender == playerCharObj) {
			Trigger_OnAfterPlayerBeAttack(param);
		}
	}

	private void GainShield(CharObject cha, int shield) {
		cha.shield += shield;
		Log.Push($"[{cha.name}] gain [{shield}] shield��current: {cha.shield}");
	}

	private void Trigger_OnBeforePlayerBeAttack(AttackParam param) {
		// Ԥ���Ĵ���������
	}

	private void Trigger_OnAfterPlayerBeAttack(AttackParam param) {
		// Ԥ���Ĵ���������
	}

	public void ExecuteAllEnemyActions() {
		foreach (var enemy in enemies) {
			enemy.enemyAction.ExecuteAction();
		}
	}

	#endregion

	#region �غϹ���

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

	#region ViewModel ͬ��

	public void SyncToViewModel() {
		if (_viewModel == null) return;

		_viewModel.currentStateType = GameStateType.Battle;
		_viewModel.turn = roundIdx;
		_viewModel.eng = mana;
		_viewModel.maxEng = 3;
		_viewModel.playerHp = playerCharObj.hp;
		_viewModel.maxPlayerHp = playerCharObj.maxHp;
		_viewModel.playerShield = playerCharObj.shield;

		// ͬ�����ƶ�����
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

		//��д��Ϣ��壬��ս���ڵĵ�ǰ������ʾ
		_viewModel.playerProp = playerCharObj.playerProp;
	}

	#endregion
}