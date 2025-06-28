using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace CardConsole.Visual;

internal class BattleView {
	private ScreenSurface _surface;
	private ScreenSurface _handCardSurface;
	private ControlHost _controls;
	private ControlHost _handCardControls;

	// Battle状态专用组件
	private PanelText turnText;
	private PanelText playerHealthText;
	private PanelText playerEnergyText;
	private PanelText playerShieldText;
	private List<EnemyView> enemyViews = new List<EnemyView>();
	private List<CardView> cardViews = new List<CardView>();
	private ButtonBox endTurnButton;
	private ButtonBox skipBattleButton;

	public BattleView(ScreenSurface surface, ScreenSurface handCardSurface) {
		_surface = surface;
		_handCardSurface = handCardSurface;

		// 为主表面创建ControlHost
		_controls = new ControlHost();
		_surface.SadComponents.Add(_controls);

		// 为手牌表面创建独立的ControlHost
		_handCardControls = new ControlHost();
		_handCardSurface.SadComponents.Add(_handCardControls);

		InitializeComponents();
	}

	private void InitializeComponents() {
		// 初始化回合结束按钮
		int buttonWidth = 12;
		int buttonHeight = 3;
		int buttonX = _surface.Width - buttonWidth - 2;
		int buttonY = _surface.Height - buttonHeight - 2;

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
	}

	public void Render(ViewModel viewModel) {
		// 渲染玩家状态
		RenderPlayerStatus(viewModel);

		// 渲染敌人
		RenderEnemies(viewModel);

		// 渲染卡牌
		RenderCards(viewModel);

		endTurnButton.IsVisible = true;
		endTurnButton.UpdateAndRedraw(default);
		skipBattleButton.IsVisible = true;
		skipBattleButton.UpdateAndRedraw(default);
	}

	private void RenderPlayerStatus(ViewModel viewModel) {
		turnText = new PanelText(new Point(3, 1), "Turn", 6, 3, Color.Wheat, _surface);
		turnText.alignType = PanelText.AlignType.Center;
		turnText.content = viewModel.turn.ToString();

		playerEnergyText = new PanelText(new Point(3, 7), "Energy", 10, 3, Color.Orange, _surface);
		playerEnergyText.alignType = PanelText.AlignType.Center;
		playerEnergyText.content = $"{viewModel.eng}/{viewModel.maxEng}";

		playerHealthText = new PanelText(new Point(3, 10), "HP", 10, 3, Color.Red, _surface);
		playerHealthText.alignType = PanelText.AlignType.Center;
		playerHealthText.content = $"{viewModel.playerHp}/{viewModel.maxPlayerHp}";

		if (viewModel.playerShield > 0) {
			if (playerShieldText == null) {
				playerShieldText = new PanelText(new Point(3, 13), "Shield", 10, 3, Color.LightBlue, _surface);
				playerShieldText.alignType = PanelText.AlignType.Center;
			}
			playerShieldText.content = viewModel.playerShield.ToString();
		}
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
		// 计算居中的起始位置
		int cardCount = viewModel.handCards.Count;
		if (cardCount == 0) return;

		int cardWidth = 17; // CardView.TotalWidth
		int cardSpacing = 2;
		int totalCardsWidth = cardCount * cardWidth + (cardCount - 1) * cardSpacing;
		int startX = (_handCardSurface.Width - totalCardsWidth) / 2;

		// 确保有足够的CardView
		while (cardViews.Count < cardCount) {
			int index = cardViews.Count;
			int xPosition = startX + index * (cardWidth + cardSpacing);
			int yPosition = 2; // 从顶部开始，留2个单位间距

			cardViews.Add(new CardView(xPosition, yPosition, _handCardSurface, _handCardControls));
		}

		// 更新现有CardView的位置并渲染
		for (int i = 0; i < cardViews.Count; i++) {
			if (i < cardCount) {
				// 重新计算位置以确保居中
				int xPosition = startX + i * (cardWidth + cardSpacing);
				cardViews[i].UpdatePosition(xPosition, 2);

				var cardModel = viewModel.handCards[i];
				cardViews[i].Render(i, cardModel, true);
			} else {
				// 隐藏多余的CardView
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

	// 计算视图总高度的属性
	public int TotalHeight => 11; // 名称(3) + 间距(0) + 生命值(3) + 间距(0) + 意图(3) + 底部间距(2)

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

		// 初始化按钮 - 按钮宽度为17，高度为5，位置在卡牌描述框的下方
		playButton = new ButtonBox(17, 5) {
			Position = new Point(anchor.x, anchor.y + 11), // 按钮位置
			Text = "Play" // 按钮文本
		};

		// 绑定按钮点击事件
		playButton.Click += PlayButton_Click;

		controls.Add(playButton);
	}

	// 添加更新位置的方法
	public void UpdatePosition(int x, int y) {
		anchor = (x, y);

		// 更新按钮位置
		playButton.Position = new Point(anchor.x, anchor.y + 11);

		// 更新文本位置
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

	// 按钮点击事件处理方法
	private void PlayButton_Click(object sender, EventArgs e) {
		GameInput.Set(GameInput.Type.CARD, new InputValue {
			intValue = idx,
		});

		playButton.IsEnabled = false; // 临时禁用按钮避免连点
		System.Threading.Tasks.Task.Delay(200).ContinueWith(t => {
			playButton.IsEnabled = true;
		});
	}

	public void Render(int idx, CardViewModel viewModel, bool isShow) {
		this.idx = idx;

		if (!isShow) {
			// 从 ControlHost 中完全移除按钮
			if (playButton.Parent != null) {
				((ControlHost)playButton.Parent).Remove(playButton);
			}
			return;
		}

		// 确保按钮在 ControlHost 中
		if (playButton.Parent == null) {
			((ControlHost)mainSurface.SadComponents.OfType<ControlHost>().First()).Add(playButton);
		}

		// 其余渲染逻辑...
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
		playButton.UpdateAndRedraw(default);
	}
}