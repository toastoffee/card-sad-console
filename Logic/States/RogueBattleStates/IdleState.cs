using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IdleState : LiteState {
	private RogueBattleState battleState;

	public IdleState(RogueBattleState battleState) {
		this.battleState = battleState;
	}

	public override void OnEnter() {
		battleState.selectCardHandler = null!;
		battleState.selectCardCtx = null!;
	}

	public void OnTick() {
		if (GameInput.Read(GameInput.Type.CARD, out var input)) {
			int idx = input.intValue;
			if (idx >= 0 && idx < battleState.tokens.Count) {
				battleState.PreUseCard(idx);
			}
		}
		if (GameInput.Read(GameInput.Type.END_TURN)) {
			battleState.EndAndPushRound();
		}
	}
}