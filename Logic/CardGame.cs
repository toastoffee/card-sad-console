using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CardGame {
	public char? lastInputChar = null;
	public static CardGame instance;
	public LiteStateEngine stateEngine;

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
	}

	public void Render() {
		var state = stateEngine.frontState as GameState;
		state.Render();
	}
}
