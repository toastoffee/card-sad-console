public partial class BattleContext {

  #region 使用卡牌逻辑

  public void PreUseCard(int idx) {
    var card = tokens[idx];
    if (card.name == nameof(CardDefine.Swap)) {
      PreuseCard_SWAP(card);
      return;
    }

    UseCard(card);
  }

  private void UseCard(CardObjcet card) {
    if (card.cost > mana) {
      Log.Push("not enough energy.");
      return;
    }

    if (card != null) {
      Log.Push($"use hand：[{card.cardModel.modelId}]");
      tokens.Remove(card);
    }

    mana -= card.cost;

    if (card.cardModel.modelId == nameof(CardDefine.Strike)) {
      DoDamageToEnemy(0, 6);
    }

    if (card.cardModel.modelId == nameof(CardDefine.Defend)) {
      ApplyShieldToPlayer(5);
    }

    if (card.cardModel.modelId == nameof(CardDefine.Hit)) {
      DoDamageToEnemy(0, 15);
    }

    if (card.cardModel.modelId == nameof(CardDefine.Swap)) {
      UseCard_SWAP();
    }

    yard.Add(card);
  }

  private void DoDamageToEnemy(int idx, int dmg) {
    var enemy = enemies[idx];
    DoAttack(new AttackParam {
      attacker = playerCharObj,
      deffender = enemy,
      dmg = dmg,
    });
  }

  private void ApplyShieldToPlayer(int shield) {
    GainShield(playerCharObj, shield);
  }

  private void PreuseCard_SWAP(CardObjcet card) {
    RogueBattleState.instance.GotoCardSelect();
    Log.Push("chose a card to swap (Weapon or Armor.)");
    var ctx = new SWAP_Context();
    selectCardHandler = (input) => {
      var selectCard = input.card;
      if (input.isBreak) {
        RogueBattleState.instance.GotoIdle();
        return;
      }
      if (selectCard.type != CardType.WEAPON && selectCard.type != CardType.ARMOR) {
        Log.Push($"u can only select Weapon or Armor.");
        return;
      }
      ctx.type = selectCard.type;
      Log.Push($"chosen：[{selectCard.cardModel.modelId}]，discard all [{EnumTrans.Get(selectCard.type)}]");
      selectCardCtx = ctx;
      UseCard(card);
      RogueBattleState.instance.GotoIdle();
    };
    selectCardCtx = ctx;
  }

  private void UseCard_SWAP() {
    var ctx = selectCardCtx as SWAP_Context;
    if (ctx == null) {
      Log.Push("[ERROR] SWAP context is null!");
      return;
    }

    var drawType = ctx.type == CardType.WEAPON ? CardType.ARMOR : CardType.WEAPON;
    var discardCnt = 0;
    for (int i = 0; i < tokens.Count; i++) {
      if (tokens[i].type == ctx.type) {
        discardCnt++;
        DiscardCard(tokens[i]);
      }
    }
    for (int i = 0; i < discardCnt; i++) {
      var draw = deck.Find(x => x.type == drawType);
      if (draw != null) {
        PickFromDeck(draw);
      }
    }
    // 清理上下文
    selectCardCtx = null;
  }

  #endregion
}
