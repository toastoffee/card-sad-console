using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardConsole.Visual;

public partial class RogueBattleState : GameState {
	public static RogueBattleState instance;

	public CharObject playerCharObj = new() {
		name = "Player",
		hp = 70,
		maxHp = 70,
	};

	public List<CharObject> enemys = new();

	public int roundIdx = 0;
	public int mana = 0;
	public Action<CardSelectInput> selectCardHandler;
	public object selectCardCtx;

	// 添加子状态引擎
	private LiteStateEngine subStateEngine;
	private IdleState idleState;
	private CardSelectingState cardSelectingState;

	public List<CardObjcet> yard = new();
	public List<CardObjcet> deck = new();
	public List<CardObjcet> tokens = new();

	private ViewModel _viewModel;

	public struct CardSelectInput {
		public bool isBreak;
		public CardObjcet card;
	}

	public RogueBattleState() {
		instance = this;
		_viewModel = CardGame.instance.viewModel;

		// 初始化子状态
		idleState = new IdleState(this);
		cardSelectingState = new CardSelectingState(this);

		// 创建子状态引擎
		subStateEngine = new LiteStateEngine(new List<LiteState> {
			idleState,
			cardSelectingState
		});
	}

	public void SyncToViewModel() {
		if (_viewModel == null) return;

		_viewModel.turn = roundIdx;
		_viewModel.eng = mana;
		_viewModel.maxEng = 3;
		_viewModel.playerHp = playerCharObj.hp;
		_viewModel.maxPlayerHp = playerCharObj.maxHp;
		_viewModel.playerShield = playerCharObj.shield; // 同步护盾值

		_viewModel.enemies.Clear();
		foreach (var enemy in enemys) {
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
	}

	public override void OnEnter() {
		SetupEnemys();

		AddOriginalToDeck("Strike", 4);
		AddOriginalToDeck("Defend", 5);
		AddOriginalToDeck("Swap", 1);
		AddOriginalToDeck("Hit", 1);

		deck.Shuffle();
		OnRoundStart();

		SyncToViewModel();
	}

	private void SetupEnemys() {
		var enemy = new CharObject {
			name = "Treeman",
			hp = 27,
			maxHp = 27,
		};
		enemy.enemyAction = new TreemanAction(enemy);
		enemys.Add(enemy);
	}

	public override void OnTick() {
		// 委托给子状态处理
		var currentState = subStateEngine.frontState;
		if (currentState is IdleState idle) {
			idle.OnTick();
		} else if (currentState is CardSelectingState cardSelecting) {
			cardSelecting.OnTick();
		}

		SyncToViewModel();
	}

	public void GotoIdle() {
		subStateEngine.ReplaceTop<IdleState>();
	}

	public void GotoCardSelect() {
		subStateEngine.ReplaceTop<CardSelectingState>();
	}

	private void AddOriginalToDeck(string id, int cnt) {
		for (int i = 0; i < cnt; i++) {
			var card = new CardObjcet();

			card.LoadFromModel(AutoModelTable<CardModel>.Read(id));
			deck.Add(card);
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
	private void DisacrdAllTokens() {
		var cnt = tokens.Count;
		if (cnt == 0) {
			return;
		}
		Log.Push($"discard {cnt} remain hands.");
		while (tokens.Count > 0) {
			DiscardCard(tokens[0]);
		}
	}

	public void EndAndPushRound() {
		Log.Push($"Turn[{roundIdx}]end.");
		DisacrdAllTokens();

		ExecuteAllEnemyActions();

		roundIdx++;
		Log.Push($"Turn[{roundIdx}]start.");

		OnRoundStart();

		SyncToViewModel();
	}

	private void OnRoundStart() {
		playerCharObj.shield = 0;
		mana = 3;
		DrawFromDeck(4);

		SyncToViewModel();
	}
}

public static class Extension {
	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		var rng = new System.Random();

		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}