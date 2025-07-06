using CardConsole.Visual;


public partial class BattleContext {
  // 战斗核心数据
  public CharObject playerCharObj = new() {
    name = "Player",
  };

  public List<CharObject> enemies = new();
  public int roundIdx = 0;
  public int mana = 0;

  // 卡牌系统
  public List<CardObjcet> yard = new();
  public List<CardObjcet> deck = new();
  public List<CardObjcet> tokens = new();

  // 卡牌选择状态
  public Action<RogueBattleState.CardSelectInput> selectCardHandler;
  public object selectCardCtx;

  private ViewModel _viewModel;

  // 将 AttackParam 定义迁移到 BattleContext 中
  public struct AttackParam {
    public CharObject attacker;
    public CharObject deffender;
    public int dmg;
  }

  // 将 SWAP_Context 定义迁移到 BattleContext 中
  private class SWAP_Context {
    public CardType type;
  }

  public BattleContext(CharObject enemy = null) {
    _viewModel = CardGame.instance.viewModel;

    Setup();

    enemy = enemy ?? GetDefaultEnemys();
    enemies.Add(enemy);

    RefreshProps();
    StartBattle();
    OnRoundStart();
  }

  private void Setup() {
    // 重置所有状态
    playerCharObj = new CharObject {
      name = "Player",
    };
    Log.PushSys(RoguePlayerData.Instance.hp.ToString());
    playerCharObj.LoadFromPlayerProp(RoguePlayerData.Instance.hp, RoguePlayerData.Instance.baseProp);

    enemies.Clear();
    roundIdx = 0;
    mana = 0;

    yard.Clear();
    deck.Clear();
    tokens.Clear();

    selectCardHandler = null;
    selectCardCtx = null;
  }

  private void StartBattle() {
    // 从装备中初始化牌库
    InitializeDeckFromEquipment();
    deck.Shuffle();

    DrawConsistCardsFromDeck();
  }

  public bool IsAllEnemiesDead() {
    bool ret = true;
    foreach (var enemy in enemies) {
      ret &= enemy.hp <= 0;
    }
    return ret;
  }

  private CharObject GetDefaultEnemys() {
    var enemy = new CharObject {
      name = "Treeman",
      hp = 27,
    };
    enemy.LoadFromPlayerProp(27, new CharProp {
      maxHp = 27,
    });
    enemy.enemyAction = new RogueBattleState.TreemanAction(enemy);
    return enemy;
  }

  private void InitializeDeckFromEquipment() {
    var playerData = RoguePlayerData.Instance;

    // 遍历所有装备槽位
    foreach (var slot in playerData.equipmentSlots) {
      if (slot.HasEquipment) {
        var gear = slot.equippedGear;

        // 从装备的卡牌记录中添加卡牌到牌库
        foreach (var cardRecord in gear.cards) {
          AddOriginalToDeck(cardRecord.card.modelId, cardRecord.cnt);
        }
      }
    }

    // 如果牌库为空，添加一些基础卡牌以防游戏无法进行
    if (deck.Count == 0) {
      Log.Push("No cards from equipment, adding basic cards.");
      AddOriginalToDeck("Strike", 2);
      AddOriginalToDeck("Defend", 2);
    }
  }

  private void DrawConsistCardsFromDeck() {
    for (int i = 0; i < deck.Count; i++) {
      var card = deck[i];
      if (card.tags.Contains(CardTag.CONSIST)) {
        PickFromDeck(card);
        i--;
      }
    }
  }

  #region 卡牌管理方法

  private void AddOriginalToDeck(string id, int cnt) {
    for (int i = 0; i < cnt; i++) {
      var card = new CardObjcet();
      card.LoadFromModel(AutoModelTable<CardModel>.Read(id));
      deck.Add(card);
    }
  }

  public void AddJourney(int cnt) {
    while (cnt > 0) {
      var add = (int)MathF.Min(playerCharObj.maxJourney - playerCharObj.journey, cnt);
      playerCharObj.journey += add;
      cnt -= add;
      if (playerCharObj.journey == playerCharObj.maxJourney) {
        playerCharObj.journey = 0;
        DrawFromDeck(1);
      }
    }
  }

  private void DrawFromDeck(int cnt) {
    for (int i = 0; i < cnt; i++) {
      if (deck.Count <= 0) {
        RefillDeckFromYard();
        i--;
        continue;
      } else {
        var card = deck[0];
        deck.RemoveAt(0);
        tokens.Add(card);
      }
    }
  }

  public void PickFromDeck(CardObjcet card) {
    if (!deck.Contains(card)) {
      Log.Push($"[ERROR] deck not contain {card.name}");
      return;
    }
    deck.Remove(card);
    tokens.Add(card);
  }

  private void RefillDeckFromYard() {
    deck = new List<CardObjcet>(yard);
    yard.Clear();
    deck.Shuffle();
  }

  public void DiscardCard(CardObjcet card) {
    tokens.Remove(card);
    yard.Add(card);
  }

  private void EndRoundDiscardHands() {
    var cnt = tokens.Count;
    if (cnt == 0) {
      return;
    }
    for (int i = 0; i < tokens.Count; i++) {
      var card = tokens[i];
      if (card.tags.Contains(CardTag.STICKY)) {
        continue;
      }
      DiscardCard(card);
      i--;
    }
  }
  #endregion

  #region 战斗逻辑
  public void ExecuteAllEnemyActions() {
    foreach (var enemy in enemies) {
      enemy.enemyAction.ExecuteAction();
    }
  }

  #endregion

  #region 回合管理

  public void EndAndPushRound() {
    EndRoundDiscardHands();

    ExecuteAllEnemyActions();

    MarkRoundEndToChars();
    roundIdx++;
    OnRoundStart();
  }

  private void OnRoundStart() {
    playerCharObj.shield = 0;
    mana = 3;
    AddJourney(playerCharObj.speed);
  }

  #endregion

  #region ViewModel 同步

  public void SyncToViewModel() {
    if (_viewModel == null) return;

    _viewModel.displayState = GameDisplayStateType.Battle;
    _viewModel.turn = roundIdx;
    _viewModel.eng = mana;
    _viewModel.maxEng = 3;
    _viewModel.playerHp = playerCharObj.hp;
    _viewModel.maxPlayerHp = playerCharObj.maxHp;
    _viewModel.playerShield = playerCharObj.shield;

    // 同步卡牌堆数据
    _viewModel.deckCount = deck.Count;
    _viewModel.discardCount = yard.Count;

    _viewModel.enemies.Clear();
    foreach (var enemy in enemies) {
      var model = new EnemyViewModel {
        name = enemy.name,
        hp = enemy.hp,
        maxHp = enemy.maxHp,
        shield = enemy.shield,
        intention = enemy.enemyAction?.ForeshowAction() ?? "unknown",
      };
      model.buffs.Clear();
      foreach (var buff in enemy.buffs) {
        model.buffs.Add(new(EnumTrans.Get(buff.buffId), buff.stack));
      }
      _viewModel.enemies.Add(model);
    }

    _viewModel.handCards.Clear();
    foreach (var card in tokens) {
      var cardModel = new CardViewModel {
        name = card.name,
        cost = card.cost,
        description = card.cardModel.desc,
        descColor = card.cardModel.cardType == CardType.MAGIC ? Col.magicGreen : Col.cardWhite,
        titleColor = Col.cardTypeColors.GetValueOrDefault(card.type, Color.White),
        header = EnumTrans.Get(card.type),
      };
      cardModel.tags.Clear();
      if (card.type == CardType.MAGIC) {
        cardModel.tags.Add("Magic");
      } else {
        cardModel.tags.AddRange(card.tags.Select(t => t.ToString()));
      }
      _viewModel.handCards.Add(cardModel);
    }

    _viewModel.journey = playerCharObj.journey;
    _viewModel.maxJourney = playerCharObj.maxJourney;

    _viewModel.playerBuffs.Clear();
    foreach (var buff in playerCharObj.buffs) {
      _viewModel.playerBuffs.Add(new(EnumTrans.Get(buff.buffId), buff.stack));
    }

    //覆写信息面板，用战斗内的当前属性显示
    _viewModel.playerProp = playerCharObj.playerProp;
  }
  #endregion
}