using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RogueInterludeState : RogueRouteState {
  protected override string GetRouteDesc() {
    return "Take a rest, and goto next battle.";
  }

  protected override List<RouteOption> GetRouteOptions() {
    var ret = new List<RouteOption> {
      new RouteOption {
        desc = "Continue",
        onSelect = () => {
          RoguePlayerData.Instance.FinishInterludeNode();
          CardGame.instance.RouteToProperState();
        }
      },
    };
    return ret;
  }
}
