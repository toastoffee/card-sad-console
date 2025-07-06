using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public partial class BattleContext {
  public class EnemyContext {
    public CharObject cha;
    public int idx;
    public EnemyActionTag nextActionTag;

    public EnemyContext(CharObject enemy, int idx) {
      this.cha = enemy;
      this.idx = idx;
    }
  }
}