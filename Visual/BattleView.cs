using SadConsole.UI;
using SadConsole.UI.Controls;

namespace CardConsole.Visual;

internal class BattleView {
  private ScreenSurface _surface;
  private ScreenSurface _handCardSurface;
  private ControlHost _controls;
  private ControlHost _handCardControls;

  // Battle״̬ר�����
  private PanelText turnText;
  private PanelText playerHealthText;
  private PanelText playerEnergyText;
  private PanelText playerShieldText;
  private PanelText deckCountText;
  private PanelText discardCountText;
  private List<EnemyView> enemyViews = new List<EnemyView>();
  private List<CardView> cardViews = new List<CardView>();
  private ButtonBox endTurnButton;
  private ButtonBox skipBattleButton;
  private ProgressBar speedProgressBar;

  public BattleView(ScreenSurface surface, ScreenSurface handCardSurface) {
    _surface = surface;
    _handCardSurface = handCardSurface;

    // Ϊ�����洴��ControlHost
    _controls = new ControlHost();
    _surface.SadComponents.Add(_controls);

    // Ϊ���Ʊ��洴��������ControlHost
    _handCardControls = new ControlHost();
    _handCardSurface.SadComponents.Add(_handCardControls);

    InitializeComponents();
  }

  private void InitializeComponents() {
    // ��ʼ���غϽ�����ť
    int buttonWidth = 12;
    int buttonHeight = 3;
    int buttonX = _surface.Width - buttonWidth - 2;
    int buttonY = _surface.Height - buttonHeight - 1;

    endTurnButton = new ButtonBox(buttonWidth, buttonHeight) {
      Position = new Point(buttonX, buttonY),
      Text = "End Turn"
    };

    endTurnButton.Click += (sender, e) => {
      GameInput.Set(GameInput.Type.END_TURN);
      endTurnButton.IsEnabled = false;
      System.Threading.Tasks.Task.Delay(200).ContinueWith(t => {
        endTurnButton.IsEnabled = true;
      });
    };
    endTurnButton.IsVisible = true;

    _controls.Add(endTurnButton);

    skipBattleButton = new ButtonBox(buttonWidth, buttonHeight) {
      Position = new Point(buttonX - buttonWidth - 2, buttonY),
      Text = "Skip"
    };

    skipBattleButton.Click += (sender, e) => {
      GameInput.Set(GameInput.Type.SKIP_BATTLE);
      skipBattleButton.IsEnabled = false;
      System.Threading.Tasks.Task.Delay(200).ContinueWith(t => {
        skipBattleButton.IsEnabled = true;
      });
    };
    skipBattleButton.IsVisible = true;
    _controls.Add(skipBattleButton);

    speedProgressBar = GetNewSpeedProgressBar();
    _controls.Add(speedProgressBar);
  }

  public void Render(ViewModel viewModel) {
    // �Ȼ�������Ϸ����ı߿�
    _surface.DrawBox(new Rectangle(0, 0, _surface.Width, _surface.Height),
      ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
        new ColoredGlyph(Color.White, Color.Black)));

    // ��Ⱦ���״̬
    RenderPlayerStatus(viewModel);

    // ��Ⱦ����
    RenderEnemies(viewModel);

    // ��Ⱦ����
    RenderCards(viewModel);

    endTurnButton.IsVisible = true;
    endTurnButton.UpdateAndRedraw(default);
    skipBattleButton.IsVisible = true;
    skipBattleButton.UpdateAndRedraw(default);
  }

  private void RenderPlayerStatus(ViewModel viewModel) {
    // �������½�λ��
    int bottomY = _surface.Height - 4; // ����ײ�4����λ
    int upperY = bottomY - 4; // �Ϸ�һ�У����1��3�߶�+1��ࣩ
    int startX = 2; // ��߾�2����λ
    int gap = 1; // �����֮��ļ��

    // ����״̬��Ŀ��
    int turnWidth = 6;
    int energyWidth = 10;
    int hpWidth = 10;
    int shieldWidth = 10;
    int deckWidth = 8;
    int discardWidth = 8;

    // ��Ⱦ�Ϸ��У����ƶ���Ϣ
    int upperCurrentX = startX;

    // ���ƶ�
    deckCountText = new PanelText(new Point(upperCurrentX, upperY), "Deck", deckWidth, 3, Color.Green, _surface);
    deckCountText.alignType = PanelText.AlignType.Center;
    deckCountText.contentColor = viewModel.deckCount > 0 ? Color.White : Color.Gray;
    deckCountText.content = viewModel.deckCount.ToString();

    // ���ƶ�
    upperCurrentX += deckWidth + gap;
    discardCountText = new PanelText(new Point(upperCurrentX, upperY), "Discard", discardWidth, 3, Color.Purple, _surface);
    discardCountText.alignType = PanelText.AlignType.Center;
    discardCountText.contentColor = viewModel.discardCount > 0 ? Color.White : Color.Gray;
    discardCountText.content = viewModel.discardCount.ToString();

    // ��Ⱦ�·��У�����״̬��Ϣ
    int currentX = startX;

    // �غ���
    turnText = new PanelText(new Point(currentX, bottomY), "Turn", turnWidth, 3, Color.Wheat, _surface);
    turnText.alignType = PanelText.AlignType.Center;
    turnText.content = viewModel.turn.ToString();

    // ����
    currentX += turnWidth + gap;
    playerEnergyText = new PanelText(new Point(currentX, bottomY), "Energy", energyWidth, 3, Color.Orange, _surface);
    playerEnergyText.alignType = PanelText.AlignType.Center;
    playerEnergyText.content = $"{viewModel.eng}/{viewModel.maxEng}";

    // Ѫ��
    currentX += energyWidth + gap;
    playerHealthText = new PanelText(new Point(currentX, bottomY), "HP", hpWidth, 3, Color.Red, _surface);
    playerHealthText.alignType = PanelText.AlignType.Center;
    playerHealthText.content = $"{viewModel.playerHp}/{viewModel.maxPlayerHp}";

    // ���ܣ�ֻ�д���0ʱ����ʾ��
    if (viewModel.playerShield > 0) {
      currentX += hpWidth + gap;
      if (playerShieldText == null) {
        playerShieldText = new PanelText(new Point(currentX, bottomY), "Shield", shieldWidth, 3, Color.LightBlue, _surface);
        playerShieldText.alignType = PanelText.AlignType.Center;
      } else {
        // ���»��ܿ��λ�ã���Ϊ�����ܻᶯ̬��ʾ/���أ�
        playerShieldText.SetPosition(new Point(currentX, bottomY));
      }
      playerShieldText.content = viewModel.playerShield.ToString();
    }

    // ������
    _surface.DrawBox(new Rectangle(speedProgressBar.Position.X - 1, speedProgressBar.Position.Y - 1, speedProgressBar.Width + 2, speedProgressBar.Height + 2),
      ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
        new ColoredGlyph(Color.AnsiYellow, Color.Black)));

    var prog = viewModel.journey / MathF.Max(viewModel.maxJourney, 1);
    speedProgressBar.Progress = prog;

    _surface.Print(speedProgressBar.Position.X, speedProgressBar.Position.Y - 1, $"Draw Card: {viewModel.journey}/{viewModel.maxJourney}", Color.AnsiYellow);
  }

  private ProgressBar GetNewSpeedProgressBar() {
    var speedProgressBar = new ProgressBar(20, 1, HorizontalAlignment.Left);
    speedProgressBar.BarColor = Color.AnsiYellow;
    speedProgressBar.Position = new Point((_surface.Width - speedProgressBar.Width) / 2, _surface.Height - 3);
    return speedProgressBar;
  }

  private void RenderEnemies(ViewModel viewModel) {
    while (enemyViews.Count < viewModel.enemies.Count) {
      int index = enemyViews.Count;
      int yPosition = 7;

      for (int j = 0; j < index; j++) {
        yPosition += enemyViews[j].TotalHeight;
      }

      enemyViews.Add(new EnemyView(50, yPosition, _surface));
    }

    for (int i = 0; i < viewModel.enemies.Count; i++) {
      enemyViews[i].Render(viewModel.enemies[i]);
    }
  }

  private void RenderCards(ViewModel viewModel) {
    // �Ȼ�����������ı߿�
    int totalWidth = _handCardSurface.Width;
    int totalHeight = _handCardSurface.Height;

    // ������������߿�
    _handCardSurface.DrawBox(new Rectangle(0, 0, totalWidth, totalHeight),
      ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
        new ColoredGlyph(Color.White, Color.Black)));

    // ���"Hand Cards"����
    _handCardSurface.Print(2, 0, "Hand Cards", Color.Yellow);

    // ������е���ʼλ��
    int cardCount = viewModel.handCards.Count;

    int cardWidth = 17; // CardView.TotalWidth
    int cardSpacing = 2;
    int totalCardsWidth = cardCount * cardWidth + (cardCount - 1) * cardSpacing;
    int startX = (totalWidth - totalCardsWidth) / 2;

    // ȷ�����㹻��CardView
    while (cardViews.Count < cardCount) {
      int index = cardViews.Count;
      int xPosition = startX + index * (cardWidth + cardSpacing);
      int yPosition = 2; // �Ӷ�����ʼ����2����λ���

      cardViews.Add(new CardView(xPosition, yPosition, _handCardSurface, _handCardControls));
    }

    // ��������CardView��λ�ò���Ⱦ
    for (int i = 0; i < cardViews.Count; i++) {
      if (i < cardCount) {
        // ���¼���λ����ȷ������
        int xPosition = startX + i * (cardWidth + cardSpacing);
        cardViews[i].UpdatePosition(xPosition, 2);

        var cardModel = viewModel.handCards[i];
        cardViews[i].Render(i, cardModel, true);
      } else {
        // ���ض����CardView
        cardViews[i].Render(i, null, false);
      }
    }
  }
}

internal class EnemyView {
  public (int x, int y) anchor;
  public PanelText enemyNameText;
  public PanelText enemyHealthText;
  public PanelText enemyIntentionText;
  private ScreenSurface mainSurface;

  // ������ͼ�ܸ߶ȵ�����
  public int TotalHeight => 11; // ����(3) + ���(0) + ����ֵ(3) + ���(0) + ��ͼ(3) + �ײ����(2)

  public EnemyView(int x, int y, ScreenSurface surface) {
    anchor = (x, y);
    mainSurface = surface;
  }

  public void Render(EnemyViewModel viewModel) {
    if (enemyNameText == null) {
      enemyNameText = new PanelText(new Point(anchor.x, anchor.y), "Enemy", 12, 3, Color.Purple, mainSurface);
      enemyNameText.alignType = PanelText.AlignType.Center;
    }
    enemyNameText.content = viewModel.name;

    if (enemyHealthText == null) {
      enemyHealthText = new PanelText(new Point(anchor.x, anchor.y + 3), "HP", 12, 3, Color.Red, mainSurface);
      enemyHealthText.alignType = PanelText.AlignType.Center;
    }
    enemyHealthText.content = $"{viewModel.hp}/{viewModel.maxHp}";

    if (enemyIntentionText == null) {
      enemyIntentionText = new PanelText(new Point(anchor.x, anchor.y + 6), "Intention", 30, 3, Color.Yellow, mainSurface);
      enemyIntentionText.alignType = PanelText.AlignType.Center;
    }
    enemyIntentionText.content = viewModel.intention;
  }
}

internal class CardView {
  public (int x, int y) anchor;
  public PanelText cardNameText;
  public PanelText costText;
  public PanelText descriptionText;
  public int idx;
  private ScreenSurface mainSurface;
  private SadConsole.UI.Controls.ButtonBox playButton;

  public int TotalWidth => 17;

  public CardView(int x, int y, ScreenSurface surface, ControlHost controls) {
    anchor = (x, y);
    mainSurface = surface;

    // ��ʼ����ť - ��ť���Ϊ17���߶�Ϊ5��λ���ڿ�����������·�
    playButton = new ButtonBox(17, 5) {
      Position = new Point(anchor.x, anchor.y + 11), // ��ťλ��
      Text = "Play" // ��ť�ı�
    };

    // �󶨰�ť����¼�
    playButton.Click += PlayButton_Click;

    controls.Add(playButton);
  }

  // ��Ӹ���λ�õķ���
  public void UpdatePosition(int x, int y) {
    anchor = (x, y);

    // ���°�ťλ��
    playButton.Position = new Point(anchor.x, anchor.y + 11);
    // �����ı�λ��
    if (cardNameText != null) {
      cardNameText.SetPosition(new Point(anchor.x, anchor.y));
    }
    if (costText != null) {
      costText.SetPosition(new Point(anchor.x + 12, anchor.y));
    }
    if (descriptionText != null) {
      descriptionText.SetPosition(new Point(anchor.x, anchor.y + 3));
    }
  }

  // ��ť����¼�������
  private void PlayButton_Click(object sender, EventArgs e) {
    GameInput.Set(GameInput.Type.CARD, new InputValue {
      intValue = idx,
    });

    playButton.IsEnabled = false; // ��ʱ���ð�ť��������
    System.Threading.Tasks.Task.Delay(200).ContinueWith(t => {
      playButton.IsEnabled = true;
    });
  }

  public void Render(int idx, CardViewModel viewModel, bool isShow) {
    this.idx = idx;

    if (!isShow) {
      playButton.IsVisible = false;
      playButton.UpdateAndRedraw(default);
      return;
    }

    // ȷ����ť�� ControlHost ��
    if (playButton.Parent == null) {
      ((ControlHost)mainSurface.SadComponents.OfType<ControlHost>().First()).Add(playButton);
    }

    // ������Ⱦ�߼�...
    if (cardNameText == null) {
      cardNameText = new PanelText(new Point(anchor.x, anchor.y), "card", 12, 3, Color.AnsiRedBright, mainSurface);
      cardNameText.alignType = PanelText.AlignType.Center;
    }
    cardNameText.content = viewModel.name;

    if (costText == null) {
      costText = new PanelText(new Point(anchor.x + 12, anchor.y), "mana", 5, 3, Color.Orange, mainSurface);
      costText.contentColor = Color.Orange;
      costText.alignType = PanelText.AlignType.Center;
    }
    costText.content = viewModel.cost.ToString();

    if (descriptionText == null) {
      descriptionText = new PanelText(new Point(anchor.x, anchor.y + 3), "desc", 17, 8, Color.Wheat, mainSurface);
      descriptionText.alignType = PanelText.AlignType.Left;
    }
    descriptionText.content = viewModel.description;

    playButton.Text = $"Play {viewModel.name}";
    playButton.IsVisible = true;
  }
}