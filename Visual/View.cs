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
		int interval = 2;
		_infoSurface.Print(1, currentY++, "Player Stats:", Color.Cyan);

		currentY += 1;
		_infoSurface.Print(1, currentY, $"HP: {viewModel.playerProp.hp} / {viewModel.playerProp.maxHp}", Color.White);
		currentY += interval;
		_infoSurface.Print(1, currentY, $"Attack: {viewModel.playerProp.atk}", Color.White);
		currentY += interval;
		_infoSurface.Print(1, currentY, $"Defense: {viewModel.playerProp.def}", Color.White);
		currentY += interval;
		_infoSurface.Print(1, currentY, $"Speed: {viewModel.playerProp.speed}", Color.White);
	}

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

		int maxGameLogs = gameLogHeight;
		int gameStartIdx = Math.Max(0, viewModel.gameLogs.Count - maxGameLogs);

		for (int i = 0; i < Math.Min(maxGameLogs, viewModel.gameLogs.Count); i++) {
			int logIndex = gameStartIdx + i;
			if (logIndex < viewModel.gameLogs.Count) {
				_logSurface.Print(1, i + 3, viewModel.gameLogs[logIndex], Color.White);
			}
		}

		int systemLogY = gameLogHeight + 4;
		_logSurface.Print(2, systemLogY, "Sys Log", Color.Yellow);

		for (int i = 1; i < _logSurface.Width - 1; i++) {
			_logSurface.Surface[i, systemLogY + 1].Glyph = '-';
			_logSurface.Surface[i, systemLogY + 1].Foreground = Color.White;
		}

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