using CardConsole.Visual;

public abstract class RogueRouteState : GameState {
	private ViewModel _viewModel;
	private List<RouteOption> _currentOptions;



	public class RouteOption {
		public string desc;
		public Action onSelect;
	}

	protected abstract string GetRouteDesc();
	protected abstract List<RouteOption> GetRouteOptions();

	public RogueRouteState() {
		_viewModel = CardGame.instance.viewModel;
	}

	public override void OnEnter() {
		NotifyUpdateOption();
	}

	public override void OnTick() {
		// 处理路线选择输入
		if (GameInput.Read(GameInput.Type.ROUTE_STATE_SELECT, out InputValue value)) {
			int selectedIndex = value.intValue;
			if (selectedIndex >= 0 && selectedIndex < _currentOptions.Count) {
				var selectedOption = _currentOptions[selectedIndex];
				selectedOption.onSelect?.Invoke();
			}
		}

		SyncToViewModel();
	}

	private void SyncToViewModel() {
		if (_viewModel == null) return;

		_viewModel.displayState = GameDisplayStateType.Route;
		_viewModel.routeDescription = GetRouteDesc();

		_viewModel.routeOptions.Clear();
		if (_currentOptions != null) {

			for (int i = 0; i < _currentOptions.Count; i++) {
				_viewModel.routeOptions.Add(new RouteOptionViewModel {
					description = _currentOptions[i].desc,
					index = i
				});
			}
		}
	}

	protected void NotifyUpdateOption() {
		_currentOptions = GetRouteOptions();
	}
}
