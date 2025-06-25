using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CardConsole.Visual;
internal class View {
	public PanelText turnText;
	public PanelText playerHealthText;
	public PanelText playerEnergyText;
	public PanelText enemyNameText;
	public PanelText enemyHealthText;
	public PanelText enemyIntentionText;

	private List<EnemyView> enemyViews = new List<EnemyView>();

	public void Render(ViewModel viewModel, ScreenSurface mainSurface) {
		turnText = new PanelText(new Point(3, 1), "Turn", 6, 3, Color.Wheat, mainSurface);
		turnText.alignType = PanelText.AlignType.Center;
		turnText.content = viewModel.turn.ToString();

		playerEnergyText = new PanelText(new Point(3, 7), "Energy", 10, 3, Color.Orange, mainSurface);
		playerEnergyText.alignType = PanelText.AlignType.Center;
		playerEnergyText.content = $"{viewModel.eng}/{viewModel.maxEng}";

		playerHealthText = new PanelText(new Point(3, 10), "HP", 10, 3, Color.Red, mainSurface);
		playerHealthText.alignType = PanelText.AlignType.Center;
		playerHealthText.content = $"{viewModel.playerHp}/{viewModel.maxPlayerHp}";

		// 确保敌人视图数量与敌人模型数量一致
		while (enemyViews.Count < viewModel.enemies.Count) {
			int index = enemyViews.Count;
			int yPosition = 7; // 初始Y位置
			
			// 计算当前敌人的Y位置，考虑前面所有敌人的高度
			for (int j = 0; j < index; j++) {
				yPosition += enemyViews[j].TotalHeight;
			}
			
			// 创建新的敌人视图，保持X位置不变，调整Y位置
			enemyViews.Add(new EnemyView(50, yPosition, mainSurface));
		}

		// 渲染每个敌人
		for (int i = 0; i < viewModel.enemies.Count; i++) {
			enemyViews[i].Render(viewModel.enemies[i]);
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
