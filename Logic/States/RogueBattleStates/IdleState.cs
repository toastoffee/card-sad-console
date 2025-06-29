using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IdleState : LiteState {
	private RogueBattleState battleState;
	private BattleContext battleContext;

	public IdleState(RogueBattleState battleState) {
		this.battleState = battleState;
	}
	
	public void SetBattleContext(BattleContext context) {
		this.battleContext = context;
	}

	public override void OnEnter() {
		if (battleContext != null) {
			battleContext.selectCardHandler = null;
			battleContext.selectCardCtx = null;
		}
	}

	public void OnTick() {
		if (battleContext == null) return;
		
		if (GameInput.Read(GameInput.Type.CARD, out var input)) {
			int idx = input.intValue;
			if (idx >= 0 && idx < battleContext.tokens.Count) {
				battleContext.PreUseCard(idx);
			}
		}
		if (GameInput.Read(GameInput.Type.END_TURN)) {
			battleContext.EndAndPushRound();
		}
	}
}