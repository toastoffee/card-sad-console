using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardConsole.Visual;

public abstract class RogueRouteState : GameState {
	private ViewModel _viewModel;
	private List<RotueOption> _currentOptions;

	public class RotueOption {
		public string desc;
		public Action onSelect;
	}

	protected abstract string GetRouteDesc();
	protected abstract List<RotueOption> GetRouteOptions();

	public RogueRouteState() {
		_viewModel = CardGame.instance.viewModel;
	}

	public override void OnEnter() {
		_currentOptions = GetRouteOptions();
		SyncToViewModel();
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

	protected void SyncToViewModel() {
		if (_viewModel == null) return;

		_viewModel.currentStateType = GameStateType.Route;
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
}
