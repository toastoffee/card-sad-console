using static RogueRouteState;

public partial class RogueBattleState : GameState {
	public static RogueBattleState instance;

	// 战斗上下文
	public BattleContext battleContext;

	// 添加子状态引擎
	private LiteStateEngine subStateEngine;
	private IdleState idleState;
	private CardSelectingState cardSelectingState;

	public struct CardSelectInput {
		public bool isBreak;
		public CardObjcet card;
	}

	public RogueBattleState() {
		instance = this;

		// 初始化子状态
		idleState = new IdleState(this);
		cardSelectingState = new CardSelectingState(this);

		// 创建子状态引擎
		subStateEngine = new LiteStateEngine(new List<LiteState> {
			idleState,
			cardSelectingState
		});
	}

	public override void OnEnter() {
		// 创建新的战斗上下文
		battleContext = new BattleContext();

		// 为子状态提供战斗上下文的引用
		idleState.SetBattleContext(battleContext);
		cardSelectingState.SetBattleContext(battleContext);

		battleContext.SyncToViewModel();
	}

	public override void OnTick() {
		if (GameInput.Read(GameInput.Type.SKIP_BATTLE)) {
			Log.PushSys("Skip battle.");
			CardGame.instance.stateEngine.ReplaceTop<RogueInitState>();
			return;
		}

		var currentState = subStateEngine.frontState;
		if (currentState is IdleState idle) {
			idle.OnTick();
		} else if (currentState is CardSelectingState cardSelecting) {
			cardSelecting.OnTick();
		}

		// 同步到ViewModel
		battleContext?.SyncToViewModel();

		// 判断是否所有敌人均死亡
		if(battleContext.playerCharObj.hp <= 0) {
			Log.PushSys("[Battle] Player is dead. Game Over.");
			CardGame.instance.stateEngine.ReplaceTop<RogueInitState>();
			return;
		}
		if (battleContext.IsAllEnemiesDead()) {
			FinishBattle();
		}
	}

	private void FinishBattle() {
		Log.PushSys("[Battle] Complete Level.");
		RoguePlayerData.Instance.hp = battleContext.playerCharObj.hp;

		stateEngine.ReplaceTop<RogueBattleFinishState>();
	}

	public void GotoIdle() {
		subStateEngine.ReplaceTop<IdleState>();
	}

	public void GotoCardSelect() {
		subStateEngine.ReplaceTop<CardSelectingState>();
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