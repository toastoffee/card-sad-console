using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RogueInitState : RogueRouteState {
	protected override string GetRouteDesc() {
		var ret = "Rogue game init.";
		return ret;
	}

	protected override List<RotueOption> GetRouteOptions() {
		var ret = new List<RotueOption> {
			new RotueOption {
				desc = "Start a new game",
				onSelect = () => {
					stateEngine.ReplaceTop<RogueBattleState>();
				}
			},
		};
		return ret;
	}
}
