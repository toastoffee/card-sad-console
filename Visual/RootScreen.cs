using SadConsole.Input;

namespace CardConsole.Visual;

internal class RootScreen : ScreenObject {

	private ScreenSurface _mainSurface;

	public PanelText turnText;

	public PanelText playerHealthText;
	public PanelText playerEnergyText;

	public PanelText enemyNameText;
	public PanelText enemyHealthText;
	public PanelText enemyIntentionText;

	public int testNumber = 1;  // in case you dont know how it works

	public ViewModel viewModel = new ViewModel();
	public View view = new View();

	public RootScreen() {
		_mainSurface = new ScreenSurface(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
		Children.Add(_mainSurface);

		_mainSurface.UseMouse = false;

		// 设置 ViewModel 测试数据
		viewModel.turn = testNumber;
		viewModel.eng = 3;
		viewModel.maxEng = 3;
		viewModel.playerHp = 70;
		viewModel.maxPlayerHp = 70;

		// 添加两个敌人用于测试
		viewModel.enemies.Add(new EnemyViewModel {
			name = "Tree",
			hp = 27,
			maxHp = 27,
			intention = "will cause [6] damage to you"
		});

		viewModel.enemies.Add(new EnemyViewModel {
			name = "Stone",
			hp = 40,
			maxHp = 40,
			intention = "preparing to defend"
		});
	}

	public override bool ProcessKeyboard(Keyboard keyboard) {
		//!!! IN CASE YOU DONT KNOW HOW IT WORKS !!!
		bool handled = false;

		if (keyboard.IsKeyPressed(Keys.Up)) {
			turnText.content = $"{testNumber++}";
			handled = true;
		}

		return handled;
	}
	public override void Update(TimeSpan delta) {
		view.Render(viewModel, _mainSurface);
	}
}
