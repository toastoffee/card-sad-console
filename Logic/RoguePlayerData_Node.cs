using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public partial class RoguePlayerData {
  private List<EnemyActionModel> simpleEnemyPool = new List<EnemyActionModel> {
    EnemyActionDefine.Bandit,
    EnemyActionDefine.LongLeggy,
    EnemyActionDefine.FurnanceWorker,
  };

  public void InitNodes() {
    nodeType = RogueNodeType.Init;
    //todo: 构造起手装备
  }

  public void FinishInitNode() {
    nodeType = RogueNodeType.Enemy;
    enemyNodeDescriptor = new RogueEnemyNodeDescriptor() {
      //for testing, randomly select an enemy action model
      enemyActionModels = new List<EnemyActionModel>() {
        simpleEnemyPool[Random.Shared.Next(simpleEnemyPool.Count)]
      }
    };
  }

  public void FinishEnemyNode() {
    nodeType = RogueNodeType.Interlude;
    //todo： 构造局间强化
  }

  public void FinishInterludeNode() {
    nodeType = RogueNodeType.Enemy;
    enemyNodeDescriptor = new RogueEnemyNodeDescriptor() {
      //for testing, randomly select an enemy action model
      enemyActionModels = new List<EnemyActionModel>() {
        simpleEnemyPool[Random.Shared.Next(simpleEnemyPool.Count)]
      }
    };
  }

  public struct BattleOut {
    public int remainHp;
  }

  public void SetBattleOut(BattleOut battleOut) {
    enemyNodeDescriptor.isKilled = true;
    hp = battleOut.remainHp;
  }
}