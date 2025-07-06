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
      tokens.Remove(card);
    }

    mana -= card.cost;

    var cardAction = card.cardModel.action;
    if (cardAction != null) {
      cardAction.Invoke(this, (action) => {
        action.invoker = playerCharObj;
        ExecuteAction(action);
      });
    }

    if (card.cardModel.modelId == nameof(CardDefine.Swap)) {
      UseCard_SWAP();
    }

    if (card.tags.Contains(CardTag.FRAGILE)) {
      return;
    }
    yard.Add(card);
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
