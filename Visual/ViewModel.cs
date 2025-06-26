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
	
	public List<string> gameLogs = new List<string>();    // 游戏相关的日志（如战斗、回合等）
	public List<string> systemLogs = new List<string>();  // 系统相关的日志（如程序运行、错误等）
}

internal class EnemyViewModel {
	public string name;
	public int hp;
	public int maxHp;
	public string intention;
}
