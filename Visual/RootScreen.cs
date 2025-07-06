using SadConsole.Input;

namespace CardConsole.Visual;

internal class RootScreen : ScreenObject {
  public ViewModel viewModel = new ViewModel();
  public View view = new View();

  private GameDisplayStateType currentDisplayedState = GameDisplayStateType.Battle;

  public RootScreen() {
    // 始终添加Info和Log表面
    Children.Add(view.GetInfoSurface());
    Children.Add(view.GetLogSurface());

    // 初始显示Battle表面
    Children.Add(view.GetBattleSurface());

    // 始终添加HandCard表面
    Children.Add(view.GetHandCardSurface());

    // 初始化游戏
    CardGame.instance = new CardGame();
    CardGame.instance.viewModel = viewModel;
    CardGame.instance.Setup();
  }

  public override bool ProcessKeyboard(Keyboard keyboard) {
    bool handled = false;
    return handled;
  }

  private int remainTick = 1;
  public override void Update(TimeSpan delta) {
    base.Update(delta);
    if (GameInput.isDirty) {
      remainTick = 3;
    }
    if (remainTick <= 0) {
      return;
    }
    remainTick--;

    // 更新游戏逻辑
    CardGame.instance.Tick();
    GameInput.Reset();

    // 检查状态切换
    if (currentDisplayedState != viewModel.displayState) {
      SwitchGameSurface(viewModel.displayState);
      currentDisplayedState = viewModel.displayState;
    }

    // 调用View渲染所有内容
    view.Render(viewModel);
  }

  private void SwitchGameSurface(GameDisplayStateType newState) {
    // 移除当前游戏表面
    switch (currentDisplayedState) {
      case GameDisplayStateType.Battle:
        Children.Remove(view.GetBattleSurface());
        break;
      case GameDisplayStateType.Route:
        Children.Remove(view.GetRouteSurface());
        break;
    }

    // 添加新的游戏表面
    switch (newState) {
      case GameDisplayStateType.Battle:
        Children.Add(view.GetBattleSurface());
        break;
      case GameDisplayStateType.Route:
        Children.Add(view.GetRouteSurface());
        break;
    }

    // 控制HandCard表面的可见性
    var handCardSurface = view.GetHandCardSurface();
    handCardSurface.IsVisible = (newState == GameDisplayStateType.Battle);
  }
}
