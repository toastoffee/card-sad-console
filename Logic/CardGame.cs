using CardConsole.Visual;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CardGame {
	public static CardGame instance;
	public LiteStateEngine stateEngine;
	public ViewModel viewModel = new();

	public CardGame() {
		instance = this;
	}

	public void Setup() {
		var playerData = RoguePlayerData.Instance;

		var battleState = new RogueBattleState();
		RogueBattleState.instance = battleState;
		RogueInitState initState = new RogueInitState();
		stateEngine = new LiteStateEngine(new List<LiteState> {
			initState,
			battleState,
		});
	}

	public void Tick() {
		var state = stateEngine.frontState as GameState;
		state.OnTick();

		// 添加日志同步
		SyncLogsToViewModel();
	}

	public void SyncLogsToViewModel() {
		if (viewModel == null) return;

		// 分别同步游戏日志和系统日志
		viewModel.gameLogs.Clear();
		viewModel.systemLogs.Clear();

		viewModel.gameLogs.AddRange(Log.gameLogs);
		viewModel.systemLogs.AddRange(Log.systemLogs);
	}
}

public struct InputValue {
	public int intValue;
}

public static class GameInput {
	public enum Type {
		CARD = 1,
		END_TURN = 2,
		ROUTE_STATE_SELECT = 3,
		SKIP_BATTLE = 4,	
		ENUM_COUNT,
	}
	public struct InputCache {
		public bool flag;
		public InputValue value;
	}
	public static int cardIdx;
	public static InputCache[] inputFlags = new InputCache[(int)Type.ENUM_COUNT];

	public static void Reset() {
		for (int i = 0; i < (int)Type.ENUM_COUNT; i++) {
			inputFlags[i].flag = false;
			inputFlags[i].value.intValue = 0;
		}
	}

	public static void Set(Type type) {
		inputFlags[(int)type].flag = true;
	}

	public static void Set(Type type, InputValue value) {
		inputFlags[(int)type].flag = true;
		inputFlags[(int)type].value = value;
	}

	public static bool Read(Type type) {
		return inputFlags[(int)type].flag;
	}

	public static bool Read(Type type, out InputValue value) {
		value = inputFlags[(int)type].value;
		return inputFlags[(int)type].flag;
	}
}