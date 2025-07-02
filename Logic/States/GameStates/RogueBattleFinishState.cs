using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RogueBattleFinishState : RogueRouteState {
	public bool hasGainMoney;

	public override void OnEnter() {
		hasGainMoney = false;
		base.OnEnter();
	}

	protected override string GetRouteDesc() {
		return "You just finished a battle.";
	}

	protected override List<RouteOption> GetRouteOptions() {
		var ret = new List<RouteOption> { };
		bool isDone = true;

		if (!hasGainMoney) {
			isDone = false;
			ret.Add(new RouteOption {
				desc = "Gain 10 money",
				onSelect = () => {
					hasGainMoney = true;
					RoguePlayerData.Instance.money += 10;
					NotifyUpdateOption();
				}
			});
		}

		if (isDone) {
			ret.Add(new RouteOption {
				desc = "Continue",
				onSelect = () => {
					stateEngine.ReplaceTop<RogueInterludeState>();
				}
			});
		}

		return ret;
	}
}
