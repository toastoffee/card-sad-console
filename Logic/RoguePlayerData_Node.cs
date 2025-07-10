using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public partial class RoguePlayerData {
  public void InitNodes() {
    nodeType = RogueNodeType.Init;
  }

  public void FinishInitNode() {
    nodeType = RogueNodeType.Enemy;
    enemyNodeDescriptor = new RogueEnemyNodeDescriptor();
  }

  public void FinishEnemyNode() {
    nodeType = RogueNodeType.Interlude;
  }

  public void FinishInterludeNode() {
    nodeType = RogueNodeType.Enemy;
    enemyNodeDescriptor = new RogueEnemyNodeDescriptor();
  }

  public struct BattleOut {
    public int remainHp;
  }

  public void SetBattleOut(BattleOut battleOut) {
    enemyNodeDescriptor.isKilled = true;
    hp = battleOut.remainHp;
  }
}