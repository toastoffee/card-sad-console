using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CardConsole.Visual;
internal class ViewModel {
	public int turn;
	public int eng;
	public int maxEng;
	public int playerHp;
	public int maxPlayerHp;
	public List<EnemyViewModel> enemies = new List<EnemyViewModel>(); // 添加敌人视图模型列表
}

internal class EnemyViewModel {
	public string name;
	public int hp;
	public int maxHp;
	public string intention;
}
