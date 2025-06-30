using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace CardConsole.Visual;

internal class View {
	private ScreenSurface _battleSurface;
	private ScreenSurface _routeSurface;
	private ScreenSurface _logSurface;
	private ScreenSurface _infoSurface;
	private ScreenSurface _handCardSurface;

	// 子视图
	private BattleView battleView;
	private RouteView routeView;

	public View() {
		int width = Game.Instance.ScreenCellsX;
		int height = Game.Instance.ScreenCellsY;
		int upperHeight = height * 7 / 10;

		// 计算各区域尺寸
		int logWidth = (int)(width * 0.25);
		int infoWidth = logWidth;
		int gameWidth = width - logWidth - infoWidth;

		// 创建表面
		_infoSurface = new ScreenSurface(infoWidth, upperHeight);
		_infoSurface.Position = new Point(0, 0);
		_infoSurface.UseMouse = false;

		// 创建两个游戏表面
		_battleSurface = new ScreenSurface(gameWidth, upperHeight);
		_battleSurface.Position = new Point(infoWidth, 0);
		_battleSurface.UseMouse = true;

		_routeSurface = new ScreenSurface(gameWidth, upperHeight);
		_routeSurface.Position = new Point(infoWidth, 0);
		_routeSurface.UseMouse = true;

		_logSurface = new ScreenSurface(logWidth, upperHeight);
		_logSurface.Position = new Point(infoWidth + gameWidth, 0);
		_logSurface.UseMouse = false;

		_handCardSurface = new ScreenSurface(width, height - upperHeight);
		_handCardSurface.Position = new Point(0, upperHeight);
		_handCardSurface.UseMouse = true;

		// 初始化子视图
		battleView = new BattleView(_battleSurface, _handCardSurface);
		routeView = new RouteView(_routeSurface);
	}

	public ScreenSurface GetBattleSurface() => _battleSurface;
	public ScreenSurface GetRouteSurface() => _routeSurface;
	public ScreenSurface GetLogSurface() => _logSurface;
	public ScreenSurface GetInfoSurface() => _infoSurface;
	public ScreenSurface GetHandCardSurface() => _handCardSurface;

	public void Render(ViewModel viewModel) {
		// 清空表面
		_battleSurface.Clear();
		_routeSurface.Clear();
		_logSurface.Clear();
		_infoSurface.Clear();
		_handCardSurface.Clear();

		// 根据状态渲染对应视图
		switch (viewModel.currentStateType) {
			case GameStateType.Battle:
				battleView.Render(viewModel);
				break;
			case GameStateType.Route:
				routeView.Render(viewModel);
				break;
		}

		// 始终渲染共享区域
		RenderLogArea(viewModel);
		RenderInfoArea(viewModel);
	}

	private void RenderInfoArea(ViewModel viewModel) {
		int totalHeight = _infoSurface.Height;

		_infoSurface.DrawBox(new Rectangle(0, 0, _infoSurface.Width, totalHeight),
			ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
				new ColoredGlyph(Color.White, Color.Black)));

		_infoSurface.Print(2, 1, "Info", Color.Yellow);

		for (int i = 1; i < _infoSurface.Width - 1; i++) {
			_infoSurface.Surface[i, 2].Glyph = '-';
			_infoSurface.Surface[i, 2].Foreground = Color.White;
		}

		int currentY = 4;

		// 渲染玩家属性区域（添加方框）
		int playerStatsHeight = 12; // 4个属性 * 2行间距 + 2行边框 + 1行标题 + 1行间距
		int playerStatsWidth = _infoSurface.Width - 2;

		// 绘制玩家属性区域边框
		_infoSurface.DrawBox(new Rectangle(1, currentY, playerStatsWidth, playerStatsHeight),
			ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
				new ColoredGlyph(Color.Gray, Color.Black)));

		// 标题
		int playerStatsLeft = 2;
		int rogueStatsLeft = 25; // Rogue stats starts at column 25
		_infoSurface.Print(playerStatsLeft, currentY + 1, "Player Stats:", Color.Cyan);
		_infoSurface.Print(rogueStatsLeft, currentY + 1, "Rogue Stats:", Color.Cyan);

		// 渲染Rogue属性
		_infoSurface.Print(rogueStatsLeft, currentY + 3, $"Money: {viewModel.playerMoney}", Color.Yellow);


		// 渲染玩家属性
		int statsY = currentY + 3;
		int interval = 2;
		_infoSurface.Print(playerStatsLeft, statsY, $"HP: {viewModel.playerProp.hp} / {viewModel.playerProp.maxHp}", Color.White);
		statsY += interval;
		_infoSurface.Print(playerStatsLeft, statsY, $"Attack: {viewModel.playerProp.atk}", Color.White);
		statsY += interval;
		_infoSurface.Print(playerStatsLeft, statsY, $"Defense: {viewModel.playerProp.def}", Color.White);
		statsY += interval;
		_infoSurface.Print(playerStatsLeft, statsY, $"Speed: {viewModel.playerProp.speed}", Color.White);

		// 更新当前Y位置，为装备区域留出空间
		currentY += playerStatsHeight + 2;

		// 渲染装备信息
		RenderEquipmentArea(viewModel, currentY);
	}

	private void RenderEquipmentArea(ViewModel viewModel, int startY) {
		int equipmentAreaHeight = 25; // 6个槽位 + 2行边框 + 1行标题 + 1行间距
		int equipmentAreaWidth = _infoSurface.Width - 2;

		// 绘制装备区域边框
		_infoSurface.DrawBox(new Rectangle(1, startY, equipmentAreaWidth, equipmentAreaHeight),
			ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
				new ColoredGlyph(Color.Gray, Color.Black)));

		// 标题
		_infoSurface.Print(2, startY + 1, "Equipment:", Color.Cyan);

		// 渲染每个装备槽位
		int slotY = startY + 3;
		int occupy = 3;
		foreach (var slot in viewModel.equipmentSlots) {
			_infoSurface.Print(2, slotY, $"{slot.slotName}:", Color.Yellow);

			// 装备名称的颜色：有装备时为白色，None时为灰色
			Color equipmentColor = slot.equipmentName == "None" ? Color.Gray : Color.White;
			_infoSurface.Print(4, slotY + 1, slot.equipmentName, equipmentColor);

			slotY += occupy; // 每个槽位占1行
		}
	}

	private static Color[] logColors = new Color[] {
		Color.White * 1.0f,
		Color.White * 1.0f,
		Color.White * 1.0f,
		Color.White * 0.9f,
		Color.White * 0.9f,
		Color.White * 0.9f,
		Color.White * 0.8f,
		Color.White * 0.8f,
		Color.White * 0.8f,
	};
	private void RenderLogArea(ViewModel viewModel) {
		int totalHeight = _logSurface.Height;
		int headerHeight = 1;
		int gameLogHeight = (totalHeight - headerHeight - 1) / 2;
		int systemLogHeight = totalHeight - headerHeight - gameLogHeight - 2;

		_logSurface.DrawBox(new Rectangle(0, 0, _logSurface.Width, totalHeight),
			ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
				new ColoredGlyph(Color.White, Color.Black)));

		_logSurface.Print(2, 1, "Game Log", Color.Yellow);

		for (int i = 1; i < _logSurface.Width - 1; i++) {
			_logSurface.Surface[i, 2].Glyph = '-';
			_logSurface.Surface[i, 2].Foreground = Color.White;
		}

		// 游戏日志：最新的显示在最上方，使用渐变颜色
		int maxGameLogs = gameLogHeight;
		int totalGameLogs = viewModel.gameLogs.Count;

		for (int i = 0; i < Math.Min(maxGameLogs, totalGameLogs); i++) {
			// 从最新的日志开始显示（倒序）
			int logIndex = totalGameLogs - 1 - i;
			if (logIndex >= 0) {
				// 计算颜色索引，最新的使用第一个颜色，然后逐渐变暗
				int colorIndex = Math.Min(i, logColors.Length - 1);
				Color logColor = logColors[colorIndex];

				_logSurface.Print(1, i + 3, $"{viewModel.gameLogs[logIndex]}", logColor);
			}
		}

		int systemLogY = gameLogHeight + 4;
		_logSurface.Print(2, systemLogY, "Sys Log", Color.Yellow);

		for (int i = 1; i < _logSurface.Width - 1; i++) {
			_logSurface.Surface[i, systemLogY + 1].Glyph = '-';
			_logSurface.Surface[i, systemLogY + 1].Foreground = Color.White;
		}

		// 系统日志：最新的显示在最上方，使用渐变颜色
		int maxSystemLogs = systemLogHeight;
		int totalSystemLogs = viewModel.systemLogs.Count;

		for (int i = 0; i < Math.Min(maxSystemLogs, totalSystemLogs); i++) {
			// 从最新的日志开始显示（倒序）
			int logIndex = totalSystemLogs - 1 - i;
			if (logIndex >= 0) {
				// 计算颜色索引，最新的使用第一个颜色，然后逐渐变暗
				int colorIndex = Math.Min(i, logColors.Length - 1);
				Color logColor = logColors[colorIndex];

				_logSurface.Print(1, systemLogY + 2 + i, viewModel.systemLogs[logIndex], logColor);
			}
		}
	}
}