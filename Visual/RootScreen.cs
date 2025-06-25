using SadConsole.Input;

namespace CardConsole.Visual;

internal class RootScreen : ScreenObject
{

    private ScreenSurface _mainSurface;

    public PanelText turnText;

    public PanelText playerHealthText;
    public PanelText playerEnergyText;

    public PanelText enemyNameText;
    public PanelText enemyHealthText;
    public PanelText enemyIntentionText;

    public int testNumber = 1;  // in case you dont know how it works

    public RootScreen()
    {
        
        _mainSurface = new ScreenSurface(Game.Instance.ScreenCellsX, Game.Instance.ScreenCellsY);
        Children.Add(_mainSurface);

        _mainSurface.UseMouse = false;

        turnText = new PanelText(new Point(3, 1), "Turn", 6, 3, Color.Wheat, _mainSurface);
        turnText.alignType = PanelText.AlignType.Center;
        turnText.content = "1";

        playerEnergyText = new PanelText(new Point(3, 7), "Energy", 10, 3, Color.Orange, _mainSurface);
        playerEnergyText.alignType = PanelText.AlignType.Center;
        playerEnergyText.content = "3/3";

        playerHealthText = new PanelText(new Point(3, 10), "HP", 10, 3, Color.Red, _mainSurface);
        playerHealthText.alignType = PanelText.AlignType.Center;
        playerHealthText.content = "70/70";

        enemyNameText = new PanelText(new Point(50, 7), "Enemy", 12, 3, Color.Purple, _mainSurface);
        enemyNameText.alignType = PanelText.AlignType.Center;
        enemyNameText.content = "Treeman";

        enemyHealthText = new PanelText(new Point(50, 10), "HP", 12, 3, Color.Red, _mainSurface);
        enemyHealthText.alignType = PanelText.AlignType.Center;
        enemyHealthText.content = "27/27";

        enemyIntentionText = new PanelText(new Point(50, 13), "Intention", 30, 3, Color.Yellow, _mainSurface);
        enemyIntentionText.alignType = PanelText.AlignType.Center;
        enemyIntentionText.content = "enemy will cause 6 damage";
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        //!!! IN CASE YOU DONT KNOW HOW IT WORKS !!!
        bool handled = false;

        if (keyboard.IsKeyPressed(Keys.Up))
        {
            turnText.content = $"{testNumber++}";
            handled = true;
        }

        return handled;
    }
}
