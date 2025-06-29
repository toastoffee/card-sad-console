using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CardSelectingState : LiteState {
    private RogueBattleState battleState;
    private BattleContext battleContext;

    public CardSelectingState(RogueBattleState battleState) {
        this.battleState = battleState;
    }
    
    public void SetBattleContext(BattleContext context) {
        this.battleContext = context;
    }

    public void OnTick() {
        if (battleContext == null) return;
        
        if (GameInput.Read(GameInput.Type.CARD, out var value)) {
            int idx = value.intValue;
            if (idx >= 0 && idx < battleContext.tokens.Count) {
                var card = battleContext.tokens[idx];
                var input = new RogueBattleState.CardSelectInput {
                    isBreak = false,
                    card = card,
                };
                battleContext.selectCardHandler?.Invoke(input);
            }
        }

        if (GameInput.Read(GameInput.Type.END_TURN)) {
            var input = new RogueBattleState.CardSelectInput {
                isBreak = true,
                card = null,
            };
            battleContext.selectCardHandler?.Invoke(input);
            GotoIdle();
        }
    }

    private void GotoIdle() {
        stateEngine.ReplaceTop<IdleState>();
    }
}