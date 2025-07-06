public partial class RogueBattleState {
  // 执行所有敌人的行动
  public void ExecuteAllEnemyActions() {
    // 这个方法已经被移动到 BattleContext 中，这里保留是为了向后兼容
    battleContext?.ExecuteAllEnemyActions();
  }

  public abstract class LegacyEnemyAction {
    public CharObject parent;
    public LegacyEnemyAction(CharObject parent) {
      this.parent = parent;
    }

    public virtual string ForeshowAction() {
      return string.Empty;
    }
    public string ForeshowAtkAction(int dmg) {
      return $"will cause [{dmg}] damage to you";
    }

    public virtual void ExecuteAction() {

    }
  }
  public class TreemanAction : LegacyEnemyAction {
    public int idx;

    public TreemanAction(CharObject parent) : base(parent) {
    }

    public override string ForeshowAction() {
      switch (idx) {
        case 0:
          return ForeshowAtkAction(6);
        default:
          break;
      }
      return null;
    }

    public override void ExecuteAction() {
      switch (idx) {
        case 0:
          instance.battleContext.ExecuteAction(BattleContext.CommonAction.enemy_attack(parent, baseDmg: 6));
          break;
        default:
          break;
      }
    }
  }
}