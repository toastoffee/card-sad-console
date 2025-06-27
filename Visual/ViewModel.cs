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
	public int playerShield; // 添加玩家护盾值
	public List<EnemyViewModel> enemies = new List<EnemyViewModel>(); // 添加敌人视图模型列表
	public List<CardViewModel> handCards = new List<CardViewModel>(); // 玩家手牌列表

    public List<string> gameLogs = new List<string>();    // 游戏相关的日志（如战斗、回合等）
	public List<string> systemLogs = new List<string>();  // 系统相关的日志（如程序运行、错误等）
}

internal class EnemyViewModel {
	public string name;
	public int hp;
	public int maxHp;
	public string intention;
}

internal class CardViewModel
{
	public string name;
	public int cost;
	public string description;
}
