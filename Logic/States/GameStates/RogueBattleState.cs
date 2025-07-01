using CardConsole.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RogueRouteState;

public partial class RogueBattleState : GameState {
	public static RogueBattleState instance;

	// 战斗上下文
	public BattleContext battleContext;

	// 添加子状态引擎
	private LiteStateEngine subStateEngine;
	private IdleState idleState;
	private CardSelectingState cardSelectingState;

	private CharObject mPresetEnemy;

	public struct CardSelectInput {
		public bool isBreak;
		public CardObjcet card;
	}

	public RogueBattleState(CharObject enemy = null) {
		instance = this;
        mPresetEnemy = enemy;

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
		battleContext = new BattleContext(mPresetEnemy);

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

		// 委托给子状态处理
		var currentState = subStateEngine.frontState;
		if (currentState is IdleState idle) {
			idle.OnTick();
		} else if (currentState is CardSelectingState cardSelecting) {
			cardSelecting.OnTick();
		}

		// 同步到ViewModel
		battleContext?.SyncToViewModel();

		// 判断是否所有敌人均死亡
		if (battleContext.IsAllEnemiesDead()) {
			Log.PushSys("[Battle] Complete Level.");
			PushRouteState();
		}

	}


	private void PushRouteState()
	{
        var weakTreeman = new CharObject
        {
            name = "Treeman",
            hp = 12,
            maxHp = 12,
        };
        weakTreeman.enemyAction = new RogueBattleState.TreemanAction(weakTreeman);

        var normalTreeman = new CharObject
        {
            name = "Treeman",
            hp = 27,
            maxHp = 27,
        };
        normalTreeman.enemyAction = new RogueBattleState.TreemanAction(normalTreeman);

        var strongTreeman = new CharObject
        {
            name = "Treeman",
            hp = 51,
            maxHp = 51,
        };
        strongTreeman.enemyAction = new RogueBattleState.TreemanAction(strongTreeman);

        CardGame.instance.stateEngine.ReplaceTop<RougeRouteTemplateState>(
            new RougeRouteTemplateState("you can select a battle and engage",
            new List<RogueRouteState.RouteOption>
            {
                    new RouteOption {
                        desc = "Battle weak treeman",
                        onSelect = () => {
                            stateEngine.ReplaceTop<RogueBattleState>(new RogueBattleState(weakTreeman));
                        }
                    },
                    new RouteOption {
                        desc = "Battle normal treeman",
                        onSelect = () => {
                            stateEngine.ReplaceTop<RogueBattleState>(new RogueBattleState(normalTreeman));
                        }
                    },
                    new RouteOption {
                        desc = "Battle strong treeman",
                        onSelect = () => {
                            stateEngine.ReplaceTop<RogueBattleState>(new RogueBattleState(strongTreeman));
                        }
                    },
            }));
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