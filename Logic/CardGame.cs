using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardConsole.Visual;

class CardGame {
	public char? lastInputChar = null;
	public static CardGame instance;
	public LiteStateEngine stateEngine;
	public ViewModel viewModel = new();

	public CardGame() {
		instance = this;
	}

    public void Setup() {
		var battleState = new RogueBattleState();
		RogueBattleState.instance = battleState;
		stateEngine = new LiteStateEngine(new List<LiteState> {
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
