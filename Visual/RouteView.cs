using SadConsole.UI;
using SadConsole.UI.Controls;

namespace CardConsole.Visual;

internal class RouteView {
  private ScreenSurface _surface;
  private ControlHost _controls;

  // Route状态专用组件
  private PanelText routeDescriptionText;
  private List<ButtonBox> routeOptionButtons = new List<ButtonBox>();

  public RouteView(ScreenSurface surface) {
    _surface = surface;
    _controls = new ControlHost();

    // 将ControlHost添加到表面
    _surface.SadComponents.Add(_controls);
  }

  public void Render(ViewModel viewModel) {
    // 先绘制路线区域的边框
    _surface.DrawBox(new Rectangle(0, 0, _surface.Width, _surface.Height),
      ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThin,
        new ColoredGlyph(Color.White, Color.Black)));

    // 渲染路线描述
    RenderRouteDescription(viewModel);

    // 渲染选项按钮
    RenderRouteOptions(viewModel);
  }

  private void RenderRouteDescription(ViewModel viewModel) {
    // 计算中部区域范围
    int totalHeight = _surface.Height;
    int middleAreaHeight = totalHeight;
    int descriptionAreaHeight = middleAreaHeight / 2;
    int descriptionY = 1;
    int descriptionWidth = _surface.Width - 10;

    if (routeDescriptionText == null) {
      routeDescriptionText = new PanelText(new Point(5, descriptionY), "Route", descriptionWidth, descriptionAreaHeight, Color.Cyan, _surface);
      routeDescriptionText.alignType = PanelText.AlignType.Center;
    } else {
      routeDescriptionText.SetPosition(new Point(5, descriptionY));
      routeDescriptionText.SetSize(descriptionWidth, descriptionAreaHeight);
    }
    routeDescriptionText.content = viewModel.routeDescription;

    // 绘制分割线
    int separatorY = descriptionY + descriptionAreaHeight;
    for (int x = 5; x < _surface.Width - 5; x++) {
      _surface.Surface[x, separatorY].Glyph = '-';
      _surface.Surface[x, separatorY].Foreground = Color.White;
    }
  }

  private void RenderRouteOptions(ViewModel viewModel) {
    // 确保选项按钮数量与选项数量一致
    while (routeOptionButtons.Count < viewModel.routeOptions.Count) {
      CreateNewOptionButton(routeOptionButtons.Count);
    }

    // 重新计算按钮位置以适应新的布局
    int totalHeight = _surface.Height;
    int middleAreaTop = totalHeight / 4;
    int middleAreaHeight = totalHeight / 2;
    int descriptionAreaHeight = middleAreaHeight / 2;
    int optionsAreaTop = middleAreaTop + descriptionAreaHeight + 1; // +1 for separator

    for (int i = 0; i < routeOptionButtons.Count; i++) {
      var button = routeOptionButtons[i];
      if (i < viewModel.routeOptions.Count) {
        var option = viewModel.routeOptions[i];
        button.Text = option.description;
        button.IsVisible = true;
        button.Tag = option.index;

        // 重新计算按钮位置
        int buttonY = optionsAreaTop + 2 + (i * (3 + 2));
        button.Position = new Point((_surface.Width - 70) / 2, buttonY);
      } else {
        button.IsVisible = false;
      }
    }
  }

  private void CreateNewOptionButton(int buttonIndex) {
    int buttonWidth = 70;
    int buttonHeight = 3;
    int buttonX = (_surface.Width - buttonWidth) / 2;
    int buttonY = _surface.Height / 2 + 5 + (buttonIndex * (buttonHeight + 2));

    var optionButton = new ButtonBox(buttonWidth, buttonHeight) {
      Position = new Point(buttonX, buttonY),
      Text = "",
      Tag = buttonIndex
    };

    optionButton.Click += (sender, e) => {
      if (sender is ButtonBox button && button.Tag is int optionIndex) {
        GameInput.Set(GameInput.Type.ROUTE_STATE_SELECT, new InputValue {
          intValue = optionIndex,
        });

        button.IsEnabled = false;
        System.Threading.Tasks.Task.Delay(200).ContinueWith(t => {
          button.IsEnabled = true;
        });
      }
    };

    _controls.Add(optionButton);
    routeOptionButtons.Add(optionButton);
  }
}