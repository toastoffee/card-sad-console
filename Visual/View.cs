using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.UI;

namespace CardConsole.Visual;
internal class View {
	private ScreenSurface _mainSurface;
	private ScreenSurface _logSurface;

	public PanelText turnText;
	public PanelText playerHealthText;
	public PanelText playerEnergyText;
	public PanelText enemyNameText;
	public PanelText enemyHealthText;
	public PanelText enemyIntentionText;
	private List<EnemyView> enemyViews = new List<EnemyView>();
	private List<CardView> cardViews = new List<CardView>();

    // 添加初始化表面的构造函数
    public View() {
		int width = Game.Instance.ScreenCellsX;
		int height = Game.Instance.ScreenCellsY;

		// 创建主表面
		_mainSurface = new ScreenSurface((int)(width * 0.75), height);
		_mainSurface.UseMouse = false;

		// 创建日志表面（右侧部分）
		int gameWidth = (int)(width * 0.75);  // 游戏区域占75%宽度 
		int logWidth = width - gameWidth;  // 日志区域占25%宽度
		_logSurface = new ScreenSurface(logWidth, height);
		_logSurface.Position = new Point(gameWidth, 0);
		_logSurface.UseMouse = false;
	}

	// 添加获取表面的方法，供RootScreen添加到其Children集合
	public ScreenSurface GetMainSurface() {
		return _mainSurface;
	}

	public ScreenSurface GetLogSurface() {
		return _logSurface;
	}

	// 修改为内部处理表面的方法
	public void Render(ViewModel viewModel) {
		// 清空表面
		_mainSurface.Clear();
		_logSurface.Clear();

		// 渲染主界面内容
		RenderMainContent(viewModel);

		// 渲染日志区域
		RenderLogArea(viewModel);
	}

	// 将原来的Render方法重命名为RenderMainContent
	private void RenderMainContent(ViewModel viewModel)
	{
		turnText = new PanelText(new Point(3, 1), "Turn", 6, 3, Color.Wheat, _mainSurface);
		turnText.alignType = PanelText.AlignType.Center;
		turnText.content = viewModel.turn.ToString();

		playerEnergyText = new PanelText(new Point(3, 7), "Energy", 10, 3, Color.Orange, _mainSurface);
		playerEnergyText.alignType = PanelText.AlignType.Center;
		playerEnergyText.content = $"{viewModel.eng}/{viewModel.maxEng}";

		playerHealthText = new PanelText(new Point(3, 10), "HP", 10, 3, Color.Red, _mainSurface);
		playerHealthText.alignType = PanelText.AlignType.Center;
		playerHealthText.content = $"{viewModel.playerHp}/{viewModel.maxPlayerHp}";

		// 确保敌人视图数量与敌人模型数量一致
		while (enemyViews.Count < viewModel.enemies.Count)
		{
			int index = enemyViews.Count;
			int yPosition = 7; // 初始Y位置

			// 计算当前敌人的Y位置，考虑前面所有敌人的高度
			for (int j = 0; j < index; j++)
			{
				yPosition += enemyViews[j].TotalHeight;
			}

			// 创建新的敌人视图，保持X位置不变，调整Y位置
			enemyViews.Add(new EnemyView(50, yPosition, _mainSurface));
		}

		// 渲染每个敌人
		for (int i = 0; i < viewModel.enemies.Count; i++)
		{
			enemyViews[i].Render(viewModel.enemies[i]);
		}

		// 确保卡牌视图数量与手牌模型数量一致
		while (cardViews.Count < viewModel.handCards.Count)
		{
			int index = cardViews.Count;
			int xPosition = 10; // 初始X位置

			for (int i = 0; i < index; i++)
			{
				xPosition += cardViews[i].TotalWidth + 2;
			}

			cardViews.Add(new CardView(xPosition, 20, _mainSurface));
		}

        // 渲染每张卡牌
        for (int i = 0; i < viewModel.handCards.Count; i++)
		{
			cardViews[i].Render(viewModel.handCards[i]);
        }

    }

    // 将日志渲染方法移到这里
    private void RenderLogArea(ViewModel viewModel) {
		// 确定日志区域的高度划分
		int totalHeight = _logSurface.Height;
		int headerHeight = 1;
		int gameLogHeight = (totalHeight - headerHeight - 1) / 2;  // 游戏日志区域高度
		int systemLogHeight = totalHeight - headerHeight - gameLogHeight - 2;  // 系统日志区域高度

		// 绘制标题和边框
		_logSurface.DrawBox(new Rectangle(0, 0, _logSurface.Width, totalHeight),
												ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
																											 new ColoredGlyph(Color.White, Color.Black)));

		// 游戏日志区域
		_logSurface.Print(2, 1, "Game Log", Color.Yellow);
		
		for (int i = 1; i < _logSurface.Width - 1; i++) {
			_logSurface.Surface[i, 2].Glyph = '-';
			_logSurface.Surface[i, 2].Foreground = Color.White;
		}

		// 显示游戏日志内容
		int maxGameLogs = gameLogHeight;
		int gameStartIdx = Math.Max(0, viewModel.gameLogs.Count - maxGameLogs);

		for (int i = 0; i < Math.Min(maxGameLogs, viewModel.gameLogs.Count); i++) {
			int logIndex = gameStartIdx + i;
			if (logIndex < viewModel.gameLogs.Count) {
				_logSurface.Print(1, i + 3, viewModel.gameLogs[logIndex], Color.White);
			}
		}

		// 系统日志区域
		int systemLogY = gameLogHeight + 4;
		_logSurface.Print(2, systemLogY, "Sys Log", Color.Yellow);
		
		for (int i = 1; i < _logSurface.Width - 1; i++) {
			_logSurface.Surface[i, systemLogY + 1].Glyph = '-';
			_logSurface.Surface[i, systemLogY + 1].Foreground = Color.White;
		}

		// 显示系统日志内容
		int maxSystemLogs = systemLogHeight;
		int systemStartIdx = Math.Max(0, viewModel.systemLogs.Count - maxSystemLogs);

		for (int i = 0; i < Math.Min(maxSystemLogs, viewModel.systemLogs.Count); i++) {
			int logIndex = systemStartIdx + i;
			if (logIndex < viewModel.systemLogs.Count) {
				_logSurface.Print(1, systemLogY + 2 + i, viewModel.systemLogs[logIndex], Color.Gray);
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

internal class CardView
{
    public (int x, int y) anchor;
	public PanelText cardNameText;
	public PanelText costText;
	public PanelText descriptionText;
	private ScreenSurface mainSurface;

	public int TotalWidth => 17;

	public CardView(int x, int y, ScreenSurface surface)
	{
		anchor = (x, y);
		mainSurface = surface;
    }

	public void Render(CardViewModel viewModel)
	{
        if (cardNameText == null)
        {
            cardNameText = new PanelText(new Point(anchor.x, anchor.y), "card", 12, 3, Color.AnsiRedBright, mainSurface);
            cardNameText.alignType = PanelText.AlignType.Center;
        }
        cardNameText.content = viewModel.name;

        if (costText == null)
        {
            costText = new PanelText(new Point(anchor.x + 12, anchor.y), "mana", 5, 3, Color.AnsiCyan, mainSurface);
            costText.alignType = PanelText.AlignType.Center;
        }
        costText.content = viewModel.cost.ToString();

        if (descriptionText == null)
        {
            descriptionText = new PanelText(new Point(anchor.x, anchor.y + 3), "desc", 17, 8, Color.Wheat, mainSurface);
            descriptionText.alignType = PanelText.AlignType.Left;
        }
        descriptionText.content = viewModel.description;

    }
}