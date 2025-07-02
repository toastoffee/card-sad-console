public class RougeRouteTemplateState : RogueRouteState {

  private string mDesc;
  private List<RouteOption> mOptions;

  public RougeRouteTemplateState(string desc, List<RouteOption> options) {
    mDesc = desc;
    mOptions = options;
  }

  protected override string GetRouteDesc() {
    return mDesc;
  }

  protected override List<RouteOption> GetRouteOptions() {
    return mOptions;
  }
}
