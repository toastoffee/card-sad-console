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

		//RogueBattleState.instance

	}

	public override bool ProcessKeyboard(Keyboard keyboard) {
		bool handled = false;
		//if (keyboard.IsKeyPressed(Keys.E)) {
		//	GameInput.Set(GameInput.Type.END_TURN);
		//	handled = true;
		//} else if (keyboard.IsKeyPressed(Keys.D1)) {
		//	GameInput.Set(GameInput.Type.CARD, new InputValue {
		//		intValue = 0,
		//	});
		//	handled = true;
		//} else if (keyboard.IsKeyPressed(Keys.D2)) {
		//	CardGame.instance.lastInputChar = '2';
		//	handled = true;
		//} else if (keyboard.IsKeyPressed(Keys.D3)) {
		//	CardGame.instance.lastInputChar = '3';
		//	handled = true;
		//} else if (keyboard.IsKeyPressed(Keys.D4)) {
		//	CardGame.instance.lastInputChar = '4';
		//	handled = true;
		//} else if (keyboard.IsKeyPressed(Keys.D5)) {
		//	CardGame.instance.lastInputChar = '5';
		//	handled = true;
		//}

		return handled;
	}

	public override void Update(TimeSpan delta) {
		// 更新游戏逻辑
		CardGame.instance.Tick();
		GameInput.Reset();
		// 调用View渲染所有内容
		view.Render(viewModel);
	}
}
