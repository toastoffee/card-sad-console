using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardSelectingState : LiteState {
    private RogueBattleState battleState;

    public CardSelectingState(RogueBattleState battleState) {
        this.battleState = battleState;
    }

    public void OnTick() {
        if (GameInput.Read(GameInput.Type.CARD, out var value)) {
            int idx = value.intValue;
            if (idx >= 0 && idx < battleState.tokens.Count) {
                var card = battleState.tokens[idx];
                var input = new RogueBattleState.CardSelectInput {
                    isBreak = false,
                    card = card,
                };
                battleState.selectCardHandler?.Invoke(input);
            }
        }

        if (GameInput.Read(GameInput.Type.END_TURN)) {
            var input = new RogueBattleState.CardSelectInput {
                isBreak = true,
                card = null!,
            };
            battleState.selectCardHandler?.Invoke(input);
            GotoIdle();
        }
    }

    private void GotoIdle() {
        stateEngine.ReplaceTop<IdleState>();
    }
}