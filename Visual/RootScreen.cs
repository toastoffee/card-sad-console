using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace CardConsole.Visual;

internal class RootScreen : ScreenObject {
	public ViewModel viewModel = new ViewModel();
	public View view = new View();

	public RootScreen() {
		Children.Add(view.GetMainSurface());
		Children.Add(view.GetLogSurface());

		// 初始化游戏
		CardGame.instance = new CardGame();
		CardGame.instance.viewModel = viewModel;
		CardGame.instance.Setup();
	}

	public override bool ProcessKeyboard(Keyboard keyboard) {
		bool handled = false;

		if (keyboard.IsKeyPressed(Keys.Up)) {
			handled = true;
		}

		return handled;
	}

	public override void Update(TimeSpan delta) {
		// 更新游戏逻辑
		CardGame.instance.Tick();

		// 调用View渲染所有内容
		view.Render(viewModel);
	}
}
