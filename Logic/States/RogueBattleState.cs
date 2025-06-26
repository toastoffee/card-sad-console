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
	public GameStateEnum state;
	public Action<CardSelectInput> selectCardHandler;
	public object selectCardCtx;

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
	}

	public void SyncToViewModel()
	{
		if (_viewModel == null) return;

		_viewModel.turn = roundIdx;
		_viewModel.eng = mana;
		_viewModel.maxEng = 3;

		_viewModel.playerHp = playerCharObj.hp;
		_viewModel.maxPlayerHp = playerCharObj.maxHp;

		_viewModel.enemies.Clear();
		foreach (var enemy in enemys)
		{
			_viewModel.enemies.Add(new EnemyViewModel
			{
				name = enemy.name,
				hp = enemy.hp,
				maxHp = enemy.maxHp,
				intention = enemy.enemyAction?.ForeshowAction() ?? "unknown"
			});
		}

		_viewModel.handCards.Clear();
		foreach (var card in tokens)
		{
			_viewModel.handCards.Add(new CardViewModel
			{
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
		if (state == GameStateEnum.IDLE) {
			OnIdleTick();
		} else if (state == GameStateEnum.CARD_SELECTING) {
			OnCardSelectingTick();
		}

		SyncToViewModel();
	}

	private void OnIdleTick() {
		int idx = -1;
		try {
			idx = Convert.ToInt32(CardGame.instance.lastInputChar?.ToString()) - 1;
		} catch { }
		if (idx >= 0 && idx < tokens.Count) {
			PreUseCard(idx);
		}
		if (CardGame.instance.lastInputChar == 'e') {
			EndAndPushRound();
		}
	}

	private void OnCardSelectingTick() {
		int idx = -1;
		try {
			idx = Convert.ToInt32(CardGame.instance.lastInputChar?.ToString()) - 1;
		} catch { }
		if (idx >= 0 && idx < tokens.Count) {
			var card = tokens[idx];
			var input = new CardSelectInput {
				isBreak = false,
				card = card,
			};
			selectCardHandler?.Invoke(input);
		}
		if (CardGame.instance.lastInputChar == 'e') {
			var input = new CardSelectInput {
				isBreak = true,
				card = null!,
			};
			selectCardHandler?.Invoke(input);
			GotoIdle();
		}
	}

	public void GotoIdle() {
		if (state == GameStateEnum.IDLE) {
			return;
		}
		state = GameStateEnum.IDLE;
		selectCardHandler = null;
		CardGame.instance.lastInputChar = null;
	}

	public void GotoCardSelect() {
		if (state == GameStateEnum.CARD_SELECTING) {
			return;
		}
		state = GameStateEnum.CARD_SELECTING;
		CardGame.instance.lastInputChar = null;
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

	private void PickFromDeck(CardObjcet card) {
		if (!deck.Contains(card)) {
			Log.Push($"[ERROR] 卡组中不包含 {card.name}");
			return;
		}
		Log.Push($"从卡组中抽取了 [{card.name}]");
		deck.Remove(card);
		tokens.Add(card);
	}

	private void RefillDeckFromYard() {
		Log.Push("重新洗牌");
		deck = new List<CardObjcet>(yard);
		yard.Clear();
		deck.Shuffle();
	}

	private void DiscardCard(CardObjcet card) {
		tokens.Remove(card);
		yard.Add(card);
	}
	private void DisacrdAllTokens() {
		var cnt = tokens.Count;
		if (cnt == 0) {
			return;
		}
		Log.Push($"弃掉{cnt}张剩余手牌");
		while (tokens.Count > 0) {
			DiscardCard(tokens[0]);
		}
	}

	private void EndAndPushRound() {
		Log.Push($"第【{roundIdx}】回合结束");
		DisacrdAllTokens();

		ExecuteAllEnemyActions();

		roundIdx++;
		Log.Push($"第【{roundIdx}】回合开始");
		CardGame.instance.lastInputChar = null;

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